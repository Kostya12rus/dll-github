// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.ScreenSpaceReflectionComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
  public sealed class ScreenSpaceReflectionComponent : PostProcessingComponentCommandBuffer<ScreenSpaceReflectionModel>
  {
    private bool k_TraceBehindObjects = true;
    private bool k_BilateralUpsample = true;
    private readonly int[] m_ReflectionTextures = new int[5];
    private bool k_HighlightSuppression;
    private bool k_TreatBackfaceHitAsMiss;

    public override DepthTextureMode GetCameraFlags()
    {
      return (DepthTextureMode) 1;
    }

    public override bool active
    {
      get
      {
        if (this.model.enabled && this.context.isGBufferAvailable)
          return !this.context.interrupted;
        return false;
      }
    }

    public override void OnEnable()
    {
      this.m_ReflectionTextures[0] = Shader.PropertyToID("_ReflectionTexture0");
      this.m_ReflectionTextures[1] = Shader.PropertyToID("_ReflectionTexture1");
      this.m_ReflectionTextures[2] = Shader.PropertyToID("_ReflectionTexture2");
      this.m_ReflectionTextures[3] = Shader.PropertyToID("_ReflectionTexture3");
      this.m_ReflectionTextures[4] = Shader.PropertyToID("_ReflectionTexture4");
    }

    public override string GetName()
    {
      return "Screen Space Reflection";
    }

    public override CameraEvent GetCameraEvent()
    {
      return (CameraEvent) 9;
    }

    public override void PopulateCommandBuffer(CommandBuffer cb)
    {
      ScreenSpaceReflectionModel.Settings settings = this.model.settings;
      Camera camera = this.context.camera;
      int num1 = settings.reflection.reflectionQuality != ScreenSpaceReflectionModel.SSRResolution.High ? 2 : 1;
      int num2 = this.context.width / num1;
      int num3 = this.context.height / num1;
      float width = (float) this.context.width;
      float height = (float) this.context.height;
      float num4 = width / 2f;
      float num5 = height / 2f;
      Material material1 = this.context.materialFactory.Get("Hidden/Post FX/Screen Space Reflection");
      material1.SetInt(ScreenSpaceReflectionComponent.Uniforms._RayStepSize, settings.reflection.stepSize);
      material1.SetInt(ScreenSpaceReflectionComponent.Uniforms._AdditiveReflection, settings.reflection.blendType != ScreenSpaceReflectionModel.SSRReflectionBlendType.Additive ? 0 : 1);
      material1.SetInt(ScreenSpaceReflectionComponent.Uniforms._BilateralUpsampling, !this.k_BilateralUpsample ? 0 : 1);
      material1.SetInt(ScreenSpaceReflectionComponent.Uniforms._TreatBackfaceHitAsMiss, !this.k_TreatBackfaceHitAsMiss ? 0 : 1);
      material1.SetInt(ScreenSpaceReflectionComponent.Uniforms._AllowBackwardsRays, !settings.reflection.reflectBackfaces ? 0 : 1);
      material1.SetInt(ScreenSpaceReflectionComponent.Uniforms._TraceBehindObjects, !this.k_TraceBehindObjects ? 0 : 1);
      material1.SetInt(ScreenSpaceReflectionComponent.Uniforms._MaxSteps, settings.reflection.iterationCount);
      material1.SetInt(ScreenSpaceReflectionComponent.Uniforms._FullResolutionFiltering, 0);
      material1.SetInt(ScreenSpaceReflectionComponent.Uniforms._HalfResolution, settings.reflection.reflectionQuality == ScreenSpaceReflectionModel.SSRResolution.High ? 0 : 1);
      material1.SetInt(ScreenSpaceReflectionComponent.Uniforms._HighlightSuppression, !this.k_HighlightSuppression ? 0 : 1);
      float num6 = width / (-2f * Mathf.Tan((float) ((double) camera.get_fieldOfView() / 180.0 * 3.14159274101257 * 0.5)));
      material1.SetFloat(ScreenSpaceReflectionComponent.Uniforms._PixelsPerMeterAtOneMeter, num6);
      material1.SetFloat(ScreenSpaceReflectionComponent.Uniforms._ScreenEdgeFading, settings.screenEdgeMask.intensity);
      material1.SetFloat(ScreenSpaceReflectionComponent.Uniforms._ReflectionBlur, settings.reflection.reflectionBlur);
      material1.SetFloat(ScreenSpaceReflectionComponent.Uniforms._MaxRayTraceDistance, settings.reflection.maxDistance);
      material1.SetFloat(ScreenSpaceReflectionComponent.Uniforms._FadeDistance, settings.intensity.fadeDistance);
      material1.SetFloat(ScreenSpaceReflectionComponent.Uniforms._LayerThickness, settings.reflection.widthModifier);
      material1.SetFloat(ScreenSpaceReflectionComponent.Uniforms._SSRMultiplier, settings.intensity.reflectionMultiplier);
      material1.SetFloat(ScreenSpaceReflectionComponent.Uniforms._FresnelFade, settings.intensity.fresnelFade);
      material1.SetFloat(ScreenSpaceReflectionComponent.Uniforms._FresnelFadePower, settings.intensity.fresnelFadePower);
      Matrix4x4 projectionMatrix = camera.get_projectionMatrix();
      Vector4 vector4;
      ((Vector4) ref vector4).\u002Ector((float) (-2.0 / ((double) width * (double) ((Matrix4x4) ref projectionMatrix).get_Item(0))), (float) (-2.0 / ((double) height * (double) ((Matrix4x4) ref projectionMatrix).get_Item(5))), (1f - ((Matrix4x4) ref projectionMatrix).get_Item(2)) / ((Matrix4x4) ref projectionMatrix).get_Item(0), (1f + ((Matrix4x4) ref projectionMatrix).get_Item(6)) / ((Matrix4x4) ref projectionMatrix).get_Item(5));
      Vector3 vector3 = !float.IsPositiveInfinity(camera.get_farClipPlane()) ? new Vector3(camera.get_nearClipPlane() * camera.get_farClipPlane(), camera.get_nearClipPlane() - camera.get_farClipPlane(), camera.get_farClipPlane()) : new Vector3(camera.get_nearClipPlane(), -1f, 1f);
      material1.SetVector(ScreenSpaceReflectionComponent.Uniforms._ReflectionBufferSize, Vector4.op_Implicit(new Vector2((float) num2, (float) num3)));
      material1.SetVector(ScreenSpaceReflectionComponent.Uniforms._ScreenSize, Vector4.op_Implicit(new Vector2(width, height)));
      material1.SetVector(ScreenSpaceReflectionComponent.Uniforms._InvScreenSize, Vector4.op_Implicit(new Vector2(1f / width, 1f / height)));
      material1.SetVector(ScreenSpaceReflectionComponent.Uniforms._ProjInfo, vector4);
      material1.SetVector(ScreenSpaceReflectionComponent.Uniforms._CameraClipInfo, Vector4.op_Implicit(vector3));
      Matrix4x4 matrix4x4_1 = (Matrix4x4) null;
      ((Matrix4x4) ref matrix4x4_1).SetRow(0, new Vector4(num4, 0.0f, 0.0f, num4));
      ((Matrix4x4) ref matrix4x4_1).SetRow(1, new Vector4(0.0f, num5, 0.0f, num5));
      ((Matrix4x4) ref matrix4x4_1).SetRow(2, new Vector4(0.0f, 0.0f, 1f, 0.0f));
      ((Matrix4x4) ref matrix4x4_1).SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1f));
      Matrix4x4 matrix4x4_2 = Matrix4x4.op_Multiply(matrix4x4_1, projectionMatrix);
      material1.SetMatrix(ScreenSpaceReflectionComponent.Uniforms._ProjectToPixelMatrix, matrix4x4_2);
      material1.SetMatrix(ScreenSpaceReflectionComponent.Uniforms._WorldToCameraMatrix, camera.get_worldToCameraMatrix());
      Material material2 = material1;
      int cameraToWorldMatrix = ScreenSpaceReflectionComponent.Uniforms._CameraToWorldMatrix;
      Matrix4x4 worldToCameraMatrix = camera.get_worldToCameraMatrix();
      Matrix4x4 inverse = ((Matrix4x4) ref worldToCameraMatrix).get_inverse();
      material2.SetMatrix(cameraToWorldMatrix, inverse);
      RenderTextureFormat renderTextureFormat = !this.context.isHdr ? (RenderTextureFormat) 0 : (RenderTextureFormat) 2;
      int roughnessTexture = ScreenSpaceReflectionComponent.Uniforms._NormalAndRoughnessTexture;
      int hitPointTexture = ScreenSpaceReflectionComponent.Uniforms._HitPointTexture;
      int blurTexture = ScreenSpaceReflectionComponent.Uniforms._BlurTexture;
      int filteredReflections = ScreenSpaceReflectionComponent.Uniforms._FilteredReflections;
      int reflectionTexture1 = ScreenSpaceReflectionComponent.Uniforms._FinalReflectionTexture;
      int tempTexture = ScreenSpaceReflectionComponent.Uniforms._TempTexture;
      cb.GetTemporaryRT(roughnessTexture, -1, -1, 0, (FilterMode) 0, (RenderTextureFormat) 0, (RenderTextureReadWrite) 1);
      cb.GetTemporaryRT(hitPointTexture, num2, num3, 0, (FilterMode) 1, (RenderTextureFormat) 2, (RenderTextureReadWrite) 1);
      for (int index = 0; index < 5; ++index)
        cb.GetTemporaryRT(this.m_ReflectionTextures[index], num2 >> index, num3 >> index, 0, (FilterMode) 1, renderTextureFormat);
      cb.GetTemporaryRT(filteredReflections, num2, num3, 0, !this.k_BilateralUpsample ? (FilterMode) 1 : (FilterMode) 0, renderTextureFormat);
      cb.GetTemporaryRT(reflectionTexture1, num2, num3, 0, (FilterMode) 0, renderTextureFormat);
      cb.Blit(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), RenderTargetIdentifier.op_Implicit(roughnessTexture), material1, 6);
      cb.Blit(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), RenderTargetIdentifier.op_Implicit(hitPointTexture), material1, 0);
      cb.Blit(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), RenderTargetIdentifier.op_Implicit(filteredReflections), material1, 5);
      cb.Blit(RenderTargetIdentifier.op_Implicit(filteredReflections), RenderTargetIdentifier.op_Implicit(this.m_ReflectionTextures[0]), material1, 8);
      for (int index = 1; index < 5; ++index)
      {
        int reflectionTexture2 = this.m_ReflectionTextures[index - 1];
        int num7 = index;
        cb.GetTemporaryRT(blurTexture, num2 >> num7, num3 >> num7, 0, (FilterMode) 1, renderTextureFormat);
        cb.SetGlobalVector(ScreenSpaceReflectionComponent.Uniforms._Axis, new Vector4(1f, 0.0f, 0.0f, 0.0f));
        cb.SetGlobalFloat(ScreenSpaceReflectionComponent.Uniforms._CurrentMipLevel, (float) index - 1f);
        cb.Blit(RenderTargetIdentifier.op_Implicit(reflectionTexture2), RenderTargetIdentifier.op_Implicit(blurTexture), material1, 2);
        cb.SetGlobalVector(ScreenSpaceReflectionComponent.Uniforms._Axis, new Vector4(0.0f, 1f, 0.0f, 0.0f));
        int reflectionTexture3 = this.m_ReflectionTextures[index];
        cb.Blit(RenderTargetIdentifier.op_Implicit(blurTexture), RenderTargetIdentifier.op_Implicit(reflectionTexture3), material1, 2);
        cb.ReleaseTemporaryRT(blurTexture);
      }
      cb.Blit(RenderTargetIdentifier.op_Implicit(this.m_ReflectionTextures[0]), RenderTargetIdentifier.op_Implicit(reflectionTexture1), material1, 3);
      cb.GetTemporaryRT(tempTexture, camera.get_pixelWidth(), camera.get_pixelHeight(), 0, (FilterMode) 1, renderTextureFormat);
      cb.Blit(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2), RenderTargetIdentifier.op_Implicit(tempTexture), material1, 1);
      cb.Blit(RenderTargetIdentifier.op_Implicit(tempTexture), RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType) 2));
      cb.ReleaseTemporaryRT(tempTexture);
    }

    private static class Uniforms
    {
      internal static readonly int _RayStepSize = Shader.PropertyToID(nameof (_RayStepSize));
      internal static readonly int _AdditiveReflection = Shader.PropertyToID(nameof (_AdditiveReflection));
      internal static readonly int _BilateralUpsampling = Shader.PropertyToID(nameof (_BilateralUpsampling));
      internal static readonly int _TreatBackfaceHitAsMiss = Shader.PropertyToID(nameof (_TreatBackfaceHitAsMiss));
      internal static readonly int _AllowBackwardsRays = Shader.PropertyToID(nameof (_AllowBackwardsRays));
      internal static readonly int _TraceBehindObjects = Shader.PropertyToID(nameof (_TraceBehindObjects));
      internal static readonly int _MaxSteps = Shader.PropertyToID(nameof (_MaxSteps));
      internal static readonly int _FullResolutionFiltering = Shader.PropertyToID(nameof (_FullResolutionFiltering));
      internal static readonly int _HalfResolution = Shader.PropertyToID(nameof (_HalfResolution));
      internal static readonly int _HighlightSuppression = Shader.PropertyToID(nameof (_HighlightSuppression));
      internal static readonly int _PixelsPerMeterAtOneMeter = Shader.PropertyToID(nameof (_PixelsPerMeterAtOneMeter));
      internal static readonly int _ScreenEdgeFading = Shader.PropertyToID(nameof (_ScreenEdgeFading));
      internal static readonly int _ReflectionBlur = Shader.PropertyToID(nameof (_ReflectionBlur));
      internal static readonly int _MaxRayTraceDistance = Shader.PropertyToID(nameof (_MaxRayTraceDistance));
      internal static readonly int _FadeDistance = Shader.PropertyToID(nameof (_FadeDistance));
      internal static readonly int _LayerThickness = Shader.PropertyToID(nameof (_LayerThickness));
      internal static readonly int _SSRMultiplier = Shader.PropertyToID(nameof (_SSRMultiplier));
      internal static readonly int _FresnelFade = Shader.PropertyToID(nameof (_FresnelFade));
      internal static readonly int _FresnelFadePower = Shader.PropertyToID(nameof (_FresnelFadePower));
      internal static readonly int _ReflectionBufferSize = Shader.PropertyToID(nameof (_ReflectionBufferSize));
      internal static readonly int _ScreenSize = Shader.PropertyToID(nameof (_ScreenSize));
      internal static readonly int _InvScreenSize = Shader.PropertyToID(nameof (_InvScreenSize));
      internal static readonly int _ProjInfo = Shader.PropertyToID(nameof (_ProjInfo));
      internal static readonly int _CameraClipInfo = Shader.PropertyToID(nameof (_CameraClipInfo));
      internal static readonly int _ProjectToPixelMatrix = Shader.PropertyToID(nameof (_ProjectToPixelMatrix));
      internal static readonly int _WorldToCameraMatrix = Shader.PropertyToID(nameof (_WorldToCameraMatrix));
      internal static readonly int _CameraToWorldMatrix = Shader.PropertyToID(nameof (_CameraToWorldMatrix));
      internal static readonly int _Axis = Shader.PropertyToID(nameof (_Axis));
      internal static readonly int _CurrentMipLevel = Shader.PropertyToID(nameof (_CurrentMipLevel));
      internal static readonly int _NormalAndRoughnessTexture = Shader.PropertyToID(nameof (_NormalAndRoughnessTexture));
      internal static readonly int _HitPointTexture = Shader.PropertyToID(nameof (_HitPointTexture));
      internal static readonly int _BlurTexture = Shader.PropertyToID(nameof (_BlurTexture));
      internal static readonly int _FilteredReflections = Shader.PropertyToID(nameof (_FilteredReflections));
      internal static readonly int _FinalReflectionTexture = Shader.PropertyToID(nameof (_FinalReflectionTexture));
      internal static readonly int _TempTexture = Shader.PropertyToID(nameof (_TempTexture));
    }

    private enum PassIndex
    {
      RayTraceStep,
      CompositeFinal,
      Blur,
      CompositeSSR,
      MinMipGeneration,
      HitPointToReflections,
      BilateralKeyPack,
      BlitDepthAsCSZ,
      PoissonBlur,
    }
  }
}
