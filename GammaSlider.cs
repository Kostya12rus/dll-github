// Decompiled with JetBrains decompiler
// Type: GammaSlider
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class GammaSlider : MonoBehaviour
{
  public PostProcessingProfile profile;
  public Slider slider;
  public Text warningText;

  private void Start()
  {
    if (!((Object) this.slider != (Object) null))
      return;
    this.slider.value = PlayerPrefs.GetFloat("gammavalue", 0.0f);
    this.SetValue(this.slider.value);
  }

  public void SetValue(float f)
  {
    this.warningText.enabled = (double) f > 0.5;
    PlayerPrefs.SetFloat("gammavalue", f);
    ColorGradingModel.Settings settings1 = new ColorGradingModel.Settings();
    ColorGradingModel.Settings settings2 = this.profile.colorGrading.settings;
    settings2.basic.postExposure = f;
    this.profile.colorGrading.settings = settings2;
  }
}
