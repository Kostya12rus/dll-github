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

public class InventoryDisplay : MonoBehaviour
{
  private List<Item> items = new List<Item>();
  [HideInInspector]
  public Inventory localplayer;
  public GameObject rootObject;
  public Texture2D blackTexture;
  public TextMeshProUGUI description;
  public Image[] itemslots;
  public int hoveredID;
  public bool isSCP;

  public void SetDescriptionByID(int id)
  {
    if (id == -1)
    {
      this.hoveredID = -1;
      this.description.text = string.Empty;
    }
    else if (this.items.Count > id)
    {
      string str = TranslationReader.Get("Inventory", this.items[id].id).Replace("\\n", Environment.NewLine);
      string label = this.items[id].label;
      this.description.text = str;
      this.hoveredID = id;
    }
    else
    {
      this.hoveredID = -1;
      this.description.text = string.Empty;
    }
  }

  private void Update()
  {
    if ((Object) this.localplayer == (Object) null)
      return;
    if (!this.rootObject.activeSelf)
      this.hoveredID = -1;
    else
      Inventory.inventoryCooldown = 0.2f;
    this.items.Clear();
    foreach (Inventory.SyncItemInfo syncItemInfo in (SyncList<Inventory.SyncItemInfo>) this.localplayer.items)
      this.items.Add(new Item(this.localplayer.availableItems[syncItemInfo.id]));
    if (Input.GetKeyDown(KeyCode.Escape) || this.isSCP)
    {
      this.rootObject.SetActive(false);
      this.hoveredID = -1;
      this.localplayer.GetComponent<FirstPersonController>().m_MouseLook.isOpenEq = this.rootObject.activeSelf;
      CursorManager.eqOpen = this.rootObject.activeSelf;
    }
    if (!this.isSCP && Input.GetKeyDown(NewInput.GetKey("Inventory")) && (!this.localplayer.GetComponent<MicroHID_GFX>().onFire && !CursorManager.pauseOpen))
    {
      this.hoveredID = -1;
      this.rootObject.SetActive(!this.rootObject.activeSelf);
      this.localplayer.GetComponent<FirstPersonController>().m_MouseLook.isOpenEq = this.rootObject.activeSelf;
      CursorManager.eqOpen = this.rootObject.activeSelf;
      if (this.rootObject.activeSelf)
        Inventory.inventoryCooldown = 0.5f;
    }
    if (Input.GetKeyDown(KeyCode.Mouse1) && this.hoveredID >= 0 && this.rootObject.activeSelf)
      this.localplayer.DropItem(this.hoveredID);
    if (Input.GetKeyDown(KeyCode.Mouse0) && this.rootObject.activeSelf)
    {
      if (this.hoveredID >= 0)
        this.localplayer.CallCmdSetUnic(this.localplayer.items[this.hoveredID].uniq);
      else
        this.localplayer.CallCmdSetUnic(-1);
      this.localplayer.NetworkcurItem = this.hoveredID < 0 ? this.hoveredID : this.items[this.hoveredID].id;
      this.localplayer.GetComponent<FirstPersonController>().m_MouseLook.isOpenEq = false;
      CursorManager.eqOpen = false;
      this.rootObject.SetActive(false);
    }
    foreach (Component itemslot in this.itemslots)
      itemslot.GetComponentInChildren<RawImage>().texture = (Texture) this.blackTexture;
    for (int index = this.itemslots.Length - 1; index >= 0; --index)
    {
      if (index < this.items.Count)
        this.itemslots[index].GetComponentInChildren<RawImage>().texture = (Texture) this.items[index].icon;
    }
  }
}
