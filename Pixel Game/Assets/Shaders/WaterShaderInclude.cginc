float4 SimplexNoise3D(float3 Pc, float3 Po, float freq, int seed)
{
	const float F3 = 1 / 3.f;
	const float G3 = 1 / 6.f;

	Pc *= freq; Po *= freq;

	float3 PcF3 = Pc * F3; //chunkPos är stort, så minska komponenterna innan de adderas
	float sc = PcF3.x + PcF3.y + PcF3.z;
	float so = (Po.x + Po.y + Po.z) * F3;
	float3 skewedPc = fmod(Pc + sc, NOISE_PERIOD);
	float3 skewedPo = Po + so;
	float3 skewedP = fmod(skewedPc + skewedPo, NOISE_PERIOD);
	if (skewedP.x < 0) skewedP.x = NOISE_PERIOD + skewedP.x; if (skewedP.x == 256) skewedP.x = 0;
	if (skewedP.y < 0) skewedP.y = NOISE_PERIOD + skewedP.y; if (skewedP.y == 256) skewedP.y = 0;
	if (skewedP.z < 0) skewedP.z = NOISE_PERIOD + skewedP.z; if (skewedP.z == 256) skewedP.z = 0;
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

	int4 hash2d;
	hash2d = et_noisePerm2dTex.Load(int3(skewedPi.xy, 0)) + skewedPi.z + seed;
	hash2d = uint4(hash2d) % NOISE_PERIOD;
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
	float3 grad = et_noise3GradTex.Load(int2(hash2d.x, 0)).xyz;
	result += simplex3Corner(Pf0, grad);

	// Noise contribution from second corner
	float3 Pf = Pf0 - PfOffset1 + G3;
	grad = et_noise3GradTex.Load(int2(gi1, 0));
	result += simplex3Corner(Pf, grad);

	// Noise contribution from third corner
	Pf = Pf0 - PfOffset2 + 2 * G3;
	grad = et_noise3GradTex.Load(int2(gi2, 0));
	result += simplex3Corner(Pf, grad);

	// Noise contribution from last corner
	Pf = Pf0 - 1 + 3 * G3;
	grad = et_noise3GradTex.Load(int2(hash2d.w + 1, 0));
	result += simplex3Corner(Pf, grad);

	//if (Pc.x == 0 && Pc.y == 0 && Pc.z == 0 && Po.x == 0 && Po.y == 0 && Po.z == 0)
	//	printf("Simplex value:");// %f", result.w);
	return 32 * result;
}