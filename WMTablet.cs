// Decompiled with JetBrains decompiler
// Type: WMTablet
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WMTablet : MonoBehaviour
{
  public GameObject[] submenus;
  public int curMenu;
  public int selectedWeapon;
  public Text ct_text;
  private int ct_amountToDrop;
  private Inventory inv;
  private WeaponManager wm;
  public GameObject list_template;
  public Transform list_parent;
  public Text list_info;
  public Color list_normalColor;
  public Color list_selectedColor;
  private List<Image> templates;
  private string translatedInfo;

  public WMTablet()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.wm = (WeaponManager) ((Component) this).GetComponent<WeaponManager>();
    if (!this.wm.get_isLocalPlayer())
    {
      Object.Destroy((Object) this);
    }
    else
    {
      this.inv = (Inventory) ((Component) this).GetComponent<Inventory>();
      for (int index = 0; index < this.wm.weapons.Length; ++index)
      {
        GameObject gameObject = (GameObject) Object.Instantiate<GameObject>((M0) this.list_template, this.list_parent);
        gameObject.get_transform().set_localScale(Vector3.get_one());
        ((Text) gameObject.GetComponentInChildren<Text>()).set_text(this.inv.availableItems[this.wm.weapons[index].inventoryID].label);
        this.templates.Add((Image) gameObject.GetComponent<Image>());
      }
      Object.Destroy((Object) this.list_template);
    }
  }

  private void Update()
  {
    if (this.inv.curItem != 19)
      return;
    bool keyDown1 = Input.GetKeyDown((KeyCode) 273);
    bool keyDown2 = Input.GetKeyDown((KeyCode) 274);
    bool keyDown3 = Input.GetKeyDown((KeyCode) 276);
    bool keyDown4 = Input.GetKeyDown((KeyCode) 275);
    if (keyDown3)
      --this.curMenu;
    this.curMenu = Mathf.Clamp(this.curMenu, 0, 2);
    for (int index = 0; index < this.submenus.Length; ++index)
      this.submenus[index].SetActive(index == this.curMenu);
    WeaponManager.Weapon weapon = this.wm.weapons[this.selectedWeapon];
    AmmoBox component = (AmmoBox) ((Component) this).GetComponent<AmmoBox>();
    if (this.curMenu == 0 && (keyDown1 || keyDown4 || keyDown2))
      ++this.curMenu;
    else if (this.curMenu == 1)
    {
      if (keyDown1)
        --this.selectedWeapon;
      if (keyDown2)
        ++this.selectedWeapon;
      this.selectedWeapon = Mathf.Clamp(this.selectedWeapon, 0, this.wm.weapons.Length - 1);
      if (keyDown4)
      {
        ++this.curMenu;
      }
      else
      {
        for (int index = 0; index < this.templates.Count; ++index)
          ((Graphic) this.templates[index]).set_color(index != this.selectedWeapon ? this.list_normalColor : this.list_selectedColor);
        if (string.IsNullOrEmpty(this.translatedInfo))
          this.translatedInfo = TranslationReader.Get("WeaponManager", 2);
        this.list_info.set_text(this.translatedInfo.Replace("[var_name]", this.inv.availableItems[weapon.inventoryID].label).Replace("[var_atype]", component.types[weapon.ammoType].label).Replace("[var_ares]", component.GetAmmo(weapon.ammoType).ToString()).Replace("[var_mag]", weapon.maxAmmo.ToString()).Replace("[var_maxdmg]", (weapon.damageOverDistance.Evaluate(0.0f) * weapon.allEffects.damageMultiplier * this.wm.overallDamagerFactor).ToString()).Replace("[var_effdmg]", (weapon.damageOverDistance.Evaluate(10f) * weapon.allEffects.damageMultiplier * this.wm.overallDamagerFactor).ToString()).Replace("[var_sps]", weapon.shotsPerSecond.ToString()).Replace("[var_custs]", (weapon.mod_barrels.Length + weapon.mod_others.Length + weapon.mod_sights.Length - 3).ToString()));
      }
    }
    else
    {
      if (this.curMenu != 2)
        return;
      if (component.GetAmmo(weapon.ammoType) >= 15)
      {
        this.ct_amountToDrop = Mathf.Clamp(this.ct_amountToDrop, 15, component.GetAmmo(weapon.ammoType));
        this.ct_text.set_text(TranslationReader.Get("WeaponManager", 3).Replace("[var_nom]", "<b>" + (object) this.ct_amountToDrop + " x " + component.types[weapon.ammoType].label + "</b>"));
      }
      else
      {
        this.ct_text.set_text(TranslationReader.Get("WeaponManager", 4).Replace("[var_nom]", component.types[weapon.ammoType].label));
        this.ct_amountToDrop = 0;
      }
      if (keyDown1)
        ++this.ct_amountToDrop;
      if (keyDown2)
        --this.ct_amountToDrop;
      if (!keyDown4)
        return;
      component.CallCmdDrop(this.ct_amountToDrop, weapon.ammoType);
    }
  }
}
