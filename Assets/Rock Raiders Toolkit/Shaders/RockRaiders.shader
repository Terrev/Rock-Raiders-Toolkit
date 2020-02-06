Shader "Rock Raiders"
{
	Properties
	{
		_Color ("Color (won't apply in-game if a texture is present)", Color) = (1, 1, 1, 1)
		_Diffuse ("Diffuse", Range(0, 1)) = 1
		[Toggle] _Additive("Additive Transparency (darker colors = more transparent)", Float) = 0
		_Transparency ("Transparency (solid colors only)", Range(0, 1)) = 0
		_Luminosity ("Luminosity (glow in the dark)", Range(0, 1)) = 0
		
		_MainTex ("Texture (Tiling and Offset aren't used here)", 2D) = "white" {}
		[Toggle] _Sequence("Texture is sequence", Float) = 0
		[Toggle] _SharedTexture("Write relative texture paths", Float) = 0
		// Texture flags
		[Toggle] _X("X", Float) = 0
		[Toggle] _Y("Y", Float) = 0
		[Toggle] _Z("Z", Float) = 1
		[Toggle] _PixelBlending("Pixel Blending", Float) = 0
		
		_TextureCenterX("Texture Center X", Float) = 0
		_TextureCenterY("Texture Center Y", Float) = 0
		_TextureCenterZ("Texture Center Z", Float) = 0
		
		_TextureSizeX("Texture Size X", Float) = 1
		_TextureSizeY("Texture Size Y", Float) = 1
		_TextureSizeZ("Texture Size Z", Float) = 1
	}
	SubShader
	{
		Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows

		sampler2D _MainTex;
		fixed4 _Color;
		float _Diffuse;
		float _Transparency;
		float _Luminosity;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf (Input i, inout SurfaceOutputStandard o)
		{
			fixed4 col = tex2D(_MainTex, i.uv_MainTex);
			col *= _Color;
			col *= _Diffuse;
			o.Albedo = col.rgb;
			
			float lol = -_Transparency + 1.0f;
			o.Alpha = lol;
			
			fixed3 lum = col.rgb * _Luminosity;
			lum *= lol;
			o.Emission = lum;
		}
		ENDCG
	}
}