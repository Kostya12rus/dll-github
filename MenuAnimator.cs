// Decompiled with JetBrains decompiler
// Type: MenuAnimator
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MenuAnimator : MonoBehaviour
{
  public GameObject kamera;
  public GameObject con1;
  public GameObject con2;
  public GameObject foc;
  public GameObject unfoc;
  public GameObject dsc;
  public GameObject lang;
  public GameObject key;

  public MenuAnimator()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    bool flag = this.con1.get_activeSelf() | this.con2.get_activeSelf() | ((MainMenuScript) ((Component) this).GetComponent<MainMenuScript>()).submenus[6].get_activeSelf() | this.dsc.get_activeSelf() | this.lang.get_activeSelf() | this.key.get_activeSelf();
    this.kamera.get_transform().set_position(Vector3.Lerp(this.kamera.get_transform().get_position(), !flag ? this.unfoc.get_transform().get_position() : this.foc.get_transform().get_position(), Time.get_deltaTime() * 2f));
    this.kamera.get_transform().set_rotation(Quaternion.Lerp(this.kamera.get_transform().get_rotation(), !flag ? this.unfoc.get_transform().get_rotation() : this.foc.get_transform().get_rotation(), Time.get_deltaTime()));
  }

  private void Start()
  {
    Timing.RunCoroutine(this._Animate(), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _Animate()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    MenuAnimator.\u003C_Animate\u003Ec__Iterator0 animateCIterator0 = new MenuAnimator.\u003C_Animate\u003Ec__Iterator0();
    return (IEnumerator<float>) animateCIterator0;
  }
}
