// Decompiled with JetBrains decompiler
// Type: TextMessage
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class TextMessage : MonoBehaviour
{
  public float spacing = 15.5f;
  public float xOffset = 3f;
  public float lerpSpeed = 3f;
  public float position;
  public float remainingLife;
  private CanvasRenderer r;

  private Vector3 GetPosition()
  {
    return new Vector3(this.xOffset, this.spacing * this.position, 0.0f);
  }

  private void Start()
  {
    this.r = this.GetComponent<CanvasRenderer>();
    this.transform.localPosition = this.GetPosition() + Vector3.down * this.spacing;
  }

  private void Update()
  {
    this.remainingLife -= Time.deltaTime;
    this.r.SetAlpha(Mathf.Clamp01(this.remainingLife * 2f));
    this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, this.GetPosition(), Time.deltaTime * this.lerpSpeed);
  }
}
