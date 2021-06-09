Shader "Unlit/NewUnlitShader"
{
    Properties
    {
		_MainTex("TileMap", 2D) = "white" {}
		Sky("Sky", 2D) = "white" {}
		Ground("Ground", 2D) = "white" {}
		GroundNormalMap("GroundNormalMap", 2D) = "white" {}
		Hash2DTex("hash2DTex", 2D) = "white" {}

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "WaterShaderInclude.cginc"

            struct VertexInput
            {
                float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

            struct VertexOutput
            {
				float4 pos : SV_POSITION;
				float2 tileMapUV : TEXCOORD0;
				float2 worldPos : TEXCOORD1;
				float2 viewPos : TEXCOORD2;
				float2 worldPosUV : TEXCOORD3;
				float2 viewPosUV : TEXCOORD4;
            };

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D Sky;
			sampler2D Ground;
			sampler2D GroundNormalMap;

			float calcFresnel(float cosAngle, bool underWater = false)
			{
				float fresnel = 1 - cosAngle;

				// We probably won't need to render the water surface from below
				// but I'll leave the code here just in case
				if (underWater)
					fresnel *= 2.5f;

				fresnel = pow(fresnel, 4);

				// scale and bias to fit to real fresnel curve
				return min((fresnel * 0.95f) + 0.05f, 1);
			}

            VertexOutput vert(VertexInput input)
            {
				VertexOutput output;
				output.pos = UnityObjectToClipPos(input.pos);
				output.tileMapUV = TRANSFORM_TEX(input.uv, _MainTex);
				output.worldPos = mul(unity_ObjectToWorld, input.pos);
				output.viewPos = UnityObjectToClipPos(input.pos);
				//output.viewPosUV = output.viewPos / float2(2.5, 1) + float2(0.3f, 0.65f);
				output.viewPosUV = output.viewPos / float2(3.5f, 2) + float2(0.5f, 0.5f);
				output.worldPosUV = abs((output.worldPos - _WorldSpaceCameraPos * 0.9) / 2.3f);
				
                return output;
            }

			fixed4 frag(VertexOutput input) : SV_Target
			{
				float3 lightDir = normalize(float3(-1, -1, -0.59));
				//fixed4 tileCol = tex2D(_MainTex, input.tileMapUV);
				//Todo: check for green-screen kind of color to enable water shading
				
				//float4 noise = simplexNoise3(float3(input.worldPos, _Time.y), 1, 1, 0);
				float4 noise = calcWaves(input.worldPos, _Time.y);
				//return noise.w;
				//float2 uvOffset = sin(input.worldPos * 10 + _Time.y * 4) / 3.f;
				float2 uvOffset = noise.xy / 20;
								
				//uvOffset.y /= 4;
				float4 skyColor = tex2D(Sky, input.viewPosUV + uvOffset);
				//skyColor = float4(0.9, 0.5, 0.5, 1);

				float2 groundUV = fmod(abs(input.worldPos - _WorldSpaceCameraPos * 0.9) / 2.3f, 1) - uvOffset;
				float4 groundColor = tex2D(Ground, groundUV);
				float4 groundNormal = normalize(tex2D(GroundNormalMap, groundUV));
				
				float4 waterColor = float4(0.1, 0.6, 0.6, 1);
				waterColor *= 1.3f;
				groundColor *= dot(lightDir, -groundNormal); //Normal mapping
				groundColor *= waterColor; //Completely transparent water
				groundColor = lerp(groundColor, float4(0, 0.1f, 0.35f, 1), 0.5f);; //Turbidness

				float3 viewDir = normalize(float3(input.viewPos, 0.25f));
				float3 normal = noise.xyz;
				float3 lightReflectionDir = reflect(lightDir, normal);
				float viewDotNormal = dot(normal, viewDir);
				float fresnel = calcFresnel(viewDotNormal);
				float4 color = lerp(groundColor, skyColor, fresnel);
				color += pow(saturate(dot(lightReflectionDir, viewDir)), 100);
				return color;
            }
            ENDCG
        }
    }
}
