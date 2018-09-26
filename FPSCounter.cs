﻿// Decompiled with JetBrains decompiler
// Type: FPSCounter
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
  private ushort k = 24;
  public Text text;
  private double framerate;
  private double frametime;

  private void Update()
  {
    float deltaTime = Time.deltaTime;
    this.framerate = Math.Round(1.0 / (double) deltaTime, 1);
    this.frametime = Math.Round((double) deltaTime * 1000.0, 1);
  }

  private void FixedUpdate()
  {
    ++this.k;
    if (this.k != (ushort) 25)
      return;
    this.k = (ushort) 0;
    this.text.text = "Framerate: " + (object) this.framerate + "   " + (object) this.frametime + "ms";
  }
}
