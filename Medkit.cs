// Decompiled with JetBrains decompiler
// Type: Medkit
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Networking;

public class Medkit : NetworkBehaviour
{
  private static int kCmdCmdUseMedkit = -2049042393;
  public Medkit.MedkitInstance[] Medkits;
  private Inventory inv;
  private PlayerStats ps;
  private KeyCode fireCode;

  private void Start()
  {
    this.inv = this.GetComponent<Inventory>();
    this.ps = this.GetComponent<PlayerStats>();
    this.fireCode = NewInput.GetKey("Shoot");
  }

  private void Update()
  {
    if (!Input.GetKeyDown(this.fireCode) || Cursor.visible || ((double) Inventory.inventoryCooldown >= 0.0 || this.ps.health >= this.ps.maxHP))
      return;
    for (int id = 0; id < this.Medkits.Length; ++id)
    {
      if (this.Medkits[id].InventoryID == this.inv.curItem)
      {
        this.inv.SetCurItem(-1);
        this.CallCmdUseMedkit(id);
        break;
      }
    }
  }

  [Command(channel = 2)]
  private void CmdUseMedkit(int id)
  {
    foreach (Inventory.SyncItemInfo syncItemInfo in (SyncList<Inventory.SyncItemInfo>) this.inv.items)
    {
      if (syncItemInfo.id == this.Medkits[id].InventoryID)
      {
        this.ps.Networkhealth = Mathf.Clamp(this.ps.health + Random.Range(this.Medkits[id].MinimumHealthRegeneration, this.Medkits[id].MaximumHealthRegeneration), 0, this.ps.ccm.klasy[this.ps.ccm.curClass].maxHP);
        this.inv.items.Remove(syncItemInfo);
        break;
      }
    }
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdUseMedkit(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdUseMedkit called on client.");
    else
      ((Medkit) obj).CmdUseMedkit((int) reader.ReadPackedUInt32());
  }

  public void CallCmdUseMedkit(int id)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdUseMedkit called on server.");
    else if (this.isServer)
    {
      this.CmdUseMedkit(id);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Medkit.kCmdCmdUseMedkit);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.WritePackedUInt32((uint) id);
      this.SendCommandInternal(writer, 2, "CmdUseMedkit");
    }
  }

  static Medkit()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (Medkit), Medkit.kCmdCmdUseMedkit, new NetworkBehaviour.CmdDelegate(Medkit.InvokeCmdCmdUseMedkit));
    NetworkCRC.RegisterBehaviour(nameof (Medkit), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }

  [Serializable]
  public struct MedkitInstance
  {
    public string Label;
    public int InventoryID;
    public int MinimumHealthRegeneration;
    public int MaximumHealthRegeneration;
  }
}
