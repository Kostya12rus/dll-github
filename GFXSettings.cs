// Decompiled with JetBrains decompiler
// Type: GFXSettings
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class GFXSettings : MonoBehaviour
{
  public GFXSettings.SliderValue[] pxlc_v;
  public GFXSettings.SliderValue[] shadows_v;
  public GFXSettings.SliderValue[] shadres_v;
  public GFXSettings.SliderValue[] shaddis_v;
  public GFXSettings.SliderValue[] vsync_v;
  public GFXSettings.SliderValue[] aa_v;
  public GFXSettings.SliderValue[] aocc_v;
  public GFXSettings.SliderValue[] cc_v;
  public GFXSettings.SliderValue[] hp_v;
  public GFXSettings.SliderValue[] blood_v;
  public GFXSettings.SliderValue[] textures_v;
  public Slider pxlc_slider;
  public Slider shadows_slider;
  public Slider shadres_slider;
  public Slider shaddis_slider;
  public Slider vsync_slider;
  public Slider aa_slider;
  public Slider aocc_slider;
  public Slider cc_slider;
  public Slider hp_slider;
  public Slider blood_slider;
  public Slider textures_slider;
  public Text pxlc_txt;
  public Text shadows_txt;
  public Text shadres_txt;
  public Text shaddis_txt;
  public Text vsync_txt;
  public Text aa_txt;
  public Text aocc_txt;
  public Text cc_txt;
  public Text hp_txt;
  public Text blood_txt;
  public Text textures_txt;

  private void Start()
  {
    this.LoadSavedSettings();
  }

  public void RefreshGUI()
  {
    if ((Object) this.pxlc_slider == (Object) null)
      return;
    this.pxlc_slider.value = (float) (QualitySettings.pixelLightCount - 6);
    this.shadows_slider.value = (float) QualitySettings.shadows;
    this.shadres_slider.value = (float) PlayerPrefs.GetInt("gfxsets_shadres", 2);
    this.shaddis_slider.value = (float) ((int) QualitySettings.shadowDistance - 25);
    this.vsync_slider.value = (float) QualitySettings.vSyncCount;
    this.aa_slider.value = (float) PlayerPrefs.GetInt("gfxsets_aa", 1);
    this.aocc_slider.value = (float) PlayerPrefs.GetInt("gfxsets_mb", 0);
    this.cc_slider.value = (float) PlayerPrefs.GetInt("gfxsets_cc", 1);
    this.hp_slider.value = (float) PlayerPrefs.GetInt("gfxsets_hp", 0);
    this.textures_slider.value = (float) PlayerPrefs.GetInt("gfxsets_textures", 0);
    for (int index = 0; index < this.blood_v.Length; ++index)
    {
      if (this.blood_v[index].en == PlayerPrefs.GetInt("gfxsets_maxblood", 250).ToString())
        this.blood_slider.value = (float) index;
    }
    this.RefreshValues();
  }

  public void RefreshValues()
  {
    this.pxlc_txt.text = this.pxlc_v[Mathf.RoundToInt(this.pxlc_slider.value)].Return();
    this.shadows_txt.text = this.shadows_v[Mathf.RoundToInt(this.shadows_slider.value)].Return();
    this.shadres_txt.text = this.shadres_v[Mathf.RoundToInt(this.shadres_slider.value)].Return();
    this.shaddis_txt.text = this.shaddis_v[Mathf.RoundToInt(this.shaddis_slider.value)].Return();
    this.vsync_txt.text = this.vsync_v[Mathf.RoundToInt(this.vsync_slider.value)].Return();
    this.blood_txt.text = this.blood_v[Mathf.RoundToInt(this.blood_slider.value)].Return();
    this.aa_txt.text = this.aa_v[Mathf.RoundToInt(this.aa_slider.value)].Return();
    this.aocc_txt.text = this.aocc_v[Mathf.RoundToInt(this.aocc_slider.value)].Return();
    this.cc_txt.text = this.cc_v[Mathf.RoundToInt(this.cc_slider.value)].Return();
    this.hp_txt.text = this.hp_v[Mathf.RoundToInt(this.hp_slider.value)].Return();
    this.textures_txt.text = this.textures_v[Mathf.RoundToInt(this.textures_slider.value)].Return();
  }

  public void SaveSettings()
  {
    PlayerPrefs.SetInt("gfxsets_pxlc", (int) this.pxlc_slider.value);
    PlayerPrefs.SetInt("gfxsets_shadows", (int) this.shadows_slider.value);
    PlayerPrefs.SetInt("gfxsets_shadres", (int) this.shadres_slider.value);
    PlayerPrefs.SetInt("gfxsets_shaddis", (int) this.shaddis_slider.value);
    PlayerPrefs.SetInt("gfxsets_vsync", (int) this.vsync_slider.value);
    PlayerPrefs.SetInt("gfxsets_aa", (int) this.aa_slider.value);
    PlayerPrefs.SetInt("gfxsets_mb", (int) this.aocc_slider.value);
    PlayerPrefs.SetInt("gfxsets_cc", (int) this.cc_slider.value);
    PlayerPrefs.SetInt("gfxsets_hp", (int) this.hp_slider.value);
    PlayerPrefs.SetInt("gfxsets_maxblood", int.Parse(this.blood_txt.text));
    PlayerPrefs.SetInt("gfxsets_textures", (int) this.textures_slider.value);
    this.LoadSavedSettings();
  }

  public void LoadSavedSettings()
  {
    QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("gfxsets_textures", 0));
    QualitySettings.pixelLightCount = Mathf.Clamp(PlayerPrefs.GetInt("gfxsets_pxlc", 4) + 6, 6, 12);
    QualitySettings.shadows = (ShadowQuality) Mathf.Clamp(PlayerPrefs.GetInt("gfxsets_shadows", 3), 0, 3);
    QualitySettings.shadowResolution = (ShadowResolution) Mathf.Clamp(PlayerPrefs.GetInt("gfxsets_shadres", 2), 0, 3);
    QualitySettings.shadowDistance = (float) Mathf.Clamp(PlayerPrefs.GetInt("gfxsets_shaddis", 10) + 25, 25, 50);
    QualitySettings.vSyncCount = Mathf.Clamp(PlayerPrefs.GetInt("gfxsets_vsync", 1), 0, 1);
    this.RefreshPPB();
    this.RefreshGUI();
  }

  private void RefreshPPB()
  {
    foreach (PostProcessingBehaviour processingBehaviour in Resources.FindObjectsOfTypeAll<PostProcessingBehaviour>())
    {
      if ((Object) processingBehaviour.profile != (Object) null && !processingBehaviour.profile.fog.enabled)
      {
        processingBehaviour.profile.antialiasing.enabled = PlayerPrefs.GetInt("gfxsets_aa", 1) == 1;
        processingBehaviour.profile.motionBlur.enabled = false;
      }
    }
  }

  [Serializable]
  public class SliderValue
  {
    public string overrideText = string.Empty;
    public string en;

    public string Return()
    {
      if (string.IsNullOrEmpty(this.overrideText))
        return this.en;
      return this.overrideText;
    }
  }
}
