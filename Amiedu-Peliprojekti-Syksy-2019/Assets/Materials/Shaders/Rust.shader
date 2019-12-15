Shader "Rust"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SecondTex("Second Texture", 2D) = "white" {}
		[PerRendererData] _Strength("_Strength", Range(0,1)) = 0
		[PerRendererData] _StartX("_StartX", Range(0,1)) = 0
		[PerRendererData] _EndX("_EndX", Range(0,1)) = 1
		[PerRendererData]_StartY("_StartY", Range(0,1)) = 0
		[PerRendererData] _EndY("_EndY", Range(0,1)) = 1
		_StencilComp("Stencil Comparison", Float) = 8
		 _Stencil("Stencil ID", Float) = 0
		 _StencilOp("Stencil Operation", Float) = 0
		 _StencilWriteMask("Stencil Write Mask", Float) = 255
		 _StencilReadMask("Stencil Read Mask", Float) = 255
		 _ColorMask("Color Mask", Float) = 15
	}
		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
			}

				Lighting Off
				ZWrite Off
				 Cull Off
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

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}
	
				sampler2D _MainTex;
				sampler2D _SecondTex;
				float _Strength;
				float _StartX, _EndX,_StartY, _EndY;

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					fixed3 secondCol = tex2D(_SecondTex, i.uv);
					float alpha = col.a * step(_StartX, i.uv.x) * step(i.uv.x, _EndX) * step(_StartY, i.uv.y) * step(i.uv.y, _EndY);
					float strength = _Strength * alpha;
					fixed4 _col = lerp(fixed4(col.rgb, col.a), fixed4(col.rgb * secondCol, alpha), strength);
					return _col;
				}
				ENDCG
			}
		}
}
