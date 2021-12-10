Shader "Hidden/Shader/LowResolution_RLPRO"
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

	TEXTURE2D_X(_InputTexture);
	TEXTURE2D_X(_InputTexture2);
	TEXTURE2D_X(_InputTexture3);
	TEXTURE2D(_Mask);
	SAMPLER(sampler_Mask);
	float _FadeMultiplier;
	#pragma shader_feature ALPHA_CHANNEL

	half downsample;
    float _Intensity;

	float4 Frag5(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float2 pos = i.texcoord * _ScreenSize.xy;
		float texelHeightOffset =1.4;
		float	texelWidthOffset = 1.4;
		float2 firstOffset = float2(texelWidthOffset, texelHeightOffset);
		float2 secondOffset = float2(2.0 * texelWidthOffset, 2.0 * texelHeightOffset);
		float2 thirdOffset = float2(3.0 * texelWidthOffset, 3.0 * texelHeightOffset);
		float2 fourthOffset = float2(4.0 * texelWidthOffset, 4.0 * texelHeightOffset);

		float2 centerTextureCoordinate = pos;
		float2 oneStepLeftTextureCoordinate = pos - firstOffset;
		float2 twoStepsLeftTextureCoordinate = pos - secondOffset;
		float2 threeStepsLeftTextureCoordinate = pos - thirdOffset;
		float2 fourStepsLeftTextureCoordinate = pos - fourthOffset;
		float2 oneStepRightTextureCoordinate = pos + firstOffset;
		float2 twoStepsRightTextureCoordinate = pos + secondOffset;
		float2 threeStepsRightTextureCoordinate = pos + thirdOffset;
		float2 fourStepsRightTextureCoordinate = pos + fourthOffset;
		float4 fragmentColor = LOAD_TEXTURE2D_X(_InputTexture3, centerTextureCoordinate) * 0.38026;

		fragmentColor += LOAD_TEXTURE2D_X(_InputTexture3, oneStepLeftTextureCoordinate) * 0.27667;
		fragmentColor += LOAD_TEXTURE2D_X(_InputTexture3, oneStepRightTextureCoordinate) * 0.27667;

		fragmentColor += LOAD_TEXTURE2D_X(_InputTexture3, twoStepsLeftTextureCoordinate) * 0.08074;
		fragmentColor += LOAD_TEXTURE2D_X(_InputTexture3, twoStepsRightTextureCoordinate) * 0.08074;

		fragmentColor += LOAD_TEXTURE2D_X(_InputTexture3, threeStepsLeftTextureCoordinate) * -0.02612;
		fragmentColor += LOAD_TEXTURE2D_X(_InputTexture3, threeStepsRightTextureCoordinate) * -0.02612;

		fragmentColor += LOAD_TEXTURE2D_X(_InputTexture3, fourStepsLeftTextureCoordinate) * -0.02143;
		fragmentColor += LOAD_TEXTURE2D_X(_InputTexture3, fourStepsRightTextureCoordinate) * -0.02143;

		half4 colIn = LOAD_TEXTURE2D_X(_InputTexture3, pos);
		float fade = 1;
		if (_FadeMultiplier > 0)
		{
			#if ALPHA_CHANNEL
			float alpha_Mask = step(0.0001, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
			#else
			float alpha_Mask = step(0.0001, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
			#endif
			fade *= alpha_Mask;
		}

		return lerp(colIn,fragmentColor,fade);
	}
	float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 col = LOAD_TEXTURE2D_X(_InputTexture,i.texcoord * _ScreenSize.xy);
		return col;
	}
	float4 Frag2(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 col = LOAD_TEXTURE2D_X(_InputTexture2,i.texcoord * _ScreenSize.xy*downsample);
		half4 colIn = LOAD_TEXTURE2D_X(_InputTexture, i.texcoord * _ScreenSize.xy);
		float fade = 1;
		if (_FadeMultiplier > 0)
		{
#if ALPHA_CHANNEL
			float alpha_Mask = step(0.0001, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
#else
			float alpha_Mask = step(0.0001, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
#endif
			fade *= alpha_Mask;
		}
		return lerp(colIn, col, fade);
	}

    ENDHLSL

    SubShader
    {
			Pass
		{
			Name "#Blit#"

			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			HLSLPROGRAM
				#pragma fragment Frag
				#pragma vertex Vert
			ENDHLSL
		}
			Pass
		{
			Name "#UpSampleBlit#"

			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			HLSLPROGRAM
				#pragma fragment Frag2
				#pragma vertex Vert
			ENDHLSL
		}
			Pass
		{
			Name "#CatMul#"

			ZWrite Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			HLSLPROGRAM
				#pragma fragment Frag5
				#pragma vertex Vert
			ENDHLSL
		}
    }
    Fallback Off
}