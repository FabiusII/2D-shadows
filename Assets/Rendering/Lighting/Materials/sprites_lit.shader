Shader "SpriteLights/sprites_lit"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_LightFactor ("Light Factor", Float) = 1.0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		    CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord1 = float2((OUT.vertex[0] / 2) + 0.5, (OUT.vertex[1] / 2) + 0.5);
				#if UNITY_UV_STARTS_AT_TOP
				OUT.texcoord1.y = 1 - OUT.texcoord1.y;
				#endif
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			sampler2D _ShadowMap;
			fixed4 _AmbientLight;
			fixed4 _SpotlightColor;
			float _LightFactor;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				fixed4 shadow = tex2D(_ShadowMap, IN.texcoord1);
				fixed4 lightColor = (shadow * _SpotlightColor * _LightFactor) + _AmbientLight;
				c.rgb *= min(lightColor, 1);
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
