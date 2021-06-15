#include "SimplexNoise.cginc"
#include "PerlinNoise.cginc"

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
	//return simplexNoise3(coords, amp, freq, seed);
	return perlinNoise3(coords, amp, freq, seed);
	//return WaveletNoise(coords.xy, coords.z, 1.24f, amp, freq);
}

float4 fbmNoise(float3 coords, float numIter, float amp, float freq, float gain, uint seed)
{
	float4 noise = 0;
	//numIter = 5;
	//amp = freq = 1;
	freq *= 1;
	int iNumIter;
	//numIter = 1;
	float iterFrac = modf(numIter, iNumIter);
	for (int i = 0; i < iNumIter; i++)
	{
		//float angle = i;
		//coords.xy = mul(float2x2(cos(angle), sin(angle),
		//						  -sin(angle), cos(angle)),
		//				  coords.xy);
		coords.xy += 100.373;
		noise += fbmLayer(coords, amp, freq, seed);
		amp *= gain;
		//gain *= min(0.75f, 2 * amp * amp);
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
	//amp = freq = 1;

	float numIter = calcFbmNumIterFromGrad(1, freq, 15, float3(coords.x, coords.y, 0));
	float3 coords3D = float3(coords.x, coords.y, time * 0.4f);
	//numIter = 1;
	float4 noise = fbmNoise(coords3D, numIter, amp, freq, gain, seed);
	
	noise.xyz = normalize(noise.xyz);
	noise = noise * 0.5f + 0.5f;

	/*float3 xTangent = float3(1, 0, noise.x);
	float3 yTangent = float3(0, 1, noise.y);
	noise.xyz = normalize(cross(xTangent, yTangent));*/
	//noise.xyz = normalize(float3(-noise.xy, 1));

	return noise;
}