// Decompiled with JetBrains decompiler
// Type: WeaponCamera
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using AmplifyBloom;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;

public class WeaponCamera : MonoBehaviour
{
  private VignetteAndChromaticAberration vaca;
  private VignetteAndChromaticAberration myvaca;
  private PostProcessingBehaviour ppbeh;
  private AmplifyBloomEffect bloom;
  public AnimationCurve intensityOverScreen;
  public float _intens;
  public float _glare;

  private void Start()
  {
    this.bloom = this.GetComponent<AmplifyBloomEffect>();
    this.ppbeh = this.GetComponent<PostProcessingBehaviour>();
    this.myvaca = this.GetComponent<VignetteAndChromaticAberration>();
    this.vaca = this.GetComponentInParent<VignetteAndChromaticAberration>();
    this.bloom.enabled = PlayerPrefs.GetInt("gfxsets_cc", 1) == 1;
  }

  private void Update()
  {
    this.myvaca = this.vaca;
    float num = this.intensityOverScreen.Evaluate((float) Screen.height);
    this.bloom.OverallIntensity = this._intens * num;
    this.bloom.LensGlareInstance.Intensity = this._glare * num;
  }
}
