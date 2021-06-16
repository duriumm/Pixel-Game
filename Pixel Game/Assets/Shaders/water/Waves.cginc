#include "SimplexNoise.cginc"
#include "PerlinNoise.cginc"

float calcFbmNumIterFromGrad(float reso, float startFreq, int maxIter, float2 pos)
{
	float dx = length(ddx(pos));
	float dy = length(ddy(pos));
	float diff = max(max(dx, dy), 0.001f);
	float endFreq = 0.5f * reso / diff;
	
	if (startFreq > endFreq)
		return 0;

	float numIter = log2(endFreq / startFreq);
	return min(numIter, maxIter);
}

float4 fbmLayer(float3 pos, float amp, float freq, float seed)
{
	//return simplexNoise3(pos, amp, freq, seed);
	return perlinNoise3(pos, amp, freq, seed);
	//return WaveletNoise(pos.xy, pos.z, 1.24f, amp, freq);
}

float4 fbmNoise(float3 pos, float numIter, float amp, float freq, float gain, uint seed)
{
	float4 noise = 0;
	int iNumIter;
	float iterFrac = modf(numIter, iNumIter);
	for (int i = 0; i < iNumIter; i++)
	{
		pos.xy += 100.373;
		noise += fbmLayer(pos, amp, freq, seed);
		amp *= gain;// *(noise.w * 0.5f + amp * 0.5f) * 7; //Uncomment for multifractal weirdness
		freq *= 2;
	}
	noise += iterFrac * fbmLayer(pos, amp, freq, seed);
	return noise;
}

float4 calcWaves(float2 pos, float time, float3 scroll, float amp, float freq, float stretch, float gain, float numIter)
{
	uint seed = 0;
	pos.x /= stretch;
	
	if (numIter < 0)
		numIter = calcFbmNumIterFromGrad(1, freq, 15, pos);
	
	float3 pos3D = float3(pos, 0);
	float3 posOffset = scroll * time;
	float4 noise = fbmNoise(pos3D + posOffset, numIter, amp, freq, gain, seed);
	noise += fbmNoise(pos3D - posOffset, numIter, amp, freq, gain, seed);
	
	return noise;
}