﻿// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.VignetteModel
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public class VignetteModel : PostProcessingModel
  {
    [SerializeField]
    private VignetteModel.Settings m_Settings = VignetteModel.Settings.defaultSettings;

    public VignetteModel.Settings settings
    {
      get
      {
        return this.m_Settings;
      }
      set
      {
        this.m_Settings = value;
      }
    }

    public override void Reset()
    {
      this.m_Settings = VignetteModel.Settings.defaultSettings;
    }

    public enum Mode
    {
      Classic,
      Masked,
    }

    [Serializable]
    public struct Settings
    {
      [Tooltip("Use the \"Classic\" mode for parametric controls. Use the \"Masked\" mode to use your own texture mask.")]
      public VignetteModel.Mode mode;
      [Tooltip("Vignette color. Use the alpha channel for transparency.")]
      [ColorUsage(false)]
      public Color color;
      [Tooltip("Sets the vignette center point (screen center is [0.5,0.5]).")]
      public Vector2 center;
      [Tooltip("Amount of vignetting on screen.")]
      [Range(0.0f, 1f)]
      public float intensity;
      [Range(0.01f, 1f)]
      [Tooltip("Smoothness of the vignette borders.")]
      public float smoothness;
      [Tooltip("Lower values will make a square-ish vignette.")]
      [Range(0.0f, 1f)]
      public float roundness;
      [Tooltip("A black and white mask to use as a vignette.")]
      public Texture mask;
      [Range(0.0f, 1f)]
      [Tooltip("Mask opacity.")]
      public float opacity;
      [Tooltip("Should the vignette be perfectly round or be dependent on the current aspect ratio?")]
      public bool rounded;

      public static VignetteModel.Settings defaultSettings
      {
        get
        {
          return new VignetteModel.Settings() { mode = VignetteModel.Mode.Classic, color = new Color(0.0f, 0.0f, 0.0f, 1f), center = new Vector2(0.5f, 0.5f), intensity = 0.45f, smoothness = 0.2f, roundness = 1f, mask = (Texture) null, opacity = 1f, rounded = false };
        }
      }
    }
  }
}
