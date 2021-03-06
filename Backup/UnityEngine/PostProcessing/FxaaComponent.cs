﻿// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.FxaaComponent
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

namespace UnityEngine.PostProcessing
{
  public sealed class FxaaComponent : PostProcessingComponentRenderTexture<AntialiasingModel>
  {
    public override bool active
    {
      get
      {
        if (this.model.enabled && this.model.settings.method == AntialiasingModel.Method.Fxaa)
          return !this.context.interrupted;
        return false;
      }
    }

    public void Render(RenderTexture source, RenderTexture destination)
    {
      AntialiasingModel.FxaaSettings fxaaSettings = this.model.settings.fxaaSettings;
      Material material = this.context.materialFactory.Get("Hidden/Post FX/FXAA");
      AntialiasingModel.FxaaQualitySettings preset1 = AntialiasingModel.FxaaQualitySettings.presets[(int) fxaaSettings.preset];
      AntialiasingModel.FxaaConsoleSettings preset2 = AntialiasingModel.FxaaConsoleSettings.presets[(int) fxaaSettings.preset];
      material.SetVector(FxaaComponent.Uniforms._QualitySettings, Vector4.op_Implicit(new Vector3(preset1.subpixelAliasingRemovalAmount, preset1.edgeDetectionThreshold, preset1.minimumRequiredLuminance)));
      material.SetVector(FxaaComponent.Uniforms._ConsoleSettings, new Vector4(preset2.subpixelSpreadAmount, preset2.edgeSharpnessAmount, preset2.edgeDetectionThreshold, preset2.minimumRequiredLuminance));
      Graphics.Blit((Texture) source, destination, material, 0);
    }

    private static class Uniforms
    {
      internal static readonly int _QualitySettings = Shader.PropertyToID(nameof (_QualitySettings));
      internal static readonly int _ConsoleSettings = Shader.PropertyToID(nameof (_ConsoleSettings));
    }
  }
}
