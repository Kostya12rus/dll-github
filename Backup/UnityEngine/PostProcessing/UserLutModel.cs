// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.UserLutModel
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public class UserLutModel : PostProcessingModel
  {
    [SerializeField]
    private UserLutModel.Settings m_Settings = UserLutModel.Settings.defaultSettings;

    public UserLutModel.Settings settings
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
      this.m_Settings = UserLutModel.Settings.defaultSettings;
    }

    [Serializable]
    public struct Settings
    {
      [Tooltip("Custom lookup texture (strip format, e.g. 256x16).")]
      public Texture2D lut;
      [Tooltip("Blending factor.")]
      [Range(0.0f, 1f)]
      public float contribution;

      public static UserLutModel.Settings defaultSettings
      {
        get
        {
          return new UserLutModel.Settings() { lut = (Texture2D) null, contribution = 1f };
        }
      }
    }
  }
}
