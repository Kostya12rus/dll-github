// Decompiled with JetBrains decompiler
// Type: TutorialUnlocker
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class TutorialUnlocker : MonoBehaviour
{
  public Button[] buttons;

  private void Start()
  {
    for (int index = 0; index < Mathf.Clamp(PlayerPrefs.GetInt("TutorialProgress", 1), 1, this.buttons.Length); ++index)
      this.buttons[index].interactable = true;
  }
}
