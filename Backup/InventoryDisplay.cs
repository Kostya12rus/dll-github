// Decompiled with JetBrains decompiler
// Type: InventoryDisplay
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class InventoryDisplay : MonoBehaviour
{
  [HideInInspector]
  public Inventory localplayer;
  public GameObject rootObject;
  public Texture2D blackTexture;
  public TextMeshProUGUI description;
  public Image[] itemslots;
  private List<Item> items;
  public int hoveredID;
  public bool isSCP;

  public InventoryDisplay()
  {
    base.\u002Ector();
  }

  public void SetDescriptionByID(int id)
  {
    if (id == -1)
    {
      this.hoveredID = -1;
      ((TMP_Text) this.description).set_text(string.Empty);
    }
    else if (this.items.Count > id)
    {
      string str = TranslationReader.Get("Inventory", this.items[id].id).Replace("\\n", Environment.NewLine);
      string label = this.items[id].label;
      ((TMP_Text) this.description).set_text(str);
      this.hoveredID = id;
    }
    else
    {
      this.hoveredID = -1;
      ((TMP_Text) this.description).set_text(string.Empty);
    }
  }

  private void Update()
  {
    if (Object.op_Equality((Object) this.localplayer, (Object) null))
      return;
    if (!this.rootObject.get_activeSelf())
      this.hoveredID = -1;
    else
      Inventory.inventoryCooldown = 0.2f;
    this.items.Clear();
    using (IEnumerator<Inventory.SyncItemInfo> enumerator = ((SyncList<Inventory.SyncItemInfo>) this.localplayer.items).GetEnumerator())
    {
      while (enumerator.MoveNext())
        this.items.Add(new Item(this.localplayer.availableItems[enumerator.Current.id]));
    }
    if (Input.GetKeyDown((KeyCode) 27) || this.isSCP)
    {
      this.rootObject.SetActive(false);
      this.hoveredID = -1;
      ((MouseLook) ((FirstPersonController) ((Component) this.localplayer).GetComponent<FirstPersonController>()).m_MouseLook).isOpenEq = (__Null) (this.rootObject.get_activeSelf() ? 1 : 0);
      CursorManager.eqOpen = this.rootObject.get_activeSelf();
    }
    if (!this.isSCP && Input.GetKeyDown(NewInput.GetKey("Inventory")) && (!((MicroHID_GFX) ((Component) this.localplayer).GetComponent<MicroHID_GFX>()).onFire && !CursorManager.pauseOpen))
    {
      this.hoveredID = -1;
      this.rootObject.SetActive(!this.rootObject.get_activeSelf());
      ((MouseLook) ((FirstPersonController) ((Component) this.localplayer).GetComponent<FirstPersonController>()).m_MouseLook).isOpenEq = (__Null) (this.rootObject.get_activeSelf() ? 1 : 0);
      CursorManager.eqOpen = this.rootObject.get_activeSelf();
      if (this.rootObject.get_activeSelf())
        Inventory.inventoryCooldown = 0.5f;
    }
    if (Input.GetKeyDown((KeyCode) 324) && this.hoveredID >= 0 && this.rootObject.get_activeSelf())
      this.localplayer.DropItem(this.hoveredID);
    if (Input.GetKeyDown((KeyCode) 323) && this.rootObject.get_activeSelf())
    {
      if (this.hoveredID >= 0)
        this.localplayer.CallCmdSetUnic(((SyncList<Inventory.SyncItemInfo>) this.localplayer.items).get_Item(this.hoveredID).uniq);
      else
        this.localplayer.CallCmdSetUnic(-1);
      this.localplayer.NetworkcurItem = this.hoveredID < 0 ? this.hoveredID : this.items[this.hoveredID].id;
      ((MouseLook) ((FirstPersonController) ((Component) this.localplayer).GetComponent<FirstPersonController>()).m_MouseLook).isOpenEq = (__Null) 0;
      CursorManager.eqOpen = false;
      this.rootObject.SetActive(false);
    }
    foreach (Component itemslot in this.itemslots)
      ((RawImage) itemslot.GetComponentInChildren<RawImage>()).set_texture((Texture) this.blackTexture);
    for (int index = this.itemslots.Length - 1; index >= 0; --index)
    {
      if (index < this.items.Count)
        ((RawImage) ((Component) this.itemslots[index]).GetComponentInChildren<RawImage>()).set_texture((Texture) this.items[index].icon);
    }
  }
}
