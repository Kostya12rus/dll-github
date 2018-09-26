// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.ColorGradingComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

namespace UnityEngine.PostProcessing
{
  public sealed class ColorGradingComponent : PostProcessingComponentRenderTexture<ColorGradingModel>
  {
    private Color[] m_pixels = new Color[256];
    private const int k_InternalLogLutSize = 32;
    private const int k_CurvePrecision = 128;
    private const float k_CurveStep = 0.0078125f;
    private Texture2D m_GradingCurves;

    public override bool active
    {
      get
      {
        if (this.model.enabled)
          return !this.context.interrupted;
        return false;
      }
    }

    private float StandardIlluminantY(float x)
    {
      return (float) (2.86999988555908 * (double) x - 3.0 * (double) x * (double) x - 0.275095075368881);
    }

    private Vector3 CIExyToLMS(float x, float y)
    {
      float num1 = 1f;
      float num2 = num1 * x / y;
      float num3 = num1 * (1f - x - y) / y;
      return new Vector3((float) (0.732800006866455 * (double) num2 + 0.42960000038147 * (double) num1 - 0.162400007247925 * (double) num3), (float) (-0.703599989414215 * (double) num2 + 1.69749999046326 * (double) num1 + 0.00609999988228083 * (double) num3), (float) (3.0 / 1000.0 * (double) num2 + 0.0136000001803041 * (double) num1 + 0.983399987220764 * (double) num3));
    }

    private Vector3 CalculateColorBalance(float temperature, float tint)
    {
      float num1 = temperature / 55f;
      float num2 = tint / 55f;
      float x = (float) (0.312709987163544 - (double) num1 * ((double) num1 >= 0.0 ? 0.0500000007450581 : 0.100000001490116));
      float y = this.StandardIlluminantY(x) + num2 * 0.05f;
      Vector3 vector3;
      ((Vector3) ref vector3).\u002Ector(0.949237f, 1.03542f, 1.08728f);
      Vector3 lms = this.CIExyToLMS(x, y);
      return new Vector3((float) (vector3.x / lms.x), (float) (vector3.y / lms.y), (float) (vector3.z / lms.z));
    }

    private static Color NormalizeColor(Color c)
    {
      float num = (float) ((c.r + c.g + c.b) / 3.0);
      if (Mathf.Approximately(num, 0.0f))
        return new Color(1f, 1f, 1f, (float) c.a);
      Color color = (Color) null;
      color.r = (__Null) (c.r / (double) num);
      color.g = (__Null) (c.g / (double) num);
      color.b = (__Null) (c.b / (double) num);
      color.a = c.a;
      return color;
    }

    private static Vector3 ClampVector(Vector3 v, float min, float max)
    {
      return new Vector3(Mathf.Clamp((float) v.x, min, max), Mathf.Clamp((float) v.y, min, max), Mathf.Clamp((float) v.z, min, max));
    }

    public static Vector3 GetLiftValue(Color lift)
    {
      Color color = ColorGradingComponent.NormalizeColor(lift);
      float num = (float) ((color.r + color.g + color.b) / 3.0);
      return ColorGradingComponent.ClampVector(new Vector3((float) ((color.r - (double) num) * 0.100000001490116 + lift.a), (float) ((color.g - (double) num) * 0.100000001490116 + lift.a), (float) ((color.b - (double) num) * 0.100000001490116 + lift.a)), -1f, 1f);
    }

    public static Vector3 GetGammaValue(Color gamma)
    {
      Color color = ColorGradingComponent.NormalizeColor(gamma);
      float num = (float) ((color.r + color.g + color.b) / 3.0);
      ref Color local = ref gamma;
      local.a = (__Null) (local.a * (gamma.a >= 0.0 ? 5.0 : 0.800000011920929));
      return ColorGradingComponent.ClampVector(new Vector3(1f / Mathf.Max(0.01f, Mathf.Pow(2f, (float) ((color.r - (double) num) * 0.5)) + (float) gamma.a), 1f / Mathf.Max(0.01f, Mathf.Pow(2f, (float) ((color.g - (double) num) * 0.5)) + (float) gamma.a), 1f / Mathf.Max(0.01f, Mathf.Pow(2f, (float) ((color.b - (double) num) * 0.5)) + (float) gamma.a)), 0.0f, 5f);
    }

    public static Vector3 GetGainValue(Color gain)
    {
      Color color = ColorGradingComponent.NormalizeColor(gain);
      float num = (float) ((color.r + color.g + color.b) / 3.0);
      ref Color local = ref gain;
      local.a = (__Null) (local.a * (gain.a <= 0.0 ? 1.0 : 3.0));
      return ColorGradingComponent.ClampVector(new Vector3(Mathf.Pow(2f, (float) ((color.r - (double) num) * 0.5)) + (float) gain.a, Mathf.Pow(2f, (float) ((color.g - (double) num) * 0.5)) + (float) gain.a, Mathf.Pow(2f, (float) ((color.b - (double) num) * 0.5)) + (float) gain.a), 0.0f, 4f);
    }

    public static void CalculateLiftGammaGain(Color lift, Color gamma, Color gain, out Vector3 outLift, out Vector3 outGamma, out Vector3 outGain)
    {
      outLift = ColorGradingComponent.GetLiftValue(lift);
      outGamma = ColorGradingComponent.GetGammaValue(gamma);
      outGain = ColorGradingComponent.GetGainValue(gain);
    }

    public static Vector3 GetSlopeValue(Color slope)
    {
      Color color = ColorGradingComponent.NormalizeColor(slope);
      float num = (float) ((color.r + color.g + color.b) / 3.0);
      ref Color local = ref slope;
      local.a = (__Null) (local.a * 0.5);
      return ColorGradingComponent.ClampVector(new Vector3((float) ((color.r - (double) num) * 0.100000001490116 + slope.a + 1.0), (float) ((color.g - (double) num) * 0.100000001490116 + slope.a + 1.0), (float) ((color.b - (double) num) * 0.100000001490116 + slope.a + 1.0)), 0.0f, 2f);
    }

    public static Vector3 GetPowerValue(Color power)
    {
      Color color = ColorGradingComponent.NormalizeColor(power);
      float num = (float) ((color.r + color.g + color.b) / 3.0);
      ref Color local = ref power;
      local.a = (__Null) (local.a * 0.5);
      return ColorGradingComponent.ClampVector(new Vector3(1f / Mathf.Max(0.01f, (float) ((color.r - (double) num) * 0.100000001490116 + power.a + 1.0)), 1f / Mathf.Max(0.01f, (float) ((color.g - (double) num) * 0.100000001490116 + power.a + 1.0)), 1f / Mathf.Max(0.01f, (float) ((color.b - (double) num) * 0.100000001490116 + power.a + 1.0))), 0.5f, 2.5f);
    }

    public static Vector3 GetOffsetValue(Color offset)
    {
      Color color = ColorGradingComponent.NormalizeColor(offset);
      float num = (float) ((color.r + color.g + color.b) / 3.0);
      ref Color local = ref offset;
      local.a = (__Null) (local.a * 0.5);
      return ColorGradingComponent.ClampVector(new Vector3((float) ((color.r - (double) num) * 0.0500000007450581 + offset.a), (float) ((color.g - (double) num) * 0.0500000007450581 + offset.a), (float) ((color.b - (double) num) * 0.0500000007450581 + offset.a)), -0.8f, 0.8f);
    }

    public static void CalculateSlopePowerOffset(Color slope, Color power, Color offset, out Vector3 outSlope, out Vector3 outPower, out Vector3 outOffset)
    {
      outSlope = ColorGradingComponent.GetSlopeValue(slope);
      outPower = ColorGradingComponent.GetPowerValue(power);
      outOffset = ColorGradingComponent.GetOffsetValue(offset);
    }

    private TextureFormat GetCurveFormat()
    {
      if (SystemInfo.SupportsTextureFormat((TextureFormat) 17))
        return (TextureFormat) 17;
      return (TextureFormat) 4;
    }

    private Texture2D GetCurveTexture()
    {
      if (Object.op_Equality((Object) this.m_GradingCurves, (Object) null))
      {
        Texture2D texture2D = new Texture2D(128, 2, this.GetCurveFormat(), false, true);
        ((Object) texture2D).set_name("Internal Curves Texture");
        ((Object) texture2D).set_hideFlags((HideFlags) 52);
        ((Texture) texture2D).set_anisoLevel(0);
        ((Texture) texture2D).set_wrapMode((TextureWrapMode) 1);
        ((Texture) texture2D).set_filterMode((FilterMode) 1);
        this.m_GradingCurves = texture2D;
      }
      ColorGradingModel.CurvesSettings curves = this.model.settings.curves;
      curves.hueVShue.Cache();
      curves.hueVSsat.Cache();
      for (int index = 0; index < 128; ++index)
      {
        float t = (float) index * (1f / 128f);
        float num1 = curves.hueVShue.Evaluate(t);
        float num2 = curves.hueVSsat.Evaluate(t);
        float num3 = curves.satVSsat.Evaluate(t);
        float num4 = curves.lumVSsat.Evaluate(t);
        this.m_pixels[index] = new Color(num1, num2, num3, num4);
        float num5 = curves.master.Evaluate(t);
        float num6 = curves.red.Evaluate(t);
        float num7 = curves.green.Evaluate(t);
        float num8 = curves.blue.Evaluate(t);
        this.m_pixels[index + 128] = new Color(num6, num7, num8, num5);
      }
      this.m_GradingCurves.SetPixels(this.m_pixels);
      this.m_GradingCurves.Apply(false, false);
      return this.m_GradingCurves;
    }

    private bool IsLogLutValid(RenderTexture lut)
    {
      if (Object.op_Inequality((Object) lut, (Object) null) && lut.IsCreated())
        return ((Texture) lut).get_height() == 32;
      return false;
    }

    private RenderTextureFormat GetLutFormat()
    {
      if (SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat) 2))
        return (RenderTextureFormat) 2;
      return (RenderTextureFormat) 0;
    }

    private void GenerateLut()
    {
      ColorGradingModel.Settings settings = this.model.settings;
      if (!this.IsLogLutValid(this.model.bakedLut))
      {
        GraphicsUtils.Destroy((Object) this.model.bakedLut);
        ColorGradingModel model = this.model;
        RenderTexture renderTexture1 = new RenderTexture(1024, 32, 0, this.GetLutFormat());
        ((Object) renderTexture1).set_name("Color Grading Log LUT");
        ((Object) renderTexture1).set_hideFlags((HideFlags) 52);
        ((Texture) renderTexture1).set_filterMode((FilterMode) 1);
        ((Texture) renderTexture1).set_wrapMode((TextureWrapMode) 1);
        ((Texture) renderTexture1).set_anisoLevel(0);
        RenderTexture renderTexture2 = renderTexture1;
        model.bakedLut = renderTexture2;
      }
      Material material = this.context.materialFactory.Get("Hidden/Post FX/Lut Generator");
      material.SetVector(ColorGradingComponent.Uniforms._LutParams, new Vector4(32f, 0.0004882813f, 1f / 64f, 1.032258f));
      material.set_shaderKeywords((string[]) null);
      ColorGradingModel.TonemappingSettings tonemapping = settings.tonemapping;
      switch (tonemapping.tonemapper)
      {
        case ColorGradingModel.Tonemapper.ACES:
          material.EnableKeyword("TONEMAPPING_FILMIC");
          break;
        case ColorGradingModel.Tonemapper.Neutral:
          material.EnableKeyword("TONEMAPPING_NEUTRAL");
          float num1 = (float) ((double) tonemapping.neutralBlackIn * 20.0 + 1.0);
          float num2 = (float) ((double) tonemapping.neutralBlackOut * 10.0 + 1.0);
          float num3 = tonemapping.neutralWhiteIn / 20f;
          float num4 = (float) (1.0 - (double) tonemapping.neutralWhiteOut / 20.0);
          float num5 = num1 / num2;
          float num6 = num3 / num4;
          float num7 = Mathf.Max(0.0f, Mathf.LerpUnclamped(0.57f, 0.37f, num5));
          float num8 = Mathf.LerpUnclamped(0.01f, 0.24f, num6);
          float num9 = Mathf.Max(0.0f, Mathf.LerpUnclamped(0.02f, 0.2f, num5));
          material.SetVector(ColorGradingComponent.Uniforms._NeutralTonemapperParams1, new Vector4(0.2f, num7, num8, num9));
          material.SetVector(ColorGradingComponent.Uniforms._NeutralTonemapperParams2, new Vector4(0.02f, 0.3f, tonemapping.neutralWhiteLevel, tonemapping.neutralWhiteClip / 10f));
          break;
      }
      material.SetFloat(ColorGradingComponent.Uniforms._HueShift, settings.basic.hueShift / 360f);
      material.SetFloat(ColorGradingComponent.Uniforms._Saturation, settings.basic.saturation);
      material.SetFloat(ColorGradingComponent.Uniforms._Contrast, settings.basic.contrast);
      material.SetVector(ColorGradingComponent.Uniforms._Balance, Vector4.op_Implicit(this.CalculateColorBalance(settings.basic.temperature, settings.basic.tint)));
      Vector3 outLift;
      Vector3 outGamma;
      Vector3 outGain;
      ColorGradingComponent.CalculateLiftGammaGain(settings.colorWheels.linear.lift, settings.colorWheels.linear.gamma, settings.colorWheels.linear.gain, out outLift, out outGamma, out outGain);
      material.SetVector(ColorGradingComponent.Uniforms._Lift, Vector4.op_Implicit(outLift));
      material.SetVector(ColorGradingComponent.Uniforms._InvGamma, Vector4.op_Implicit(outGamma));
      material.SetVector(ColorGradingComponent.Uniforms._Gain, Vector4.op_Implicit(outGain));
      Vector3 outSlope;
      Vector3 outPower;
      Vector3 outOffset;
      ColorGradingComponent.CalculateSlopePowerOffset(settings.colorWheels.log.slope, settings.colorWheels.log.power, settings.colorWheels.log.offset, out outSlope, out outPower, out outOffset);
      material.SetVector(ColorGradingComponent.Uniforms._Slope, Vector4.op_Implicit(outSlope));
      material.SetVector(ColorGradingComponent.Uniforms._Power, Vector4.op_Implicit(outPower));
      material.SetVector(ColorGradingComponent.Uniforms._Offset, Vector4.op_Implicit(outOffset));
      material.SetVector(ColorGradingComponent.Uniforms._ChannelMixerRed, Vector4.op_Implicit(settings.channelMixer.red));
      material.SetVector(ColorGradingComponent.Uniforms._ChannelMixerGreen, Vector4.op_Implicit(settings.channelMixer.green));
      material.SetVector(ColorGradingComponent.Uniforms._ChannelMixerBlue, Vector4.op_Implicit(settings.channelMixer.blue));
      material.SetTexture(ColorGradingComponent.Uniforms._Curves, (Texture) this.GetCurveTexture());
      Graphics.Blit((Texture) null, this.model.bakedLut, material, 0);
    }

    public override void Prepare(Material uberMaterial)
    {
      if (this.model.isDirty || !this.IsLogLutValid(this.model.bakedLut))
      {
        this.GenerateLut();
        this.model.isDirty = false;
      }
      uberMaterial.EnableKeyword(!this.context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.PreGradingLog) ? "COLOR_GRADING" : "COLOR_GRADING_LOG_VIEW");
      RenderTexture bakedLut = this.model.bakedLut;
      uberMaterial.SetTexture(ColorGradingComponent.Uniforms._LogLut, (Texture) bakedLut);
      uberMaterial.SetVector(ColorGradingComponent.Uniforms._LogLut_Params, Vector4.op_Implicit(new Vector3(1f / (float) ((Texture) bakedLut).get_width(), 1f / (float) ((Texture) bakedLut).get_height(), (float) ((Texture) bakedLut).get_height() - 1f)));
      float num = Mathf.Exp(this.model.settings.basic.postExposure * 0.6931472f);
      uberMaterial.SetFloat(ColorGradingComponent.Uniforms._ExposureEV, num);
    }

    public void OnGUI()
    {
      RenderTexture bakedLut = this.model.bakedLut;
      Rect rect;
      ref Rect local = ref rect;
      Rect viewport = this.context.viewport;
      double num1 = (double) ((Rect) ref viewport).get_x() * (double) Screen.get_width() + 8.0;
      double num2 = 8.0;
      double width = (double) ((Texture) bakedLut).get_width();
      double height = (double) ((Texture) bakedLut).get_height();
      ((Rect) ref local).\u002Ector((float) num1, (float) num2, (float) width, (float) height);
      GUI.DrawTexture(rect, (Texture) bakedLut);
    }

    public override void OnDisable()
    {
      GraphicsUtils.Destroy((Object) this.m_GradingCurves);
      GraphicsUtils.Destroy((Object) this.model.bakedLut);
      this.m_GradingCurves = (Texture2D) null;
      this.model.bakedLut = (RenderTexture) null;
    }

    private static class Uniforms
    {
      internal static readonly int _LutParams = Shader.PropertyToID(nameof (_LutParams));
      internal static readonly int _NeutralTonemapperParams1 = Shader.PropertyToID(nameof (_NeutralTonemapperParams1));
      internal static readonly int _NeutralTonemapperParams2 = Shader.PropertyToID(nameof (_NeutralTonemapperParams2));
      internal static readonly int _HueShift = Shader.PropertyToID(nameof (_HueShift));
      internal static readonly int _Saturation = Shader.PropertyToID(nameof (_Saturation));
      internal static readonly int _Contrast = Shader.PropertyToID(nameof (_Contrast));
      internal static readonly int _Balance = Shader.PropertyToID(nameof (_Balance));
      internal static readonly int _Lift = Shader.PropertyToID(nameof (_Lift));
      internal static readonly int _InvGamma = Shader.PropertyToID(nameof (_InvGamma));
      internal static readonly int _Gain = Shader.PropertyToID(nameof (_Gain));
      internal static readonly int _Slope = Shader.PropertyToID(nameof (_Slope));
      internal static readonly int _Power = Shader.PropertyToID(nameof (_Power));
      internal static readonly int _Offset = Shader.PropertyToID(nameof (_Offset));
      internal static readonly int _ChannelMixerRed = Shader.PropertyToID(nameof (_ChannelMixerRed));
      internal static readonly int _ChannelMixerGreen = Shader.PropertyToID(nameof (_ChannelMixerGreen));
      internal static readonly int _ChannelMixerBlue = Shader.PropertyToID(nameof (_ChannelMixerBlue));
      internal static readonly int _Curves = Shader.PropertyToID(nameof (_Curves));
      internal static readonly int _LogLut = Shader.PropertyToID(nameof (_LogLut));
      internal static readonly int _LogLut_Params = Shader.PropertyToID(nameof (_LogLut_Params));
      internal static readonly int _ExposureEV = Shader.PropertyToID(nameof (_ExposureEV));
    }
  }
}
