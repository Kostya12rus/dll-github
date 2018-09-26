// Decompiled with JetBrains decompiler
// Type: AmmoBox
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class AmmoBox : NetworkBehaviour
{
  private static int kCmdCmdDrop = -1122225972;
  private Inventory inv;
  private CharacterClassManager ccm;
  public AmmoBox.AmmoType[] types;
  [SyncVar(hook = "SetAmount")]
  public string amount;

  public void SetAmount(string am)
  {
    this.Networkamount = am;
  }

  public void SetOneAmount(int type, string value)
  {
    string[] strArray = this.amount.Split(':');
    strArray[type] = value;
    this.SetAmount(strArray[0] + ":" + strArray[1] + ":" + strArray[2]);
  }

  private void Start()
  {
    this.inv = this.GetComponent<Inventory>();
    this.ccm = this.GetComponent<CharacterClassManager>();
  }

  public void SetAmmoAmount()
  {
    int[] ammoTypes = this.ccm.klasy[this.ccm.curClass].ammoTypes;
    this.Networkamount = ammoTypes[0].ToString() + ":" + (object) ammoTypes[1] + ":" + (object) ammoTypes[2];
  }

  public int GetAmmo(int type)
  {
    int result = 0;
    if (this.amount.Contains(":"))
    {
      if (!int.TryParse(this.amount.Split(':')[Mathf.Clamp(type, 0, 2)], out result))
        MonoBehaviour.print((object) "Parse failed");
    }
    return result;
  }

  [Command(channel = 2)]
  public void CmdDrop(int _toDrop, int type)
  {
    for (int type1 = 0; type1 < 3; ++type1)
    {
      if (type1 == type)
      {
        _toDrop = Mathf.Clamp(_toDrop, 0, this.GetAmmo(type1));
        if (_toDrop >= 15)
        {
          string[] strArray = this.amount.Split(':');
          strArray[type1] = (this.GetAmmo(type1) - _toDrop).ToString();
          this.inv.SetPickup(this.types[type1].inventoryID, (float) _toDrop, this.transform.position, this.inv.camera.transform.rotation);
          this.Networkamount = strArray[0] + ":" + strArray[1] + ":" + strArray[2];
        }
      }
    }
  }

  private void UNetVersion()
  {
  }

  public string Networkamount
  {
    get
    {
      return this.amount;
    }
    [param: In] set
    {
      string str = value;
      ref string local = ref this.amount;
      int num = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetAmount(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<string>(str, ref local, (uint) num);
    }
  }

  protected static void InvokeCmdCmdDrop(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdDrop called on client.");
    else
      ((AmmoBox) obj).CmdDrop((int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32());
  }

  public void CallCmdDrop(int _toDrop, int type)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdDrop called on server.");
    else if (this.isServer)
    {
      this.CmdDrop(_toDrop, type);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) AmmoBox.kCmdCmdDrop);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.WritePackedUInt32((uint) _toDrop);
      writer.WritePackedUInt32((uint) type);
      this.SendCommandInternal(writer, 2, "CmdDrop");
    }
  }

  static AmmoBox()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (AmmoBox), AmmoBox.kCmdCmdDrop, new NetworkBehaviour.CmdDelegate(AmmoBox.InvokeCmdCmdDrop));
    NetworkCRC.RegisterBehaviour(nameof (AmmoBox), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.amount);
      return true;
    }
    bool flag = false;
    if (((int) this.syncVarDirtyBits & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.amount);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.amount = reader.ReadString();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetAmount(reader.ReadString());
    }
  }

  [Serializable]
  public class AmmoType
  {
    public string label;
    public int inventoryID;
  }
}
