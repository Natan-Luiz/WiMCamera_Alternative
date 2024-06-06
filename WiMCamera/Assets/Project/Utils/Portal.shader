Shader "Unlit/Portal"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "" {}
        _InactiveColour("Inactive Colour", Color) = (1, 1, 1, 0)
        _multiplierValue("IPD", float) = 0.01
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100
        ZWrite OFF
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float4 eyeData : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO //Insert
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int displayMask;
            float4 _InactiveColour;

            float _multiplierValue;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                o.eyeData = float4(unity_StereoEyeIndex, 0, 0, 0);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy;
                
                uv = uv / i.screenPos.w;

                
                

               float multiplier = i.eyeData.x == 0 ?  - _multiplierValue.x : _multiplierValue.x;
               uv.x = (uv.x) + multiplier;

                fixed4 portalCol = tex2D(_MainTex, uv);

                return portalCol;// *displayMask + _InactiveColour * (1 - displayMask);
            }
            ENDCG
        }
    }
}
