// Decompiled with JetBrains decompiler
// Type: SensitivitySlider
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class SensitivitySlider : MonoBehaviour
{
  public Slider slider;

  private void Start()
  {
    this.OnValueChanged(PlayerPrefs.GetFloat("Sens", 1f));
    this.slider.value = PlayerPrefs.GetFloat("Sens", 1f);
  }

  public void OnValueChanged(float vol)
  {
    PlayerPrefs.SetFloat("Sens", vol);
    Sensitivity.sens = this.slider.value;
  }
}
