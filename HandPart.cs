// Decompiled with JetBrains decompiler
// Type: HandPart
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class HandPart : MonoBehaviour
{
  public GameObject part;
  public int id;
  public Animator anim;
  private Inventory inv;

  private void Start()
  {
    if ((Object) this.anim == (Object) null)
      this.anim = this.GetComponentsInParent<Animator>()[0];
    if (!((Object) this.inv == (Object) null))
      return;
    this.inv = this.GetComponentInParent<Inventory>();
  }

  public void UpdateItem()
  {
    this.part.SetActive(this.inv.curItem == this.id);
  }
}
