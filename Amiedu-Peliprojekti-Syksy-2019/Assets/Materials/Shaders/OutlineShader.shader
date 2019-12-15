Shader "OutlineShader"
{
	Properties
	{
		 _MainTex("Texture", 2D) = "white" {}
		 _Thickness("_Thickness", Range(0,5)) = 0
		 _Color("_Color", Color) = (1,1,1,1)
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
				"PreviewType" = "Plane"
			}
			Cull Off
		   Lighting Off
		   ZWrite Off
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

				float _Thickness;
				fixed4 _Color;
				sampler2D _MainTex;
				sampler2D _SecondTex;
				float4 _MainTex_TexelSize;

			   fixed4 frag(v2f i) : SV_Target
				{
			   fixed4 col = tex2D(_MainTex, i.uv);

				fixed myAlpha = col.a;
				fixed upAlpha = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y * _Thickness)).a;
				fixed downAlpha = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y * _Thickness)).a;
				fixed rightAlpha = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x * _Thickness, 0)).a;
				fixed leftAlpha = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x *  _Thickness, 0)).a;

				float alphas = ceil(clamp(downAlpha + upAlpha + leftAlpha + rightAlpha, 0, 1));
				return lerp(col, _Color, alphas - ceil(myAlpha));
				}
				ENDCG
			}
		}
}
