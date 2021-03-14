Shader "RSLib/Sprite Blink"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" { }
		_Color ("Tint", Color) = (1, 1, 1, 1)
		_BlinkColor ("Blink Color", Color) = (1, 1, 1, 0)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1, 1, 1, 1)
		[HideInInspector] _Flip("Flip", Vector) = (1, 1, 1, 1)
		[HideInInspector] _AlphaTex("External Alpha", 2D) = "white" { }
		[HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

			#pragma vertex SpriteVert
			#pragma fragment BlinkSpriteFrag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			fixed4 _BlinkColor;

			fixed4 BlinkSpriteFrag (v2f IN) : SV_Target
			{
				fixed4 texel = SampleSpriteTexture (IN.texcoord);
				fixed4 c = texel * IN.color * (1 - _BlinkColor.a) + _BlinkColor * _BlinkColor.a * texel.a;
				c *= _Color.a;
				c.rgb *= c.a;
				return c;
			}

			ENDCG
		}
	}
}