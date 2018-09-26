// Decompiled with JetBrains decompiler
// Type: MaterialLanguageReplacer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class MaterialLanguageReplacer : MonoBehaviour
{
  public Material englishVersion;

  private void Start()
  {
    this.GetComponent<Renderer>().material = this.englishVersion;
  }
}
