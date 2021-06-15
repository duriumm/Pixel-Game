static const int Permutations[512] =
{
	151, 160, 137, 91, 90, 15,
	131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
	190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
	88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
	77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
	102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
	135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
	5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
	223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
	129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
	251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
	49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
	138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,

	151, 160, 137, 91, 90, 15,
	131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
	190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
	88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
	77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
	102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
	135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
	5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
	223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
	129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
	251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
	49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
	138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
};

float fade(float t) 
{
	return t * t * t * (t * (t * 6 - 15) + 10);
}

float fadeDeriv(float3 t) 
{
	return 30 * t * t * (t * (t - 2) + 1);
}

//float lerp(float t, float a, float b) {
	/*return a + t * (b - a);
}*/


float4 corner(float3 offset, float3 gradient)
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

float grad(int hash, float x, float y, float z) {
	// CONVERT LO 4 BITS OF HASH CODE INTO 12 GRADIENT DIRECTIONS.
	int h = hash & 15;
	float u = h < 8 ? x : y;
	float v = h < 4 ? y : ((h == 12 || h == 14) ? x : z);
	return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
}

//float perlinNoise3(float3 pos, float amp, float freq, int seed)
//{
//	pos *= freq;
//
//	// FIND UNIT CUBE THAT CONTAINS POINT.
//	int X = (int)floor(pos.x) & 255;
//	int Y = (int)floor(pos.y) & 255;
//	int Z = (int)floor(pos.z) & 255;
//
//	// FIND RELATIVE X,Y,Z OF POINT IN CUBE.
//	float x = pos.x - floor(pos.x);
//	float y = pos.y - floor(pos.y);
//	float z = pos.z - floor(pos.z);
//
//	// COMPUTE FADE CURVES FOR EACH OF X,Y,Z.
//	float u = fade(x);
//	float v = fade(y);
//	float w = fade(z);
//
//	// HASH COORDINATES OF THE 8 CUBE CORNERS
//	int A = Permutations[X] + Y;
//	int AA = Permutations[A] + Z;
//	int AB = Permutations[A + 1] + Z;
//	int B = Permutations[X + 1] + Y;
//	int BA = Permutations[B] + Z;
//	int BB = Permutations[B + 1] + Z;
//
//	// AND ADD BLENDED RESULTS FROM  8 CORNERS OF CUBE
//	float noise = lerp(w, lerp(v, lerp(u, grad(p[AA], x, y, z),    // CORNER 0      
//		grad(p[BA], x - 1, y, z)),                       // CORNER 1                              
//		lerp(u, grad(p[AB], x, y - 1, z),                // CORNER 2                        
//			grad(p[BB], x - 1, y - 1, z))),                  // CORNER 3                      
//		lerp(v, lerp(u, grad(p[AA + 1], x, y, z - 1),    // CORNER 4            
//			grad(p[BA + 1], x - 1, y, z - 1)),               // CORNER 5                  
//			lerp(u, grad(p[AB + 1], x, y - 1, z - 1),        // CORNER 6  
//				grad(p[BB + 1], x - 1, y - 1, z - 1))));         // CORNER 7
//	return noise * amp;
//}

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
	return Permutations[Permutations[Permutations[x] + y] + z];
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
	//amp = freq = 1;
	p *= freq;
	int xi0 = ((int)floor(p.x)) & 255;
	int yi0 = ((int)floor(p.y)) & 255;
	int zi0 = ((int)floor(p.z)) & 255;

	int xi1 = (xi0 + 1) & 255;
	int yi1 = (yi0 + 1) & 255;
	int zi1 = (zi0 + 1) & 255;

	float tx = p.x - floor(p.x);
	float ty = p.y - floor(p.y);
	float tz = p.z - floor(p.z);

	float u = quintic(tx);
	float v = quintic(ty);
	float w = quintic(tz);

	// generate vectors going from the grid points to p
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
	
	/*int A = Permutations[X] + Y;
	int AA = Permutations[A] + Z;
	int AB = Permutations[A + 1] + Z;
	int B = Permutations[X + 1] + Y;
	int BA = Permutations[B] + Z;
	int BB = Permutations[B + 1] + Z;*/
	
	/*float a = gradientDotV(Permutations(xi0, yi0, zi0), x0, y0, z0);
	float b = grad(hash(xi1, yi0, zi0), x1, y0, z0);
	float c = gradientDotV(hash(xi0, yi1, zi0), x0, y1, z0);
	float d = gradientDotV(hash(xi1, yi1, zi0), x1, y1, z0);
	float e = gradientDotV(hash(xi0, yi0, zi1), x0, y0, z1);
	float f = gradientDotV(hash(xi1, yi0, zi1), x1, y0, z1);
	float g = gradientDotV(hash(xi0, yi1, zi1), x0, y1, z1);
	float h = gradientDotV(hash(xi1, yi1, zi1), x1, y1, z1); */


	float du = quinticDeriv(tx);
	float dv = quinticDeriv(ty);
	float dw = quinticDeriv(tz);

	//float4 noise;
	//noise = corner(float3(x0, y0, z0), a);
	//noise += corner(float3(1 - x0, y0, z0), b);
	//noise += corner(float3(x0, 1 - y0, z0), c);
	//noise += corner(float3(1 - x0, 1 - y0, z0), d);
	//noise += corner(float3(x0, y0, 1 - z0), e);
	//noise += corner(float3(1 - x0, y0, 1 - z0), f);
	//noise += corner(float3(x0, 1 - y0, 1 - z0), g);
	//noise += corner(float3(1 - x0, 1 - y0, 1 - z0), h);
	//noise.w *= amp;
	//noise.xyz *= freq;
	//return noise;

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

float WaveletNoiseValue(float2 p, float z, float k, float amp, float freq)
{
	p *= freq;
	// https://www.shadertoy.com/view/wsBfzK
	float d = 0., s = 1., m = 0., a;
	for (float i = 0.; i < 4.; i++) {
		float2 q = p * s, g = frac(floor(q) * float2(123.34, 233.53));
		g += dot(g, g + 23.234);
		a = frac(g.x * g.y) * 1e3;// +z*(mod(g.x+g.y, 2.)-1.); // add vorticity
		float2x2 mat = float2x2(cos(a), sin(a), -sin(a), cos(a));
		q = mul(mat, (frac(q) - .5f));
		d += sin(q.x * 10. + z) * smoothstep(.25, .0, dot(q, q)) / s;
		p = mul(float2x2(.54f, .84f, -.84f, .54f), p) + i;
		m += 1. / s;
		s *= k;
	}
	return d * amp / m;
}

float4 WaveletNoise(float2 p, float z, float k, float amp, float freq)
{
	//freq = amp = 1;
	float value = WaveletNoiseValue(p.xy, z, 1.24f, amp, freq);
	float dx = WaveletNoiseValue(float2(p.x + 1.f * freq, p.y), z, 1.24f, amp, freq) - value;
	float dy = WaveletNoiseValue(float2(p.x, p.y + 1.f * freq), z, 1.24f, amp, freq) - value;
	float3 tanX = float3(1, 0, dx * freq);
	float3 tanY = float3(0, 1, dy * freq);
	float3 normal = normalize(cross(tanX, tanY));
	return float4(normal, value);
}

