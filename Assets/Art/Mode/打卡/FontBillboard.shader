Shader "Hidden/FontBillboard"
{
    Properties
    {
        //[PerRendererData]
        _MainTex ("Texture", 2D) = "white" {}
        [MaterialToggle]_Verical("Vercial",Range(0,1))=1
        _Flip("Flip",Range(-1,1))=1
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Cull Off ZWrite Off ZTest Always
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityUI.cginc"
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color    : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color    : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO 
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed _Verical;
            fixed4 _Color;
            fixed _Flip;
            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v); 
                UNITY_INITIALIZE_OUTPUT(v2f, o); 
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); 

				float3 center = float3(0,0,0);
				//�ӽǷ���������������ȥ����ĵ�
				float3 view = mul(unity_WorldToObject,float4(_WorldSpaceCameraPos,1));
				float3 normalDir = view - center;
				//���淨�ߵı仯�����_Verical=1����Ϊ���淨�ߣ�����Ϊ���Ϸ���
				normalDir.y = normalDir.y*_Verical;
				//��һ��
				normalDir = normalize(normalDir);
				float3 upDir = abs(normalDir.y) > 0.999 ? float3(0, 0, 1) : float3(0, 1, 0);
				//���  cross(A,B)����������Ԫ�����Ĳ��(cross product)��ע�⣬���������������Ԫ����
				float3 rightDir = normalize(cross(upDir,normalDir));
				upDir = normalize(cross(normalDir, rightDir));
				//�������ĵ�ƫ��
				float3 centerOffs = v.vertex.xyz - center;
				//λ�õı任
				float3 localPos = center + rightDir * centerOffs.x * _Flip + upDir * centerOffs.y + normalDir * centerOffs.z;
                o.vertex = UnityObjectToClipPos(localPos);
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                #ifdef UNITY_UI_ALPHACLIP
                clip (col.a - 0.001);
                #endif
                col.rgb += i.color * col.a;
                return col;
            }
            ENDCG
        }
    }
}
