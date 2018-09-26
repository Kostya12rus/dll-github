// Decompiled with JetBrains decompiler
// Type: WorkStationUpgrader
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WorkStationUpgrader : NetworkBehaviour
{
  private int curWeapon;
  private int slotID;
  private int curMod;
  private WorkStation ws;
  private WeaponManager manager;
  public Button ss_sight;
  public Button ss_barrel;
  public Button ss_other;
  public RawImage ms_icon;
  public TextMeshProUGUI ms_header;
  public TextMeshProUGUI ms_stats;
  public TextMeshProUGUI ms_curModSize;

  private void Start()
  {
    this.ws = this.GetComponent<WorkStation>();
  }

  public void ChangeWeapon(Button button)
  {
    int result = 0;
    int.TryParse(button.name, out result);
    this.ws.ChangeScreen("slots");
    this.curWeapon = result;
    this.slotID = -1;
    this.RefreshSlotSelector();
  }

  public void ChangeSlot(Button button)
  {
    int result = 0;
    int.TryParse(button.name.Remove(1), out result);
    this.ws.ChangeScreen("mods");
    this.slotID = result;
    this.curMod = this.manager.weapons[this.curWeapon].GetMod((ModPrefab.ModType) this.slotID);
    this.RefreshModSelector();
  }

  public void NextMod()
  {
    if (this.curMod < this.GetModLength(this.slotID) - 1)
      ++this.curMod;
    this.RefreshModSelector();
  }

  public void PrevMod()
  {
    if (this.curMod > 0)
      --this.curMod;
    this.RefreshModSelector();
  }

  private int GetModLength(int slot)
  {
    if (slot == 0)
      return this.manager.weapons[this.curWeapon].mod_sights.Length;
    if (slot == 1)
      return this.manager.weapons[this.curWeapon].mod_barrels.Length;
    return this.manager.weapons[this.curWeapon].mod_others.Length;
  }

  private void RefreshSlotSelector()
  {
    if ((Object) this.manager == (Object) null)
      this.manager = PlayerManager.localPlayer.GetComponent<WeaponManager>();
    this.ss_sight.interactable = this.manager.weapons[this.curWeapon].mod_sights.Length > 1;
    this.ss_sight.GetComponent<TextMeshProUGUI>().text = this.ss_sight.GetComponent<TextMeshProUGUI>().text.Remove(this.ss_sight.GetComponent<TextMeshProUGUI>().text.IndexOf('('));
    TextMeshProUGUI component1 = this.ss_sight.GetComponent<TextMeshProUGUI>();
    component1.text = component1.text + "(" + (object) (this.manager.weapons[this.curWeapon].mod_sights.Length - 1) + " ready)";
    this.ss_barrel.interactable = this.manager.weapons[this.curWeapon].mod_barrels.Length > 1;
    this.ss_barrel.GetComponent<TextMeshProUGUI>().text = this.ss_barrel.GetComponent<TextMeshProUGUI>().text.Remove(this.ss_barrel.GetComponent<TextMeshProUGUI>().text.IndexOf('('));
    TextMeshProUGUI component2 = this.ss_barrel.GetComponent<TextMeshProUGUI>();
    component2.text = component2.text + "(" + (object) (this.manager.weapons[this.curWeapon].mod_barrels.Length - 1) + " ready)";
    this.ss_other.interactable = this.manager.weapons[this.curWeapon].mod_others.Length > 1;
    this.ss_other.GetComponent<TextMeshProUGUI>().text = this.ss_other.GetComponent<TextMeshProUGUI>().text.Remove(this.ss_other.GetComponent<TextMeshProUGUI>().text.IndexOf('('));
    TextMeshProUGUI component3 = this.ss_other.GetComponent<TextMeshProUGUI>();
    component3.text = component3.text + "(" + (object) (this.manager.weapons[this.curWeapon].mod_others.Length - 1) + " ready)";
  }

  private void RefreshModSelector()
  {
    if (this.slotID == 0)
    {
      this.ms_header.text = this.manager.weapons[this.curWeapon].mod_sights[this.curMod].name;
      this.ms_icon.texture = this.manager.weapons[this.curWeapon].mod_sights[this.curMod].icon;
      this.ms_stats.text = this.manager.weapons[this.curWeapon].GetStats(ModPrefab.ModType.Sight, this.curMod);
    }
    else if (this.slotID == 1)
    {
      this.ms_header.text = this.manager.weapons[this.curWeapon].mod_barrels[this.curMod].name;
      this.ms_icon.texture = this.manager.weapons[this.curWeapon].mod_barrels[this.curMod].icon;
      this.ms_stats.text = this.manager.weapons[this.curWeapon].GetStats(ModPrefab.ModType.Barrel, this.curMod);
    }
    else if (this.slotID == 2)
    {
      this.ms_header.text = this.manager.weapons[this.curWeapon].mod_others[this.curMod].name;
      this.ms_icon.texture = this.manager.weapons[this.curWeapon].mod_others[this.curMod].icon;
      this.ms_stats.text = this.manager.weapons[this.curWeapon].GetStats(ModPrefab.ModType.Other, this.curMod);
    }
    this.ms_curModSize.text = this.curMod.ToString() + " / " + (object) (this.GetModLength(this.slotID) - 1);
    this.manager.weapons[this.curWeapon].ChangeMod((ModPrefab.ModType) this.slotID, this.curMod, true, PlayerManager.localPlayer.GetComponent<WeaponManager>().flashlightEnabled);
  }

  private void UNetVersion()
  {
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
