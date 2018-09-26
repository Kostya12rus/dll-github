// Decompiled with JetBrains decompiler
// Type: BrowserLerp
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class BrowserLerp : MonoBehaviour
{
  public float speed = 2f;
  private Vector3 prevPos;
  private RectTransform rectTransform;
  private Vector3 targetPos;

  private void Start()
  {
    this.rectTransform = this.GetComponent<RectTransform>();
  }

  private void LateUpdate()
  {
    this.targetPos += this.rectTransform.localPosition - this.prevPos;
    this.rectTransform.localPosition = this.prevPos;
    this.rectTransform.localPosition = Vector3.Lerp(this.rectTransform.localPosition, this.targetPos, (float) ((double) Time.deltaTime * (double) this.speed * 4.0));
    this.prevPos = this.rectTransform.localPosition;
  }
}
