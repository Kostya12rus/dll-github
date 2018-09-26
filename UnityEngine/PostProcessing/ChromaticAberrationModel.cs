// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.ChromaticAberrationModel
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public class ChromaticAberrationModel : PostProcessingModel
  {
    [SerializeField]
    private ChromaticAberrationModel.Settings m_Settings = ChromaticAberrationModel.Settings.defaultSettings;

    public ChromaticAberrationModel.Settings settings
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
      this.m_Settings = ChromaticAberrationModel.Settings.defaultSettings;
    }

    [Serializable]
    public struct Settings
    {
      [Tooltip("Shift the hue of chromatic aberrations.")]
      public Texture2D spectralTexture;
      [Tooltip("Amount of tangential distortion.")]
      [Range(0.0f, 1f)]
      public float intensity;

      public static ChromaticAberrationModel.Settings defaultSettings
      {
        get
        {
          return new ChromaticAberrationModel.Settings() { spectralTexture = (Texture2D) null, intensity = 0.1f };
        }
      }
    }
  }
}
