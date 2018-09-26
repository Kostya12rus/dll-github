// Decompiled with JetBrains decompiler
// Type: Rid
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class Rid : MonoBehaviour
{
  public string id;

  public Rid()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (!string.IsNullOrEmpty(this.id))
      return;
    this.id = ((Object) ((Renderer) ((Component) this).GetComponentInChildren<MeshRenderer>()).get_material().get_mainTexture()).get_name();
  }
}
