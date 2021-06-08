// Simplex noise reference:
// http://staffwww.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
// The w component of the return value contains the noise value at the specified coordinate
// The xyz compenents contain the derivative

#define NoisePeriod 256

//float4 randomIndices2D[NoisePeriod * NoisePeriod];
float3 randomGradients[NoisePeriod * 2];
sampler2D hash2DTex;

uint4 getHash(uint3 pos, uint seed)
{
	//uint4 hash = (int4)randomIndices2D[pos.x + pos.y * NoisePeriod] + pos.z + seed;
	//return (uint4)hash % NoisePeriod;
	return (uint4)tex2D(hash2DTex, (float2)pos.xy) + pos.z + seed;
}

float4 simplex3Corner(float3 f, float3 g)
{
	float4 result = float4(0, 0, 0, 0);
	float t = 0.6f - dot(f, f);
	if (t > 0)
	{
		float gdotf = dot(g, f);
		float t2 = t * t;
		float t4 = t2 * t2;
		result.w = t4 * gdotf;
		result.xyz = t4 * g - 8 * t * t2 * gdotf * f;
	}
	return result;
}


float4 simplexNoise3(float3 Pc, float3 Po, float freq, int seed)
{
	const float F3 = 1 / 3.f;
	const float G3 = 1 / 6.f;

	Pc *= freq; Po *= freq;

	float3 PcF3 = Pc * F3;
	float sc = PcF3.x + PcF3.y + PcF3.z;
	float so = (Po.x + Po.y + Po.z) * F3;
	float3 skewedPc = fmod(Pc + sc, NoisePeriod);
	float3 skewedPo = Po + so;
	float3 skewedP = fmod(skewedPc + skewedPo, NoisePeriod);
	if (skewedP.x < 0) skewedP.x = NoisePeriod + skewedP.x; if (skewedP.x == 256) skewedP.x = 0;
	if (skewedP.y < 0) skewedP.y = NoisePeriod + skewedP.y; if (skewedP.y == 256) skewedP.y = 0;
	if (skewedP.z < 0) skewedP.z = NoisePeriod + skewedP.z; if (skewedP.z == 256) skewedP.z = 0;
	//if (skewedP.x<0) skewedP.x *= -1;
	//if (skewedP.y<0) skewedP.y *= -1;
	//if (skewedP.z<0) skewedP.z *= -1;
	int3 skewedPi;
	float3 Pf0 = modf(skewedP, skewedPi);
	float t = (Pf0.x + Pf0.y + Pf0.z) * G3;
	Pf0 = Pf0 - t;

	//Pc = fmod(Pc, 256);
	//Pc = fmod(Pc+Po,256);
	/*Pc = Pc+Po;
	float sc = (Pc.x + Pc.y + Pc.z)*F3;
	float3 skewedP = floor(Pc + sc);
	int3 skewedPi = (int3)(skewedP);
	float t = (skewedP.x + skewedP.y + skewedP.z) * G3;
	float3 Pf0 = Pc - (skewedP - t);*/

	
	uint4 hash2d = getHash(skewedPi, seed);
	return tex2D(hash2DTex, skewedP.xy) / 80;

	//hash2d = noisePerm2dTex.Load(int3((hash2d.xx + seed) % NOISE_PERIOD,0)) ;
	//hash2d = ((uint4)irand(hash2d) % NOISE_PERIOD);
	int gi1; //Gradient index for second corner
	int gi2; //Gradient index for third corner
	float3 PfOffset1 = float3(0, 0, 0); // Offsets for second corner in (i,j,k) coords
	float3 PfOffset2 = float3(0, 0, 0); // Offsets for third corner in (i,j,k) coords

	if (Pf0.x >= Pf0.y)
	{
		if (Pf0.y >= Pf0.z) { gi1 = hash2d.y; gi2 = hash2d.w; PfOffset1 = float3(1, 0, 0); PfOffset2 = float3(1, 1, 0); }      // X Y Z order
		else if (Pf0.x >= Pf0.z) { gi1 = hash2d.y; gi2 = hash2d.y + 1; PfOffset1 = float3(1, 0, 0); PfOffset2 = float3(1, 0, 1); }    // X Z Y order 
		else { gi1 = hash2d.x + 1; gi2 = hash2d.y + 1; PfOffset1 = float3(0, 0, 1); PfOffset2 = float3(1, 0, 1); }  // Z X Y order
	}
	else
	{ // x<y
		if (Pf0.y < Pf0.z) { gi1 = hash2d.x + 1; gi2 = hash2d.z + 1; PfOffset1 = float3(0, 0, 1); PfOffset2 = float3(0, 1, 1); } // Z Y X order
		else if (Pf0.x < Pf0.z) { gi1 = hash2d.z; gi2 = hash2d.z + 1; PfOffset1 = float3(0, 1, 0); PfOffset2 = float3(0, 1, 1); }   // Y Z X order
		else { gi1 = hash2d.z; gi2 = hash2d.w; PfOffset1 = float3(0, 1, 0); PfOffset2 = float3(1, 1, 0); }     // Y X Z order
	}

	//float n=0;float3 d=float3(0,0,0);
	float4 result = float4(0, 0, 0, 0);
	//hash2d=0;
	//gi1=gi2=0;

	//// Noise contribution from first corner
	float3 grad = randomGradients[hash2d.x];
	result += simplex3Corner(Pf0, grad);

	// Noise contribution from second corner
	float3 Pf = Pf0 - PfOffset1 + G3;
	grad = randomGradients[gi1];
	result += simplex3Corner(Pf, grad);

	// Noise contribution from third corner
	Pf = Pf0 - PfOffset2 + 2 * G3;
	grad = randomGradients[gi2];
	result += simplex3Corner(Pf, grad);

	// Noise contribution from last corner
	Pf = Pf0 - 1 + 3 * G3;
	grad = randomGradients[hash2d.w];
	result += simplex3Corner(Pf, grad);
	return 32 * result;
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
	float4 noise = simplexNoise3(pos, float3(0,0,0), freq, seed);
	///amp *= h2.w + 0.5f;
	noise.w *= amp;
	noise.xyz *= freq;
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