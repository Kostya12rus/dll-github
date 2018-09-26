// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.FogComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
  public sealed class FogComponent : PostProcessingComponentCommandBuffer<FogModel>
  {
    private const string k_ShaderString = "Hidden/Post FX/Fog";

    public override bool active
    {
      get
      {
        if (this.model.enabled && this.context.isGBufferAvailable && RenderSettings.get_fog())
          return !this.context.interrupted;
        return false;
      }
    }

    public override string GetName()
    {
      return "Fog";
    }

    public override DepthTextureMode GetCameraFlags()
    {
      return (DepthTextureMode) 1;
    }

    public override CameraEvent GetCameraEvent()
    {
      return (CameraEvent) 13;
    }

    public override void PopulateCommandBuffer(CommandBuffer cb)
    {
      FogModel.Settings settings = this.model.settings;
      Material material = this.context.materialFactory.Get("Hidden/Post FX/Fog");
      material.set_shaderKeywords((string[]) null);
      Color color1;
      if (GraphicsUtils.isLinearColorSpace)
      {
        Color fogColor = RenderSettings.get_fogColor();
        color1 = ((Color) ref fogColor).get_linear();
      }
      else
        color1 = RenderSettings.get_fogColor();
      Color color2 = color1;
      material.SetColor(FogComponent.Uniforms._FogColor, color2);
      material.SetFloat(FogComponent.Uniforms._Density, RenderSettings.get_fogDensity());
      material.SetFloat(FogComponent.Uniforms._Start, RenderSettings.get_fogStartDistance());
      material.SetFloat(FogComponent.Uniforms._End, RenderSettings.get_fogEndDistance());
      FogMode fogMode = RenderSettings.get_fogMode();
      if (fogMode != 1)
      {
        if (fogMode != 2)
        {
          if (fogMode == 3)
            material.EnableKeyword("FOG_EXP2");
        }
        else
          material.EnableKeyword("FOG_EXP");
      }
      else
        material.EnableKeyword("FOG_LINEAR");
      RenderTextureFormat renderTextureFormat = !this.context.isHdr ? (RenderTextureFormat) 7 : (RenderTextureFormat) 9;
      cb.GetTemporaryRT(FogComponent.Uniforms._TempRT, this.context.width, this.context.height, 24, (FilterMode) 1, renderTextureFormat);
      cb.Blit(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), RenderTargetIdentifier.op_Implicit(FogComponent.Uniforms._TempRT));
      cb.Blit(RenderTargetIdentifier.op_Implicit(FogComponent.Uniforms._TempRT), RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), material, !settings.excludeSkybox ? 0 : 1);
      cb.ReleaseTemporaryRT(FogComponent.Uniforms._TempRT);
    }

    private static class Uniforms
    {
      internal static readonly int _FogColor = Shader.PropertyToID(nameof (_FogColor));
      internal static readonly int _Density = Shader.PropertyToID(nameof (_Density));
      internal static readonly int _Start = Shader.PropertyToID(nameof (_Start));
      internal static readonly int _End = Shader.PropertyToID(nameof (_End));
      internal static readonly int _TempRT = Shader.PropertyToID(nameof (_TempRT));
    }
  }
}
