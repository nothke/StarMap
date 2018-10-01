Shader "Geometry/Star - Screen Aware"
{
	Properties
	{
		_SpriteTex("Base (RGB)", 2D) = "white" {}
		_BaseSize("Base Size", Range(0, 3)) = 0.0
		_Size("Size", Range(0, 3)) = 0.5
		_MaxClipSize("Maximum Screen Size", Float) = 3
	}

		SubShader
		{


			Pass
			{
				Blend One One
				ZWrite Off

				Tags
				{
				"RenderType" = "Transparent"
				"IgnoreProjector" = "True"
				}

				LOD 200

				CGPROGRAM
					#pragma target 5.0
					#pragma vertex VS_Main
					#pragma fragment FS_Main
					#pragma geometry GS_Main
					#include "UnityCG.cginc" 

			// **************************************************************
			// Data structures												*
			// **************************************************************
			struct GS_INPUT
			{
				float4	pos		: POSITION;
				float3	normal	: NORMAL;
				float2  tex0	: TEXCOORD0;
				float4	col		: COLOR;
			};

			struct FS_INPUT
			{
				float4	pos		: POSITION;
				float2  tex0	: TEXCOORD0;
				float4	col		: COLOR;
			};


			// **************************************************************
			// Vars															*
			// **************************************************************

			float _MaxClipSize;
			float _Size;
			float _BaseSize;

			//float4x4 _VP;
			Texture2D _SpriteTex;
			SamplerState sampler_SpriteTex;

			// **************************************************************
			// Shader Programs												*
			// **************************************************************

			// Vertex Shader ------------------------------------------------
			GS_INPUT VS_Main(appdata_full v)
			{
				GS_INPUT output = (GS_INPUT)0;

				output.pos = mul(unity_ObjectToWorld, v.vertex);
				output.normal = v.normal;
				output.tex0 = float2(0, 0);
				output.col = v.color;

				return output;
			}



			// Geometry Shader -----------------------------------------------------
			[maxvertexcount(4)]
			void GS_Main(point GS_INPUT p[1], inout TriangleStream<FS_INPUT> triStream)
			{
				float3 up = UNITY_MATRIX_IT_MV[1].xyz;
				float3 right = -UNITY_MATRIX_IT_MV[0].xyz;

				//float4 clipPos = UnityObjectToClipPos(p[0].pos);

				float halfS = 0.5f * (_BaseSize + _Size * p[0].col.a);

				// calculate distance in clipspace
				float3 clipA = UnityObjectToClipPos(p[0].pos - right);
				float3 clipB = UnityObjectToClipPos(p[0].pos + right);

				float clipWidth = abs(clipB - clipA);
				//float screenWidth = abs(clipB - clipA);
				//screenWidth = clamp(1, 1000000, screenWidth);
				//float w = screenWidth;
				//if (w < 5) halfS = 5;
				//halfS *= w;

				//halfS *= screenWidth * 2;

				halfS *= clamp(1, _MaxClipSize, clipWidth);

				float4 v[4];
				v[0] = float4(p[0].pos + halfS * right - halfS * up, 1.0f);
				v[1] = float4(p[0].pos + halfS * right + halfS * up, 1.0f);
				v[2] = float4(p[0].pos + -halfS * right - halfS * up, 1.0f);
				v[3] = float4(p[0].pos + -halfS * right + halfS * up, 1.0f);

				FS_INPUT pIn;

				float4 cp = UnityObjectToClipPos(p[0].pos);

				pIn.col = p[0].col;
				float dx = 1 / _ScreenParams.x * 1000 * halfS;
				float dy = 1 / _ScreenParams.y * 1000 * halfS;

				pIn.pos = cp + float4(dx,-dy,0,0);
				pIn.tex0 = float2(1.0f, 0.0f);
				triStream.Append(pIn);

				pIn.pos = cp + float4(dx, dy, 0, 0);
				pIn.tex0 = float2(1.0f, 1.0f);
				triStream.Append(pIn);

				pIn.pos = cp + float4(-dx, -dy, 0, 0);
				pIn.tex0 = float2(0.0f, 0.0f);
				triStream.Append(pIn);

				pIn.pos = cp + float4(-dx, dy, 0, 0);
				pIn.tex0 = float2(0.0f, 1.0f);
				triStream.Append(pIn);
			}



			// Fragment Shader -----------------------------------------------
			float4 FS_Main(FS_INPUT input) : COLOR
			{
				return _SpriteTex.Sample(sampler_SpriteTex, input.tex0) * input.col * 2;
			}

		ENDCG
	}
		}
}
