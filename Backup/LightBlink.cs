// Decompiled with JetBrains decompiler
// Type: LightBlink
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class LightBlink : MonoBehaviour
{
  private float startIntes;
  public float noshadowIntensMultiplier;
  public float innerVariationPercent;
  private float outerVaration;
  private float curOut;
  private float curIn;
  private float innerVariation;
  public float FREQ;
  private Light l;
  public bool disabled;
  private int i;

  public LightBlink()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (QualitySettings.get_shadows() != null)
      this.noshadowIntensMultiplier = 1f;
    this.startIntes = ((Light) ((Component) this).GetComponent<Light>()).get_intensity() * 1.2f;
    this.outerVaration = (float) ((double) this.startIntes * (double) this.noshadowIntensMultiplier / 10.0);
    this.innerVariation = (float) ((double) this.startIntes * (double) this.noshadowIntensMultiplier * ((double) this.innerVariationPercent / 100.0));
    this.l = (Light) ((Component) this).GetComponent<Light>();
    this.RandomOuter();
    if ((double) this.innerVariationPercent >= 100.0)
      return;
    this.InvokeRepeating("RefreshLight", 0.0f, 1f / this.FREQ);
  }

  private void FixedUpdate()
  {
    if (!this.disabled && (double) this.innerVariationPercent == 100.0)
    {
      ++this.i;
      if (this.i <= 3)
        return;
      this.i = 0;
      ((Behaviour) this.l).set_enabled(!((Behaviour) this.l).get_enabled());
    }
    else
      ((Behaviour) this.l).set_enabled(true);
  }

  private void RandomOuter()
  {
    this.curOut = Random.Range(-this.outerVaration, this.outerVaration);
    this.Invoke(nameof (RandomOuter), (float) Random.Range(1, 3));
  }

  private void RefreshLight()
  {
    if (!this.disabled)
    {
      this.curIn = Random.Range(this.startIntes * this.noshadowIntensMultiplier + this.innerVariation, this.startIntes * this.noshadowIntensMultiplier - this.innerVariation);
      this.l.set_intensity(this.curIn + this.curOut);
    }
    else
    {
      ((Behaviour) this.l).set_enabled(true);
      this.l.set_intensity(this.startIntes);
    }
  }
}
