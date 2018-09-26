// Decompiled with JetBrains decompiler
// Type: TranslationBrowserButton
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TranslationBrowserButton : MonoBehaviour
{
  public void OnClick()
  {
    PlayerPrefs.SetString("translation_path", this.GetComponent<Text>().text);
    SceneManager.LoadScene(0);
  }
}
