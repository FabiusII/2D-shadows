// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "SpriteLights/shadows"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Off
		Lighting Off
		ZWrite On
		ZTest Always
		Pass
		{
			Stencil
			{
				Ref 1
				Comp NotEqual
				Pass DecrSat
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

            struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord: TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
            
            float4 _LightPos;
            fixed4 _ShadowColor;

			v2f vert (appdata v)
			{
				v2f o;
                
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float4 fromLightDirection = (worldPos - _LightPos);
                fromLightDirection = float4(fromLightDirection[0], fromLightDirection[1], 0, 0);
                
                //TODO constant offset. normalize with light range instead
                worldPos += v.texcoord[0] * fromLightDirection * 999999;
                
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				return _ShadowColor;
			}
			ENDCG
		}
	}
}
