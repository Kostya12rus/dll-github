// Decompiled with JetBrains decompiler
// Type: Intercom
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using RemoteAdmin;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Intercom : NetworkBehaviour
{
  private static int kCmdCmdSetTransmit = 1248049261;
  private string content = string.Empty;
  private CharacterClassManager ccm;
  private Transform area;
  public float triggerDistance;
  private float speechTime;
  private float cooldownAfter;
  public float speechRemainingTime;
  public float remainingCooldown;
  public Text txt;
  [SyncVar(hook = "SetSpeaker")]
  public GameObject speaker;
  public static Intercom host;
  public GameObject start_sound;
  public GameObject stop_sound;
  private bool inUse;
  private bool isTransmitting;
  private NetworkInstanceId ___speakerNetId;
  private static int kRpcRpcPlaySound;
  private static int kRpcRpcUpdateText;

  private void SetSpeaker(GameObject go)
  {
    this.Networkspeaker = go;
  }

  private void Log(string s)
  {
  }

  [DebuggerHidden]
  private IEnumerator<float> _StartTransmitting(GameObject sp)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Intercom.\u003C_StartTransmitting\u003Ec__Iterator0() { sp = sp, \u0024this = this };
  }

  private void Start()
  {
    if (TutorialManager.status)
      return;
    this.txt = GameObject.Find("IntercomMonitor").GetComponent<Text>();
    this.ccm = this.GetComponent<CharacterClassManager>();
    this.area = GameObject.Find("IntercomSpeakingZone").transform;
    this.speechTime = (float) ConfigFile.ServerConfig.GetInt("intercom_max_speech_time", 20);
    this.cooldownAfter = (float) ConfigFile.ServerConfig.GetInt("intercom_cooldown", 180);
    Timing.RunCoroutine(this._FindHost());
    Timing.RunCoroutine(this._CheckForInput());
    if (!this.isLocalPlayer || !this.isServer)
      return;
    this.InvokeRepeating("RefreshText", 5f, 7f);
  }

  private void RefreshText()
  {
    this.CallRpcUpdateText(this.content);
  }

  [DebuggerHidden]
  private IEnumerator<float> _FindHost()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    Intercom.\u003C_FindHost\u003Ec__Iterator1 findHostCIterator1 = new Intercom.\u003C_FindHost\u003Ec__Iterator1();
    return (IEnumerator<float>) findHostCIterator1;
  }

  [ClientRpc]
  public void RpcPlaySound(bool start, int transmitterID)
  {
    if (PlayerManager.localPlayer.GetComponent<QueryProcessor>().PlayerId == transmitterID)
      AchievementManager.Achieve("isthisthingon");
    Object.Destroy((Object) Object.Instantiate<GameObject>(!start ? this.stop_sound : this.start_sound), 10f);
  }

  private void Update()
  {
    if (TutorialManager.status || !this.isLocalPlayer || !this.isServer)
      return;
    this.UpdateText();
  }

  private void UpdateText()
  {
    this.content = (double) this.remainingCooldown <= 0.0 ? (!((Object) this.speaker != (Object) null) ? "READY" : "TRANSMITTING...\nTIME LEFT - " + (object) Mathf.CeilToInt(this.speechRemainingTime)) : "RESTARTING\n" + (object) Mathf.CeilToInt(this.remainingCooldown);
    if (!(this.content != this.txt.text))
      return;
    this.CallRpcUpdateText(this.content);
  }

  [ClientRpc(channel = 2)]
  private void RpcUpdateText(string t)
  {
    try
    {
      this.txt.text = t;
    }
    catch
    {
    }
  }

  public void RequestTransmission(GameObject spk)
  {
    if ((Object) spk == (Object) null)
    {
      this.SetSpeaker((GameObject) null);
    }
    else
    {
      if ((double) this.remainingCooldown > 0.0 && !this.GetComponent<ServerRoles>().BypassMode || this.inUse)
        return;
      this.inUse = true;
      Timing.RunCoroutine(this._StartTransmitting(spk), Segment.Update);
    }
  }

  [DebuggerHidden]
  private IEnumerator<float> _CheckForInput()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Intercom.\u003C_CheckForInput\u003Ec__Iterator2() { \u0024this = this };
  }

  private bool ClientAllowToSpeak()
  {
    try
    {
      return (double) Vector3.Distance(this.transform.position, this.area.position) < (double) this.triggerDistance && Input.GetKey(NewInput.GetKey("Voice Chat")) && this.ccm.klasy[this.ccm.curClass].team != Team.SCP;
    }
    catch
    {
      return false;
    }
  }

  private bool ServerAllowToSpeak()
  {
    if ((double) Vector3.Distance(this.transform.position, this.area.position) < (double) this.triggerDistance)
      return this.ccm.klasy[this.ccm.curClass].team != Team.SCP;
    return false;
  }

  [Command(channel = 2)]
  private void CmdSetTransmit(bool player)
  {
    if (player)
    {
      if (!this.ServerAllowToSpeak())
        return;
      Intercom.host.RequestTransmission(this.gameObject);
    }
    else
    {
      if (!((Object) Intercom.host.speaker == (Object) this.gameObject))
        return;
      Intercom.host.RequestTransmission((GameObject) null);
    }
  }

  private void UNetVersion()
  {
  }

  public GameObject Networkspeaker
  {
    get
    {
      return this.speaker;
    }
    [param: In] set
    {
      GameObject newGameObject = value;
      ref GameObject local1 = ref this.speaker;
      int num = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetSpeaker(value);
        this.syncVarHookGuard = false;
      }
      ref NetworkInstanceId local2 = ref this.___speakerNetId;
      this.SetSyncVarGameObject(newGameObject, ref local1, (uint) num, ref local2);
    }
  }

  protected static void InvokeCmdCmdSetTransmit(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdSetTransmit called on client.");
    else
      ((Intercom) obj).CmdSetTransmit(reader.ReadBoolean());
  }

  public void CallCmdSetTransmit(bool player)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdSetTransmit called on server.");
    else if (this.isServer)
    {
      this.CmdSetTransmit(player);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Intercom.kCmdCmdSetTransmit);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(player);
      this.SendCommandInternal(writer, 2, "CmdSetTransmit");
    }
  }

  protected static void InvokeRpcRpcPlaySound(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcPlaySound called on server.");
    else
      ((Intercom) obj).RpcPlaySound(reader.ReadBoolean(), (int) reader.ReadPackedUInt32());
  }

  protected static void InvokeRpcRpcUpdateText(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcUpdateText called on server.");
    else
      ((Intercom) obj).RpcUpdateText(reader.ReadString());
  }

  public void CallRpcPlaySound(bool start, int transmitterID)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcPlaySound called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) Intercom.kRpcRpcPlaySound);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(start);
      writer.WritePackedUInt32((uint) transmitterID);
      this.SendRPCInternal(writer, 0, "RpcPlaySound");
    }
  }

  public void CallRpcUpdateText(string t)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcUpdateText called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) Intercom.kRpcRpcUpdateText);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(t);
      this.SendRPCInternal(writer, 2, "RpcUpdateText");
    }
  }

  static Intercom()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (Intercom), Intercom.kCmdCmdSetTransmit, new NetworkBehaviour.CmdDelegate(Intercom.InvokeCmdCmdSetTransmit));
    Intercom.kRpcRpcPlaySound = 239129888;
    NetworkBehaviour.RegisterRpcDelegate(typeof (Intercom), Intercom.kRpcRpcPlaySound, new NetworkBehaviour.CmdDelegate(Intercom.InvokeRpcRpcPlaySound));
    Intercom.kRpcRpcUpdateText = 1243388753;
    NetworkBehaviour.RegisterRpcDelegate(typeof (Intercom), Intercom.kRpcRpcUpdateText, new NetworkBehaviour.CmdDelegate(Intercom.InvokeRpcRpcUpdateText));
    NetworkCRC.RegisterBehaviour(nameof (Intercom), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.speaker);
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
      writer.Write(this.speaker);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.___speakerNetId = reader.ReadNetworkId();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetSpeaker(reader.ReadGameObject());
    }
  }

  public override void PreStartClient()
  {
    if (this.___speakerNetId.IsEmpty())
      return;
    this.Networkspeaker = ClientScene.FindLocalObject(this.___speakerNetId);
  }
}
