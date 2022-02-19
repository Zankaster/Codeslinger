Shader "Custom/Distortion"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}

	}

	SubShader
	{
		Pass
		{

		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#include "UnityCG.cginc"
		#define CURVATURE
		#pragma target 3.0

		uniform sampler2D _MainTex;
		uniform float _Distortion; // 0.1f

		float2 RadialDistortion(float2 coord)
		{
			float2 cc = coord - 0.5f;
			float dist = dot(cc, cc) * _Distortion;
			return (coord + cc * (0.5f + dist) * dist);
		}

		float4 frag(v2f_img i) : COLOR
		{
			float2 xy = RadialDistortion(i.uv);
			float4 col = tex2D(_MainTex, xy);
			return float4(col.rgb, 1.0f);
		}

		ENDCG
		}
	}
}