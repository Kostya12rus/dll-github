// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.DepthOfFieldComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace UnityEngine.PostProcessing
{
  public sealed class DepthOfFieldComponent : PostProcessingComponentRenderTexture<DepthOfFieldModel>
  {
    private const string k_ShaderString = "Hidden/Post FX/Depth Of Field";
    private RenderTexture m_CoCHistory;
    private const float k_FilmHeight = 0.024f;

    public override bool active
    {
      get
      {
        if (this.model.enabled)
          return !this.context.interrupted;
        return false;
      }
    }

    public override DepthTextureMode GetCameraFlags()
    {
      return (DepthTextureMode) 1;
    }

    private float CalculateFocalLength()
    {
      DepthOfFieldModel.Settings settings = this.model.settings;
      if (!settings.useCameraFov)
        return settings.focalLength / 1000f;
      return 0.012f / Mathf.Tan(0.5f * (this.context.camera.get_fieldOfView() * ((float) Math.PI / 180f)));
    }

    private float CalculateMaxCoCRadius(int screenHeight)
    {
      return Mathf.Min(0.05f, (float) ((double) this.model.settings.kernelSize * 4.0 + 6.0) / (float) screenHeight);
    }

    private bool CheckHistory(int width, int height)
    {
      if (Object.op_Inequality((Object) this.m_CoCHistory, (Object) null) && this.m_CoCHistory.IsCreated() && ((Texture) this.m_CoCHistory).get_width() == width)
        return ((Texture) this.m_CoCHistory).get_height() == height;
      return false;
    }

    private RenderTextureFormat SelectFormat(RenderTextureFormat primary, RenderTextureFormat secondary)
    {
      if (SystemInfo.SupportsRenderTextureFormat(primary))
        return primary;
      if (SystemInfo.SupportsRenderTextureFormat(secondary))
        return secondary;
      return (RenderTextureFormat) 7;
    }

    public void Prepare(RenderTexture source, Material uberMaterial, bool antialiasCoC, Vector2 taaJitter, float taaBlending)
    {
      DepthOfFieldModel.Settings settings = this.model.settings;
      RenderTextureFormat format1 = (RenderTextureFormat) 9;
      RenderTextureFormat format2 = this.SelectFormat((RenderTextureFormat) 16, (RenderTextureFormat) 15);
      float focalLength = this.CalculateFocalLength();
      float num1 = Mathf.Max(settings.focusDistance, focalLength);
      float num2 = (float) ((Texture) source).get_width() / (float) ((Texture) source).get_height();
      float num3 = (float) ((double) focalLength * (double) focalLength / ((double) settings.aperture * ((double) num1 - (double) focalLength) * 0.0240000002086163 * 2.0));
      float maxCoCradius = this.CalculateMaxCoCRadius(((Texture) source).get_height());
      Material material = this.context.materialFactory.Get("Hidden/Post FX/Depth Of Field");
      material.SetFloat(DepthOfFieldComponent.Uniforms._Distance, num1);
      material.SetFloat(DepthOfFieldComponent.Uniforms._LensCoeff, num3);
      material.SetFloat(DepthOfFieldComponent.Uniforms._MaxCoC, maxCoCradius);
      material.SetFloat(DepthOfFieldComponent.Uniforms._RcpMaxCoC, 1f / maxCoCradius);
      material.SetFloat(DepthOfFieldComponent.Uniforms._RcpAspect, 1f / num2);
      RenderTexture rt1 = this.context.renderTextureFactory.Get(this.context.width, this.context.height, 0, format2, (RenderTextureReadWrite) 1, (FilterMode) 1, (TextureWrapMode) 1, "FactoryTempTexture");
      Graphics.Blit((Texture) null, rt1, material, 0);
      if (antialiasCoC)
      {
        material.SetTexture(DepthOfFieldComponent.Uniforms._CoCTex, (Texture) rt1);
        float num4 = !this.CheckHistory(this.context.width, this.context.height) ? 0.0f : taaBlending;
        material.SetVector(DepthOfFieldComponent.Uniforms._TaaParams, Vector4.op_Implicit(new Vector3((float) taaJitter.x, (float) taaJitter.y, num4)));
        RenderTexture temporary = RenderTexture.GetTemporary(this.context.width, this.context.height, 0, format2);
        Graphics.Blit((Texture) this.m_CoCHistory, temporary, material, 1);
        this.context.renderTextureFactory.Release(rt1);
        if (Object.op_Inequality((Object) this.m_CoCHistory, (Object) null))
          RenderTexture.ReleaseTemporary(this.m_CoCHistory);
        this.m_CoCHistory = rt1 = temporary;
      }
      RenderTexture renderTexture = this.context.renderTextureFactory.Get(this.context.width / 2, this.context.height / 2, 0, format1, (RenderTextureReadWrite) 0, (FilterMode) 1, (TextureWrapMode) 1, "FactoryTempTexture");
      material.SetTexture(DepthOfFieldComponent.Uniforms._CoCTex, (Texture) rt1);
      Graphics.Blit((Texture) source, renderTexture, material, 2);
      RenderTexture rt2 = this.context.renderTextureFactory.Get(this.context.width / 2, this.context.height / 2, 0, format1, (RenderTextureReadWrite) 0, (FilterMode) 1, (TextureWrapMode) 1, "FactoryTempTexture");
      Graphics.Blit((Texture) renderTexture, rt2, material, (int) (3 + settings.kernelSize));
      Graphics.Blit((Texture) rt2, renderTexture, material, 7);
      uberMaterial.SetVector(DepthOfFieldComponent.Uniforms._DepthOfFieldParams, Vector4.op_Implicit(new Vector3(num1, num3, maxCoCradius)));
      if (this.context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.FocusPlane))
      {
        uberMaterial.EnableKeyword("DEPTH_OF_FIELD_COC_VIEW");
        this.context.Interrupt();
      }
      else
      {
        uberMaterial.SetTexture(DepthOfFieldComponent.Uniforms._DepthOfFieldTex, (Texture) renderTexture);
        uberMaterial.SetTexture(DepthOfFieldComponent.Uniforms._DepthOfFieldCoCTex, (Texture) rt1);
        uberMaterial.EnableKeyword("DEPTH_OF_FIELD");
      }
      this.context.renderTextureFactory.Release(rt2);
    }

    public override void OnDisable()
    {
      if (Object.op_Inequality((Object) this.m_CoCHistory, (Object) null))
        RenderTexture.ReleaseTemporary(this.m_CoCHistory);
      this.m_CoCHistory = (RenderTexture) null;
    }

    private static class Uniforms
    {
      internal static readonly int _DepthOfFieldTex = Shader.PropertyToID(nameof (_DepthOfFieldTex));
      internal static readonly int _DepthOfFieldCoCTex = Shader.PropertyToID(nameof (_DepthOfFieldCoCTex));
      internal static readonly int _Distance = Shader.PropertyToID(nameof (_Distance));
      internal static readonly int _LensCoeff = Shader.PropertyToID(nameof (_LensCoeff));
      internal static readonly int _MaxCoC = Shader.PropertyToID(nameof (_MaxCoC));
      internal static readonly int _RcpMaxCoC = Shader.PropertyToID(nameof (_RcpMaxCoC));
      internal static readonly int _RcpAspect = Shader.PropertyToID(nameof (_RcpAspect));
      internal static readonly int _MainTex = Shader.PropertyToID(nameof (_MainTex));
      internal static readonly int _CoCTex = Shader.PropertyToID(nameof (_CoCTex));
      internal static readonly int _TaaParams = Shader.PropertyToID(nameof (_TaaParams));
      internal static readonly int _DepthOfFieldParams = Shader.PropertyToID(nameof (_DepthOfFieldParams));
    }
  }
}
