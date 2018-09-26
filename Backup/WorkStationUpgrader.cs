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

  public WorkStationUpgrader()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.ws = (WorkStation) ((Component) this).GetComponent<WorkStation>();
  }

  public void ChangeWeapon(Button button)
  {
    int result = 0;
    int.TryParse(((Object) button).get_name(), out result);
    this.ws.ChangeScreen("slots");
    this.curWeapon = result;
    this.slotID = -1;
    this.RefreshSlotSelector();
  }

  public void ChangeSlot(Button button)
  {
    int result = 0;
    int.TryParse(((Object) button).get_name().Remove(1), out result);
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
    if (Object.op_Equality((Object) this.manager, (Object) null))
      this.manager = (WeaponManager) PlayerManager.localPlayer.GetComponent<WeaponManager>();
    ((Selectable) this.ss_sight).set_interactable(this.manager.weapons[this.curWeapon].mod_sights.Length > 1);
    ((TMP_Text) ((Component) this.ss_sight).GetComponent<TextMeshProUGUI>()).set_text(((TMP_Text) ((Component) this.ss_sight).GetComponent<TextMeshProUGUI>()).get_text().Remove(((TMP_Text) ((Component) this.ss_sight).GetComponent<TextMeshProUGUI>()).get_text().IndexOf('(')));
    M0 component1 = ((Component) this.ss_sight).GetComponent<TextMeshProUGUI>();
    ((TMP_Text) component1).set_text(((TMP_Text) component1).get_text() + "(" + (object) (this.manager.weapons[this.curWeapon].mod_sights.Length - 1) + " ready)");
    ((Selectable) this.ss_barrel).set_interactable(this.manager.weapons[this.curWeapon].mod_barrels.Length > 1);
    ((TMP_Text) ((Component) this.ss_barrel).GetComponent<TextMeshProUGUI>()).set_text(((TMP_Text) ((Component) this.ss_barrel).GetComponent<TextMeshProUGUI>()).get_text().Remove(((TMP_Text) ((Component) this.ss_barrel).GetComponent<TextMeshProUGUI>()).get_text().IndexOf('(')));
    M0 component2 = ((Component) this.ss_barrel).GetComponent<TextMeshProUGUI>();
    ((TMP_Text) component2).set_text(((TMP_Text) component2).get_text() + "(" + (object) (this.manager.weapons[this.curWeapon].mod_barrels.Length - 1) + " ready)");
    ((Selectable) this.ss_other).set_interactable(this.manager.weapons[this.curWeapon].mod_others.Length > 1);
    ((TMP_Text) ((Component) this.ss_other).GetComponent<TextMeshProUGUI>()).set_text(((TMP_Text) ((Component) this.ss_other).GetComponent<TextMeshProUGUI>()).get_text().Remove(((TMP_Text) ((Component) this.ss_other).GetComponent<TextMeshProUGUI>()).get_text().IndexOf('(')));
    M0 component3 = ((Component) this.ss_other).GetComponent<TextMeshProUGUI>();
    ((TMP_Text) component3).set_text(((TMP_Text) component3).get_text() + "(" + (object) (this.manager.weapons[this.curWeapon].mod_others.Length - 1) + " ready)");
  }

  private void RefreshModSelector()
  {
    if (this.slotID == 0)
    {
      ((TMP_Text) this.ms_header).set_text(this.manager.weapons[this.curWeapon].mod_sights[this.curMod].name);
      this.ms_icon.set_texture(this.manager.weapons[this.curWeapon].mod_sights[this.curMod].icon);
      ((TMP_Text) this.ms_stats).set_text(this.manager.weapons[this.curWeapon].GetStats(ModPrefab.ModType.Sight, this.curMod));
    }
    else if (this.slotID == 1)
    {
      ((TMP_Text) this.ms_header).set_text(this.manager.weapons[this.curWeapon].mod_barrels[this.curMod].name);
      this.ms_icon.set_texture(this.manager.weapons[this.curWeapon].mod_barrels[this.curMod].icon);
      ((TMP_Text) this.ms_stats).set_text(this.manager.weapons[this.curWeapon].GetStats(ModPrefab.ModType.Barrel, this.curMod));
    }
    else if (this.slotID == 2)
    {
      ((TMP_Text) this.ms_header).set_text(this.manager.weapons[this.curWeapon].mod_others[this.curMod].name);
      this.ms_icon.set_texture(this.manager.weapons[this.curWeapon].mod_others[this.curMod].icon);
      ((TMP_Text) this.ms_stats).set_text(this.manager.weapons[this.curWeapon].GetStats(ModPrefab.ModType.Other, this.curMod));
    }
    ((TMP_Text) this.ms_curModSize).set_text(this.curMod.ToString() + " / " + (object) (this.GetModLength(this.slotID) - 1));
    this.manager.weapons[this.curWeapon].ChangeMod((ModPrefab.ModType) this.slotID, this.curMod, true, ((WeaponManager) PlayerManager.localPlayer.GetComponent<WeaponManager>()).flashlightEnabled);
  }

  private void UNetVersion()
  {
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
