// Decompiled with JetBrains decompiler
// Type: YAxisInventer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class YAxisInventer : MonoBehaviour
{
  public Toggle toggle;

  private void Start()
  {
    this.toggle.isOn = PlayerPrefs.GetInt("y_invert", 0) == 1;
    this.ChangeState(this.toggle.isOn);
  }

  public void ChangeState(bool b)
  {
    PlayerPrefs.SetInt("y_invert", !b ? 0 : 1);
    MouseLook.invert = b;
  }
}
