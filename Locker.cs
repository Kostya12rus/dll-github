// Decompiled with JetBrains decompiler
// Type: Locker
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class Locker : NetworkBehaviour
{
  public Vector3 localPos;
  public float searchTime;
  public int[] ids;
  [SyncVar]
  public bool isTaken;

  public int GetItem()
  {
    if (this.isTaken)
      return -1;
    return this.ids[Random.Range(0, this.ids.Length)];
  }

  public void SetTaken(bool b)
  {
    this.NetworkisTaken = b;
  }

  public void SetupPos()
  {
    this.localPos = this.transform.localPosition;
  }

  public void Update()
  {
    this.transform.localPosition = this.localPos;
  }

  private void UNetVersion()
  {
  }

  public bool NetworkisTaken
  {
    get
    {
      return this.isTaken;
    }
    [param: In] set
    {
      this.SetSyncVar<bool>(value, ref this.isTaken, 1U);
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.isTaken);
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
      writer.Write(this.isTaken);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.isTaken = reader.ReadBoolean();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.isTaken = reader.ReadBoolean();
    }
  }
}
