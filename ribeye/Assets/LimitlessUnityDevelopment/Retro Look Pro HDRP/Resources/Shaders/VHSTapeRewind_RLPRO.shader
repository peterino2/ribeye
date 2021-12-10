Shader "Hidden/Shader/VHSTapeRewind_RLPRO"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    half _Fade;
    half _Intencity;

    // List of properties to control your post process effect
    float _Intensity;
    TEXTURE2D_X(_InputTexture);


		float4 Frag0(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        float2 uv = i.texcoord ;
        float2 displacementSampleUV = float2(uv.x + (_Time.x + 20.) * 75.0, uv.y) * _ScreenSize.xy;

        float da = _Intencity;

        float displacement = LOAD_TEXTURE2D_X(_InputTexture,  displacementSampleUV).x * da;

        float2 displacementDirection = float2(cos(displacement * 6.28318530718), sin(displacement * 6.28318530718));
        float2 displacedUV = (uv * _ScreenSize.xy + displacementDirection * displacement) ;
        float4 shade = LOAD_TEXTURE2D_X(_InputTexture, displacedUV);
        float4 main = LOAD_TEXTURE2D_X(_InputTexture,  uv * _ScreenSize.xy);
        return  float4(lerp(main, shade, _Fade));
	}



    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "#VHS Tape Rewind#"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment Frag0
                #pragma vertex Vert
            ENDHLSL
        }

    }
    Fallback Off
}