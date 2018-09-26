// Decompiled with JetBrains decompiler
// Type: CPPS_Sniper
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.PostProcessing;

public class CPPS_Sniper : CustomPostProcessingSight
{
  private void Start()
  {
    this.wm = (WeaponManager) ((Component) this).GetComponentInParent<WeaponManager>();
    if (!this.wm.get_isLocalPlayer())
      Object.Destroy((Object) this);
    else
      this.ppb = (PostProcessingBehaviour) ((Component) this.wm.weaponModelCamera).GetComponent<PostProcessingBehaviour>();
  }

  private void Update()
  {
    this.canvas.SetActive(((Object) this.ppb.profile).get_name().Equals(((Object) this.targetProfile).get_name()));
  }
}
