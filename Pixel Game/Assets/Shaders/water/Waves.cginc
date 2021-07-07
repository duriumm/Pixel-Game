#include "SimplexNoise.cginc"
#include "PerlinNoise.cginc"

//Calculate number of fBm iterations (noise layers)
float calcFbmNumIterFromGrad(float resolutionFactor, float startFreq, int maxIter, float2 pos)
{
	//Calculate difference in input coords between adjacent pixels
	float dx = length(ddx(pos));
	float dy = length(ddy(pos));
	float diff = max(max(dx, dy), 0.001f);

	// Calculate maximum frequency
	// `resolutionFactor` should be set to 1 for maximum details without exceeding the Nykvist limit
	// Lower values results in fewer iterations and less detail
	float endFreq = 0.5f * resolutionFactor / diff;
	
	// If specified start frequency is too big to be represented with current
	// screen resolution and camera distance, then there is no point in doing any iterations
	if (startFreq > endFreq)
		return 0;

	//Calculate number of iterations, assuming frequency is doubled every iteration
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
	//Separate iteration count into integer and fractional part
	float iterFrac = modf(numIter, iNumIter);
	for (int i = 0; i < iNumIter; i++)
	{
		noise += fbmLayer(pos, amp, freq, seed);
		pos.xy += 100.373; //Shift by an arbitrary value to reduce grid artifacts
		amp *= gain;// *(noise.w * 0.5f + amp * 0.5f) * 7; //Uncomment for multifractal weirdness
		freq *= 2;
	}
	noise += iterFrac * fbmLayer(pos, amp, freq, seed);
	return noise;
}

float4 calcWaves(float2 pos, float time, float3 scroll, float amp, float freq, float stretch, float gain, float resolutionFactor)
{
	uint seed = 0;
	pos.x /= stretch;
	
	float numIter = calcFbmNumIterFromGrad(resolutionFactor, freq, 15, pos);
	
	float3 pos3D = float3(pos, 0);
	float3 posOffset = scroll * time;
	float4 noise = fbmNoise(pos3D + posOffset, numIter, amp, freq, gain, seed);
	//Duplicate noise, and scroll in opposite direction
	noise += fbmNoise(pos3D - posOffset, numIter, amp, freq, gain, seed);
	return noise;
}