Shader "Cg texturing with alpha blending" {
    Properties{
       _MainTex("RGBA Texture Image", 2D) = "white" {}
    _ScrollSpeeds("Scroll Speeds", vector) = (-5, 0, 0, 0)
        _StencilComp("Stencil Comparison", Float) = 8
         _Stencil("Stencil ID", Float) = 0
         _StencilOp("Stencil Operation", Float) = 0
         _StencilWriteMask("Stencil Write Mask", Float) = 255
         _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.2
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }
        SubShader{
           Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        Stencil
        {
            Ref[_Stencil]
            Comp equal
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
           Pass {
              Cull Front // first render the back faces
              ZWrite Off // don't write to depth buffer 
        // in order not to occlude other objects
     Blend SrcAlpha OneMinusSrcAlpha
        // blend based on the fragment's alpha value
         
     CGPROGRAM

     #pragma vertex vert  
     #pragma fragment frag 
     #pragma multi_compile_fog
     #include "UnityCG.cginc"
     uniform sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _ScrollSpeeds;

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float2 uv : TEXCOORD0;
        UNITY_FOG_COORDS(1)
            float4 vertex : SV_POSITION;
    };

     v2f vert(appdata input)
     {
        v2f output;

        output.vertex = UnityObjectToClipPos(input.vertex);
        output.uv = TRANSFORM_TEX(input.uv, _MainTex);

        output.uv += _ScrollSpeeds * _Time.x;

        UNITY_TRANSFER_FOG(output, output.vertex)

        return output;
     }

     fixed4 frag(v2f i) : SV_Target
     {
         // sample the texture
         fixed4 col = tex2D(_MainTex, i.uv);
     col.a *= 0.25;
     // apply fog
     UNITY_APPLY_FOG(i.fogCoord, col);
     return col;
     }

     ENDCG
  }

  Pass {
     Cull Back // now render the front faces
     ZWrite Off // don't write to depth buffer 
         // in order not to occlude other objects
      Blend SrcAlpha OneMinusSrcAlpha
         // blend based on the fragment's alpha value

      CGPROGRAM

      #pragma vertex vert  
     #pragma fragment frag 
     #pragma multi_compile_fog
     #include "UnityCG.cginc"
     uniform sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _ScrollSpeeds;

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float2 uv : TEXCOORD0;
        UNITY_FOG_COORDS(1)
            float4 vertex : SV_POSITION;
    };

     v2f vert(appdata input)
     {
        v2f output;

        output.vertex = UnityObjectToClipPos(input.vertex);
        output.uv = TRANSFORM_TEX(input.uv, _MainTex);

        output.uv += _ScrollSpeeds * _Time.x;

        UNITY_TRANSFER_FOG(output, output.vertex)

        return output;
     }

     fixed4 frag(v2f i) : SV_Target
     {
         // sample the texture
         fixed4 col = tex2D(_MainTex, i.uv);
     col.a *= 0.25;
     // apply fog
     UNITY_APPLY_FOG(i.fogCoord, col);
     return col;
     }

     ENDCG
   }
    }
        Fallback "Unlit/Transparent"
}