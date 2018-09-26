// Decompiled with JetBrains decompiler
// Type: SignBlink
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

public class SignBlink : MonoBehaviour
{
  public bool verticalText;
  private string startText;
  private const string alphabet = "QWERTYUIOPASDFGHJKLZXCVBNM01234567890!@#$%^&*()-_=+[]{}/<>";

  public void Play(int duration)
  {
    if (this.startText == string.Empty)
      this.startText = this.GetComponent<TextMeshProUGUI>().text;
    else
      this.GetComponent<TextMeshProUGUI>().text = this.startText;
  }
}
