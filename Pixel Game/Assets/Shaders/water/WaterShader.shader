Shader "Unlit/NewUnlitShader"
{
    Properties
    {
		[NoScaleOffset]_MainTex("TileMap", 2D) = "white" {}
		[Toggle]_EnableSky("Enable sky", float) = 1
		[NoScaleOffset]_Sky("Sky", 2D) = "white" {}
		_SkyScaleX("Sky scale X", float) = 1
		_SkyScaleY("Sky scale Y", float) = 1
		_SkyOffsetX("Sky offset X", float) = 0
		_SkyOffsetY("Sky offset Y", float) = 0
		[Color]_SkyTint("Sky tint", color) = (1, 1, 1, 1)
		_FogDensity("Fog density", range(0, 1)) = 0
		[Color]_FogCol("Fog color", color) = (1, 1, 1, 1)
		
		[Toggle]_EnableGround("Enable ground", float) = 1
		[NoScaleOffset]_Ground("Ground", 2D) = "white" {}
		[NoScaleOffset]_GroundNormalMap("GroundNormalMap", 2D) = "black" {}
		_GroundScaleX("Ground scale X", float) = 1
		_GroundScaleY("Ground scale Y", float) = 1
		[Color]_GroundTint("Ground tint", color) = (1, 1, 1, 1)
		_Coustics("Caustics", 2DArray) = "white" {}
		_GroundParallax("Ground parallax", range(0, 1)) = 0.9
		[Space]
		_WaveAmplitude("Wave amplitude", float) = 0.5
		_WaveFrequency("Wave frequency", float) = 1
		_WaveStretch("Wave stretch", float) = 2
		_WaveFbmGain("Wave fBm gain", float) = 0.3
		_WaveFbmIterations("Wave fBm iterations", range(-1, 15)) = -1
		_WaveScroll("Wave Scroll", vector) = (0, 0.4, 0.1, 1)
		_LightDistortionFromWaves("Light distortion from waves", float) = 0.05
		[Space]
		_WaterOpacity("Water opacity", float) = 0.5
		[Color]_WaterCol("Water color", color) = (0, 0.1, 0.35, 1)
		[Space]
		_CameraHeight("Camera height", float) = 0.25
		_SpecPow("Specular power", float) = 100
		[Color]_SpecCol("Specular color", color) = (1, 1, 1, 1)
		_DirToSun("Direction to sun", vector) = (1, 1, 0.6)

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
				float2 viewPos : TEXCOORD2;
				float2 skyUV : TEXCOORD3;
            };

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _EnableSky;
			sampler2D _Sky;
			float _SkyOffsetX;
			float _SkyOffsetY;
			float _SkyScaleX;
			float _SkyScaleY;
			float4 _SkyTint;
			float4 _FogCol;
			float _FogDensity;
			float _EnableGround;
			sampler2D _Ground;
			sampler2D _GroundNormalMap;
			float _GroundScaleX;
			float _GroundScaleY;
			float4 _GroundTint;
			float _GroundParallax;
			
			float4 _WaveScroll;
			float _WaveAmplitude;
			float _WaveFrequency;
			float _WaveStretch;
			float _WaveFbmGain;
			float _WaveFbmIterations;
			float _LightDistortionFromWaves;

			float4 _WaterCol;
			float _WaterOpacity;
			float _CameraHeight;
			float _SpecPow;
			float4 _SpecCol;
			float3 _DirToSun;

			float _RenderMode;

			//Calculates factor to blend between ground and sky depending on difference between viewing angle and water surface angle
			float calcFresnel(float cosAngle, bool underWater = false)
			{
				float fresnel = 1 - cosAngle;

				// We probably won't need to render the water surface from below
				// but I'll leave it here just in case
				if (underWater)
					fresnel *= 2.5f;

				fresnel = pow(fresnel, 4);

				// scale and bias to fit real fresnel curve
				return min((fresnel * 0.95f) + 0.05f, 1);
			}

			//Used to mirror sky when texture coords are out of bounds
			float2 mirrorUV(float2 uv)
			{
				if (uv.x > 1)
					uv.x = 2 - uv.x;
				if (uv.y > 1)
					uv.y = 2 - uv.y;
				if (uv.x < 0)
					uv.x *= -1;
				if (uv.y < 0)
					uv.y *= -1;
				return uv;
			}

			//Vertex shader
            VertexOutput vert(VertexInput input)
            {
				VertexOutput output;
				//Transform to clip space
				output.pos = UnityObjectToClipPos(input.pos);
				//Get coords for tile texture (will probably not be needed)
				//output.tileMapUV = TRANSFORM_TEX(input.uv, _MainTex);
				
				//Get position in world space
				//This can be used together with fmod to create custom sized tiling
				output.worldPos = mul(unity_ObjectToWorld, input.pos);
				//Get position in view space (relative to camera)
				output.viewPos = UnityObjectToClipPos(input.pos);
				
				//Stretch sky texture from top-left to bottom-right of screen
				output.skyUV = output.viewPos * 0.5f + 0.5f; 
				
				float2 skyScale = float2(_SkyScaleX, _SkyScaleY);
				output.skyUV /= skyScale; //User-specified scale
				output.skyUV += (skyScale - 1) * 0.5f / skyScale; //Center texture after scale
				output.skyUV -= float2(_SkyOffsetX, _SkyOffsetY); //User-specified offset
				
				return output;
            }

			//Fragment shader
			fixed4 frag(VertexOutput input) : SV_Target
			{
				float3 dirToSun = normalize(_DirToSun);
				//Sample tile texture
				//fixed4 tileCol = tex2D(_MainTex, input.tileMapUV);
				
				float4 waves = calcWaves(
					input.worldPos,
					_Time.y,
					_WaveScroll.xyz * _WaveScroll.w,
					_WaveAmplitude,
					_WaveFrequency,
					_WaveStretch,
					_WaveFbmGain,
					_WaveFbmIterations
				);
				//Convert noise derivative to normal vector
				float3 waterNormal = normalize(float3(-waves.xy, 1));
				
				//Various render modes which can help when tweaking parameters and debugging
				switch (_RenderMode)
				{
				case RenderMode_Normals:
					return float4(saturate(waterNormal), 1);
				case RenderMode_Deriv:
					return saturate(float4(waves.xyz, 1));
				case RenderMode_Diffuse:
					return saturate(_WaterCol * dot(waterNormal, dirToSun) + _WaterCol * 1);
				case RenderMode_Heightmap:
					return waves.w * 0.5f + 0.5f;
				}

				float2 uvOffset = waterNormal.xy * _LightDistortionFromWaves;
				float4 result;

				if (_EnableGround == 1) //"Enable ground" box is checked in inspector
				{
					//Calculate tiled tex coords for ground
					float2 groundUV = fmod(abs(input.worldPos - _WorldSpaceCameraPos * _GroundParallax) / float2(_GroundScaleX, _GroundScaleY), 1) - uvOffset;
					float4 groundCol = tex2D(_Ground, groundUV);
					float3 groundNormal = tex2D(_GroundNormalMap, groundUV).xyz;
					if (any(groundNormal)) //If there is a normalmap
						groundNormal = groundNormal * 2 - 1;
					else
						groundNormal = float3(0, 0, 1); //Use default normal pointing upwards
					float sunlight = saturate(dot(dirToSun, normalize(groundNormal)) + 0.3f); //Diffuse + ambient lighting
					groundCol *= sunlight * _GroundTint;
					//Blend between ground and water color
					result = lerp(groundCol, _WaterCol, _WaterOpacity); 
				}
				else
					result = _WaterCol;
						
				float3 viewDir = normalize(float3(input.viewPos, _CameraHeight));
				float viewDotNormal = dot(waterNormal, viewDir); //Angle between water surface and viewer
				if (_EnableSky == 1) //"Enable sky" box is checked in inspector
				{
					float2 skyUV = mirrorUV(input.skyUV);
					float4 skyCol = tex2D(_Sky, skyUV + uvOffset) * _SkyTint;
					skyCol = lerp(skyCol, _FogCol, _FogDensity);
					float fresnel = calcFresnel(viewDotNormal);
					result = lerp(result, skyCol, fresnel);
				}
				float3 lightReflectionDir = reflect(-dirToSun, waterNormal);
				//Specular lighting
				result += pow(saturate(dot(lightReflectionDir, viewDir)), _SpecPow) * _SpecCol;
				return result;
            }
            ENDCG
        }
    }
}
