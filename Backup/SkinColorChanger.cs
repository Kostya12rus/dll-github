// Decompiled with JetBrains decompiler
// Type: SkinColorChanger
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class SkinColorChanger : MonoBehaviour
{
  public Material ci;
  public Material mtf;
  public Material classd;
  public Material scientist;
  public Material guard;
  private int lastClass;

  public SkinColorChanger()
  {
    base.\u002Ector();
  }

  private void OnEnable()
  {
    Renderer component = (Renderer) ((Component) this).GetComponent<SkinnedMeshRenderer>();
    CharacterClassManager componentInParent = (CharacterClassManager) ((Component) this).GetComponentInParent<CharacterClassManager>();
    if (this.lastClass == componentInParent.curClass)
      return;
    this.lastClass = componentInParent.curClass;
    if (componentInParent.klasy[componentInParent.curClass].team == Team.MTF)
    {
      if (componentInParent.curClass == 15)
        component.set_sharedMaterial(this.guard);
      else
        component.set_sharedMaterial(this.mtf);
    }
    else if (componentInParent.klasy[componentInParent.curClass].team == Team.CHI)
      component.set_sharedMaterial(this.ci);
    else if (componentInParent.klasy[componentInParent.curClass].team == Team.RSC)
      component.set_sharedMaterial(this.scientist);
    else
      component.set_sharedMaterial(this.classd);
  }
}
