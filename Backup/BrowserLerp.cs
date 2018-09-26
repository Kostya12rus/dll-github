// Decompiled with JetBrains decompiler
// Type: BrowserLerp
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class BrowserLerp : MonoBehaviour
{
  private Vector3 prevPos;
  private RectTransform rectTransform;
  private Vector3 targetPos;
  public float speed;

  public BrowserLerp()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.rectTransform = (RectTransform) ((Component) this).GetComponent<RectTransform>();
  }

  private void LateUpdate()
  {
    BrowserLerp browserLerp = this;
    browserLerp.targetPos = Vector3.op_Addition(browserLerp.targetPos, Vector3.op_Subtraction(((Transform) this.rectTransform).get_localPosition(), this.prevPos));
    ((Transform) this.rectTransform).set_localPosition(this.prevPos);
    ((Transform) this.rectTransform).set_localPosition(Vector3.Lerp(((Transform) this.rectTransform).get_localPosition(), this.targetPos, (float) ((double) Time.get_deltaTime() * (double) this.speed * 4.0)));
    this.prevPos = ((Transform) this.rectTransform).get_localPosition();
  }
}
