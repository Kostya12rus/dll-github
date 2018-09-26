// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.BuiltinDebugViewsModel
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public class BuiltinDebugViewsModel : PostProcessingModel
  {
    [SerializeField]
    private BuiltinDebugViewsModel.Settings m_Settings = BuiltinDebugViewsModel.Settings.defaultSettings;

    public BuiltinDebugViewsModel.Settings settings
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

    public bool willInterrupt
    {
      get
      {
        if (!this.IsModeActive(BuiltinDebugViewsModel.Mode.None) && !this.IsModeActive(BuiltinDebugViewsModel.Mode.EyeAdaptation) && (!this.IsModeActive(BuiltinDebugViewsModel.Mode.PreGradingLog) && !this.IsModeActive(BuiltinDebugViewsModel.Mode.LogLut)))
          return !this.IsModeActive(BuiltinDebugViewsModel.Mode.UserLut);
        return false;
      }
    }

    public override void Reset()
    {
      this.settings = BuiltinDebugViewsModel.Settings.defaultSettings;
    }

    public bool IsModeActive(BuiltinDebugViewsModel.Mode mode)
    {
      return this.m_Settings.mode == mode;
    }

    [Serializable]
    public struct DepthSettings
    {
      [Tooltip("Scales the camera far plane before displaying the depth map.")]
      [Range(0.0f, 1f)]
      public float scale;

      public static BuiltinDebugViewsModel.DepthSettings defaultSettings
      {
        get
        {
          return new BuiltinDebugViewsModel.DepthSettings() { scale = 1f };
        }
      }
    }

    [Serializable]
    public struct MotionVectorsSettings
    {
      [Tooltip("Opacity of the source render.")]
      [Range(0.0f, 1f)]
      public float sourceOpacity;
      [Tooltip("Opacity of the per-pixel motion vector colors.")]
      [Range(0.0f, 1f)]
      public float motionImageOpacity;
      [Tooltip("Because motion vectors are mainly very small vectors, you can use this setting to make them more visible.")]
      [Min(0.0f)]
      public float motionImageAmplitude;
      [Tooltip("Opacity for the motion vector arrows.")]
      [Range(0.0f, 1f)]
      public float motionVectorsOpacity;
      [Tooltip("The arrow density on screen.")]
      [Range(8f, 64f)]
      public int motionVectorsResolution;
      [Tooltip("Tweaks the arrows length.")]
      [Min(0.0f)]
      public float motionVectorsAmplitude;

      public static BuiltinDebugViewsModel.MotionVectorsSettings defaultSettings
      {
        get
        {
          return new BuiltinDebugViewsModel.MotionVectorsSettings() { sourceOpacity = 1f, motionImageOpacity = 0.0f, motionImageAmplitude = 16f, motionVectorsOpacity = 1f, motionVectorsResolution = 24, motionVectorsAmplitude = 64f };
        }
      }
    }

    public enum Mode
    {
      None,
      Depth,
      Normals,
      MotionVectors,
      AmbientOcclusion,
      EyeAdaptation,
      FocusPlane,
      PreGradingLog,
      LogLut,
      UserLut,
    }

    [Serializable]
    public struct Settings
    {
      public BuiltinDebugViewsModel.Mode mode;
      public BuiltinDebugViewsModel.DepthSettings depth;
      public BuiltinDebugViewsModel.MotionVectorsSettings motionVectors;

      public static BuiltinDebugViewsModel.Settings defaultSettings
      {
        get
        {
          return new BuiltinDebugViewsModel.Settings() { mode = BuiltinDebugViewsModel.Mode.None, depth = BuiltinDebugViewsModel.DepthSettings.defaultSettings, motionVectors = BuiltinDebugViewsModel.MotionVectorsSettings.defaultSettings };
        }
      }
    }
  }
}
