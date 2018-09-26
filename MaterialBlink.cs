// Decompiled with JetBrains decompiler
// Type: MaterialBlink
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class MaterialBlink : MonoBehaviour
{
  public Color lowestColor = Color.white;
  public Color highestColor = Color.white;
  public float speed = 1f;
  public float colorMultiplier = 1f;
  public Material materal;
  private float time;

  private void Update()
  {
    this.time += Time.deltaTime * this.speed;
    if ((double) this.time > 1.0)
      --this.time;
    this.materal.SetColor("_EmissionColor", Color.Lerp(this.lowestColor, this.highestColor, Mathf.Abs(Mathf.Lerp(-1f, 1f, this.time))) * this.colorMultiplier);
  }

  private void OnDisable()
  {
    this.materal.SetColor("_EmissionColor", this.highestColor);
  }
}
