// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.PostProcessingProfile
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

namespace UnityEngine.PostProcessing
{
  public class PostProcessingProfile : ScriptableObject
  {
    public BuiltinDebugViewsModel debugViews;
    public FogModel fog;
    public AntialiasingModel antialiasing;
    public AmbientOcclusionModel ambientOcclusion;
    public ScreenSpaceReflectionModel screenSpaceReflection;
    public DepthOfFieldModel depthOfField;
    public MotionBlurModel motionBlur;
    public EyeAdaptationModel eyeAdaptation;
    public BloomModel bloom;
    public ColorGradingModel colorGrading;
    public UserLutModel userLut;
    public ChromaticAberrationModel chromaticAberration;
    public GrainModel grain;
    public VignetteModel vignette;
    public DitheringModel dithering;

    public PostProcessingProfile()
    {
      base.\u002Ector();
    }
  }
}
