// Decompiled with JetBrains decompiler
// Type: CPPS_NV
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class CPPS_NV : CustomPostProcessingSight
{
  public Slider distanceSlider;
  public AnimationCurve sliderValueOverDistance;
  public Text infoText;

  private void Start()
  {
    this.wm = this.GetComponentInParent<WeaponManager>();
    if (!this.wm.isLocalPlayer)
    {
      Object.Destroy((Object) this);
    }
    else
    {
      this.ppb = this.wm.weaponModelCamera.GetComponent<PostProcessingBehaviour>();
      CustomPostProcessingSight.singleton = (CustomPostProcessingSight) this;
    }
  }

  private void Update()
  {
    if (this.ppb.profile.name.Equals(this.targetProfile.name))
    {
      this.canvas.SetActive(true);
      CustomPostProcessingSight.raycast_bool = Physics.Raycast(new Ray(this.ppb.transform.position, this.ppb.transform.forward), out CustomPostProcessingSight.raycast_hit, 100f, (int) this.wm.raycastMask);
      this.distanceSlider.value = this.sliderValueOverDistance.Evaluate(!CustomPostProcessingSight.raycast_bool ? 100f : CustomPostProcessingSight.raycast_hit.distance);
      this.infoText.text = this.GetAmmoLeft().ToString("00");
    }
    else
      this.canvas.SetActive(false);
  }
}
