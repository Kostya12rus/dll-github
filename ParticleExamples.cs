// Decompiled with JetBrains decompiler
// Type: ParticleExamples
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class ParticleExamples
{
  public string title;
  [TextArea]
  public string description;
  public bool isWeaponEffect;
  public GameObject particleSystemGO;
  public Vector3 particlePosition;
  public Vector3 particleRotation;
}
