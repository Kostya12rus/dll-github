// Decompiled with JetBrains decompiler
// Type: ShootingRange
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class ShootingRange : MonoBehaviour
{
  public bool isOnRange;
  private string curDamage;
  private float remainingTime;
  private Text txt;

  public ShootingRange()
  {
    base.\u002Ector();
  }

  public void PrintDamage(float dmg)
  {
    if (!this.isOnRange)
      return;
    this.txt = (Text) GameObject.Find("ShootingText").GetComponent<Text>();
    this.curDamage = "-" + (object) (float) ((double) Mathf.Round(dmg * 100f) / 100.0) + " HP";
    this.remainingTime = 3f;
  }

  private void Update()
  {
    if (!this.isOnRange)
      return;
    if ((double) this.remainingTime > 0.0)
    {
      this.txt.set_text(this.curDamage);
      this.remainingTime -= Time.get_deltaTime();
    }
    else if (Object.op_Inequality((Object) this.txt, (Object) null))
      this.txt.set_text(string.Empty);
    Camera component = (Camera) ((Component) ((Component) this).GetComponentInChildren<GlobalFog>()).GetComponent<Camera>();
    bool flag = ((Component) this).get_transform().get_position().x > 1500.0 & ((Component) this).get_transform().get_position().y > -10.0;
    ((GlobalFog) ((Component) this).GetComponentInChildren<GlobalFog>()).startDistance = !flag ? (__Null) 0.0 : (__Null) 200.0;
    ((FirstPersonController) ((Component) this).GetComponent<FirstPersonController>()).rangeSpeed = (__Null) (flag ? 1 : 0);
    component.set_farClipPlane(!flag ? 47f : 1000f);
  }
}
