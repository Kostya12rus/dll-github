// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.FogModel
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public class FogModel : PostProcessingModel
  {
    [SerializeField]
    private FogModel.Settings m_Settings = FogModel.Settings.defaultSettings;

    public FogModel.Settings settings
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
      this.m_Settings = FogModel.Settings.defaultSettings;
    }

    [Serializable]
    public struct Settings
    {
      [Tooltip("Should the fog affect the skybox?")]
      public bool excludeSkybox;

      public static FogModel.Settings defaultSettings
      {
        get
        {
          return new FogModel.Settings() { excludeSkybox = true };
        }
      }
    }
  }
}
