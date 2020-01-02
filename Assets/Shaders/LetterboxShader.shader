Shader "Unlit/LetterboxShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", COLOR) = (0, 0, 0, 1)
		_Width ("Width", float) = 1080
		_Height ("Height", float) = 1920
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _Color;
			float4 _MainTex_ST;
			float _Width;
			float _Height;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col.x *= _Color.x;
				col.y *= _Color.y;
				col.z *= _Color.z;
				if(_Height / _Width >= 16.0 / 9.0)
				{
					float rawDeltaY = (_Height - (_Width / 9.0 * 16.0)) / 2.0;
					float clampedDeltaY = rawDeltaY / _Height;
					if(clampedDeltaY <= i.uv.y && i.uv.y <= 1 - clampedDeltaY)
					{
						col.w = 0;
					}
				}
				else
				{
					float rawDeltaX = (_Width - (_Height / 16.0 * 9.0)) / 2.0;
					float clampedDeltaX = rawDeltaX / _Width;
					if(clampedDeltaX <= i.uv.x && i.uv.x <= 1 - clampedDeltaX)
					{
						col.w = 0;
					}
				}
				return col;
			}
			ENDCG
		}
	}
}
