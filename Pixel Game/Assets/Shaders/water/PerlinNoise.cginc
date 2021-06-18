// Improved Perlin 3D noise, based on this code:
// https://www.scratchapixel.com/code.php?id=57&origin=/lessons/procedural-generation-virtual-worlds/perlin-noise-part-2
// Converted to HLSL and fixed some errors in derivative calculations
// which would cause visual artifacts for non-integer z inputs


#include "PermutationLookup.cginc"

float grad(int hash, float x, float y, float z) {
	// CONVERT LO 4 BITS OF HASH CODE INTO 12 GRADIENT DIRECTIONS.
	int h = hash & 15;
	float u = h < 8 ? x : y;
	float v = h < 4 ? y : ((h == 12 || h == 14) ? x : z);
	return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
}

float quintic(float t)
{
	return t * t * t * (t * (t * 6 - 15) + 10);
}

float quinticDeriv(float t)
{
	return 30 * t * t * (t * (t - 2) + 1);
}

int hash(int x, int y, int z)
{
	return perm[perm[perm[x] + y] + z];
}

float gradientDotV(
	// a value between 0 and 255
	int perm,
	// vector from one of the corners of the cell to the point where the noise function is computed
	float x, float y, float z) 
	{
		switch (perm & 15)
		{
		case  0: return  x + y; // (1,1,0) 
		case  1: return -x + y; // (-1,1,0) 
		case  2: return  x - y; // (1,-1,0) 
		case  3: return -x - y; // (-1,-1,0) 
		case  4: return  x + z; // (1,0,1) 
		case  5: return -x + z; // (-1,0,1) 
		case  6: return  x - z; // (1,0,-1) 
		case  7: return -x - z; // (-1,0,-1) 
		case  8: return  y + z; // (0,1,1), 
		case  9: return -y + z; // (0,-1,1), 
		case 10: return  y - z; // (0,1,-1), 
		case 11: return -y - z; // (0,-1,-1) 
		case 12: return  y + x; // (1,1,0) 
		case 13: return -x + y; // (-1,1,0) 
		case 14: return -y + z; // (0,-1,1) 
		case 15: return -y - z; // (0,-1,-1) 
	}
}

float4 perlinNoise3(float3 p, float amp, float freq, int seed)
{
	p *= freq;

	//Cube origin coords
	int xi0 = ((int)floor(p.x)) & 255;
	int yi0 = ((int)floor(p.y)) & 255;
	int zi0 = ((int)floor(p.z)) & 255;

	//Opposite cube corner
	int xi1 = (xi0 + 1) & 255;
	int yi1 = (yi0 + 1) & 255;
	int zi1 = (zi0 + 1) & 255;

	//Offset from cube origin to current position
	float tx = p.x - floor(p.x);
	float ty = p.y - floor(p.y);
	float tz = p.z - floor(p.z);

	// Do quintic interpolation
	float u = quintic(tx);
	float v = quintic(ty);
	float w = quintic(tz);

	// x0/y0/z0 is offset from cube origin
	// x1/y1/z1 is offset from opposite cube corner
	float x0 = tx, x1 = tx - 1;
	float y0 = ty, y1 = ty - 1;
	float z0 = tz, z1 = tz - 1;

	float a = gradientDotV(hash(xi0, yi0, zi0), x0, y0, z0);
	float b = gradientDotV(hash(xi1, yi0, zi0), x1, y0, z0);
	float c = gradientDotV(hash(xi0, yi1, zi0), x0, y1, z0);
	float d = gradientDotV(hash(xi1, yi1, zi0), x1, y1, z0);
	float e = gradientDotV(hash(xi0, yi0, zi1), x0, y0, z1);
	float f = gradientDotV(hash(xi1, yi0, zi1), x1, y0, z1);
	float g = gradientDotV(hash(xi0, yi1, zi1), x0, y1, z1);
	float h = gradientDotV(hash(xi1, yi1, zi1), x1, y1, z1);
	
	float du = quinticDeriv(tx);
	float dv = quinticDeriv(ty);
	float dw = quinticDeriv(tz);

	float k0 = a;
	float k1 = (b - a);
	float k2 = (c - a);
	float k3 = (e - a);
	float k4 = (a + d - b - c);
	float k5 = (a + f - b - e);
	float k6 = (a + g - c - e);
	float k7 = (b + c + e + h - a - d - f - g);

	float3 derivs;
	derivs.x = du * (k1 + k4 * v + k5 * w + k7 * v * w);
	derivs.y = dv * (k2 + k4 * u + k6 * w + k7 * u * w);
	derivs.z = dw * (k3 + k5 * u + k6 * v + k7 * u * v);

	float value = k0 + k1 * u + k2 * v + k3 * w + k4 * u * v + k5 * u * w + k6 * v * w + k7 * u * v * w;
	return float4(derivs * freq, value) * amp;
}
