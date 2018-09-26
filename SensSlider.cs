// Decompiled with JetBrains decompiler
// Type: SensSlider
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SensSlider : MonoBehaviour
{
  public AudioMixer master;
  public Slider slider;
  public Text optionalValueText;

  private void Start()
  {
    this.OnValueChanged((float) PlayerPrefs.GetInt("Volume", 0));
    this.slider.value = (float) PlayerPrefs.GetInt("Volume", 0);
    this.master.SetFloat("volume", (float) PlayerPrefs.GetInt("Volume", 0));
    this.optionalValueText.text = PlayerPrefs.GetInt("Volume", 0).ToString() + " dB";
  }

  public void OnValueChanged(float vol)
  {
    this.master.SetFloat("volume", vol);
    PlayerPrefs.SetInt("Volume", (int) vol);
    if (!((Object) this.optionalValueText != (Object) null))
      return;
    this.optionalValueText.text = ((int) vol).ToString() + " dB";
  }
}
