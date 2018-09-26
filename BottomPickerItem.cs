// Decompiled with JetBrains decompiler
// Type: BottomPickerItem
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class BottomPickerItem : MonoBehaviour
{
  private string key;
  private int id;

  public BottomPickerItem()
  {
    base.\u002Ector();
  }

  public void SetupButton(string k, int i)
  {
    this.key = k;
    this.id = i;
  }

  public void Submit()
  {
    PlayerPrefs.SetInt(this.key, this.id);
  }
}
