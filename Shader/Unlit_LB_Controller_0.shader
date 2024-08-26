Shader "Unlit/LB_Controller" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_AlbedoTint ("Abledo Tint", Vector) = (1,1,1,1)
		_MaskTex ("Mask Texture", 2D) = "black" {}
		_UnderMaskColor ("Under Mask Color", Vector) = (0,1,0,1)
		_ButtonIndex ("Button Index", Float) = -1
		[Header(MotionVectors)] [Toggle(DisableMotionVectors)] _DisableMotionVectors ("Disable Motion Vectors", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}