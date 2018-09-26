// Decompiled with JetBrains decompiler
// Type: Medkit
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Medkit : NetworkBehaviour
{
  private static int kCmdCmdUseMedkit = -2049042393;
  public Medkit.MedkitInstance[] Medkits;
  private Inventory inv;
  private PlayerStats ps;
  private KeyCode fireCode;

  public Medkit()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.inv = (Inventory) ((Component) this).GetComponent<Inventory>();
    this.ps = (PlayerStats) ((Component) this).GetComponent<PlayerStats>();
    this.fireCode = NewInput.GetKey("Shoot");
  }

  private void Update()
  {
    if (!Input.GetKeyDown(this.fireCode) || Cursor.get_visible() || ((double) Inventory.inventoryCooldown >= 0.0 || this.ps.health >= this.ps.maxHP))
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
    using (IEnumerator<Inventory.SyncItemInfo> enumerator = ((SyncList<Inventory.SyncItemInfo>) this.inv.items).GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        Inventory.SyncItemInfo current = enumerator.Current;
        if (current.id == this.Medkits[id].InventoryID)
        {
          this.ps.Networkhealth = Mathf.Clamp(this.ps.health + Random.Range(this.Medkits[id].MinimumHealthRegeneration, this.Medkits[id].MaximumHealthRegeneration), 0, this.ps.ccm.klasy[this.ps.ccm.curClass].maxHP);
          ((SyncList<Inventory.SyncItemInfo>) this.inv.items).Remove(current);
          break;
        }
      }
    }
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeCmdCmdUseMedkit(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdUseMedkit called on client.");
    else
      ((Medkit) obj).CmdUseMedkit((int) reader.ReadPackedUInt32());
  }

  public void CallCmdUseMedkit(int id)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdUseMedkit called on server.");
    else if (this.get_isServer())
    {
      this.CmdUseMedkit(id);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Medkit.kCmdCmdUseMedkit);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.WritePackedUInt32((uint) id);
      this.SendCommandInternal(networkWriter, 2, "CmdUseMedkit");
    }
  }

  static Medkit()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Medkit), Medkit.kCmdCmdUseMedkit, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdUseMedkit)));
    NetworkCRC.RegisterBehaviour(nameof (Medkit), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
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
