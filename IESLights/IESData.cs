// Decompiled with JetBrains decompiler
// Type: IESLights.IESData
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;

namespace IESLights
{
  public class IESData
  {
    public List<float> VerticalAngles { get; set; }

    public List<float> HorizontalAngles { get; set; }

    public List<List<float>> CandelaValues { get; set; }

    public List<List<float>> NormalizedValues { get; set; }

    public PhotometricType PhotometricType { get; set; }

    public VerticalType VerticalType { get; set; }

    public HorizontalType HorizontalType { get; set; }

    public int PadBeforeAmount { get; set; }

    public int PadAfterAmount { get; set; }

    public float HalfSpotlightFov { get; set; }
  }
}
