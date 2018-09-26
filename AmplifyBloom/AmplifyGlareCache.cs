// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.AmplifyGlareCache
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyGlareCache
  {
    [SerializeField]
    internal AmplifyStarlineCache[] Starlines;
    [SerializeField]
    internal Vector4 AverageWeight;
    [SerializeField]
    internal Vector4[,] CromaticAberrationMat;
    [SerializeField]
    internal int TotalRT;
    [SerializeField]
    internal GlareDefData GlareDef;
    [SerializeField]
    internal StarDefData StarDef;
    [SerializeField]
    internal int CurrentPassCount;

    public AmplifyGlareCache()
    {
      this.Starlines = new AmplifyStarlineCache[4];
      this.CromaticAberrationMat = new Vector4[4, 8];
      for (int index = 0; index < 4; ++index)
        this.Starlines[index] = new AmplifyStarlineCache();
    }

    public void Destroy()
    {
      for (int index = 0; index < 4; ++index)
        this.Starlines[index].Destroy();
      this.Starlines = (AmplifyStarlineCache[]) null;
      this.CromaticAberrationMat = (Vector4[,]) null;
    }
  }
}
