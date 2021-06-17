//Wavelet noise based on this code:
//https://www.shadertoy.com/view/wsBfzK
//Calculates derivatives numerically, needs some tweaking
//Perlin or simplex noise may be better alternatives

float WaveletNoiseValue(float2 p, float z, float k, float amp, float freq)
{
	p *= freq;
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
	float value = WaveletNoiseValue(p.xy, z, 1.24f, amp, freq);
	float dx = WaveletNoiseValue(float2(p.x + 1.f * freq, p.y), z, 1.24f, amp, freq) - value;
	float dy = WaveletNoiseValue(float2(p.x, p.y + 1.f * freq), z, 1.24f, amp, freq) - value;
	return float4(float3(dx, dy, 0), value);
}

