Shader "Unlit/Plot3D_Channel"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Transparent+1" }
		LOD 100
	
		ZTest Off
		Cull Off

		Pass
		{ 
		    
		Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work

			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
			
				float4 vertex : SV_POSITION;
				float4 color : TEXCOORD1;

                float4 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;

                float3 dataBoundsMin = float3(-1, -1, -1);
                float3 dataBoundsMax = float3(1, 1, 1);

                float3 dataBoundsSize = dataBoundsMax - dataBoundsMin;



                //v.vertex.xyz = v.vertex.xyz / 2;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;

                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			


                clip(i.worldPos.x + 5.0f);

				return i.color;
			}
			ENDCG
		}
	}
}
