// Decompiled with JetBrains decompiler
// Type: Hitmarker
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class Hitmarker : MonoBehaviour
{
  private float t = 10f;
  public static Hitmarker singleton;
  public AnimationCurve size;
  public AnimationCurve opacity;
  public CanvasRenderer targetImage;
  private float multiplier;

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
    this.t += Time.deltaTime;
    this.targetImage.SetAlpha(this.opacity.Evaluate(this.t));
    this.targetImage.transform.localScale = Vector3.one * this.size.Evaluate(this.t) * this.multiplier;
  }
}
