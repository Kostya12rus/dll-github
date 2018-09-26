// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.PostProcessingModel
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public abstract class PostProcessingModel
  {
    [GetSet("enabled")]
    [SerializeField]
    private bool m_Enabled;

    public bool enabled
    {
      get
      {
        return this.m_Enabled;
      }
      set
      {
        this.m_Enabled = value;
        if (!value)
          return;
        this.OnValidate();
      }
    }

    public abstract void Reset();

    public virtual void OnValidate()
    {
    }
  }
}
