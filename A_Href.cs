﻿// Decompiled with JetBrains decompiler
// Type: A_Href
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class A_Href : MonoBehaviour
{
  public A_Href()
  {
    base.\u002Ector();
  }

  public void Click(string url)
  {
    Application.OpenURL(url);
  }
}