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

	//float t0 = 0.6 - x0*x0 - y0*y0 - z0*z0;
	//if (t0<0)
	//	n0 = 0.0f;
	//else
	//{
	//	t0 *= t0;
	//	n0 = t0 * t0 * dot(grad3[gi0], x0, y0, z0);
	//}
	//float t1 = 0.6f - x1 * x1 - y1 * y1 - z1*z1;
	//if(t1<0)
	//	n1 = 0.0;
	//else
	//{
	//	t1 *= t1;
	//	n1 = t1 * t1 * dot(grad3[gi1], x1, y1, z1);
	//}
	//float t2 = 0.6f - x2*x2 - y2*y2 - z2*z2;
	//if(t2<0)
	//	n2 = 0.0f;
	//else 
	//{      
	//	t2 *= t2;
	//	n2 = t2 * t2 * dot(grad3[gi2], x2, y2, z2);
	//}
	//float t3 = 0.6f - x3*x3 - y3*y3 - z3*z3;
	//if (t3<0) n3 = 0.0;
	//else 
	//{
	//	t3 *= t3;
	//	n3 = t3 * t3 * dot(grad3[gi3], x3, y3, z3);
	//}
	//// Add contributions from each corner to get the final noise value.
	//// The result is scaled to stay just inside [-1,1]
	//return 32.0*(n0 + n1 + n2 + n3);
}

float calcFbmNumIterFromGrad(float reso, inout float startFreq, int maxIter, float3 coords)
{
	float dx = length(ddx(coords));
	float dy = length(ddy(coords));

	//float diff = length(float2(dx,dy));
	float diff = max(dx, dy);

	if (diff == 0)
		diff = 0.001f;
	float freq2 = 0.5f * reso / diff;
	startFreq = min(freq2, startFreq);
	float numIter = log2(freq2 / startFreq);
	return min(numIter, maxIter);
}

float4 fbmLayer(float pos, float amp, float freq,float seed)
{
	float4 noise = simplexNoise3(pos, amp, freq, seed);
	///amp *= h2.w + 0.5f;
	return noise;
}

float4 fbmNoise(float3 pos, float numIter, float amp, float freq, float gain, uint seed)
{
	float4 h = 0;
	int iNumIter;
	float iterFrac = modf(numIter, iNumIter);
	for (int i = 0; i < iNumIter; i++)
	{
		h += fbmLayer(pos, amp, freq, seed);
		amp *= gain;
		freq *= 2;
	}
	h += iterFrac * fbmLayer(pos, amp, freq, seed);
	//h *= amp;
	
	//h.w+=200;
	//h*=startAmp;
	//h.w+=10;
	//return numIter/30;
	return h;
}

float4 calcWaves(float2 pos, float time)
{
	float amp = 0.3f;
	float freq = 1.f;
	float gain = 0.5f;
	float speed = 1;
	uint seed = 0;

	float numIter = calcFbmNumIterFromGrad(1, freq, 15, float3(pos.x, pos.y, 0));
	//numIter = 1;
	
	float3 pos3D = float3(pos.x, pos.y, 2 * time);
	float4 noiseRet;
	noiseRet = fbmNoise(pos3D, numIter, amp, freq, gain, seed);
	//noiseRet = fbmCellular(float3(vertPos.x, clock*0.2f, vertPos.z), numIter, fbmi);
	//noiseRet = float4(0,1,0,0);
	return noiseRet;
}


static const float waterFbmi_amp = 1.15f;
static const float waterFbmi_freq = 0.001;
static const float waterFbmi_gain = 0.634f;
static const float waterFbmi_scrollSpeed = 0.2f;
static const float waterFbmi_animSpeed = 2;