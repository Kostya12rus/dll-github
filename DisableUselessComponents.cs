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
  private CharacterClassManager _ccm;
  private NicknameSync _ns;
  private bool _added;
  [SerializeField]
  private Behaviour[] uselessComponents;
  [SyncVar(hook = "SetName")]
  private string label;
  [SyncVar(hook = "SetServer")]
  public bool isDedicated;

  public DisableUselessComponents()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this._ns = (NicknameSync) ((Component) this).GetComponent<NicknameSync>();
    if (NetworkServer.get_active())
      this.CmdSetName(!this.get_isLocalPlayer() ? "Player" : "Host", this.get_isLocalPlayer() && ServerStatic.IsDedicated);
    this._ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    if (!this.get_isLocalPlayer())
    {
      Object.DestroyImmediate((Object) ((Component) this).GetComponent<FirstPersonController>());
      foreach (Behaviour uselessComponent in this.uselessComponents)
        uselessComponent.set_enabled(false);
      Object.Destroy((Object) ((Component) this).GetComponent<CharacterController>());
    }
    else
    {
      PlayerManager.localPlayer = ((Component) this).get_gameObject();
      PlayerManager.spect = (SpectatorManager) ((Component) this).GetComponent<SpectatorManager>();
      ((Behaviour) ((Component) this).GetComponent<FirstPersonController>()).set_enabled(false);
    }
  }

  private void FixedUpdate()
  {
    if (!this._added && this._ccm.IsVerified && !string.IsNullOrEmpty(this._ns.myNick))
    {
      this._added = true;
      if (!this.isDedicated)
        PlayerManager.singleton.AddPlayer(((Component) this).get_gameObject());
      if (NetworkServer.get_active())
        ServerLogs.AddLog(ServerLogs.Modules.Networking, "Player connected and authenticated from IP" + (string) this.get_connectionToClient().address + " with SteamID " + (!string.IsNullOrEmpty(((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).SteamId) ? ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).SteamId : "(unavailable)") + " and nickname " + ((NicknameSync) ((Component) this).GetComponent<NicknameSync>()).myNick, ServerLogs.ServerLogType.ConnectionUpdate);
    }
    ((Object) this).set_name(this.label);
  }

  private void OnDestroy()
  {
    if (this.get_isLocalPlayer() || !Object.op_Inequality((Object) PlayerManager.singleton, (Object) null))
      return;
    PlayerManager.singleton.RemovePlayer(((Component) this).get_gameObject());
  }

  [ServerCallback]
  private void CmdSetName(string n, bool b)
  {
    if (!NetworkServer.get_active())
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
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetName(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<string>((M0) str, (M0&) ref local, (uint) num);
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
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetServer(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<bool>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.label);
      writer.Write(this.isDedicated);
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
      writer.Write(this.label);
    }
    if (((int) this.get_syncVarDirtyBits() & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.isDedicated);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
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
