Shader "Jormungandr/SimplestCutout" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Geometry" }
		AlphaTest Greater 0.5
		LOD 200
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert	
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct v2f
			{
				float4 position : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_full v) 
			{
				v2f o;

				o.position = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.uv = v.texcoord;

				return o;
			}

			half4 frag(v2f IN) : COLOR
			{
				return tex2D(_MainTex, IN.uv) * IN.color;
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
