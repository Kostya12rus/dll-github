// Decompiled with JetBrains decompiler
// Type: IESLights.EXRData
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace IESLights
{
  public struct EXRData
  {
    public Color[] Pixels;
    public uint Width;
    public uint Height;

    public EXRData(Color[] pixels, int width, int height)
    {
      this.Pixels = pixels;
      this.Width = (uint) width;
      this.Height = (uint) height;
    }
  }
}
