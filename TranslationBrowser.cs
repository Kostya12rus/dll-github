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
  public Text instancePrefab;
  public Transform parent;
  private List<GameObject> spawns;

  public TranslationBrowser()
  {
    base.\u002Ector();
  }

  private void OnEnable()
  {
    string[] directories = Directory.GetDirectories("Translations");
    using (List<GameObject>.Enumerator enumerator = this.spawns.GetEnumerator())
    {
      while (enumerator.MoveNext())
        Object.Destroy((Object) enumerator.Current);
    }
    foreach (string str in directories)
    {
      Text text = (Text) Object.Instantiate<Text>((M0) this.instancePrefab, this.parent);
      ((Component) text).get_transform().set_localScale(Vector3.get_one());
      text.set_text(str.Remove(0, str.IndexOf("\\") + 1));
      this.spawns.Add(((Component) text).get_gameObject());
    }
  }
}
