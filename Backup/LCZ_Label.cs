// Decompiled with JetBrains decompiler
// Type: LCZ_Label
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class LCZ_Label : MonoBehaviour
{
  public MeshRenderer chRend;
  public MeshRenderer numRend;

  public LCZ_Label()
  {
    base.\u002Ector();
  }

  public void Refresh(Material ch, Material num, string err)
  {
    ((Renderer) this.chRend).set_sharedMaterial(ch);
    if (Object.op_Equality((Object) ((Renderer) this.chRend).get_sharedMaterial(), (Object) null))
      Debug.Log((object) err);
    ((Renderer) this.numRend).set_sharedMaterial(num);
  }
}
