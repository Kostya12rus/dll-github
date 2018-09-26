// Decompiled with JetBrains decompiler
// Type: WarningAwawke
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class WarningAwawke : MonoBehaviour
{
  public Toggle toggle;

  private void Awake()
  {
    if (!(PlayerPrefs.GetString("warningToggle", "false") == "true"))
      return;
    this.gameObject.SetActive(false);
  }

  public void Close()
  {
    if (this.toggle.isOn)
      PlayerPrefs.SetString("warningToggle", "true");
    this.gameObject.SetActive(false);
  }
}
