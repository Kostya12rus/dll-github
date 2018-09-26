// Decompiled with JetBrains decompiler
// Type: ChopperAutostart
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class ChopperAutostart : NetworkBehaviour
{
  [SyncVar(hook = "SetState")]
  public bool isLanded = true;

  public void SetState(bool b)
  {
    this.NetworkisLanded = b;
    this.RefreshState();
  }

  private void Start()
  {
    this.RefreshState();
  }

  private void RefreshState()
  {
    this.GetComponent<Animator>().SetBool("IsLanded", this.isLanded);
  }

  private void UNetVersion()
  {
  }

  public bool NetworkisLanded
  {
    get
    {
      return this.isLanded;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.isLanded;
      int num2 = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetState(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<bool>(num1 != 0, ref local, (uint) num2);
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.isLanded);
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
      writer.Write(this.isLanded);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.isLanded = reader.ReadBoolean();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetState(reader.ReadBoolean());
    }
  }
}
