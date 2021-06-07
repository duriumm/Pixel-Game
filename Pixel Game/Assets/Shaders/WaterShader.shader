Shader "Unlit/NewUnlitShader"
{
    Properties
    {
		_MainTex("TileMap", 2D) = "white" {}
		Sky("Sky", 2D) = "white" {}
		Ground("Ground", 2D) = "white" {}
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

            struct appdata
            {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

            struct v2f
            {
				float2 tileMapUV : TEXCOORD0;
				float2 worldPos : TEXCOORD1;
				float2 viewPos : TEXCOORD2;
				float2 viewPosUV : TEXCOORD3;
				float4 vertex : SV_POSITION;
            };

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D Sky;
			sampler2D Ground;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.tileMapUV = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.viewPos = UnityObjectToClipPos(v.vertex);
				o.viewPosUV = o.viewPos / float2(3.5, 2) + float2(0.3f, 0.66f);

                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				float3 lightDir = normalize(float3(-1, -1, -0.59));
				//fixed4 tileCol = tex2D(_MainTex, i.tileMapUV);
				//Todo: check for green-screen kind of color to enable water shading

o				float2 uvOffset = sin(i.worldPos * 10 + _Time.y * 4) / 3.f;
				uvOffset.y /= 4;
				float4 skyColor = tex2D(Sky, i.viewPosUV);
				//return skyColor;
				float4 groundColor = tex2D(Ground, fmod(abs((i.worldPos - _WorldSpaceCameraPos * 0.9) / 2.3f), 1));
				//float4 groundColor = tex2D(Ground, fmod(abs(i.viewPos) * 2, 1));
				groundColor *= fixed4(0.1, 0.6, 0.6, 1); //Turbidness
				//return groundColor;
				//skyColor = fixed4(1, 1, 1, 1);
				//groundColor = fixed4(0, 0.05, 0.3, 1);
				float3 viewDir = normalize(float3(i.viewPos, 0.25f));
				float3 normal = normalize(float3(uvOffset, 1));
				float3 lightReflectionDir = reflect(lightDir, normal);
				float viewDotNormal = dot(normal, viewDir);
				float fresnel = calcFresnel(viewDotNormal);
				float4 color = lerp(groundColor, skyColor, fresnel);
				color += pow(saturate(dot(lightReflectionDir, viewDir)), 100);
				return color;
				//rturn float4(0, 0, fmod(i.world.x, 2) / 2, 1); 
				//return float4(0, 0, fmod(abs(i.view.x), 1), 1);
            }

			
            ENDCG
        }
    }
}
