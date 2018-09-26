// Decompiled with JetBrains decompiler
// Type: Ragdoll
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

public class Ragdoll : NetworkBehaviour
{
  [SyncVar(hook = "SetOwner")]
  public Ragdoll.Info owner;
  [SyncVar(hook = "SetRecall")]
  public bool allowRecall;

  public void SetOwner(Ragdoll.Info s)
  {
    this.Networkowner = s;
  }

  private void Start()
  {
    this.Invoke("Unfr", 0.1f);
    this.Invoke("Refreeze", 7f);
  }

  public void SetRecall(bool b)
  {
    this.NetworkallowRecall = b;
  }

  private void Refreeze()
  {
    foreach (Object componentsInChild in this.GetComponentsInChildren<CharacterJoint>())
      Object.Destroy(componentsInChild);
    foreach (Object componentsInChild in this.GetComponentsInChildren<Rigidbody>())
      Object.Destroy(componentsInChild);
  }

  private void Unfr()
  {
    foreach (Rigidbody componentsInChild in this.GetComponentsInChildren<Rigidbody>())
      componentsInChild.isKinematic = false;
    foreach (Collider componentsInChild in this.GetComponentsInChildren<Collider>())
      componentsInChild.enabled = true;
  }

  private void UNetVersion()
  {
  }

  public Ragdoll.Info Networkowner
  {
    get
    {
      return this.owner;
    }
    [param: In] set
    {
      Ragdoll.Info info = value;
      ref Ragdoll.Info local = ref this.owner;
      int num = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetOwner(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<Ragdoll.Info>(info, ref local, (uint) num);
    }
  }

  public bool NetworkallowRecall
  {
    get
    {
      return this.allowRecall;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.allowRecall;
      int num2 = 2;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetRecall(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<bool>(num1 != 0, ref local, (uint) num2);
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      GeneratedNetworkCode._WriteInfo_Ragdoll(writer, this.owner);
      writer.Write(this.allowRecall);
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
      GeneratedNetworkCode._WriteInfo_Ragdoll(writer, this.owner);
    }
    if (((int) this.syncVarDirtyBits & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.allowRecall);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.owner = GeneratedNetworkCode._ReadInfo_Ragdoll(reader);
      this.allowRecall = reader.ReadBoolean();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.SetOwner(GeneratedNetworkCode._ReadInfo_Ragdoll(reader));
      if ((num & 2) == 0)
        return;
      this.SetRecall(reader.ReadBoolean());
    }
  }

  [Serializable]
  public struct Info
  {
    public string ownerHLAPI_id;
    public string steamClientName;
    public PlayerStats.HitInfo deathCause;
    public int charclass;

    public Info(string owner, string nick, PlayerStats.HitInfo info, int cc)
    {
      this.ownerHLAPI_id = owner;
      this.steamClientName = nick;
      this.charclass = cc;
      this.deathCause = info;
    }
  }
}
