﻿// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.AmplifyPassCache
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyPassCache
  {
    [SerializeField]
    internal Vector4[] Offsets;
    [SerializeField]
    internal Vector4[] Weights;

    public AmplifyPassCache()
    {
      this.Offsets = new Vector4[16];
      this.Weights = new Vector4[16];
    }

    public void Destroy()
    {
      this.Offsets = (Vector4[]) null;
      this.Weights = (Vector4[]) null;
    }
  }
}
