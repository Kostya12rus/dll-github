﻿// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.BloomModel
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public class BloomModel : PostProcessingModel
  {
    [SerializeField]
    private BloomModel.Settings m_Settings = BloomModel.Settings.defaultSettings;

    public BloomModel.Settings settings
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
      this.m_Settings = BloomModel.Settings.defaultSettings;
    }

    [Serializable]
    public struct BloomSettings
    {
      [Tooltip("Strength of the bloom filter.")]
      [Min(0.0f)]
      public float intensity;
      [Tooltip("Filters out pixels under this level of brightness.")]
      [Min(0.0f)]
      public float threshold;
      [Tooltip("Makes transition between under/over-threshold gradual (0 = hard threshold, 1 = soft threshold).")]
      [Range(0.0f, 1f)]
      public float softKnee;
      [Tooltip("Changes extent of veiling effects in a screen resolution-independent fashion.")]
      [Range(1f, 7f)]
      public float radius;
      [Tooltip("Reduces flashing noise with an additional filter.")]
      public bool antiFlicker;

      public float thresholdLinear
      {
        set
        {
          this.threshold = Mathf.LinearToGammaSpace(value);
        }
        get
        {
          return Mathf.GammaToLinearSpace(this.threshold);
        }
      }

      public static BloomModel.BloomSettings defaultSettings
      {
        get
        {
          return new BloomModel.BloomSettings() { intensity = 0.5f, threshold = 1.1f, softKnee = 0.5f, radius = 4f, antiFlicker = false };
        }
      }
    }

    [Serializable]
    public struct LensDirtSettings
    {
      [Tooltip("Dirtiness texture to add smudges or dust to the lens.")]
      public Texture texture;
      [Tooltip("Amount of lens dirtiness.")]
      [Min(0.0f)]
      public float intensity;

      public static BloomModel.LensDirtSettings defaultSettings
      {
        get
        {
          return new BloomModel.LensDirtSettings() { texture = (Texture) null, intensity = 3f };
        }
      }
    }

    [Serializable]
    public struct Settings
    {
      public BloomModel.BloomSettings bloom;
      public BloomModel.LensDirtSettings lensDirt;

      public static BloomModel.Settings defaultSettings
      {
        get
        {
          return new BloomModel.Settings() { bloom = BloomModel.BloomSettings.defaultSettings, lensDirt = BloomModel.LensDirtSettings.defaultSettings };
        }
      }
    }
  }
}
