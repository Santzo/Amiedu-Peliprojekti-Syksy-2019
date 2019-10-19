Shader "Sprites/Distortion"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SecondTex("Second Texture", 2D) = "white" {}
		_Color("Color", color) = (1,1,1,1)
		_MagnitudeX("MagnitudeX", Range(0,0.2)) = 0
		_MagnitudeY("MagnitudeY", Range(0,0.2)) = 0
	}

	SubShader
	{
		Tags
		{
			"PreviewType" = "Plane"
			"Queue" = "Transparent"
		}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 _Color;
			float _MagnitudeX;
			float _MagnitudeY;
			sampler2D _MainTex;
			sampler2D _SecondTex;

			float4 frag(v2f i) : SV_Target
			{
				float2 distuv = float2(i.uv.x + _Time.x, i.uv.y + _Time.x);

				float2 disp = tex2D(_SecondTex, distuv).xy;
				disp.x = ((disp.x * 2) - 1) * _MagnitudeX;
				disp.y = ((disp.y * 2) - 1) * _MagnitudeY;
				float4 color1 = tex2D(_MainTex, i.uv + disp);

				return color1;
			}
			ENDCG
		}
	}
}