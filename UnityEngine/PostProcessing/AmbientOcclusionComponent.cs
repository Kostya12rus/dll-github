// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.AmbientOcclusionComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
  public sealed class AmbientOcclusionComponent : PostProcessingComponentCommandBuffer<AmbientOcclusionModel>
  {
    private readonly RenderTargetIdentifier[] m_MRT = new RenderTargetIdentifier[2]{ RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 10), RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2) };
    private const string k_BlitShaderString = "Hidden/Post FX/Blit";
    private const string k_ShaderString = "Hidden/Post FX/Ambient Occlusion";

    private AmbientOcclusionComponent.OcclusionSource occlusionSource
    {
      get
      {
        if (this.context.isGBufferAvailable && !this.model.settings.forceForwardCompatibility)
          return AmbientOcclusionComponent.OcclusionSource.GBuffer;
        return this.model.settings.highPrecision && (!this.context.isGBufferAvailable || this.model.settings.forceForwardCompatibility) ? AmbientOcclusionComponent.OcclusionSource.DepthTexture : AmbientOcclusionComponent.OcclusionSource.DepthNormalsTexture;
      }
    }

    private bool ambientOnlySupported
    {
      get
      {
        if (this.context.isHdr && (this.model.settings.ambientOnly && this.context.isGBufferAvailable))
          return !this.model.settings.forceForwardCompatibility;
        return false;
      }
    }

    public override bool active
    {
      get
      {
        if (this.model.enabled && (double) this.model.settings.intensity > 0.0)
          return !this.context.interrupted;
        return false;
      }
    }

    public override DepthTextureMode GetCameraFlags()
    {
      DepthTextureMode depthTextureMode = (DepthTextureMode) 0;
      if (this.occlusionSource == AmbientOcclusionComponent.OcclusionSource.DepthTexture)
        depthTextureMode = (DepthTextureMode) (depthTextureMode | 1);
      if (this.occlusionSource != AmbientOcclusionComponent.OcclusionSource.GBuffer)
        depthTextureMode = (DepthTextureMode) (depthTextureMode | 2);
      return depthTextureMode;
    }

    public override string GetName()
    {
      return "Ambient Occlusion";
    }

    public override CameraEvent GetCameraEvent()
    {
      if (this.ambientOnlySupported && !this.context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.AmbientOcclusion))
        return (CameraEvent) 21;
      return (CameraEvent) 12;
    }

    public override void PopulateCommandBuffer(CommandBuffer cb)
    {
      AmbientOcclusionModel.Settings settings = this.model.settings;
      Material material1 = this.context.materialFactory.Get("Hidden/Post FX/Blit");
      Material material2 = this.context.materialFactory.Get("Hidden/Post FX/Ambient Occlusion");
      material2.set_shaderKeywords((string[]) null);
      material2.SetFloat(AmbientOcclusionComponent.Uniforms._Intensity, settings.intensity);
      material2.SetFloat(AmbientOcclusionComponent.Uniforms._Radius, settings.radius);
      material2.SetFloat(AmbientOcclusionComponent.Uniforms._Downsample, !settings.downsampling ? 1f : 0.5f);
      material2.SetInt(AmbientOcclusionComponent.Uniforms._SampleCount, (int) settings.sampleCount);
      if (!this.context.isGBufferAvailable && RenderSettings.get_fog())
      {
        material2.SetVector(AmbientOcclusionComponent.Uniforms._FogParams, Vector4.op_Implicit(new Vector3(RenderSettings.get_fogDensity(), RenderSettings.get_fogStartDistance(), RenderSettings.get_fogEndDistance())));
        FogMode fogMode = RenderSettings.get_fogMode();
        if (fogMode != 1)
        {
          if (fogMode != 2)
          {
            if (fogMode == 3)
              material2.EnableKeyword("FOG_EXP2");
          }
          else
            material2.EnableKeyword("FOG_EXP");
        }
        else
          material2.EnableKeyword("FOG_LINEAR");
      }
      else
        material2.EnableKeyword("FOG_OFF");
      int width = this.context.width;
      int height = this.context.height;
      int num = !settings.downsampling ? 1 : 2;
      int occlusionTexture1 = AmbientOcclusionComponent.Uniforms._OcclusionTexture1;
      cb.GetTemporaryRT(occlusionTexture1, width / num, height / num, 0, (FilterMode) 1, (RenderTextureFormat) 0, (RenderTextureReadWrite) 1);
      cb.Blit((Texture) null, RenderTargetIdentifier.op_Implicit(occlusionTexture1), material2, (int) this.occlusionSource);
      int occlusionTexture2 = AmbientOcclusionComponent.Uniforms._OcclusionTexture2;
      cb.GetTemporaryRT(occlusionTexture2, width, height, 0, (FilterMode) 1, (RenderTextureFormat) 0, (RenderTextureReadWrite) 1);
      cb.SetGlobalTexture(AmbientOcclusionComponent.Uniforms._MainTex, RenderTargetIdentifier.op_Implicit(occlusionTexture1));
      cb.Blit(RenderTargetIdentifier.op_Implicit(occlusionTexture1), RenderTargetIdentifier.op_Implicit(occlusionTexture2), material2, this.occlusionSource != AmbientOcclusionComponent.OcclusionSource.GBuffer ? 3 : 4);
      cb.ReleaseTemporaryRT(occlusionTexture1);
      int occlusionTexture = AmbientOcclusionComponent.Uniforms._OcclusionTexture;
      cb.GetTemporaryRT(occlusionTexture, width, height, 0, (FilterMode) 1, (RenderTextureFormat) 0, (RenderTextureReadWrite) 1);
      cb.SetGlobalTexture(AmbientOcclusionComponent.Uniforms._MainTex, RenderTargetIdentifier.op_Implicit(occlusionTexture2));
      cb.Blit(RenderTargetIdentifier.op_Implicit(occlusionTexture2), RenderTargetIdentifier.op_Implicit(occlusionTexture), material2, 5);
      cb.ReleaseTemporaryRT(occlusionTexture2);
      if (this.context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.AmbientOcclusion))
      {
        cb.SetGlobalTexture(AmbientOcclusionComponent.Uniforms._MainTex, RenderTargetIdentifier.op_Implicit(occlusionTexture));
        cb.Blit(RenderTargetIdentifier.op_Implicit(occlusionTexture), RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), material2, 8);
        this.context.Interrupt();
      }
      else if (this.ambientOnlySupported)
      {
        cb.SetRenderTarget(this.m_MRT, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2));
        cb.DrawMesh(GraphicsUtils.quad, Matrix4x4.get_identity(), material2, 0, 7);
      }
      else
      {
        RenderTextureFormat renderTextureFormat = !this.context.isHdr ? (RenderTextureFormat) 7 : (RenderTextureFormat) 9;
        int tempRt = AmbientOcclusionComponent.Uniforms._TempRT;
        cb.GetTemporaryRT(tempRt, this.context.width, this.context.height, 0, (FilterMode) 1, renderTextureFormat);
        cb.Blit(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), RenderTargetIdentifier.op_Implicit(tempRt), material1, 0);
        cb.SetGlobalTexture(AmbientOcclusionComponent.Uniforms._MainTex, RenderTargetIdentifier.op_Implicit(tempRt));
        cb.Blit(RenderTargetIdentifier.op_Implicit(tempRt), RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), material2, 6);
        cb.ReleaseTemporaryRT(tempRt);
      }
      cb.ReleaseTemporaryRT(occlusionTexture);
    }

    private static class Uniforms
    {
      internal static readonly int _Intensity = Shader.PropertyToID(nameof (_Intensity));
      internal static readonly int _Radius = Shader.PropertyToID(nameof (_Radius));
      internal static readonly int _FogParams = Shader.PropertyToID(nameof (_FogParams));
      internal static readonly int _Downsample = Shader.PropertyToID(nameof (_Downsample));
      internal static readonly int _SampleCount = Shader.PropertyToID(nameof (_SampleCount));
      internal static readonly int _OcclusionTexture1 = Shader.PropertyToID(nameof (_OcclusionTexture1));
      internal static readonly int _OcclusionTexture2 = Shader.PropertyToID(nameof (_OcclusionTexture2));
      internal static readonly int _OcclusionTexture = Shader.PropertyToID(nameof (_OcclusionTexture));
      internal static readonly int _MainTex = Shader.PropertyToID(nameof (_MainTex));
      internal static readonly int _TempRT = Shader.PropertyToID(nameof (_TempRT));
    }

    private enum OcclusionSource
    {
      DepthTexture,
      DepthNormalsTexture,
      GBuffer,
    }
  }
}
