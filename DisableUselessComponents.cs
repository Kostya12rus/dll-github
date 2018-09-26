// Decompiled with JetBrains decompiler
// Type: DisableUselessComponents
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class DisableUselessComponents : NetworkBehaviour
{
  [SyncVar(hook = "SetName")]
  private string label = "Player";
  [SyncVar(hook = "SetServer")]
  public bool isDedicated = true;
  private CharacterClassManager _ccm;
  private NicknameSync _ns;
  private bool _added;
  [SerializeField]
  private Behaviour[] uselessComponents;

  private void Start()
  {
    this._ns = this.GetComponent<NicknameSync>();
    if (NetworkServer.active)
      this.CmdSetName(!this.isLocalPlayer ? "Player" : "Host", this.isLocalPlayer && ServerStatic.IsDedicated);
    this._ccm = this.GetComponent<CharacterClassManager>();
    if (!this.isLocalPlayer)
    {
      Object.DestroyImmediate((Object) this.GetComponent<FirstPersonController>());
      foreach (Behaviour uselessComponent in this.uselessComponents)
        uselessComponent.enabled = false;
      Object.Destroy((Object) this.GetComponent<CharacterController>());
    }
    else
    {
      PlayerManager.localPlayer = this.gameObject;
      PlayerManager.spect = this.GetComponent<SpectatorManager>();
      this.GetComponent<FirstPersonController>().enabled = false;
    }
  }

  private void FixedUpdate()
  {
    if (!this._added && this._ccm.IsVerified && !string.IsNullOrEmpty(this._ns.myNick))
    {
      this._added = true;
      if (!this.isDedicated)
        PlayerManager.singleton.AddPlayer(this.gameObject);
      if (NetworkServer.active)
        ServerLogs.AddLog(ServerLogs.Modules.Networking, "Player connected and authenticated from IP" + this.connectionToClient.address + " with SteamID " + (!string.IsNullOrEmpty(this.GetComponent<CharacterClassManager>().SteamId) ? this.GetComponent<CharacterClassManager>().SteamId : "(unavailable)") + " and nickname " + this.GetComponent<NicknameSync>().myNick, ServerLogs.ServerLogType.ConnectionUpdate);
    }
    this.name = this.label;
  }

  private void OnDestroy()
  {
    if (this.isLocalPlayer || !((Object) PlayerManager.singleton != (Object) null))
      return;
    PlayerManager.singleton.RemovePlayer(this.gameObject);
  }

  [ServerCallback]
  private void CmdSetName(string n, bool b)
  {
    if (!NetworkServer.active)
      return;
    this.SetName(n);
    this.SetServer(b);
  }

  private void SetName(string n)
  {
    this.Networklabel = n;
  }

  private void SetServer(bool b)
  {
    this.NetworkisDedicated = b;
  }

  private void UNetVersion()
  {
  }

  public string Networklabel
  {
    get
    {
      return this.label;
    }
    [param: In] set
    {
      string str = value;
      ref string local = ref this.label;
      int num = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetName(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<string>(str, ref local, (uint) num);
    }
  }

  public bool NetworkisDedicated
  {
    get
    {
      return this.isDedicated;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.isDedicated;
      int num2 = 2;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetServer(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<bool>(num1 != 0, ref local, (uint) num2);
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.label);
      writer.Write(this.isDedicated);
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
      writer.Write(this.label);
    }
    if (((int) this.syncVarDirtyBits & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.isDedicated);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.label = reader.ReadString();
      this.isDedicated = reader.ReadBoolean();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.SetName(reader.ReadString());
      if ((num & 2) == 0)
        return;
      this.SetServer(reader.ReadBoolean());
    }
  }
}
