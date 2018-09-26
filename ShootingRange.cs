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

  public void PrintDamage(float dmg)
  {
    if (!this.isOnRange)
      return;
    this.txt = GameObject.Find("ShootingText").GetComponent<Text>();
    this.curDamage = "-" + (object) (float) ((double) Mathf.Round(dmg * 100f) / 100.0) + " HP";
    this.remainingTime = 3f;
  }

  private void Update()
  {
    if (!this.isOnRange)
      return;
    if ((double) this.remainingTime > 0.0)
    {
      this.txt.text = this.curDamage;
      this.remainingTime -= Time.deltaTime;
    }
    else if ((Object) this.txt != (Object) null)
      this.txt.text = string.Empty;
    Camera component = this.GetComponentInChildren<GlobalFog>().GetComponent<Camera>();
    bool flag = (double) this.transform.position.x > 1500.0 & (double) this.transform.position.y > -10.0;
    this.GetComponentInChildren<GlobalFog>().startDistance = !flag ? 0.0f : 200f;
    this.GetComponent<FirstPersonController>().rangeSpeed = flag;
    component.farClipPlane = !flag ? 47f : 1000f;
  }
}
