// Decompiled with JetBrains decompiler
// Type: MarkupStyle
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class MarkupStyle
{
  [Header("Transform")]
  public Vector2 position = Vector2.get_zero();
  public Vector2 size = new Vector2(100f, 100f);
  [Header("Main Styles")]
  public Color mainColor = Color.get_clear();
  public Color outlineColor = Color.get_white();
  [Header("Text")]
  public TextAnchor textAlignment = (TextAnchor) 4;
  public string textContent = string.Empty;
  public Color textColor = Color.get_white();
  public Color textOutlineColor = Color.get_black();
  public int fontSize = 20;
  [Header("Background Image")]
  public string imageUrl = string.Empty;
  public Color imageColor = Color.get_white();
  public float rotation;
  public float outlineSize;
  public bool raycast;
  public float textOutlineSize;
  public int fontID;
}
