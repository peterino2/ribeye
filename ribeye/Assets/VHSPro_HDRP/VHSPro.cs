using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

using VladStorm;
// namespace VladStorm {


[Serializable, VolumeComponentMenu("Post-processing/VHS Pro")]
public sealed class VHSPro : CustomPostProcessVolumeComponent, IPostProcessComponent {


    // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > HDRP Default Settings).
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;


    //Toggles
    public bool g_pixel = true;
    public bool g_color = true;
    public bool g_palette = true;

    public bool g_crt = true;
    public bool g_noise = true;
    public bool g_jitter = true;
    public bool g_signal = true;
    public bool g_feedback = true;
    public bool g_extra = false;
    public bool g_bypass = false;   


    //Screen
    public BoolParameter            pixelOn = new BoolParameter(false);
    public IntParameter             screenResPresetId = new IntParameter(2);
    public NoInterpIntParameter     screenWidth = new NoInterpIntParameter(640);
    public NoInterpIntParameter     screenHeight = new NoInterpIntParameter(480);

    //Color encoding
    public BoolParameter            colorOn = new BoolParameter(false);
    public IntParameter             colorMode = new IntParameter(2);
    public BoolParameter            colorSyncedOn = new BoolParameter(true);
    public ClampedIntParameter      bitsR = new ClampedIntParameter(2,0,255);
    public ClampedIntParameter      bitsG = new ClampedIntParameter(2,0,255);
    public ClampedIntParameter      bitsB = new ClampedIntParameter(2,0,255);
    public ClampedIntParameter      bitsSynced = new ClampedIntParameter(2,0,255); 
    public ClampedIntParameter      bitsGray = new ClampedIntParameter(1,0,255);
    public ColorParameter           grayscaleColor = new ColorParameter(Color.white);

    //Dither
    public BoolParameter            ditherOn = new BoolParameter(false);
    public NoInterpIntParameter     ditherMode = new NoInterpIntParameter(3);
    public ClampedFloatParameter    ditherAmount = new ClampedFloatParameter(1f, -1f, 3f);

    //Palette
    public BoolParameter            paletteOn = new BoolParameter(false);
    public NoInterpIntParameter     paletteId = new NoInterpIntParameter(0);
    public ClampedIntParameter      paletteDelta = new ClampedIntParameter(5,0,30);
    public TextureParameter         paletteTex = new TextureParameter(null);
    PalettePreset paletteCustom; 
    string paletteCustomName = ""; //for automatic update when drag and drop texture
    bool paletteCustomInit = false; 

    //crt
    public BoolParameter bleedOn  = new BoolParameter(false); 
    public NoInterpIntParameter crtMode = new NoInterpIntParameter(0); 
    public ClampedFloatParameter bleedAmount  = new ClampedFloatParameter(1f, 0f, 5f);


    //Noise
    public BoolParameter noiseResGlobal  = new BoolParameter(true); 
    public NoInterpIntParameter noiseResWidth = new NoInterpIntParameter(640);
    public NoInterpIntParameter noiseResHeight = new NoInterpIntParameter(480);

    public BoolParameter filmgrainOn  = new BoolParameter(false);
    public ClampedFloatParameter filmGrainAmount = new ClampedFloatParameter(0.016f, 0f, 1f); 

    public BoolParameter signalNoiseOn  = new BoolParameter(false);
    public ClampedFloatParameter signalNoiseAmount = new ClampedFloatParameter(0.3f, 0f, 1f);
    public ClampedFloatParameter signalNoisePower  = new ClampedFloatParameter(0.83f, 0f, 1f);

    public BoolParameter lineNoiseOn  = new BoolParameter(false);
    public ClampedFloatParameter lineNoiseAmount = new ClampedFloatParameter(1f, 0f, 10f);
    public ClampedFloatParameter lineNoiseSpeed = new ClampedFloatParameter(5f, 0f, 10f);

    public BoolParameter tapeNoiseOn  = new BoolParameter(false);
    public ClampedFloatParameter tapeNoiseTH = new ClampedFloatParameter(0.63f, 0f, 1.5f);
    public ClampedFloatParameter tapeNoiseAmount = new ClampedFloatParameter(1f, 0f, 1.5f); 
    public ClampedFloatParameter tapeNoiseSpeed = new ClampedFloatParameter(1f, 0f, 1.5f);

    //Jitter
    public BoolParameter scanLinesOn  = new BoolParameter(false);
    public ClampedFloatParameter scanLineWidth = new ClampedFloatParameter(10f,0f,20f);
    
    public BoolParameter linesFloatOn  = new BoolParameter(false); 
    public ClampedFloatParameter linesFloatSpeed = new ClampedFloatParameter(1f,-3f,3f);
    public BoolParameter stretchOn  = new BoolParameter(false);

    public BoolParameter jitterHOn  = new BoolParameter(false);
    public ClampedFloatParameter jitterHAmount = new ClampedFloatParameter(.5f,0f,5f);
    public BoolParameter jitterVOn  = new BoolParameter(false); 
    public ClampedFloatParameter jitterVAmount = new ClampedFloatParameter(1f,0f,15f);
    public ClampedFloatParameter jitterVSpeed = new ClampedFloatParameter(1f,0f,5f);

    public BoolParameter twitchHOn  = new BoolParameter(false);
    public ClampedFloatParameter twitchHFreq = new ClampedFloatParameter(1f,0f,5f);
    public BoolParameter twitchVOn  = new BoolParameter(false);
    public ClampedFloatParameter twitchVFreq = new ClampedFloatParameter(1f,0f,5f);
    
    //Signal Tweak
    public BoolParameter signalTweakOn  = new BoolParameter(false); 
    public ClampedFloatParameter signalAdjustY = new ClampedFloatParameter(0f,-0.25f, 0.25f);
    public ClampedFloatParameter signalAdjustI = new ClampedFloatParameter(0f,-0.25f, 0.25f);
    public ClampedFloatParameter signalAdjustQ = new ClampedFloatParameter(0f,-0.25f, 0.25f);
    public ClampedFloatParameter signalShiftY = new ClampedFloatParameter(1f,-2f, 2f);
    public ClampedFloatParameter signalShiftI = new ClampedFloatParameter(1f,-2f, 2f);
    public ClampedFloatParameter signalShiftQ = new ClampedFloatParameter(1f,-2f, 2f);

    //Feedback
    public BoolParameter feedbackOn  = new BoolParameter(false); 
    public ClampedFloatParameter feedbackThresh = new ClampedFloatParameter(.1f, 0f, 1f);
    public ClampedFloatParameter feedbackAmount = new ClampedFloatParameter(2.0f, 0f, 3f);  
    public ClampedFloatParameter feedbackFade = new ClampedFloatParameter(.82f, 0f, 1f);
    public ColorParameter feedbackColor = new ColorParameter(new Color(1f,.5f,0f)); 
    public BoolParameter feedbackDebugOn  = new BoolParameter(false); 
    public int feedbackMode = 0; 

    //Tools 
    public BoolParameter independentTimeOn  = new BoolParameter(false); 
    float _time = 0f;

    //Bypass     
    public BoolParameter            bypassOn = new BoolParameter(false);  
    public TextureParameter         bypassTex = new TextureParameter(null);



    //Materials, Textures and RTH
    Material mat1 = null;        //1st pass
    Material mat_vhs = null;     //2nd pass vhs bleeding + mix with feedback
    Material mat_tape = null;    //tape noise
    Material mat_fb = null;      //feedback

    RTHandle texTape = null;       //tape noise texture
    Vector2Int texTapeSize;

    RTHandle pass1out = null;           //first pass output
    RTHandle texFeedback = null;        //feedback buffer
    RTHandle texFeedbackLast = null;    //feedback prev frame
    RTHandle texLast = null;            //prev frame
    RTHandle texBypassRTH;

    public bool IsActive(){

        //everything is off by default
        if(pixelOn.value==false &&
            colorOn.value==false &&
            ditherOn.value==false &&
            paletteOn.value==false &&
            bleedOn.value==false &&
            filmgrainOn.value==false &&
            signalNoiseOn.value==false &&
            lineNoiseOn.value==false &&
            tapeNoiseOn.value==false &&
            scanLinesOn.value==false &&
            linesFloatOn.value==false &&
            jitterHOn.value==false &&
            jitterVOn.value==false &&
            twitchHOn.value==false &&
            twitchVOn.value==false &&
            signalTweakOn.value==false &&
            feedbackOn.value==false &&
            bypassOn.value==false) {
            return false;
        }

        return true;
    } 


    

    public override void Setup() {        
        // VHSHelper.Init(); //init palettes and resolution presets
    }

    void InitTextures(HDCamera camera, Vector4 _ResN){

        //load shaders by name, materials represents the shaders
        if(mat1==null){
            CoreUtils.Destroy(mat1);
            LoadMat(ref mat1,       "Hidden/Shader/VHSPro_pass1");
        }

        if(mat_vhs==null){
            CoreUtils.Destroy(mat_vhs);
            LoadMat(ref mat_vhs,    "Hidden/Shader/VHSPro_bleed");
        }

        if(mat_tape==null){
            CoreUtils.Destroy(mat_tape);
            LoadMat(ref mat_tape,   "Hidden/Shader/VHSPro_tape");
        }

        if(mat_fb==null){
            CoreUtils.Destroy(mat_fb);
            LoadMat(ref mat_fb,     "Hidden/Shader/VHSPro_feedback");
        }

        //load RTH
        if(pass1out==null){
            RTHandles.Release(pass1out);
            pass1out = InitRTH(camera.actualWidth, camera.actualHeight);
        }


        //custom palette
        if( VHSHelper.GetPalettes()[paletteId.value].isCustom==true){
    
            if( paletteTex.value!=null && 
                (paletteCustomInit==false || (paletteCustomName != paletteTex.value.name) )){

                //custom palette texture
                Texture2D paletteTex2 = (Texture2D)paletteTex.value;
                paletteCustom =  new PalettePreset(paletteTex2); //to sort
                paletteCustomName = paletteTex.value.name;
                paletteCustomInit = true;
                // Debug.Log("Updating paletteTex. name "+paletteCustomInit+" "+paletteCustomName+" size "+paletteCustom.texSortedWidth);
            }    

        }            

        if(tapeNoiseOn.value || filmgrainOn.value || lineNoiseOn.value)
        if(texTape==null || texTapeSize.x!=((int)_ResN.x) || texTapeSize.y!=((int)_ResN.y)){
        
            RTHandles.Release(texTape);
            texTape = InitRTH((int)_ResN.x, (int)_ResN.y);
            texTapeSize = new Vector2Int((int)_ResN.x, (int)_ResN.y);
        }

        if(feedbackOn.value){

            if(texFeedback==null){
                RTHandles.Release(texFeedback);
                texFeedback = InitRTH(camera.actualWidth, camera.actualHeight); 
            }
            
            if(texFeedbackLast==null){
                RTHandles.Release(texFeedbackLast);
                texFeedbackLast = InitRTH(camera.actualWidth, camera.actualHeight); 
            }

            if(texLast==null){
                RTHandles.Release(texLast);
                texLast = InitRTH(camera.actualWidth, camera.actualHeight); 
            }
        }

        if(bypassOn.value)
        if(texBypassRTH==null){
            RTHandles.Release(texBypassRTH);
            texBypassRTH = InitRTH(camera.actualWidth, camera.actualHeight);             
        }

    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination) {
        
        //init palettes and resolutions
        VHSHelper.Init();

        //calulations 
        if(independentTimeOn.value){ _time = Time.unscaledTime; }
        else{                        _time = Time.time; }


        //original screen resolution (.xy resolution .zw one pixel)
        Vector4 _ResOg = new Vector4(camera.actualWidth, camera.actualHeight,
                                        1f/((float)camera.actualWidth), 
                                        1f/((float)camera.actualHeight)); 


        ResPreset resPreset = VHSHelper.GetResPresets()[screenResPresetId.value];
        if(resPreset.isCustom!=true){
            screenWidth.value  = resPreset.screenWidth;
            screenHeight.value = resPreset.screenHeight;
        }
        if(resPreset.isFirst==true || pixelOn.value==false){
            screenWidth.value  = camera.actualWidth;
            screenHeight.value = camera.actualHeight;
        }

        //resolution after pixelation
        Vector4 _Res = new Vector4(screenWidth.value, screenHeight.value, 0f,0f);
        _Res[2] = 1f/_Res.x;                                    
        _Res[3] = 1f/_Res.y;                                    

        //noise resolution
        Vector4 _ResN = new Vector4(_Res.x, _Res.y, _Res.z, _Res.w);
        if(!noiseResGlobal.value){
            _ResN = new Vector4(noiseResWidth.value, noiseResHeight.value, 0f, 0f);
            _ResN[2] = 1f/_ResN.x;                                    
            _ResN[3] = 1f/_ResN.y;                                                
        }

        //init RTHs
        InitTextures(camera, _ResN);


        //screen params
        mat1.SetFloat("_time",      _time);  
        mat1.SetVector("_ResOg",    _ResOg);
        mat1.SetVector("_Res",      _Res);
        mat1.SetVector("_ResN",      _ResN);

        //Pixelation
        //...

        //Color decimat1ion
        FeatureToggle(mat1, colorOn.value, "_colorOn");             //"VHS_COLOR"
        mat1.SetInt("_colorMode",                colorMode.value);
        mat1.SetInt("_colorSyncedOn",            colorSyncedOn.value?1:0);

        mat1.SetInt("bitsR",                     bitsR.value);
        mat1.SetInt("bitsG",                     bitsG.value);
        mat1.SetInt("bitsB",                     bitsB.value);
        mat1.SetInt("bitsSynced",                bitsSynced.value);

        mat1.SetInt("bitsGray",                  bitsGray.value);
        mat1.SetColor("grayscaleColor",          grayscaleColor.value);        

        FeatureToggle(mat1, ditherOn.value, "_ditherOn"); //"VHS_DITHER"        
        mat1.SetInt("_ditherMode",            ditherMode.value);
        mat1.SetFloat("ditherAmount",         ditherAmount.value);


        //Signal Tweak
        FeatureToggle(mat1, signalTweakOn.value, "_signalTweakOn"); //"VHS_SIGNAL_TWEAK_ON"

        mat1.SetFloat("signalAdjustY", signalAdjustY.value);
        mat1.SetFloat("signalAdjustI", signalAdjustI.value);
        mat1.SetFloat("signalAdjustQ", signalAdjustQ.value);

        mat1.SetFloat("signalShiftY", signalShiftY.value);
        mat1.SetFloat("signalShiftI", signalShiftI.value);
        mat1.SetFloat("signalShiftQ", signalShiftQ.value);


        //Palette
        FeatureToggle(mat1, paletteOn.value, "_paletteOn");         //"VHS_PALETTE"

        // if(paletteOn.value){}
        if(VHSHelper.GetPalettes()[paletteId.value].isCustom==false){
            PalettePreset pal = VHSHelper.GetPalettes()[paletteId.value];
            mat1.SetTexture("_PaletteTex",   pal.texSorted);
            mat1.SetInt("_ResPalette",       pal.texSortedWidth);
            // Debug.Log(VHSHelper.GetPalettes()[paletteId.value].texPalSorted);
        }
        else{ //custom


            // UpdateCustomPalette();  
            // if(paletteCustom!=null){
            mat1.SetTexture("_PaletteTex",    paletteCustom.texSorted );
            mat1.SetInt("_ResPalette",        paletteCustom.texSortedWidth );                       
            // }              

        }

        mat1.SetInt("paletteDelta",           paletteDelta.value);
        

        //VHS 1st Pass (Distortions, Decimations)
        FeatureToggle(mat1, filmgrainOn.value, "_filmgrainOn" );        //"VHS_FILMGRAIN_ON"
        FeatureToggle(mat1, tapeNoiseOn.value, "_tapeNoiseOn" );        //"VHS_TAPENOISE_ON"
        FeatureToggle(mat1, lineNoiseOn.value, "_lineNoiseOn" );        //"VHS_LINENOISE_ON"

        
        //Jitter & Twitch
        FeatureToggle(mat1, jitterHOn.value, "_jitterHOn");             //"VHS_JITTER_H_ON"
        mat1.SetFloat("jitterHAmount", jitterHAmount.value);

        FeatureToggle(mat1, jitterVOn.value, "_jitterVOn");             //"VHS_JITTER_V_ON"
        mat1.SetFloat("jitterVAmount", jitterVAmount.value);
        mat1.SetFloat("jitterVSpeed", jitterVSpeed.value);

        FeatureToggle(mat1, linesFloatOn.value, "_linesFloatOn");       //"VHS_LINESFLOAT_ON"     
        mat1.SetFloat("linesFloatSpeed", linesFloatSpeed.value);

        FeatureToggle(mat1, twitchHOn.value, "_twitchHOn");             //"VHS_TWITCH_H_ON"
        mat1.SetFloat("twitchHFreq", twitchHFreq.value);

        FeatureToggle(mat1, twitchVOn.value, "_twitchVOn");             //"VHS_TWITCH_V_ON"
        mat1.SetFloat("twitchVFreq", twitchVFreq.value);

        FeatureToggle(mat1, scanLinesOn.value, "_scanLinesOn");         //"VHS_SCANLINES_ON"
        mat1.SetFloat("scanLineWidth", scanLineWidth.value);
        
        FeatureToggle(mat1, signalNoiseOn.value, "_signalNoiseOn");     //"VHS_YIQNOISE_ON"
        mat1.SetFloat("signalNoisePower", signalNoisePower.value);
        mat1.SetFloat("signalNoiseAmount", signalNoiseAmount.value);

        FeatureToggle(mat1, stretchOn.value, "_stretchOn");             //"VHS_STRETCH_ON"


        //Noises
        if(tapeNoiseOn.value || filmgrainOn.value || lineNoiseOn.value){

            mat_tape.SetFloat("_time", _time);  

            FeatureToggle(mat_tape, filmgrainOn.value, "_filmgrainOn"); //"VHS_FILMGRAIN_ON"
            mat_tape.SetFloat("filmGrainAmount", filmGrainAmount.value);
            
            FeatureToggle(mat_tape, tapeNoiseOn.value, "_tapeNoiseOn"); //"VHS_TAPENOISE_ON"
            mat_tape.SetFloat("tapeNoiseTH", tapeNoiseTH.value);
            mat_tape.SetFloat("tapeNoiseAmount", tapeNoiseAmount.value);
            mat_tape.SetFloat("tapeNoiseSpeed", tapeNoiseSpeed.value);
            
            FeatureToggle(mat_tape, lineNoiseOn.value, "_lineNoiseOn"); //"VHS_LINENOISE_ON"
            mat_tape.SetFloat("lineNoiseAmount", lineNoiseAmount.value);
            mat_tape.SetFloat("lineNoiseSpeed", lineNoiseSpeed.value);

            HDUtils.DrawFullScreen(cmd, mat_tape, texTape);       
            
            mat1.SetTexture("_TapeTex",         texTape);
            mat1.SetFloat("tapeNoiseAmount", tapeNoiseAmount.value);          

        }


        //VHS 2nd Pass (Bleed)
        mat_vhs.SetFloat("_time",  _time);  
        mat_vhs.SetVector("_ResOg", _ResOg);//  - resolution before pixelation
        mat_vhs.SetVector("_Res",   _Res);//  - resolution after pixelation

        //CRT       
        FeatureToggle(mat_vhs, bleedOn.value, "_bleedOn"); //"VHS_BLEED_ON"

        //v2.1
        mat_vhs.SetInt("_crtMode",            crtMode.value);
        // mat_vhs.DisableKeyword("VHS_OLD_THREE_PHASE");
        // mat_vhs.DisableKeyword("VHS_THREE_PHASE");
        // mat_vhs.DisableKeyword("VHS_TWO_PHASE");           
        //       if(crtMode.value==0){ mat_vhs.EnableKeyword("VHS_OLD_THREE_PHASE"); }
        // else if(crtMode.value==1){ mat_vhs.EnableKeyword("VHS_THREE_PHASE"); }
        // else if(crtMode.value==2){ mat_vhs.EnableKeyword("VHS_TWO_PHASE"); }

        mat_vhs.SetFloat("bleedAmount", bleedAmount.value);


        //1st pass
        //Bypass Texture
        if(bypassOn.value){
            Graphics.Blit(bypassTex.value, texBypassRTH.rt); //TODO maybe copy texture instead of graph.blit
            // cmd.CopyTexture(bypassTex.value, texBypassRTH);  
            mat1.SetTexture("_InputTexture", texBypassRTH);     
        }else{
            mat1.SetTexture("_InputTexture", source);
        }
        HDUtils.DrawFullScreen(cmd, mat1, pass1out);    



        if(feedbackOn.value){
 
            //recalc feedback buffer
            mat_fb.SetFloat("feedbackThresh",   feedbackThresh.value);
            mat_fb.SetFloat("feedbackAmount",   feedbackAmount.value);
            mat_fb.SetFloat("feedbackFade",     feedbackFade.value);
            mat_fb.SetColor("feedbackColor",    feedbackColor.value);

            mat_fb.SetTexture("_InputTexture",      pass1out);
            mat_fb.SetTexture("_LastTex",           texLast);
            mat_fb.SetTexture("_FeedbackTex",       texFeedbackLast);

            HDUtils.DrawFullScreen(cmd, mat_fb, texFeedback); //texFeedback2

            cmd.CopyTexture(texFeedback, texFeedbackLast);  //save prev frame feedback
            cmd.CopyTexture(pass1out, texLast);             //save prev frame color
        
        }

        mat_vhs.SetInt("feedbackOn",            feedbackOn.value?1:0);
        mat_vhs.SetInt("feedbackDebugOn",       feedbackDebugOn.value?1:0);
        if(feedbackOn.value || feedbackDebugOn.value){
            mat_vhs.SetTexture("_FeedbackTex",      texFeedback);
        }

        //2nd pass
        mat_vhs.SetTexture("_InputTexture",     pass1out);
        HDUtils.DrawFullScreen(cmd, mat_vhs, destination);  


    }


    public override void Cleanup(){

        //materials 
        CoreUtils.Destroy(mat1);
        CoreUtils.Destroy(mat_vhs);
        CoreUtils.Destroy(mat_tape);
        CoreUtils.Destroy(mat_fb);

        //RTH
        RTHandles.Release(texTape);
        RTHandles.Release(pass1out);
        RTHandles.Release(texFeedback);
        RTHandles.Release(texFeedbackLast);
        RTHandles.Release(texLast);
        // RTHandles.Release(texClear);

    }

    //Helper Tools
    void FeatureToggle(Material mat, bool propVal, string propName){  

        //v2.1 uniforms instead of shader features 
        mat.SetInt(propName,            propVal?1:0);
        //turn on/off shader features
        // if(propVal)     mat.EnableKeyword(featureName);
        // else            mat.DisableKeyword(featureName);
    }

    void LoadMat(ref Material m, string shaderName){
        if( Shader.Find(shaderName)!=null ) 
            m = new Material(Shader.Find(shaderName));
        else 
            Debug.LogError($"Unable to find shader '{shaderName}'. Post Process Volume VHSPro is unable to load.");
    }

    //inits default RTH
    RTHandle InitRTH(int width, int height){

        return RTHandles.Alloc(
            width, 
            height,
            TextureXR.slices, 
            filterMode: FilterMode.Point, 
            dimension: TextureXR.dimension, 
            useDynamicScale: true, 
            enableRandomWrite: true                
        );

    }


}
// }

