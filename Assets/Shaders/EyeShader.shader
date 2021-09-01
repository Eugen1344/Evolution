Shader "Custom/Eye"{
    //show values to edit in inspector
    Properties{
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
    }

    SubShader{
    	Tags { "RenderType"="Opaque" }
        // markers that specify that we don't need culling
        // or comparing/writing to the depth buffer
        Cull Off
        ZWrite On
        ZTest Always

        Pass{
            CGPROGRAM
            //include useful shader functions
            #include "UnityCG.cginc"

            //define vertex and fragment shader
            #pragma vertex vert
            #pragma fragment frag

            //the rendered screen so far
            sampler2D _MainTex;


            //the object data that's put into the vertex shader
            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            //the vertex shader
           sampler2D _CameraDepthTexture;
 
struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float4 scrPos : TEXCOORD1;
};
 
v2f vert (appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.scrPos = ComputeScreenPos(o.vertex);
    return o;
}
 
fixed4 frag (v2f i) : SV_Target
{
    fixed4 col = tex2D(_MainTex, i.uv);
    float depth = (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));
    depth = Linear01Depth(depth);
    return depth;
}
            ENDCG
        }
    }
}