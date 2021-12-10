Shader "Hidden/Shader/VHSPro_pass1"{

   HLSLINCLUDE

   #pragma target 4.5
   #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

   #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
   #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
   #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
   #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
   #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"


   struct Attributes {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
   };

   struct Varyings {
      float4 positionCS : SV_POSITION;
      float2 texcoord   : TEXCOORD0;
      // //test
      // float2 texcoord1   : TEXCOORD1;
      // float2 texcoord2   : TEXCOORD2;
      UNITY_VERTEX_OUTPUT_STEREO
   };


   Varyings Vert(Attributes input) {
      Varyings output;
      UNITY_SETUP_INSTANCE_ID(input);
      UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
      output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
      output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
      // output.texcoord1 = GetFullScreenTriangleTexCoord(input.vertexID);
      // output.texcoord2 = GetFullScreenTriangleTexCoord(input.vertexID);
      return output;
   }

   // List of properties to control your post process effect
   TEXTURE2D_X(_InputTexture);

   float    _time;
   float4   _ResOg;  // before pixelation (.xy resolution, .zw one pixel )
   float4   _Res;    // after pixelation  (.xy resolution, .zw one pixel )
   float4   _ResN;   // noise resolution  (.xy resolution, .zw one pixel )


   //functions from graphics adapter pro
   //(used in both .hlsl)
   int _colorOn;            //VHS_COLOR 
   int _paletteOn;          //VHS_PALETTE
   int _ditherOn;           //VHS_DITHER              
   int _signalTweakOn;      //VHS_SIGNAL_TWEAK_ON
   TEXTURE2D_X(_PaletteTex);
   #include "gap.hlsl"  


   //function for vhs distortion    
   TEXTURE2D_X(_TapeTex);   
   #include "vhs_pass1.hlsl" 


   float4 CustomPostProcess(Varyings input) : SV_Target {

      UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

      float3 outColor = vhs(input);    
      return float4(outColor, 1);
      
   }

   ENDHLSL

   SubShader {

      Pass {
            Name "VHSPro"

         ZWrite Off
         ZTest Always
         Blend Off
         Cull Off

         HLSLPROGRAM
             #pragma fragment CustomPostProcess
             #pragma vertex Vert
         ENDHLSL
      }
   }
   Fallback Off
}
