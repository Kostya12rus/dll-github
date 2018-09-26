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

  private void Update()
  {
    bool flag = this.con1.activeSelf | this.con2.activeSelf | this.GetComponent<MainMenuScript>().submenus[6].activeSelf | this.dsc.activeSelf | this.lang.activeSelf | this.key.activeSelf;
    this.kamera.transform.position = Vector3.Lerp(this.kamera.transform.position, !flag ? this.unfoc.transform.position : this.foc.transform.position, Time.deltaTime * 2f);
    this.kamera.transform.rotation = Quaternion.Lerp(this.kamera.transform.rotation, !flag ? this.unfoc.transform.rotation : this.foc.transform.rotation, Time.deltaTime);
  }

  private void Start()
  {
    Timing.RunCoroutine(this._Animate(), Segment.FixedUpdate);
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
