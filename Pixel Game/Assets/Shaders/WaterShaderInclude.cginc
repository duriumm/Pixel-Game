// Simplex 3D noise, based on Stefan Gustavson's code:
// http://staffwww.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf

#define NoisePeriod 256

static const uint perm[512] = { 151,160,137,91,90,15,
	131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
	190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
	88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
	77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
	102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
	135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
	5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
	223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
	129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
	251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
	49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
	138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,

	151,160,137,91,90,15,
	131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
	190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
	88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
	77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
	102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
	135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
	5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
	223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
	129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
	251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
	49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
	138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
};

static const float3 grad3[12] =
{
	float3(1,1,0), float3(-1,1,0), float3(1,-1,0), float3(-1,-1,0),
	float3(1,0,1), float3(-1,0,1), float3(1,0,-1), float3(-1,0,-1),
	float3(0,1,1), float3(0,-1,1), float3(0,1,-1), float3(0,-1,-1)
};

float4 simplex3Corner(float3 offset, float3 gradient)
{
	float4 result = float4(0, 0, 0, 0);
	float t = 0.5f - dot(offset, offset);

	if (t > 0)
	{
		float offsetAlongGradient = dot(gradient, offset);
		float t2 = t * t;
		float t4 = t2 * t2;
		result.w = t4 * offsetAlongGradient;
		result.xyz = t4 * gradient - 8 * t * t2 * offsetAlongGradient * offset;
	}
	return result;
}

// The w component of the return value contains the noise value at the specified coordinate
// The xyz compenents contain the derivative
float4 simplexNoise3(float3 coords, float amp, float freq, int seed)
{
	coords *= freq;

	//The sample point's offsets from the 4 cell corners
	float3 offsetsFromCorners[4];
	
	//Position of the corners in skewed space
	int3 skewedCorners[4];
	
	// Skew the input space to determine which simplex cell we're in
	// Floor it to get the cell origin in skewed space
	float F3 = 1.0f / 3.0f;
	skewedCorners[0] = floor(coords + (coords.x + coords.y + coords.z) * F3);

	// Unskew cell origin back to (x,y,z) space
	float G3 = 1.0f / 6.0f;
	float3 firstCorner = skewedCorners[0] - (skewedCorners[0].x + skewedCorners[0].y + skewedCorners[0].z) * G3;
		
	offsetsFromCorners[0] = coords - firstCorner;
	
	// For the 3D case, the simplex shape is a slightly irregular tetrahedron.
	// Determine which simplex we are in.
	int3 skewedSecondCornerOffset;
	int3 skewedThirdCornerOffset;
	if (offsetsFromCorners[0].x >= offsetsFromCorners[0].y)
	{
		if (offsetsFromCorners[0].y >= offsetsFromCorners[0].z)
		{ 
			// X Y Z order
			skewedSecondCornerOffset = int3(1, 0, 0);
			skewedThirdCornerOffset = int3(1, 1, 0);
		} 
		else if (offsetsFromCorners[0].x >= offsetsFromCorners[0].z)
		{
			// X Z Y order
			skewedSecondCornerOffset = int3(1, 0, 0);
			skewedThirdCornerOffset = int3(1, 0, 1);
		}
		else
		{
			// Z X Y order
			skewedSecondCornerOffset = int3(0, 0, 1);
			skewedThirdCornerOffset = int3(1, 0, 1);
		}
	}
	else 
	{ 
		if (offsetsFromCorners[0].y < offsetsFromCorners[0].z)
		{ 
			// Z Y X order
			skewedSecondCornerOffset = int3(0, 0, 1);
			skewedThirdCornerOffset = int3(0, 1, 1);
		} 
		else if(offsetsFromCorners[0].x < offsetsFromCorners[0].z)
		{
			// Y Z X order
			skewedSecondCornerOffset = int3(0, 1, 0);
			skewedThirdCornerOffset = int3(0, 1, 1);
		} 
		else
		{
			// Y X Z order
			skewedSecondCornerOffset = int3(0, 1, 0);
			skewedThirdCornerOffset = int3(1, 1, 0);
		} 
	}
	// A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
	// a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
	// a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z),
	// where c = 1/6.
	// Sample offsets from three remaining corners in (x,y,z) coords
	offsetsFromCorners[1] = offsetsFromCorners[0] - skewedSecondCornerOffset + G3; 
	offsetsFromCorners[2] = offsetsFromCorners[0] - skewedThirdCornerOffset + 2.0f * G3; 
	offsetsFromCorners[3] = offsetsFromCorners[0] - 1.0 + 3.0f * G3;

	skewedCorners[0] = skewedCorners[0] & 255;
	skewedCorners[1] = skewedCorners[0] + skewedSecondCornerOffset;
	skewedCorners[2] = skewedCorners[0] + skewedThirdCornerOffset;
	skewedCorners[3] = skewedCorners[0] + 1;
	
	// Calculate the contribution from the four corners
	float4 noise = 0;
	for (int i = 0; i < 4; i++)
	{
		float gradientIndex = perm[skewedCorners[i].x + perm[skewedCorners[i].y + perm[skewedCorners[i].z]]] % 12;
		float3 gradient = grad3[gradientIndex];
		noise += simplex3Corner(offsetsFromCorners[i], gradient);
	}
	noise.xyz *= freq;
	noise *= amp;
	return 32 * noise;
}

float calcFbmNumIterFromGrad(float reso, inout float startFreq, int maxIter, float3 coords)
{
	float dx = length(ddx(coords));
	float dy = length(ddy(coords));
	float diff = max(dx, dy);

	if (diff == 0)
		diff = 0.001f;
	float freq2 = 0.5f * reso / diff;
	startFreq = min(freq2, startFreq);
	float numIter = log2(freq2 / startFreq);
	return min(numIter, maxIter);
}

float4 fbmLayer(float3 coords, float amp, float freq,float seed)
{
	return simplexNoise3(coords, amp, freq, seed);
}

float4 fbmNoise(float3 coords, float numIter, float amp, float freq, float gain, uint seed)
{
	float4 noise = 0;
	int iNumIter;
	float iterFrac = modf(numIter, iNumIter);
	for (int i = 0; i < iNumIter; i++)
	{
		noise += fbmLayer(coords, amp, freq, seed);
		amp *= gain;
		freq *= 2;
	}
	noise += iterFrac * fbmLayer(coords, amp, freq, seed);
	return noise;
}

float4 calcWaves(float2 coords, float time)
{
	float amp = 0.5f;
	float freq = 1;
	float gain = 0.3;
	float speed = 1;
	uint seed = 0;
	coords.x /= 2;

	float numIter = calcFbmNumIterFromGrad(1, freq, 15, float3(coords.x, coords.y, 0));
	float3 coords3D = float3(coords.x, coords.y, time * 0.5f);
	float4 noise = fbmNoise(coords3D, numIter, amp, freq, gain, seed);
	noise.xyz = normalize(float3(-noise.xy, 1));
	return noise;
}