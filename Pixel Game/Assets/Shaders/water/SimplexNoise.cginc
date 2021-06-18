// Simplex 3D noise, based on Stefan Gustavson's code here:
// http://staffwww.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
// Converted to HLSL and added calculation of analytical derivative

#include "PermutationLookup.cginc"

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

// The w component of the return value contains the noise value
// The xyz compenents contain the noise derivative
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
