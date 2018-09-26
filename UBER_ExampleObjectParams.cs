// Decompiled with JetBrains decompiler
// Type: UBER_ExampleObjectParams
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class UBER_ExampleObjectParams
{
  public GameObject target;
  public string materialProperty;
  public MeshRenderer renderer;
  public int submeshIndex;
  public Vector2 SliderRange;
  public string Title;
  public string MatParamName;
  [TextArea(2, 5)]
  public string Description;
}
