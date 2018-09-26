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

  public GFXSettings()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.LoadSavedSettings();
  }

  public void RefreshGUI()
  {
    if (Object.op_Equality((Object) this.pxlc_slider, (Object) null))
      return;
    this.pxlc_slider.set_value((float) (QualitySettings.get_pixelLightCount() - 6));
    this.shadows_slider.set_value((float) QualitySettings.get_shadows());
    this.shadres_slider.set_value((float) PlayerPrefs.GetInt("gfxsets_shadres", 2));
    this.shaddis_slider.set_value((float) ((int) QualitySettings.get_shadowDistance() - 25));
    this.vsync_slider.set_value((float) QualitySettings.get_vSyncCount());
    this.aa_slider.set_value((float) PlayerPrefs.GetInt("gfxsets_aa", 1));
    this.aocc_slider.set_value((float) PlayerPrefs.GetInt("gfxsets_mb", 0));
    this.cc_slider.set_value((float) PlayerPrefs.GetInt("gfxsets_cc", 1));
    this.hp_slider.set_value((float) PlayerPrefs.GetInt("gfxsets_hp", 0));
    this.textures_slider.set_value((float) PlayerPrefs.GetInt("gfxsets_textures", 0));
    for (int index = 0; index < this.blood_v.Length; ++index)
    {
      if (this.blood_v[index].en == PlayerPrefs.GetInt("gfxsets_maxblood", 250).ToString())
        this.blood_slider.set_value((float) index);
    }
    this.RefreshValues();
  }

  public void RefreshValues()
  {
    this.pxlc_txt.set_text(this.pxlc_v[Mathf.RoundToInt(this.pxlc_slider.get_value())].Return());
    this.shadows_txt.set_text(this.shadows_v[Mathf.RoundToInt(this.shadows_slider.get_value())].Return());
    this.shadres_txt.set_text(this.shadres_v[Mathf.RoundToInt(this.shadres_slider.get_value())].Return());
    this.shaddis_txt.set_text(this.shaddis_v[Mathf.RoundToInt(this.shaddis_slider.get_value())].Return());
    this.vsync_txt.set_text(this.vsync_v[Mathf.RoundToInt(this.vsync_slider.get_value())].Return());
    this.blood_txt.set_text(this.blood_v[Mathf.RoundToInt(this.blood_slider.get_value())].Return());
    this.aa_txt.set_text(this.aa_v[Mathf.RoundToInt(this.aa_slider.get_value())].Return());
    this.aocc_txt.set_text(this.aocc_v[Mathf.RoundToInt(this.aocc_slider.get_value())].Return());
    this.cc_txt.set_text(this.cc_v[Mathf.RoundToInt(this.cc_slider.get_value())].Return());
    this.hp_txt.set_text(this.hp_v[Mathf.RoundToInt(this.hp_slider.get_value())].Return());
    this.textures_txt.set_text(this.textures_v[Mathf.RoundToInt(this.textures_slider.get_value())].Return());
  }

  public void SaveSettings()
  {
    PlayerPrefs.SetInt("gfxsets_pxlc", (int) this.pxlc_slider.get_value());
    PlayerPrefs.SetInt("gfxsets_shadows", (int) this.shadows_slider.get_value());
    PlayerPrefs.SetInt("gfxsets_shadres", (int) this.shadres_slider.get_value());
    PlayerPrefs.SetInt("gfxsets_shaddis", (int) this.shaddis_slider.get_value());
    PlayerPrefs.SetInt("gfxsets_vsync", (int) this.vsync_slider.get_value());
    PlayerPrefs.SetInt("gfxsets_aa", (int) this.aa_slider.get_value());
    PlayerPrefs.SetInt("gfxsets_mb", (int) this.aocc_slider.get_value());
    PlayerPrefs.SetInt("gfxsets_cc", (int) this.cc_slider.get_value());
    PlayerPrefs.SetInt("gfxsets_hp", (int) this.hp_slider.get_value());
    PlayerPrefs.SetInt("gfxsets_maxblood", int.Parse(this.blood_txt.get_text()));
    PlayerPrefs.SetInt("gfxsets_textures", (int) this.textures_slider.get_value());
    this.LoadSavedSettings();
  }

  public void LoadSavedSettings()
  {
    QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("gfxsets_textures", 0));
    QualitySettings.set_pixelLightCount(Mathf.Clamp(PlayerPrefs.GetInt("gfxsets_pxlc", 4) + 6, 6, 12));
    QualitySettings.set_shadows((ShadowQuality) Mathf.Clamp(PlayerPrefs.GetInt("gfxsets_shadows", 3), 0, 3));
    QualitySettings.set_shadowResolution((ShadowResolution) Mathf.Clamp(PlayerPrefs.GetInt("gfxsets_shadres", 2), 0, 3));
    QualitySettings.set_shadowDistance((float) Mathf.Clamp(PlayerPrefs.GetInt("gfxsets_shaddis", 10) + 25, 25, 50));
    QualitySettings.set_vSyncCount(Mathf.Clamp(PlayerPrefs.GetInt("gfxsets_vsync", 1), 0, 1));
    this.RefreshPPB();
    this.RefreshGUI();
  }

  private void RefreshPPB()
  {
    foreach (PostProcessingBehaviour processingBehaviour in (PostProcessingBehaviour[]) Resources.FindObjectsOfTypeAll<PostProcessingBehaviour>())
    {
      if (Object.op_Inequality((Object) processingBehaviour.profile, (Object) null) && !processingBehaviour.profile.fog.enabled)
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
