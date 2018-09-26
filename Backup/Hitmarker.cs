// Decompiled with JetBrains decompiler
// Type: Hitmarker
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class Hitmarker : MonoBehaviour
{
  public static Hitmarker singleton;
  public AnimationCurve size;
  public AnimationCurve opacity;
  private float t;
  public CanvasRenderer targetImage;
  private float multiplier;

  public Hitmarker()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    Hitmarker.singleton = this;
  }

  public static void Hit(float size = 1f)
  {
    Hitmarker.singleton.Trigger(size);
  }

  private void Trigger(float size = 1f)
  {
    this.t = 0.0f;
    this.multiplier = size;
  }

  private void Update()
  {
    if ((double) this.t >= 10.0)
      return;
    this.t += Time.get_deltaTime();
    this.targetImage.SetAlpha(this.opacity.Evaluate(this.t));
    ((Component) this.targetImage).get_transform().set_localScale(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_one(), this.size.Evaluate(this.t)), this.multiplier));
  }
}
