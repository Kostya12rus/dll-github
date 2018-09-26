// Decompiled with JetBrains decompiler
// Type: ModPrefab
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class ModPrefab : MonoBehaviour
{
  public string label;
  public int weaponId;
  public ModPrefab.ModType modType;
  public int modId;
  public bool firstperson;
  public GameObject gameObject;

  private void Start()
  {
    WeaponManager componentInParent = this.GetComponentInParent<WeaponManager>();
    componentInParent.forceSyncModsNextFrame = true;
    if (this.modType == ModPrefab.ModType.Sight)
    {
      if (this.firstperson)
        componentInParent.weapons[this.weaponId].mod_sights[this.modId].prefab_firstperson = this.gameObject;
      else
        componentInParent.weapons[this.weaponId].mod_sights[this.modId].prefab_thirdperson = this.gameObject;
    }
    if (this.modType == ModPrefab.ModType.Barrel)
    {
      if (this.firstperson)
        componentInParent.weapons[this.weaponId].mod_barrels[this.modId].prefab_firstperson = this.gameObject;
      else
        componentInParent.weapons[this.weaponId].mod_barrels[this.modId].prefab_thirdperson = this.gameObject;
    }
    if (this.modType != ModPrefab.ModType.Other)
      return;
    if (this.firstperson)
      componentInParent.weapons[this.weaponId].mod_others[this.modId].prefab_firstperson = this.gameObject;
    else
      componentInParent.weapons[this.weaponId].mod_others[this.modId].prefab_thirdperson = this.gameObject;
  }

  public enum ModType
  {
    Sight,
    Barrel,
    Other,
  }
}
