// Decompiled with JetBrains decompiler
// Type: MusicSlider
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
  public AudioMixer master;
  public Slider slider;
  public Text optionalValueText;
  public string keyName;

  public MusicSlider()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    this.keyName += "-new";
  }

  private void Start()
  {
    this.slider.set_value(PlayerPrefs.GetFloat(this.keyName, 1f));
    this.OnValueChanged(this.slider.get_value());
  }

  public void OnValueChanged(float vol)
  {
    if (Object.op_Inequality((Object) this.optionalValueText, (Object) null))
      this.optionalValueText.set_text(Mathf.RoundToInt(vol * 100f).ToString() + " %");
    PlayerPrefs.SetFloat(this.keyName, vol);
    this.master.SetFloat(this.keyName.Remove(this.keyName.Length - 4), (double) vol == 0.0 ? -144f : 20f * Mathf.Log10(vol));
  }
}
