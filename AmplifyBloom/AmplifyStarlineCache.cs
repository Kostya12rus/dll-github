// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.AmplifyStarlineCache
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyStarlineCache
  {
    [SerializeField]
    internal AmplifyPassCache[] Passes;

    public AmplifyStarlineCache()
    {
      this.Passes = new AmplifyPassCache[4];
      for (int index = 0; index < 4; ++index)
        this.Passes[index] = new AmplifyPassCache();
    }

    public void Destroy()
    {
      for (int index = 0; index < 4; ++index)
        this.Passes[index].Destroy();
      this.Passes = (AmplifyPassCache[]) null;
    }
  }
}
