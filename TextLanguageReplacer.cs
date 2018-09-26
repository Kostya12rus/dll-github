// Decompiled with JetBrains decompiler
// Type: TextLanguageReplacer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextLanguageReplacer : MonoBehaviour
{
  private MenuMusicManager mng;
  [Multiline]
  public string englishVersion;
  public string keyName;
  public int index;

  public void UpdateString()
  {
    string str = TranslationReader.Get(this.keyName, this.index);
    while (str.Contains("\\n"))
      str = str.Replace("\\n", Environment.NewLine);
    if (str == "NO_TRANSLATION" || str == "TRANSLATION_ERROR")
    {
      Debug.Log((object) ("Missing translation! " + this.keyName + (object) this.index));
      str = this.englishVersion;
    }
    if ((Object) this.GetComponent<TextMeshProUGUI>() != (Object) null)
      this.GetComponent<TextMeshProUGUI>().text = str;
    else
      this.GetComponent<Text>().text = str;
  }

  private void Awake()
  {
    this.UpdateString();
  }
}
