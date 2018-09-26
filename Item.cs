// Decompiled with JetBrains decompiler
// Type: Item
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class Item
{
  public float pickingtime = 1f;
  public string label;
  public Texture2D icon;
  public GameObject prefab;
  public string[] permissions;
  public GameObject firstpersonModel;
  public float durability;
  public bool noEquipable;
  public Texture crosshair;
  public Color crosshairColor;
  [HideInInspector]
  public int id;

  public Item(Item item)
  {
    this.label = item.label;
    this.icon = item.icon;
    this.prefab = item.prefab;
    this.pickingtime = item.pickingtime;
    this.permissions = item.permissions;
    this.firstpersonModel = item.firstpersonModel;
    this.durability = item.durability;
    this.id = item.id;
    this.crosshair = item.crosshair;
    this.crosshairColor = item.crosshairColor;
  }
}
