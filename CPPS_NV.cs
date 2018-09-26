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
    this.wm = (WeaponManager) ((Component) this).GetComponentInParent<WeaponManager>();
    if (!this.wm.get_isLocalPlayer())
    {
      Object.Destroy((Object) this);
    }
    else
    {
      this.ppb = (PostProcessingBehaviour) ((Component) this.wm.weaponModelCamera).GetComponent<PostProcessingBehaviour>();
      CustomPostProcessingSight.singleton = (CustomPostProcessingSight) this;
    }
  }

  private void Update()
  {
    if (((Object) this.ppb.profile).get_name().Equals(((Object) this.targetProfile).get_name()))
    {
      this.canvas.SetActive(true);
      CustomPostProcessingSight.raycast_bool = Physics.Raycast(new Ray(((Component) this.ppb).get_transform().get_position(), ((Component) this.ppb).get_transform().get_forward()), ref CustomPostProcessingSight.raycast_hit, 100f, LayerMask.op_Implicit(this.wm.raycastMask));
      this.distanceSlider.set_value(this.sliderValueOverDistance.Evaluate(!CustomPostProcessingSight.raycast_bool ? 100f : ((RaycastHit) ref CustomPostProcessingSight.raycast_hit).get_distance()));
      this.infoText.set_text(this.GetAmmoLeft().ToString("00"));
    }
    else
      this.canvas.SetActive(false);
  }
}
