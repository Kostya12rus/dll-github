// Decompiled with JetBrains decompiler
// Type: DetectorBlink
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class DetectorBlink : MonoBehaviour
{
  public Material mat;
  private bool state;

  private void Start()
  {
    this.Blink();
  }

  private void Blink()
  {
    this.state = !this.state;
    int num = !this.state ? 0 : 2;
    this.mat.SetColor("_EmissionColor", new Color((float) num, (float) num, (float) num));
    this.Invoke(nameof (Blink), !this.state ? 1.3f : 0.2f);
  }
}
