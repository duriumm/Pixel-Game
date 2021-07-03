Shader "Unlit/NewUnlitShader"
{
    Properties
    {
		[Toggle] _UseTileMap("Ue tilemap", float) = 0
		[NoScaleOffset]_MainTex("TileMap", 2D) = "white" {}
		_Sky("Sky", 2D) = "white" {}
		[Toggle]_UseSkyPlane("Use sky plane", float) = 1
		[NoScaleOffset]_SkyCube("Sky cube", Cube) = "" {}
		[Color]_SkyTint("Sky tint", color) = (1, 1, 1, 1)
		_FogDensity("Fog density", range(0, 1)) = 0
		[Color]_FogCol("Fog color", color) = (1, 1, 1, 1)
		_Ground("Ground", 2D) = "white" {}
		[NoScaleOffset]_GroundNormalMap("GroundNormalMap", 2D) = "black" {}
		_GroundScaleX("Ground scale X", float) = 1
		_GroundScaleY("Ground scale Y", float) = 1
		[Color]_GroundTint("Ground tint", color) = (1, 1, 1, 1)
		_Caustics("Caustics", 2DArray) = "black" {}
		_CausticsContrast("Caustics contrast", float) = 1
		_CausticsBrightness("Caustics brightness", float) = 1
		_GroundParallax("Ground parallax", range(0, 1)) = 0.9
		[Space]
		_WaveAmplitude("Wave amplitude", float) = 0.5
		_WaveFrequency("Wave frequency", float) = 1
		_WaveStretch("Wave stretch", float) = 2
		_WaveFbmGain("Wave fBm gain", float) = 0.3
		_WaveFbmReso("Wave fBm resolution", range(0, 1)) = 1
		_WaveScroll("Wave Scroll", vector) = (0, 0.4, 0.1, 1)
		_LightDistortionFromWaves("Light distortion from waves", float) = 0.05
		[Space]
		_WaterOpacity("Water opacity", Range(0, 1)) = 0.5
		[Color]_ShallowWaterCol("Shallow water color", color) = (0, 0.1, 0.35, 1)
		[Color]_DeepWaterCol("Deep water color", color) = (0, 0.2, 0.13, 1)
		[Toggle]_SwapColors("Swap colors", float) = 0
		_SpecExp("Specular exponent", float) = 100
		_SpecScale("Speculare scale", float) = 1
		[Color]_SpecCol("Specular color", color) = (1, 1, 1, 1)
		_AmbientAmount("Ambient amount", float) = 0.4
		_DirToSun("Direction to sun", vector) = (1, 1, 0.6)
		[Toggle]_EnableSky("Enable sky", float) = 1
		[Toggle]_EnableGround("Enable ground", float) = 1
		[Space]
		_FresnelExp("Fresnel exponent", float) = 5
		_FresnelScale("Fresnel scale", float) = 1
		_FresnelOffset("Fresnel offset", float) = 0
		_CameraHeightScale("Camera height scale", float ) = 1
		[Enum(Standard, 0, Normals, 1, NoiseDerivatives, 2, DiffuseLighting, 3, Heightmap, 4)]
		_RenderMode("Render mode", float) = 0
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma require 2darray

			#include "UnityCG.cginc"
			#include "Waves.cginc"

			static const float RenderMode_Normals = 1;
			static const float RenderMode_Deriv = 2;
			static const float RenderMode_Diffuse = 3;
			static const float RenderMode_Heightmap = 4;

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
				float3 viewPos : TEXCOORD2;
				float2 skyUV : TEXCOORD3;
				float2 groundUV : TEXCOORD4;
				float2 causticsUV : TEXCOORD5;
            };

			float _UseTileMap;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Sky;
			float _UseSkyPlane;
			samplerCUBE _SkyCube;
			float4 _Sky_ST;
			float4 _SkyTint;
			float4 _FogCol;
			float _FogDensity;
			sampler2D _Ground;
			float4 _Ground_ST;
			sampler2D _GroundNormalMap;
			float4 _GroundTint;
			UNITY_DECLARE_TEX2DARRAY(_Caustics);
			float4 _Caustics_ST;
			float _CausticsContrast;
			float _CausticsBrightness;
			float _GroundParallax;
			
			float4 _WaveScroll;
			float _WaveAmplitude;
			float _WaveFrequency;
			float _WaveStretch;
			float _WaveFbmGain;
			float _WaveFbmReso;
			float _LightDistortionFromWaves;

			float4 _ShallowWaterCol;
			float4 _DeepWaterCol;
			float _SwapColors;
			float _WaterOpacity;
			float _SpecExp;
			float _SpecScale;
			float4 _SpecCol;
			float _AmbientAmount;
			float3 _DirToSun;
			float _FresnelExp;
			float _FresnelScale;
			float _FresnelOffset;
			float _CameraHeightScale;

			float _RenderMode;
			float _EnableSky;
			float _EnableGround;
			
			// Calculates factor used to blend between ground and sky depending on difference between viewing angle and water surface angle
			// Reference: Shader Based Water Effects Whitepaper by Imagination Technologies Limited
			// http://cdn.imgtec.com/sdk-documentation/Shader+Based+Water+Effects.Whitepaper.pdf
			float calcFresnel(float cosAngle, bool underWater = false)
			{
				float fresnel = 1 - cosAngle;

				// We probably won't need to render the water surface from below
				// but I'll leave it here just in case
				if (underWater)
					fresnel *= 2.5f;

				fresnel = pow(fresnel, _FresnelExp);
				return min((fresnel * 0.97963f * _FresnelScale) + 0.02037f + _FresnelOffset, 1);
			}

			//Vertex shader
            VertexOutput vert(VertexInput input)
            {
				VertexOutput output;
				//Transform to clip space
				output.pos = UnityObjectToClipPos(input.pos);
				output.tileMapUV = TRANSFORM_TEX(input.uv, _MainTex);
				
				//Get position in world space
				//This can be used to create custom sized tiling that works better than using input.uv
				output.worldPos = mul(unity_ObjectToWorld, input.pos);
				//Get position in view space (relative to camera)
				output.viewPos = UnityObjectToViewPos(input.pos);
				output.viewPos.z *= _CameraHeightScale;
							
				//Stretch sky texture from top-left to bottom-right of screen
				output.skyUV = output.pos * 0.5f + 0.5f;
				
				output.skyUV *= _Sky_ST.xy; //User-specified scale
				output.skyUV += 0.5f - 0.5 * _Sky_ST.xy; //Center texture after scale
				output.skyUV += _Sky_ST.zw; //User-specified offset
				
				output.groundUV = (output.worldPos - _WorldSpaceCameraPos * _GroundParallax) * _Ground_ST.xy + _Ground_ST.zw;
				output.causticsUV = (output.worldPos - _WorldSpaceCameraPos * _GroundParallax) * _Caustics_ST.xy + _Caustics_ST.zw;
				return output;
            }

			//Fragment shader
			fixed4 frag(VertexOutput input) : SV_Target
			{
				float3 dirToSun = normalize(_DirToSun);
				
				float4 waves = calcWaves(
					input.worldPos,
					_Time.y,
					_WaveScroll.xyz * _WaveScroll.w,
					_WaveAmplitude,
					_WaveFrequency,
					_WaveStretch,
					_WaveFbmGain,
					_WaveFbmReso
				);
				//Convert noise derivative to normal vector
				float3 waterNormal = normalize(float3(-waves.xy, 1));
				
				float2 uvOffset = waterNormal.xy * _LightDistortionFromWaves;
								
				input.viewPos *= -1;
				input.viewPos.z += waves.w;
				float3 viewDir = normalize(input.viewPos);
				float viewDotNormal = dot(waterNormal, viewDir); //Angle between water surface and viewer

				//Blend between two water colors to simulate subsurface scattering as suggested by Inigo Quilez
				float waterColBlend = _SwapColors ? 1 - viewDotNormal : viewDotNormal;
				float4 waterCol = lerp(_ShallowWaterCol, _DeepWaterCol, waterColBlend);
				
				float diffuseLight = dot(waterNormal, dirToSun);
				float diffuseAmbientLight = min(diffuseLight + _AmbientAmount, 1);
				diffuseAmbientLight = max(diffuseAmbientLight, _AmbientAmount);
				//Modulate water color with diffuse + ambient lighting
				waterCol *= diffuseAmbientLight;

				//Sample tile texture
				if (_UseTileMap == 1)
				{
					fixed4 tileCol = tex2D(_MainTex, input.tileMapUV + uvOffset);
					waterCol *= tileCol;
				}
				//Various render modes which can help when tweaking parameters and debugging
				switch (_RenderMode)
				{
				case RenderMode_Normals:
					return float4(waterNormal * 0.5f + 0.5f, 1);
				case RenderMode_Deriv:
					return saturate(float4(waves.xyz, 1));
				case RenderMode_Diffuse:
					return waterCol;
				case RenderMode_Heightmap:
					return waves.w * 0.5f + 0.5f;
				}

				float4 result;

				if (_EnableGround == 1) //"Enable ground" checkbox is checked in inspector
				{
					float2 groundUV = input.groundUV + uvOffset;
					float4 groundCol = tex2D(_Ground, groundUV);
					float3 groundNormal = tex2D(_GroundNormalMap, groundUV).xyz;
					if (any(groundNormal)) //If there is a normalmap
						groundNormal = groundNormal * 2 - 1;
					else
						groundNormal = float3(0, 0, 1); //Use default normal pointing upwards
					float sunlight = saturate(dot(dirToSun, normalize(groundNormal)) + 0.3f); //Diffuse + ambient lighting
					groundCol *= sunlight * _GroundTint;
					
					if (_CausticsBrightness > 0)
					{
						//Sample caustics texture frame
						float causticsCol = UNITY_SAMPLE_TEX2DARRAY(_Caustics, float3(input.causticsUV + uvOffset, fmod(_Time.y * 16, 16))).x;
						//Apply user-specified brightness
						causticsCol = pow(causticsCol, _CausticsContrast);
						//Apply user-specified contrast
						causticsCol *= _CausticsBrightness;
						//Apply final caustics color to ground
						groundCol = saturate(groundCol + causticsCol);
					}
					//Blend between ground and water color
					result = lerp(groundCol, waterCol, _WaterOpacity);
				}
				else
					result = waterCol;
						
				if (_EnableSky == 1) //"Enable sky" checkbox is checked in inspector
				{
					//waterNormal = float3(0, 0, 1);
					float3 envMapReflectionVector = reflect(viewDir.xzy, waterNormal);
					float4 skyCol;
					if (_UseSkyPlane == 0)
						skyCol = texCUBE(_SkyCube, envMapReflectionVector);
					else
						skyCol = tex2D(_Sky, input.skyUV + uvOffset);
					skyCol *= _SkyTint;
					skyCol = lerp(skyCol, _FogCol, _FogDensity);
					float fresnel = calcFresnel(viewDotNormal);
					result = lerp(result, skyCol, fresnel);
				}
				float3 lightReflectionDir = reflect(-dirToSun, waterNormal);
				//Specular lighting
				result += pow(max(dot(lightReflectionDir, viewDir), 0), _SpecExp) * _SpecCol * _SpecScale;
				return result;
            }
            ENDCG
        }
    }
}
