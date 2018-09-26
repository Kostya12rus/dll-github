// Decompiled with JetBrains decompiler
// Type: TextMessage
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class TextMessage : MonoBehaviour
{
  public float spacing;
  public float xOffset;
  public float lerpSpeed;
  public float position;
  public float remainingLife;
  private CanvasRenderer r;

  public TextMessage()
  {
    base.\u002Ector();
  }

  private Vector3 GetPosition()
  {
    return new Vector3(this.xOffset, this.spacing * this.position, 0.0f);
  }

  private void Start()
  {
    this.r = (CanvasRenderer) ((Component) this).GetComponent<CanvasRenderer>();
    ((Component) this).get_transform().set_localPosition(Vector3.op_Addition(this.GetPosition(), Vector3.op_Multiply(Vector3.get_down(), this.spacing)));
  }

  private void Update()
  {
    this.remainingLife -= Time.get_deltaTime();
    this.r.SetAlpha(Mathf.Clamp01(this.remainingLife * 2f));
    ((Component) this).get_transform().set_localPosition(Vector3.Lerp(((Component) this).get_transform().get_localPosition(), this.GetPosition(), Time.get_deltaTime() * this.lerpSpeed));
  }
}
