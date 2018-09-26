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
      return DepthTextureMode.Depth;
    }

    private float CalculateFocalLength()
    {
      DepthOfFieldModel.Settings settings = this.model.settings;
      if (!settings.useCameraFov)
        return settings.focalLength / 1000f;
      return 0.012f / Mathf.Tan(0.5f * (this.context.camera.fieldOfView * ((float) Math.PI / 180f)));
    }

    private float CalculateMaxCoCRadius(int screenHeight)
    {
      return Mathf.Min(0.05f, (float) ((double) this.model.settings.kernelSize * 4.0 + 6.0) / (float) screenHeight);
    }

    private bool CheckHistory(int width, int height)
    {
      if ((Object) this.m_CoCHistory != (Object) null && this.m_CoCHistory.IsCreated() && this.m_CoCHistory.width == width)
        return this.m_CoCHistory.height == height;
      return false;
    }

    private RenderTextureFormat SelectFormat(RenderTextureFormat primary, RenderTextureFormat secondary)
    {
      if (SystemInfo.SupportsRenderTextureFormat(primary))
        return primary;
      if (SystemInfo.SupportsRenderTextureFormat(secondary))
        return secondary;
      return RenderTextureFormat.Default;
    }

    public void Prepare(RenderTexture source, Material uberMaterial, bool antialiasCoC, Vector2 taaJitter, float taaBlending)
    {
      DepthOfFieldModel.Settings settings = this.model.settings;
      RenderTextureFormat format1 = RenderTextureFormat.DefaultHDR;
      RenderTextureFormat format2 = this.SelectFormat(RenderTextureFormat.R8, RenderTextureFormat.RHalf);
      float focalLength = this.CalculateFocalLength();
      float x = Mathf.Max(settings.focusDistance, focalLength);
      float num = (float) source.width / (float) source.height;
      float y = (float) ((double) focalLength * (double) focalLength / ((double) settings.aperture * ((double) x - (double) focalLength) * 0.0240000002086163 * 2.0));
      float maxCoCradius = this.CalculateMaxCoCRadius(source.height);
      Material mat = this.context.materialFactory.Get("Hidden/Post FX/Depth Of Field");
      mat.SetFloat(DepthOfFieldComponent.Uniforms._Distance, x);
      mat.SetFloat(DepthOfFieldComponent.Uniforms._LensCoeff, y);
      mat.SetFloat(DepthOfFieldComponent.Uniforms._MaxCoC, maxCoCradius);
      mat.SetFloat(DepthOfFieldComponent.Uniforms._RcpMaxCoC, 1f / maxCoCradius);
      mat.SetFloat(DepthOfFieldComponent.Uniforms._RcpAspect, 1f / num);
      RenderTexture renderTexture1 = this.context.renderTextureFactory.Get(this.context.width, this.context.height, 0, format2, RenderTextureReadWrite.Linear, FilterMode.Bilinear, TextureWrapMode.Clamp, "FactoryTempTexture");
      Graphics.Blit((Texture) null, renderTexture1, mat, 0);
      if (antialiasCoC)
      {
        mat.SetTexture(DepthOfFieldComponent.Uniforms._CoCTex, (Texture) renderTexture1);
        float z = !this.CheckHistory(this.context.width, this.context.height) ? 0.0f : taaBlending;
        mat.SetVector(DepthOfFieldComponent.Uniforms._TaaParams, (Vector4) new Vector3(taaJitter.x, taaJitter.y, z));
        RenderTexture temporary = RenderTexture.GetTemporary(this.context.width, this.context.height, 0, format2);
        Graphics.Blit((Texture) this.m_CoCHistory, temporary, mat, 1);
        this.context.renderTextureFactory.Release(renderTexture1);
        if ((Object) this.m_CoCHistory != (Object) null)
          RenderTexture.ReleaseTemporary(this.m_CoCHistory);
        this.m_CoCHistory = renderTexture1 = temporary;
      }
      RenderTexture dest = this.context.renderTextureFactory.Get(this.context.width / 2, this.context.height / 2, 0, format1, RenderTextureReadWrite.Default, FilterMode.Bilinear, TextureWrapMode.Clamp, "FactoryTempTexture");
      mat.SetTexture(DepthOfFieldComponent.Uniforms._CoCTex, (Texture) renderTexture1);
      Graphics.Blit((Texture) source, dest, mat, 2);
      RenderTexture renderTexture2 = this.context.renderTextureFactory.Get(this.context.width / 2, this.context.height / 2, 0, format1, RenderTextureReadWrite.Default, FilterMode.Bilinear, TextureWrapMode.Clamp, "FactoryTempTexture");
      Graphics.Blit((Texture) dest, renderTexture2, mat, (int) (3 + settings.kernelSize));
      Graphics.Blit((Texture) renderTexture2, dest, mat, 7);
      uberMaterial.SetVector(DepthOfFieldComponent.Uniforms._DepthOfFieldParams, (Vector4) new Vector3(x, y, maxCoCradius));
      if (this.context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.FocusPlane))
      {
        uberMaterial.EnableKeyword("DEPTH_OF_FIELD_COC_VIEW");
        this.context.Interrupt();
      }
      else
      {
        uberMaterial.SetTexture(DepthOfFieldComponent.Uniforms._DepthOfFieldTex, (Texture) dest);
        uberMaterial.SetTexture(DepthOfFieldComponent.Uniforms._DepthOfFieldCoCTex, (Texture) renderTexture1);
        uberMaterial.EnableKeyword("DEPTH_OF_FIELD");
      }
      this.context.renderTextureFactory.Release(renderTexture2);
    }

    public override void OnDisable()
    {
      if ((Object) this.m_CoCHistory != (Object) null)
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
