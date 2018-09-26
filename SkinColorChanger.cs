// Decompiled with JetBrains decompiler
// Type: SkinColorChanger
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class SkinColorChanger : MonoBehaviour
{
  private int lastClass = -1;
  public Material ci;
  public Material mtf;
  public Material classd;
  public Material scientist;
  public Material guard;

  private void OnEnable()
  {
    Renderer component = (Renderer) this.GetComponent<SkinnedMeshRenderer>();
    CharacterClassManager componentInParent = this.GetComponentInParent<CharacterClassManager>();
    if (this.lastClass == componentInParent.curClass)
      return;
    this.lastClass = componentInParent.curClass;
    if (componentInParent.klasy[componentInParent.curClass].team == Team.MTF)
    {
      if (componentInParent.curClass == 15)
        component.sharedMaterial = this.guard;
      else
        component.sharedMaterial = this.mtf;
    }
    else if (componentInParent.klasy[componentInParent.curClass].team == Team.CHI)
      component.sharedMaterial = this.ci;
    else if (componentInParent.klasy[componentInParent.curClass].team == Team.RSC)
      component.sharedMaterial = this.scientist;
    else
      component.sharedMaterial = this.classd;
  }
}
