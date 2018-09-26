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
  private string content;
  private bool inUse;
  private bool isTransmitting;
  private NetworkInstanceId ___speakerNetId;
  private static int kRpcRpcPlaySound;
  private static int kRpcRpcUpdateText;

  public Intercom()
  {
    base.\u002Ector();
  }

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
    return (IEnumerator<float>) new Intercom.\u003C_StartTransmitting\u003Ec__Iterator0()
    {
      sp = sp,
      \u0024this = this
    };
  }

  private void Start()
  {
    if (TutorialManager.status)
      return;
    this.txt = (Text) GameObject.Find("IntercomMonitor").GetComponent<Text>();
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this.area = GameObject.Find("IntercomSpeakingZone").get_transform();
    this.speechTime = (float) ConfigFile.ServerConfig.GetInt("intercom_max_speech_time", 20);
    this.cooldownAfter = (float) ConfigFile.ServerConfig.GetInt("intercom_cooldown", 180);
    Timing.RunCoroutine(this._FindHost());
    Timing.RunCoroutine(this._CheckForInput());
    if (!this.get_isLocalPlayer() || !this.get_isServer())
      return;
    ((MonoBehaviour) this).InvokeRepeating("RefreshText", 5f, 7f);
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
    if (((QueryProcessor) PlayerManager.localPlayer.GetComponent<QueryProcessor>()).PlayerId == transmitterID)
      AchievementManager.Achieve("isthisthingon");
    Object.Destroy((Object) Object.Instantiate<GameObject>(!start ? (M0) this.stop_sound : (M0) this.start_sound), 10f);
  }

  private void Update()
  {
    if (TutorialManager.status || !this.get_isLocalPlayer() || !this.get_isServer())
      return;
    this.UpdateText();
  }

  private void UpdateText()
  {
    this.content = (double) this.remainingCooldown <= 0.0 ? (!Object.op_Inequality((Object) this.speaker, (Object) null) ? "READY" : "TRANSMITTING...\nTIME LEFT - " + (object) Mathf.CeilToInt(this.speechRemainingTime)) : "RESTARTING\n" + (object) Mathf.CeilToInt(this.remainingCooldown);
    if (!(this.content != this.txt.get_text()))
      return;
    this.CallRpcUpdateText(this.content);
  }

  [ClientRpc(channel = 2)]
  private void RpcUpdateText(string t)
  {
    try
    {
      this.txt.set_text(t);
    }
    catch
    {
    }
  }

  public void RequestTransmission(GameObject spk)
  {
    if (Object.op_Equality((Object) spk, (Object) null))
    {
      this.SetSpeaker((GameObject) null);
    }
    else
    {
      if ((double) this.remainingCooldown > 0.0 && !((ServerRoles) ((Component) this).GetComponent<ServerRoles>()).BypassMode || this.inUse)
        return;
      this.inUse = true;
      Timing.RunCoroutine(this._StartTransmitting(spk), (Segment) 0);
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
      return (double) Vector3.Distance(((Component) this).get_transform().get_position(), this.area.get_position()) < (double) this.triggerDistance && Input.GetKey(NewInput.GetKey("Voice Chat")) && this.ccm.klasy[this.ccm.curClass].team != Team.SCP;
    }
    catch
    {
      return false;
    }
  }

  private bool ServerAllowToSpeak()
  {
    if ((double) Vector3.Distance(((Component) this).get_transform().get_position(), this.area.get_position()) < (double) this.triggerDistance)
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
      Intercom.host.RequestTransmission(((Component) this).get_gameObject());
    }
    else
    {
      if (!Object.op_Equality((Object) Intercom.host.speaker, (Object) ((Component) this).get_gameObject()))
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
      GameObject gameObject = value;
      ref GameObject local1 = ref this.speaker;
      int num = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetSpeaker(value);
        this.set_syncVarHookGuard(false);
      }
      ref NetworkInstanceId local2 = ref this.___speakerNetId;
      this.SetSyncVarGameObject((GameObject) gameObject, (GameObject&) ref local1, (uint) num, ref local2);
    }
  }

  protected static void InvokeCmdCmdSetTransmit(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSetTransmit called on client.");
    else
      ((Intercom) obj).CmdSetTransmit(reader.ReadBoolean());
  }

  public void CallCmdSetTransmit(bool player)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSetTransmit called on server.");
    else if (this.get_isServer())
    {
      this.CmdSetTransmit(player);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Intercom.kCmdCmdSetTransmit);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(player);
      this.SendCommandInternal(networkWriter, 2, "CmdSetTransmit");
    }
  }

  protected static void InvokeRpcRpcPlaySound(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcPlaySound called on server.");
    else
      ((Intercom) obj).RpcPlaySound(reader.ReadBoolean(), (int) reader.ReadPackedUInt32());
  }

  protected static void InvokeRpcRpcUpdateText(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcUpdateText called on server.");
    else
      ((Intercom) obj).RpcUpdateText(reader.ReadString());
  }

  public void CallRpcPlaySound(bool start, int transmitterID)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcPlaySound called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Intercom.kRpcRpcPlaySound);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(start);
      networkWriter.WritePackedUInt32((uint) transmitterID);
      this.SendRPCInternal(networkWriter, 0, "RpcPlaySound");
    }
  }

  public void CallRpcUpdateText(string t)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcUpdateText called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) Intercom.kRpcRpcUpdateText);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(t);
      this.SendRPCInternal(networkWriter, 2, "RpcUpdateText");
    }
  }

  static Intercom()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Intercom), Intercom.kCmdCmdSetTransmit, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSetTransmit)));
    Intercom.kRpcRpcPlaySound = 239129888;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Intercom), Intercom.kRpcRpcPlaySound, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcPlaySound)));
    Intercom.kRpcRpcUpdateText = 1243388753;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (Intercom), Intercom.kRpcRpcUpdateText, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcUpdateText)));
    NetworkCRC.RegisterBehaviour(nameof (Intercom), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write((GameObject) this.speaker);
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
      writer.Write((GameObject) this.speaker);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.___speakerNetId = reader.ReadNetworkId();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetSpeaker((GameObject) reader.ReadGameObject());
    }
  }

  public virtual void PreStartClient()
  {
    if (((NetworkInstanceId) ref this.___speakerNetId).IsEmpty())
      return;
    this.Networkspeaker = (GameObject) ClientScene.FindLocalObject(this.___speakerNetId);
  }
}
