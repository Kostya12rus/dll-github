// Decompiled with JetBrains decompiler
// Type: ButtonStages
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ButtonStages : MonoBehaviour
{
  public ButtonStages.DoorType[] inspectorTypes;
  public static ButtonStages.DoorType[] types;

  public ButtonStages()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    ButtonStages.types = this.inspectorTypes;
  }

  [Serializable]
  public class Stage
  {
    public Sprite texture;
    public Material mat;
    [Multiline]
    public string info;
  }

  [Serializable]
  public class DoorType
  {
    public ButtonStages.Stage[] stages;
    public string description;
  }
}
