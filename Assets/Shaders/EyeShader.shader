Shader "Custom/Eye"{
	//show values to edit in inspector
	Properties{
		[HideInInspector] _MainTex("Texture", 2D) = "white" {}
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			// markers that specify that we don't need culling
			// or comparing/writing to the depth buffer
			Cull Off
			ZWrite On
			ZTest Always
			
			Pass 
         {
             Name "ShadowCaster"
             Tags { "LightMode" = "ShadowCaster" }
             
             Fog {Mode Off}
             ZWrite On ZTest LEqual Cull Off
             Offset 1, 1
 
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile_shadowcaster
             #include "UnityCG.cginc"
 
             struct v2f 
             { 
                 V2F_SHADOW_CASTER;
             };
 
             v2f vert( appdata_base v )
             {
                 v2f o;
                 TRANSFER_SHADOW_CASTER(o)
                 return o;
             }
 
             float4 frag( v2f i ) : SV_Target
             {
                 SHADOW_CASTER_FRAGMENT(i)
             }
             ENDCG
 
         }

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
	struct appdata {
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

v2f vert(appdata v)
{
	v2f o;
	o.vertex = v.vertex;
	o.uv = v.uv;
	o.scrPos = ComputeScreenPos(o.vertex);
	return o;
}

fixed4 frag(v2f i) : SV_Target
{
	fixed4 col = tex2D(_MainTex, i.uv);
	float depth = tex2D(_CameraDepthTexture, i.uv);
	depth = depth * _ProjectionParams.z;
	depth = length(ObjSpaceViewDir(i.vertex)) / _ProjectionParams.z;
	
	return depth;
}
			ENDCG
		}
	}
}