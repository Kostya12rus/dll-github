// Decompiled with JetBrains decompiler
// Type: FullscreenToggle
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
  public bool isOn;
  public GameObject checkmark;

  private void OnEnable()
  {
    this.isOn = PlayerPrefs.GetInt("SavedFullscreen", 1) != 0;
    this.checkmark.SetActive(this.isOn);
  }

  public void Click()
  {
    this.isOn = !this.isOn;
    this.checkmark.SetActive(this.isOn);
    PlayerPrefs.SetInt("SavedFullscreen", !this.isOn ? 0 : 1);
    ResolutionManager.ChangeFullscreen(this.isOn);
  }
}
