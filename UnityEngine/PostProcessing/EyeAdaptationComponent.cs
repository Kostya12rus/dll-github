﻿// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.EyeAdaptationComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace UnityEngine.PostProcessing
{
  public sealed class EyeAdaptationComponent : PostProcessingComponentRenderTexture<EyeAdaptationModel>
  {
    private readonly RenderTexture[] m_AutoExposurePool = new RenderTexture[2];
    private bool m_FirstFrame = true;
    private ComputeShader m_EyeCompute;
    private ComputeBuffer m_HistogramBuffer;
    private int m_AutoExposurePingPing;
    private RenderTexture m_CurrentAutoExposure;
    private RenderTexture m_DebugHistogram;
    private static uint[] s_EmptyHistogramBuffer;
    private const int k_HistogramBins = 64;
    private const int k_HistogramThreadX = 16;
    private const int k_HistogramThreadY = 16;

    public override bool active
    {
      get
      {
        if (this.model.enabled && SystemInfo.supportsComputeShaders)
          return !this.context.interrupted;
        return false;
      }
    }

    public void ResetHistory()
    {
      this.m_FirstFrame = true;
    }

    public override void OnEnable()
    {
      this.m_FirstFrame = true;
    }

    public override void OnDisable()
    {
      foreach (Object @object in this.m_AutoExposurePool)
        GraphicsUtils.Destroy(@object);
      if (this.m_HistogramBuffer != null)
        this.m_HistogramBuffer.Release();
      this.m_HistogramBuffer = (ComputeBuffer) null;
      if ((Object) this.m_DebugHistogram != (Object) null)
        this.m_DebugHistogram.Release();
      this.m_DebugHistogram = (RenderTexture) null;
    }

    private Vector4 GetHistogramScaleOffsetRes()
    {
      EyeAdaptationModel.Settings settings = this.model.settings;
      float x = 1f / (float) (settings.logMax - settings.logMin);
      float y = (float) -settings.logMin * x;
      return new Vector4(x, y, Mathf.Floor((float) this.context.width / 2f), Mathf.Floor((float) this.context.height / 2f));
    }

    public Texture Prepare(RenderTexture source, Material uberMaterial)
    {
      EyeAdaptationModel.Settings settings = this.model.settings;
      if ((Object) this.m_EyeCompute == (Object) null)
        this.m_EyeCompute = Resources.Load<ComputeShader>("Shaders/EyeHistogram");
      Material mat = this.context.materialFactory.Get("Hidden/Post FX/Eye Adaptation");
      mat.shaderKeywords = (string[]) null;
      if (this.m_HistogramBuffer == null)
        this.m_HistogramBuffer = new ComputeBuffer(64, 4);
      if (EyeAdaptationComponent.s_EmptyHistogramBuffer == null)
        EyeAdaptationComponent.s_EmptyHistogramBuffer = new uint[64];
      Vector4 histogramScaleOffsetRes = this.GetHistogramScaleOffsetRes();
      RenderTexture renderTexture1 = this.context.renderTextureFactory.Get((int) histogramScaleOffsetRes.z, (int) histogramScaleOffsetRes.w, 0, source.format, RenderTextureReadWrite.Default, FilterMode.Bilinear, TextureWrapMode.Clamp, "FactoryTempTexture");
      Graphics.Blit((Texture) source, renderTexture1);
      if ((Object) this.m_AutoExposurePool[0] == (Object) null || !this.m_AutoExposurePool[0].IsCreated())
        this.m_AutoExposurePool[0] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
      if ((Object) this.m_AutoExposurePool[1] == (Object) null || !this.m_AutoExposurePool[1].IsCreated())
        this.m_AutoExposurePool[1] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
      this.m_HistogramBuffer.SetData((Array) EyeAdaptationComponent.s_EmptyHistogramBuffer);
      int kernel = this.m_EyeCompute.FindKernel("KEyeHistogram");
      this.m_EyeCompute.SetBuffer(kernel, "_Histogram", this.m_HistogramBuffer);
      this.m_EyeCompute.SetTexture(kernel, "_Source", (Texture) renderTexture1);
      this.m_EyeCompute.SetVector("_ScaleOffsetRes", histogramScaleOffsetRes);
      this.m_EyeCompute.Dispatch(kernel, Mathf.CeilToInt((float) renderTexture1.width / 16f), Mathf.CeilToInt((float) renderTexture1.height / 16f), 1);
      this.context.renderTextureFactory.Release(renderTexture1);
      settings.highPercent = Mathf.Clamp(settings.highPercent, 1.01f, 99f);
      settings.lowPercent = Mathf.Clamp(settings.lowPercent, 1f, settings.highPercent - 0.01f);
      mat.SetBuffer("_Histogram", this.m_HistogramBuffer);
      mat.SetVector(EyeAdaptationComponent.Uniforms._Params, new Vector4(settings.lowPercent * 0.01f, settings.highPercent * 0.01f, Mathf.Exp(settings.minLuminance * 0.6931472f), Mathf.Exp(settings.maxLuminance * 0.6931472f)));
      mat.SetVector(EyeAdaptationComponent.Uniforms._Speed, (Vector4) new Vector2(settings.speedDown, settings.speedUp));
      mat.SetVector(EyeAdaptationComponent.Uniforms._ScaleOffsetRes, histogramScaleOffsetRes);
      mat.SetFloat(EyeAdaptationComponent.Uniforms._ExposureCompensation, settings.keyValue);
      if (settings.dynamicKeyValue)
        mat.EnableKeyword("AUTO_KEY_VALUE");
      if (this.m_FirstFrame || !Application.isPlaying)
      {
        this.m_CurrentAutoExposure = this.m_AutoExposurePool[0];
        Graphics.Blit((Texture) null, this.m_CurrentAutoExposure, mat, 1);
        Graphics.Blit((Texture) this.m_AutoExposurePool[0], this.m_AutoExposurePool[1]);
      }
      else
      {
        int exposurePingPing = this.m_AutoExposurePingPing;
        int num1;
        RenderTexture renderTexture2 = this.m_AutoExposurePool[(num1 = exposurePingPing + 1) % 2];
        int num2;
        RenderTexture dest = this.m_AutoExposurePool[(num2 = num1 + 1) % 2];
        Graphics.Blit((Texture) renderTexture2, dest, mat, (int) settings.adaptationType);
        int num3;
        this.m_AutoExposurePingPing = (num3 = num2 + 1) % 2;
        this.m_CurrentAutoExposure = dest;
      }
      if (this.context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.EyeAdaptation))
      {
        if ((Object) this.m_DebugHistogram == (Object) null || !this.m_DebugHistogram.IsCreated())
        {
          RenderTexture renderTexture2 = new RenderTexture(256, 128, 0, RenderTextureFormat.ARGB32);
          renderTexture2.filterMode = FilterMode.Point;
          renderTexture2.wrapMode = TextureWrapMode.Clamp;
          this.m_DebugHistogram = renderTexture2;
        }
        mat.SetFloat(EyeAdaptationComponent.Uniforms._DebugWidth, (float) this.m_DebugHistogram.width);
        Graphics.Blit((Texture) null, this.m_DebugHistogram, mat, 2);
      }
      this.m_FirstFrame = false;
      return (Texture) this.m_CurrentAutoExposure;
    }

    public void OnGUI()
    {
      if ((Object) this.m_DebugHistogram == (Object) null || !this.m_DebugHistogram.IsCreated())
        return;
      GUI.DrawTexture(new Rect((float) ((double) this.context.viewport.x * (double) Screen.width + 8.0), 8f, (float) this.m_DebugHistogram.width, (float) this.m_DebugHistogram.height), (Texture) this.m_DebugHistogram);
    }

    private static class Uniforms
    {
      internal static readonly int _Params = Shader.PropertyToID(nameof (_Params));
      internal static readonly int _Speed = Shader.PropertyToID(nameof (_Speed));
      internal static readonly int _ScaleOffsetRes = Shader.PropertyToID(nameof (_ScaleOffsetRes));
      internal static readonly int _ExposureCompensation = Shader.PropertyToID(nameof (_ExposureCompensation));
      internal static readonly int _AutoExposure = Shader.PropertyToID(nameof (_AutoExposure));
      internal static readonly int _DebugWidth = Shader.PropertyToID(nameof (_DebugWidth));
    }
  }
}
