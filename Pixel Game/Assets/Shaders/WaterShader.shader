Shader "Unlit/NewUnlitShader"
{
    Properties
    {
		_MainTex("TileMap", 2D) = "white" {}
		Sky("Sky", 2D) = "white" {}
		Ground("Ground", 2D) = "white" {}
		GroundNormalMap("GroundNormalMap", 2D) = "white" {}

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
				output.viewPosUV = output.viewPos / float2(3.5, 2) + float2(0.3f, 0.5f);
				output.worldPosUV = abs((output.worldPos - _WorldSpaceCameraPos * 0.9) / 2.3f);

                return output;
            }

			fixed4 frag(VertexOutput input) : SV_Target
			{
				float3 lightDir = normalize(float3(-1, -1, -0.59));
				//fixed4 tileCol = tex2D(_MainTex, input.tileMapUV);
				//Todo: check for green-screen kind of color to enable water shading

				float2 uvOffset = sin(input.worldPos * 10 + _Time.y * 4) / 3.f;
				uvOffset.y /= 4;
				float4 skyColor = tex2D(Sky, input.viewPosUV);
				//return skyColor;
				//float4 groundColor = tex2D(Ground, input.worldPosUV);
				float2 groundUV = fmod(abs(input.worldPos - _WorldSpaceCameraPos * 0.9) / 2.3f, 1);
				float4 groundColor = tex2D(Ground, groundUV);
				float4 groundNormal = normalize(tex2D(GroundNormalMap, groundUV));
				//return groundNormal;
				
				//float4 groundColor = tex2D(Ground, fmod(abs(input.viewPos) * 2, 1));
				groundColor *= fixed4(0.1, 0.6, 0.6, 1); //Turbidness
				groundColor *= 1.3f;
				groundColor *= dot(lightDir, -groundNormal);

				float3 viewDir = normalize(float3(input.viewPos, 0.25f));
				float3 normal = normalize(float3(uvOffset, 1));
				float3 lightReflectionDir = reflect(lightDir, normal);
				float viewDotNormal = dot(normal, viewDir);
				float fresnel = calcFresnel(viewDotNormal);
				float4 color = lerp(groundColor, skyColor, fresnel);
				color += pow(saturate(dot(lightReflectionDir, viewDir)), 100);
				return color;
				//rturn float4(0, 0, fmod(input.world.x, 2) / 2, 1); 
				//return float4(0, 0, fmod(abs(input.view.x), 1), 1);
            }

			
            ENDCG
        }
    }
}
