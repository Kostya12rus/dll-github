// Decompiled with JetBrains decompiler
// Type: ServerTime
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using GameConsole;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class ServerTime : NetworkBehaviour
{
  private static int kCmdCmdSetTime = 648282655;
  [SyncVar]
  public int timeFromStartup;
  public static int time;
  private const int allowedDeviation = 2;

  public ServerTime()
  {
    base.\u002Ector();
  }

  public static bool CheckSynchronization(int myTime)
  {
    int num = Mathf.Abs(myTime - ServerTime.time);
    if (num > 2)
      Console.singleton.AddLog("Damage sync error.", new Color32(byte.MaxValue, (byte) 200, (byte) 0, byte.MaxValue), false);
    return num <= 2;
  }

  private void Update()
  {
    if (!(((Object) this).get_name() == "Host"))
      return;
    ServerTime.time = this.timeFromStartup;
  }

  private void Start()
  {
    if (!this.get_isLocalPlayer() || !this.get_isServer())
      return;
    ((MonoBehaviour) this).InvokeRepeating("IncreaseTime", 1f, 1f);
  }

  private void IncreaseTime()
  {
    this.TransmitData(this.timeFromStartup + 1);
  }

  [ClientCallback]
  private void TransmitData(int timeFromStartup)
  {
    if (!NetworkClient.get_active())
      return;
    this.CallCmdSetTime(timeFromStartup);
  }

  [Command(channel = 12)]
  private void CmdSetTime(int t)
  {
    this.NetworktimeFromStartup = t;
  }

  private void UNetVersion()
  {
  }

  public int NetworktimeFromStartup
  {
    get
    {
      return this.timeFromStartup;
    }
    [param: In] set
    {
      this.SetSyncVar<int>((M0) value, (M0&) ref this.timeFromStartup, 1U);
    }
  }

  protected static void InvokeCmdCmdSetTime(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSetTime called on client.");
    else
      ((ServerTime) obj).CmdSetTime((int) reader.ReadPackedUInt32());
  }

  public void CallCmdSetTime(int t)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSetTime called on server.");
    else if (this.get_isServer())
    {
      this.CmdSetTime(t);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) ServerTime.kCmdCmdSetTime);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.WritePackedUInt32((uint) t);
      this.SendCommandInternal(networkWriter, 12, "CmdSetTime");
    }
  }

  static ServerTime()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (ServerTime), ServerTime.kCmdCmdSetTime, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSetTime)));
    NetworkCRC.RegisterBehaviour(nameof (ServerTime), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.WritePackedUInt32((uint) this.timeFromStartup);
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
      writer.WritePackedUInt32((uint) this.timeFromStartup);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.timeFromStartup = (int) reader.ReadPackedUInt32();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.timeFromStartup = (int) reader.ReadPackedUInt32();
    }
  }
}
