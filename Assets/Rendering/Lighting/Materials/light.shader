Shader "SpriteLights/light"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float distance : TEXCOORD0;
			};
			
			float4 _LightPos;
			float _LightRange;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float dist = distance(_LightPos.xy, v.vertex.xy);
			    o.distance = dist / _LightRange;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
			    float squaredNormalizedDistance = pow(i.distance, 2);
				return fixed4(1,1,1,1) * saturate(1 - squaredNormalizedDistance);
			}
			ENDCG
		}
	}
}
