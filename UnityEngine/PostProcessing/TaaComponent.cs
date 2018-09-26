// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.TaaComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace UnityEngine.PostProcessing
{
  public sealed class TaaComponent : PostProcessingComponentRenderTexture<AntialiasingModel>
  {
    private readonly RenderBuffer[] m_MRT = new RenderBuffer[2];
    private bool m_ResetHistory = true;
    private const string k_ShaderString = "Hidden/Post FX/Temporal Anti-aliasing";
    private const int k_SampleCount = 8;
    private int m_SampleIndex;
    private RenderTexture m_HistoryTexture;

    public override bool active
    {
      get
      {
        if (this.model.enabled && (this.model.settings.method == AntialiasingModel.Method.Taa && SystemInfo.get_supportsMotionVectors() && SystemInfo.get_supportedRenderTargetCount() >= 2))
          return !this.context.interrupted;
        return false;
      }
    }

    public override DepthTextureMode GetCameraFlags()
    {
      return (DepthTextureMode) 5;
    }

    public Vector2 jitterVector { get; private set; }

    public void ResetHistory()
    {
      this.m_ResetHistory = true;
    }

    public void SetProjectionMatrix(Func<Vector2, Matrix4x4> jitteredFunc)
    {
      Vector2 offset = Vector2.op_Multiply(this.GenerateRandomOffset(), this.model.settings.taaSettings.jitterSpread);
      this.context.camera.set_nonJitteredProjectionMatrix(this.context.camera.get_projectionMatrix());
      if (jitteredFunc != null)
        this.context.camera.set_projectionMatrix(jitteredFunc(offset));
      else
        this.context.camera.set_projectionMatrix(!this.context.camera.get_orthographic() ? this.GetPerspectiveProjectionMatrix(offset) : this.GetOrthographicProjectionMatrix(offset));
      this.context.camera.set_useJitteredProjectionMatrixForTransparentRendering(false);
      ref Vector2 local1 = ref offset;
      local1.x = (__Null) (local1.x / (double) this.context.width);
      ref Vector2 local2 = ref offset;
      local2.y = (__Null) (local2.y / (double) this.context.height);
      this.context.materialFactory.Get("Hidden/Post FX/Temporal Anti-aliasing").SetVector(TaaComponent.Uniforms._Jitter, Vector4.op_Implicit(offset));
      this.jitterVector = offset;
    }

    public void Render(RenderTexture source, RenderTexture destination)
    {
      Material material = this.context.materialFactory.Get("Hidden/Post FX/Temporal Anti-aliasing");
      material.set_shaderKeywords((string[]) null);
      AntialiasingModel.TaaSettings taaSettings = this.model.settings.taaSettings;
      if (this.m_ResetHistory || Object.op_Equality((Object) this.m_HistoryTexture, (Object) null) || (((Texture) this.m_HistoryTexture).get_width() != ((Texture) source).get_width() || ((Texture) this.m_HistoryTexture).get_height() != ((Texture) source).get_height()))
      {
        if (Object.op_Implicit((Object) this.m_HistoryTexture))
          RenderTexture.ReleaseTemporary(this.m_HistoryTexture);
        this.m_HistoryTexture = RenderTexture.GetTemporary(((Texture) source).get_width(), ((Texture) source).get_height(), 0, source.get_format());
        ((Object) this.m_HistoryTexture).set_name("TAA History");
        Graphics.Blit((Texture) source, this.m_HistoryTexture, material, 2);
      }
      material.SetVector(TaaComponent.Uniforms._SharpenParameters, new Vector4(taaSettings.sharpen, 0.0f, 0.0f, 0.0f));
      material.SetVector(TaaComponent.Uniforms._FinalBlendParameters, new Vector4(taaSettings.stationaryBlending, taaSettings.motionBlending, 6000f, 0.0f));
      material.SetTexture(TaaComponent.Uniforms._MainTex, (Texture) source);
      material.SetTexture(TaaComponent.Uniforms._HistoryTex, (Texture) this.m_HistoryTexture);
      RenderTexture temporary = RenderTexture.GetTemporary(((Texture) source).get_width(), ((Texture) source).get_height(), 0, source.get_format());
      ((Object) temporary).set_name("TAA History");
      this.m_MRT[0] = destination.get_colorBuffer();
      this.m_MRT[1] = temporary.get_colorBuffer();
      Graphics.SetRenderTarget(this.m_MRT, source.get_depthBuffer());
      GraphicsUtils.Blit(material, !this.context.camera.get_orthographic() ? 0 : 1);
      RenderTexture.ReleaseTemporary(this.m_HistoryTexture);
      this.m_HistoryTexture = temporary;
      this.m_ResetHistory = false;
    }

    private float GetHaltonValue(int index, int radix)
    {
      float num1 = 0.0f;
      float num2 = 1f / (float) radix;
      while (index > 0)
      {
        num1 += (float) (index % radix) * num2;
        index /= radix;
        num2 /= (float) radix;
      }
      return num1;
    }

    private Vector2 GenerateRandomOffset()
    {
      Vector2 vector2;
      ((Vector2) ref vector2).\u002Ector(this.GetHaltonValue(this.m_SampleIndex & 1023, 2), this.GetHaltonValue(this.m_SampleIndex & 1023, 3));
      if (++this.m_SampleIndex >= 8)
        this.m_SampleIndex = 0;
      return vector2;
    }

    private Matrix4x4 GetPerspectiveProjectionMatrix(Vector2 offset)
    {
      float num1 = Mathf.Tan((float) Math.PI / 360f * this.context.camera.get_fieldOfView());
      float num2 = num1 * this.context.camera.get_aspect();
      ref Vector2 local1 = ref offset;
      local1.x = (__Null) (local1.x * ((double) num2 / (0.5 * (double) this.context.width)));
      ref Vector2 local2 = ref offset;
      local2.y = (__Null) (local2.y * ((double) num1 / (0.5 * (double) this.context.height)));
      float num3 = ((float) offset.x - num2) * this.context.camera.get_nearClipPlane();
      float num4 = ((float) offset.x + num2) * this.context.camera.get_nearClipPlane();
      float num5 = ((float) offset.y + num1) * this.context.camera.get_nearClipPlane();
      float num6 = ((float) offset.y - num1) * this.context.camera.get_nearClipPlane();
      Matrix4x4 matrix4x4 = (Matrix4x4) null;
      ((Matrix4x4) ref matrix4x4).set_Item(0, 0, (float) (2.0 * (double) this.context.camera.get_nearClipPlane() / ((double) num4 - (double) num3)));
      ((Matrix4x4) ref matrix4x4).set_Item(0, 1, 0.0f);
      ((Matrix4x4) ref matrix4x4).set_Item(0, 2, (float) (((double) num4 + (double) num3) / ((double) num4 - (double) num3)));
      ((Matrix4x4) ref matrix4x4).set_Item(0, 3, 0.0f);
      ((Matrix4x4) ref matrix4x4).set_Item(1, 0, 0.0f);
      ((Matrix4x4) ref matrix4x4).set_Item(1, 1, (float) (2.0 * (double) this.context.camera.get_nearClipPlane() / ((double) num5 - (double) num6)));
      ((Matrix4x4) ref matrix4x4).set_Item(1, 2, (float) (((double) num5 + (double) num6) / ((double) num5 - (double) num6)));
      ((Matrix4x4) ref matrix4x4).set_Item(1, 3, 0.0f);
      ((Matrix4x4) ref matrix4x4).set_Item(2, 0, 0.0f);
      ((Matrix4x4) ref matrix4x4).set_Item(2, 1, 0.0f);
      ((Matrix4x4) ref matrix4x4).set_Item(2, 2, (float) (-((double) this.context.camera.get_farClipPlane() + (double) this.context.camera.get_nearClipPlane()) / ((double) this.context.camera.get_farClipPlane() - (double) this.context.camera.get_nearClipPlane())));
      ((Matrix4x4) ref matrix4x4).set_Item(2, 3, (float) (-(2.0 * (double) this.context.camera.get_farClipPlane() * (double) this.context.camera.get_nearClipPlane()) / ((double) this.context.camera.get_farClipPlane() - (double) this.context.camera.get_nearClipPlane())));
      ((Matrix4x4) ref matrix4x4).set_Item(3, 0, 0.0f);
      ((Matrix4x4) ref matrix4x4).set_Item(3, 1, 0.0f);
      ((Matrix4x4) ref matrix4x4).set_Item(3, 2, -1f);
      ((Matrix4x4) ref matrix4x4).set_Item(3, 3, 0.0f);
      return matrix4x4;
    }

    private Matrix4x4 GetOrthographicProjectionMatrix(Vector2 offset)
    {
      float orthographicSize = this.context.camera.get_orthographicSize();
      float num1 = orthographicSize * this.context.camera.get_aspect();
      ref Vector2 local1 = ref offset;
      local1.x = (__Null) (local1.x * ((double) num1 / (0.5 * (double) this.context.width)));
      ref Vector2 local2 = ref offset;
      local2.y = (__Null) (local2.y * ((double) orthographicSize / (0.5 * (double) this.context.height)));
      float num2 = (float) offset.x - num1;
      float num3 = (float) offset.x + num1;
      float num4 = (float) offset.y + orthographicSize;
      float num5 = (float) offset.y - orthographicSize;
      return Matrix4x4.Ortho(num2, num3, num5, num4, this.context.camera.get_nearClipPlane(), this.context.camera.get_farClipPlane());
    }

    public override void OnDisable()
    {
      if (Object.op_Inequality((Object) this.m_HistoryTexture, (Object) null))
        RenderTexture.ReleaseTemporary(this.m_HistoryTexture);
      this.m_HistoryTexture = (RenderTexture) null;
      this.m_SampleIndex = 0;
      this.ResetHistory();
    }

    private static class Uniforms
    {
      internal static int _Jitter = Shader.PropertyToID(nameof (_Jitter));
      internal static int _SharpenParameters = Shader.PropertyToID(nameof (_SharpenParameters));
      internal static int _FinalBlendParameters = Shader.PropertyToID(nameof (_FinalBlendParameters));
      internal static int _HistoryTex = Shader.PropertyToID(nameof (_HistoryTex));
      internal static int _MainTex = Shader.PropertyToID(nameof (_MainTex));
    }
  }
}
