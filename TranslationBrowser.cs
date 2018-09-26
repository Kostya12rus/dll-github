// Decompiled with JetBrains decompiler
// Type: TranslationBrowser
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TranslationBrowser : MonoBehaviour
{
  private List<GameObject> spawns = new List<GameObject>();
  public Text instancePrefab;
  public Transform parent;

  private void OnEnable()
  {
    string[] directories = Directory.GetDirectories("Translations");
    foreach (Object spawn in this.spawns)
      Object.Destroy(spawn);
    foreach (string str in directories)
    {
      Text text = Object.Instantiate<Text>(this.instancePrefab, this.parent);
      text.transform.localScale = Vector3.one;
      text.text = str.Remove(0, str.IndexOf("\\") + 1);
      this.spawns.Add(text.gameObject);
    }
  }
}
