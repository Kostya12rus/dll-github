// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.MotionBlurModel
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public class MotionBlurModel : PostProcessingModel
  {
    [SerializeField]
    private MotionBlurModel.Settings m_Settings = MotionBlurModel.Settings.defaultSettings;

    public MotionBlurModel.Settings settings
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
      this.m_Settings = MotionBlurModel.Settings.defaultSettings;
    }

    [Serializable]
    public struct Settings
    {
      [Tooltip("The angle of rotary shutter. Larger values give longer exposure.")]
      [Range(0.0f, 360f)]
      public float shutterAngle;
      [Tooltip("The amount of sample points, which affects quality and performances.")]
      [Range(4f, 32f)]
      public int sampleCount;
      [Tooltip("The strength of multiple frame blending. The opacity of preceding frames are determined from this coefficient and time differences.")]
      [Range(0.0f, 1f)]
      public float frameBlending;

      public static MotionBlurModel.Settings defaultSettings
      {
        get
        {
          return new MotionBlurModel.Settings() { shutterAngle = 270f, sampleCount = 10, frameBlending = 0.0f };
        }
      }
    }
  }
}
