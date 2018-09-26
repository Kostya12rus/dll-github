// Decompiled with JetBrains decompiler
// Type: BlastDoor
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (NetworkIdentity))]
public class BlastDoor : NetworkBehaviour
{
  [SyncVar(hook = "SetClosed")]
  public bool isClosed;

  public BlastDoor()
  {
    base.\u002Ector();
  }

  public void SetClosed(bool b)
  {
    this.NetworkisClosed = b;
    if (!this.isClosed)
      return;
    ((Animator) ((Component) this).GetComponent<Animator>()).SetTrigger("Close");
  }

  private void UNetVersion()
  {
  }

  public bool NetworkisClosed
  {
    get
    {
      return this.isClosed;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.isClosed;
      int num2 = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetClosed(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<bool>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.isClosed);
      return true;
    }
    bool flag = false;
    if (((int) this.get_syncVarDirtyBits() & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.isClosed);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.isClosed = reader.ReadBoolean();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetClosed(reader.ReadBoolean());
    }
  }
}
