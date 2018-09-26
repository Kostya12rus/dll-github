// Decompiled with JetBrains decompiler
// Type: CharacterClassManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using RemoteAdmin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PostProcessing;

public class CharacterClassManager : NetworkBehaviour
{
  private static int kCmdCmdSendToken = 970325235;
  public Class[] klasy;
  private static Class[] _staticClasses;
  public List<Team> classTeamQueue;
  public GameObject unfocusedCamera;
  private GameObject _plyCam;
  private CentralAuthInterface _centralAuthInt;
  private static GameObject _host;
  public float ciPercentage;
  private float _aliveTime;
  public int forceClass;
  private int _seed;
  private int _prevId;
  public bool OnlineMode;
  public bool GodMode;
  private bool _wasAnytimeAlive;
  private bool _commandtokensent;
  public string AuthToken;
  [SerializeField]
  private AudioClip bell;
  [SerializeField]
  private AudioClip bell_dead;
  [HideInInspector]
  public GameObject myModel;
  [HideInInspector]
  public GameObject charCamera;
  private Scp049PlayerScript _scp049;
  private Scp049_2PlayerScript _scp0492;
  private Scp079PlayerScript _scp079;
  private Scp106PlayerScript _scp106;
  private Scp173PlayerScript _scp173;
  private Scp096PlayerScript _scp096;
  private Scp939PlayerScript _scp939;
  private LureSubjectContainer _lureSpj;
  [SyncVar(hook = "SetClassID")]
  public int curClass;
  [SyncVar(hook = "SyncDeathPos")]
  public Vector3 deathPosition;
  [SyncVar(hook = "SetUnit")]
  public int ntfUnit;
  [SyncVar(hook = "SetRoundStart")]
  public bool roundStarted;
  [SyncVar(hook = "SetVerification")]
  public bool IsVerified;
  [SyncVar(hook = "SetSteamId")]
  public string SteamId;
  private static int kRpcRpcPlaceBlood;
  private static int kTargetRpcTargetConsolePrint;
  private static int kCmdCmdRequestContactEmail;
  private static int kCmdCmdRequestServerConfig;
  private static int kCmdCmdRequestServerGroups;
  private static int kCmdCmdRequestHideTag;
  private static int kCmdCmdRequestShowTag;
  private static int kCmdCmdSuicide;
  private static int kTargetRpcTargetSetDisconnectError;
  private static int kCmdCmdConfirmDisconnect;
  private static int kCmdCmdRegisterEscape;
  private static int kCmdCmdRequestDeathScreen;
  private static int kTargetRpcTargetDeathScreen;

  public CharacterClassManager()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.OnlineMode = ConfigFile.ServerConfig.GetBool("online_mode", true);
    this._centralAuthInt = new CentralAuthInterface(this, this.get_isServer());
    this._lureSpj = (LureSubjectContainer) Object.FindObjectOfType<LureSubjectContainer>();
    this._scp049 = (Scp049PlayerScript) ((Component) this).GetComponent<Scp049PlayerScript>();
    this._scp0492 = (Scp049_2PlayerScript) ((Component) this).GetComponent<Scp049_2PlayerScript>();
    this._scp079 = (Scp079PlayerScript) ((Component) this).GetComponent<Scp079PlayerScript>();
    this._scp106 = (Scp106PlayerScript) ((Component) this).GetComponent<Scp106PlayerScript>();
    this._scp173 = (Scp173PlayerScript) ((Component) this).GetComponent<Scp173PlayerScript>();
    this._scp096 = (Scp096PlayerScript) ((Component) this).GetComponent<Scp096PlayerScript>();
    this._scp939 = (Scp939PlayerScript) ((Component) this).GetComponent<Scp939PlayerScript>();
    this.forceClass = ConfigFile.ServerConfig.GetInt("server_forced_class", -1);
    this.ciPercentage = (float) ConfigFile.ServerConfig.GetInt("ci_on_start_percent", 10);
    ((MonoBehaviour) this).StartCoroutine(this.Init());
    string str = ConfigFile.ServerConfig.GetString("team_respawn_queue", "401431403144144");
    this.classTeamQueue.Clear();
    foreach (char ch in str)
    {
      int result = 4;
      if (!int.TryParse(ch.ToString(), out result))
        result = 4;
      this.classTeamQueue.Add((Team) result);
    }
    while (this.classTeamQueue.Count < ((NetworkManager) NetworkManager.singleton).get_maxConnections())
      this.classTeamQueue.Add(Team.CDP);
    if (!this.get_isLocalPlayer() && TutorialManager.status)
      this.ApplyProperties();
    if (this.get_isLocalPlayer())
    {
      for (int v = 0; v < this.klasy.Length; ++v)
      {
        if (this.klasy[v].team != Team.SCP)
        {
          this.klasy[v].fullName = TranslationReader.Get("Class_Names", v);
          this.klasy[v].description = TranslationReader.Get("Class_Descriptions", v);
        }
      }
      CharacterClassManager._staticClasses = this.klasy;
      if (SteamManager.Initialized)
      {
        CentralAuth.singleton.GenerateToken((ICentralAuth) this._centralAuthInt);
      }
      else
      {
        GameConsole.Console.singleton.AddLog("Steam not initialized - sending empty auth token.\nIf server is using online mode, you will probably get kicked.", Color32.op_Implicit(Color.get_red()), false);
        this.CallCmdSendToken(string.Empty);
      }
    }
    else if (CharacterClassManager._staticClasses == null || CharacterClassManager._staticClasses.Length == 0)
    {
      for (int v = 0; v < this.klasy.Length; ++v)
      {
        this.klasy[v].description = TranslationReader.Get("Class_Descriptions", v);
        if (this.klasy[v].team != Team.SCP)
          this.klasy[v].fullName = TranslationReader.Get("Class_Names", v);
      }
    }
    else
      this.klasy = CharacterClassManager._staticClasses;
  }

  private void Update()
  {
    if (this.curClass == 2)
      this._aliveTime = 0.0f;
    else
      this._aliveTime += Time.get_deltaTime();
    if (this.get_isLocalPlayer())
    {
      if (ServerStatic.IsDedicated)
        CursorManager.isServerOnly = true;
      if (this.get_isServer())
        this.AllowContain();
    }
    if (this._prevId != this.curClass)
    {
      this.RefreshPlyModel(-1);
      this._prevId = this.curClass;
    }
    if (!(((Object) this).get_name() == "Host"))
      return;
    Radio.roundStarted = this.roundStarted;
  }

  private void SetVerification(bool b)
  {
    this.NetworkIsVerified = b;
  }

  public void SetUnit(int unit)
  {
    this.NetworkntfUnit = unit;
  }

  public void SyncDeathPos(Vector3 v)
  {
    this.NetworkdeathPosition = v;
  }

  public void SetSteamId(string i)
  {
    this.NetworkSteamId = i;
  }

  [ServerCallback]
  public void AllowContain()
  {
    if (!NetworkServer.get_active() || TutorialManager.status)
      return;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if ((double) Vector3.Distance(player.get_transform().get_position(), ((Component) this._lureSpj).get_transform().get_position()) < 1.97000002861023)
      {
        CharacterClassManager component1 = (CharacterClassManager) player.GetComponent<CharacterClassManager>();
        PlayerStats component2 = (PlayerStats) player.GetComponent<PlayerStats>();
        if (component1.klasy[component1.curClass].team != Team.SCP && component1.curClass != 2)
        {
          component2.HurtPlayer(new PlayerStats.HitInfo(10000f, "WORLD", "LURE", 0), player);
          this._lureSpj.SetState(true);
        }
      }
    }
  }

  [ClientRpc]
  public void RpcPlaceBlood(Vector3 pos, int type, float f)
  {
    ((BloodDrawer) ((Component) this).GetComponent<BloodDrawer>()).PlaceUnderneath(pos, type, f);
  }

  [TargetRpc(channel = 2)]
  public void TargetConsolePrint(NetworkConnection connection, string text, string color)
  {
    Color.get_grey();
    color = color.ToLower();
    Color color1;
    if (color != null)
    {
      // ISSUE: reference to a compiler-generated field
      if (CharacterClassManager.\u003C\u003Ef__switch\u0024map0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        CharacterClassManager.\u003C\u003Ef__switch\u0024map0 = new Dictionary<string, int>(7)
        {
          {
            "red",
            0
          },
          {
            "cyan",
            1
          },
          {
            "blue",
            2
          },
          {
            "magenta",
            3
          },
          {
            "white",
            4
          },
          {
            "green",
            5
          },
          {
            "yellow",
            6
          }
        };
      }
      int num;
      // ISSUE: reference to a compiler-generated field
      if (CharacterClassManager.\u003C\u003Ef__switch\u0024map0.TryGetValue(color, out num))
      {
        switch (num)
        {
          case 0:
            color1 = Color.get_red();
            goto label_13;
          case 1:
            color1 = Color.get_cyan();
            goto label_13;
          case 2:
            color1 = Color.get_blue();
            goto label_13;
          case 3:
            color1 = Color.get_magenta();
            goto label_13;
          case 4:
            color1 = Color.get_white();
            goto label_13;
          case 5:
            color1 = Color.get_green();
            goto label_13;
          case 6:
            color1 = Color.get_yellow();
            goto label_13;
        }
      }
    }
    color1 = Color.get_grey();
label_13:
    GameConsole.Console.singleton.AddLog("[MESSAGE FROM SERVER] " + text, Color32.op_Implicit(color1), false);
  }

  public bool IsHuman()
  {
    if (this.curClass > 0 && this.klasy[this.curClass].team != Team.SCP)
      return this.klasy[this.curClass].team != Team.RIP;
    return false;
  }

  public bool IsTargetForSCPs()
  {
    if (this.curClass > 0 && this.klasy[this.curClass].team != Team.SCP && this.klasy[this.curClass].team != Team.RIP)
      return this.klasy[this.curClass].team != Team.CHI;
    return false;
  }

  [DebuggerHidden]
  private IEnumerator Init()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new CharacterClassManager.\u003CInit\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  [Command(channel = 2)]
  public void CmdSendToken(string token)
  {
    if (ConfigFile.ServerConfig.GetBool("online_mode", true))
    {
      if (this._commandtokensent)
      {
        if (!this.get_isLocalPlayer())
          ServerConsole.Disconnect(this.get_connectionToClient(), "Your client sent second authentication token.");
      }
      else if (string.IsNullOrEmpty(token) || this._commandtokensent)
      {
        if (!this.get_isLocalPlayer())
          ServerConsole.Disconnect(this.get_connectionToClient(), "Your client sent an empty authentication token. Make sure you are running the game by steam.");
        else
          this.SetVerification(true);
      }
      else
      {
        CentralAuth.singleton.StartValidateToken((ICentralAuth) this._centralAuthInt, token);
        this.AuthToken = token;
      }
    }
    this._commandtokensent = true;
  }

  [Command(channel = 2)]
  public void CmdRequestContactEmail()
  {
    if (((ServerRoles) ((Component) this).GetComponent<ServerRoles>()).RemoteAdmin || ((ServerRoles) ((Component) this).GetComponent<ServerRoles>()).Staff)
      this.CallTargetConsolePrint(this.get_connectionToClient(), "Contact email address: " + ConfigFile.ServerConfig.GetString("contact_email", string.Empty), "green");
    else
      this.CallTargetConsolePrint(this.get_connectionToClient(), "You don't have permissions to execute this command.", "red");
  }

  [Command(channel = 2)]
  public void CmdRequestServerConfig()
  {
    YamlConfig serverConfig = ConfigFile.ServerConfig;
    if (((ServerRoles) ((Component) this).GetComponent<ServerRoles>()).RemoteAdmin || ((ServerRoles) ((Component) this).GetComponent<ServerRoles>()).Staff)
      this.CallTargetConsolePrint(this.get_connectionToClient(), "Extended server configuration:\nServer name: " + serverConfig.GetString("server_name", string.Empty) + "\nServer IP: " + serverConfig.GetString("server_ip", string.Empty) + "\nCurrent Server IP:: " + CustomNetworkManager.Ip + "\nServer pastebin ID: " + serverConfig.GetString("serverinfo_pastebin_id", string.Empty) + "\nServer max players: " + (object) serverConfig.GetInt("max_players", 0) + "\nOnline mode: " + (object) serverConfig.GetBool("online_mode", false) + "\nRA password authentication: " + (object) ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).OverridePasswordEnabled + "\nIP banning: " + (object) serverConfig.GetBool("ip_banning", false) + "\nWhitelist: " + (object) serverConfig.GetBool("enable_whitelist", false) + "\nQuery status: " + (object) serverConfig.GetBool("enable_query", false) + " with port shift " + (object) serverConfig.GetInt("query_port_shift", 0) + "\nFriendly fire: " + (object) serverConfig.GetBool("friendly_fire", false) + "\nMap seed: " + (object) serverConfig.GetInt("map_seed", 0), "green");
    else
      this.CallTargetConsolePrint(this.get_connectionToClient(), "Basic server configuration:\nServer name: " + serverConfig.GetString("server_name", string.Empty) + "\nServer IP: " + serverConfig.GetString("server_ip", string.Empty) + "\nServer pastebin ID: " + serverConfig.GetString("serverinfo_pastebin_id", string.Empty) + "\nServer max players: " + (object) serverConfig.GetInt("max_players", 0) + "\nRA password authentication: " + (object) ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).OverridePasswordEnabled + "\nOnline mode: " + (object) serverConfig.GetBool("online_mode", false) + "\nWhitelist: " + (object) serverConfig.GetBool("enable_whitelist", false) + "\nFriendly fire: " + (object) serverConfig.GetBool("friendly_fire", false) + "\nMap seed: " + (object) serverConfig.GetInt("map_seed", 0), "green");
  }

  [Command(channel = 2)]
  public void CmdRequestServerGroups()
  {
    string str = "Groups defined on this server:";
    Dictionary<string, UserGroup> allGroups = ServerStatic.PermissionsHandler.GetAllGroups();
    ServerRoles.NamedColor[] namedColors = ((ServerRoles) ((Component) this).GetComponent<ServerRoles>()).NamedColors;
    foreach (KeyValuePair<string, UserGroup> keyValuePair in allGroups)
    {
      KeyValuePair<string, UserGroup> permentry = keyValuePair;
      try
      {
        str = str + "\n" + permentry.Key + " (" + (object) permentry.Value.Permissions + ") - <color=#" + ((IEnumerable<ServerRoles.NamedColor>) namedColors).FirstOrDefault<ServerRoles.NamedColor>((Func<ServerRoles.NamedColor, bool>) (x => x.Name == permentry.Value.BadgeColor)).ColorHex + ">" + permentry.Value.BadgeText + "</color> in color " + permentry.Value.BadgeColor;
      }
      catch
      {
        str = str + "\n" + permentry.Key + " (" + (object) permentry.Value.Permissions + ") - " + permentry.Value.BadgeText + " in color " + permentry.Value.BadgeColor;
      }
      if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.KickingAndShortTermBanning))
        str += " K";
      if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.BanningUpToDay))
        str += " B1";
      if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.LongTermBanning))
        str += " B2";
      if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ForceclassSelf))
        str += " FSE";
      if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ForceclassToSpectator))
        str += " FSP";
      if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ForceclassWithoutRestrictions))
        str += " FC";
      if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.GivingItems))
        str += " G";
      if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.WarheadEvents))
        str += " EW";
      if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.RespawnEvents))
        str += " ERS";
      if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.RoundEvents))
        str += " ERD";
      if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.SetGroup))
        str += " SG";
      if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.FacilityManagement))
        str += " FM";
    }
    this.CallTargetConsolePrint(this.get_connectionToClient(), "Defined groups on server " + str, "grey");
  }

  [Command(channel = 2)]
  public void CmdRequestHideTag()
  {
    ServerRoles component = (ServerRoles) ((Component) this).GetComponent<ServerRoles>();
    component.HiddenBadge = component.MyText;
    component.SetBadgeUpdate(string.Empty);
    component.SetText(string.Empty);
    component.SetColor("default");
    component.NetworkGlobalSet = false;
    component.RefreshHiddenTag();
    this.CallTargetConsolePrint(this.get_connectionToClient(), "Badge hidden.", "green");
  }

  [Command(channel = 2)]
  public void CmdRequestShowTag(bool global)
  {
    ServerRoles component = (ServerRoles) ((Component) this).GetComponent<ServerRoles>();
    if (global)
    {
      if (string.IsNullOrEmpty(component.PrevBadge))
      {
        this.CallTargetConsolePrint(this.get_connectionToClient(), "You don't have global tag.", "magenta");
      }
      else
      {
        component.SetBadgeUpdate(component.PrevBadge);
        component.NetworkGlobalSet = true;
        component.HiddenBadge = string.Empty;
        component.CallRpcResetFixed();
        this.CallTargetConsolePrint(this.get_connectionToClient(), "Global tag refreshed.", "green");
      }
    }
    else
    {
      component.SetBadgeUpdate(string.Empty);
      component.HiddenBadge = string.Empty;
      component.CallRpcResetFixed();
      component.RefreshPermissions();
      this.CallTargetConsolePrint(this.get_connectionToClient(), "Local tag refreshed.", "green");
    }
  }

  [Command]
  public void CmdSuicide(PlayerStats.HitInfo hitInfo)
  {
    hitInfo.amount = (double) hitInfo.amount != 0.0 ? hitInfo.amount : 999799f;
    ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).HurtPlayer(hitInfo, ((Component) this).get_gameObject());
  }

  public void ForceRoundStart()
  {
    if (!NetworkServer.get_active())
      return;
    ServerLogs.AddLog(ServerLogs.Modules.Logger, "Round has been started.", ServerLogs.ServerLogType.GameEvent);
    ServerConsole.AddLog("New round has been started.");
    RoundStart.singleton.Networkinfo = "started";
  }

  [TargetRpc(channel = 2)]
  private void TargetSetDisconnectError(NetworkConnection conn, string message)
  {
    ((CustomNetworkManager) NetworkManager.singleton).disconnectMessage = message;
    this.CallCmdConfirmDisconnect();
  }

  [Command(channel = 2)]
  private void CmdConfirmDisconnect()
  {
    if (this.get_connectionToClient() == null || !this.get_connectionToClient().get_isConnected())
      return;
    this.get_connectionToClient().Disconnect();
    this.get_connectionToClient().Dispose();
  }

  public void DisconnectClient(NetworkConnection conn, string message)
  {
    this.CallTargetSetDisconnectError(conn, message);
    Timing.RunCoroutine(this._DisconnectAfterTimeout(conn), (Segment) 0);
  }

  [DebuggerHidden]
  private IEnumerator<float> _DisconnectAfterTimeout(NetworkConnection conn)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new CharacterClassManager.\u003C_DisconnectAfterTimeout\u003Ec__Iterator1()
    {
      conn = conn
    };
  }

  public void InitSCPs()
  {
    if (this.curClass == -1 || TutorialManager.status)
      return;
    Class c = this.klasy[this.curClass];
    this._scp049.Init(this.curClass, c);
    this._scp0492.Init(this.curClass, c);
    this._scp106.Init(this.curClass, c);
    this._scp173.Init(this.curClass, c);
    this._scp096.Init(this.curClass, c);
    this._scp939.Init(this.curClass, c);
  }

  public void RegisterEscape()
  {
    this.CallCmdRegisterEscape();
  }

  [Command(channel = 2)]
  private void CmdRegisterEscape()
  {
    if ((double) Vector3.Distance(((Component) this).get_transform().get_position(), ((Escape) ((Component) this).GetComponent<Escape>()).worldPosition) >= (double) (((Escape) ((Component) this).GetComponent<Escape>()).radius * 2))
      return;
    CharacterClassManager component = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    if (this.klasy[component.curClass].team == Team.CDP)
    {
      this.SetPlayersClass(8, ((Component) this).get_gameObject());
      ++RoundSummary.escaped_ds;
    }
    if (this.klasy[component.curClass].team != Team.RSC)
      return;
    this.SetPlayersClass(4, ((Component) this).get_gameObject());
    ++RoundSummary.escaped_scientists;
  }

  public bool IsScpButNotZombie()
  {
    if (this.curClass >= 0 && this.curClass != 10)
      return this.klasy[this.curClass].team == Team.SCP;
    return false;
  }

  public void ApplyProperties()
  {
    Class @class = this.klasy[this.curClass];
    this.InitSCPs();
    if (this.curClass != 2)
      this._wasAnytimeAlive = true;
    if (@class.team == Team.MTF)
      AchievementManager.Achieve("arescue");
    if (@class.team == Team.CHI)
      AchievementManager.Achieve("chaos");
    Inventory component1 = (Inventory) ((Component) this).GetComponent<Inventory>();
    PlyMovementSync component2 = (PlyMovementSync) ((Component) this).GetComponent<PlyMovementSync>();
    try
    {
      ((FootstepSync) ((Component) this).GetComponent<FootstepSync>()).SetLoundness(@class.team, @class.fullName.Contains("939"));
    }
    catch
    {
    }
    ((SpectatorManager) PlayerManager.localPlayer.GetComponent<SpectatorManager>()).RefreshList();
    if (this.get_isLocalPlayer())
    {
      ((FirstPersonController) ((Component) this).GetComponent<FirstPersonController>()).isSCP = (__Null) (@class.team == Team.SCP ? 1 : 0);
      DiscordManager.singleton.ChangePreset(this.curClass);
      ((Radio) ((Component) this).GetComponent<Radio>()).UpdateClass();
      ((Handcuffs) ((Component) this).GetComponent<Handcuffs>()).CallCmdTarget((GameObject) null);
      ((WeaponManager) ((Component) this).GetComponent<WeaponManager>()).flashlightEnabled = true;
      ((Searching) ((Component) this).GetComponent<Searching>()).Init(@class.team == Team.SCP | @class.team == Team.RIP);
    }
    if (@class.team == Team.RIP)
    {
      if (this.get_isServer())
      {
        component2.SetPosition(new Vector3(0.0f, 2048f, 0.0f));
        component2.SetRotation(0.0f);
      }
      if (this.get_isLocalPlayer())
      {
        component1.NetworkcurItem = -1;
        ((Behaviour) ((Component) this).GetComponent<FirstPersonController>()).set_enabled(false);
        if (this.curClass != 2 || Radio.roundStarted)
        {
          if (this._wasAnytimeAlive)
            this.CallCmdRequestDeathScreen();
          else
            ((StartScreen) Object.FindObjectOfType<StartScreen>()).PlayAnimation(this.curClass);
          ((HorrorSoundController) ((Component) this).GetComponent<HorrorSoundController>()).horrorSoundSource.PlayOneShot(this.bell_dead);
        }
        ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).maxHP = @class.maxHP;
        ((Behaviour) this.unfocusedCamera.GetComponent<Camera>()).set_enabled(false);
        ((Behaviour) this.unfocusedCamera.GetComponent<PostProcessingBehaviour>()).set_enabled(false);
      }
    }
    else
    {
      if (NetworkServer.get_active())
      {
        GameObject randomPosition = ((SpawnpointManager) Object.FindObjectOfType<SpawnpointManager>()).GetRandomPosition(this.curClass);
        if (Object.op_Inequality((Object) randomPosition, (Object) null))
        {
          component2.SetPosition(randomPosition.get_transform().get_position());
          PlyMovementSync plyMovementSync = component2;
          Quaternion rotation = randomPosition.get_transform().get_rotation();
          // ISSUE: variable of the null type
          __Null y = ((Quaternion) ref rotation).get_eulerAngles().y;
          plyMovementSync.SetRotation((float) y);
        }
        else
          component2.SetPosition(this.deathPosition);
      }
      if (this.get_isLocalPlayer())
      {
        ((Scp106PlayerScript) ((Component) this).GetComponent<Scp106PlayerScript>()).SetDoors();
        component1.NetworkcurItem = -1;
        ((StartScreen) Object.FindObjectOfType<StartScreen>()).PlayAnimation(this.curClass);
        if (!((HorrorSoundController) ((Component) this).GetComponent<HorrorSoundController>()).horrorSoundSource.get_isPlaying())
          ((HorrorSoundController) ((Component) this).GetComponent<HorrorSoundController>()).horrorSoundSource.PlayOneShot(this.bell);
        ((MonoBehaviour) this).Invoke("EnableFPC", 0.2f);
        ((Radio) ((Component) this).GetComponent<Radio>()).ResetPreset();
        ((MonoBehaviour) ((Component) this).GetComponent<Radio>()).Invoke("ResetPreset", 2f);
        FirstPersonController component3 = (FirstPersonController) ((Component) this).GetComponent<FirstPersonController>();
        PlayerStats component4 = (PlayerStats) ((Component) this).GetComponent<PlayerStats>();
        ((Behaviour) this.unfocusedCamera.GetComponent<Camera>()).set_enabled(true);
        ((Behaviour) this.unfocusedCamera.GetComponent<PostProcessingBehaviour>()).set_enabled(true);
        component3.m_WalkSpeed = (__Null) (double) @class.walkSpeed;
        component3.m_RunSpeed = (__Null) (double) @class.runSpeed;
        component3.m_UseHeadBob = (__Null) (@class.useHeadBob ? 1 : 0);
        component3.m_JumpSpeed = (__Null) (double) @class.jumpSpeed;
        int maxHp = @class.maxHP;
        component4.maxHP = maxHp;
        ((UserMainInterface) Object.FindObjectOfType<UserMainInterface>()).lerpedHP = (float) maxHp;
        SkyboxFollower.iAm939 = @class.fullName.Contains("939");
      }
      else
        ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).maxHP = @class.maxHP;
    }
    this._scp049.iAm049 = this.curClass == 5;
    this._scp0492.iAm049_2 = this.curClass == 10;
    this._scp096.iAm096 = this.curClass == 9;
    this._scp106.iAm106 = this.curClass == 3;
    this._scp173.iAm173 = this.curClass == 0;
    this._scp939.iAm939 = this.curClass >= 0 && this.curClass < this.klasy.Length && this.klasy[this.curClass].fullName.Contains("939");
    if (this.get_isLocalPlayer())
    {
      ((InventoryDisplay) Object.FindObjectOfType<InventoryDisplay>()).isSCP = this.curClass == 2 | @class.team == Team.SCP;
      ((InterfaceColorAdjuster) Object.FindObjectOfType<InterfaceColorAdjuster>()).ChangeColor(@class.classColor);
    }
    this.RefreshPlyModel(-1);
    QueryProcessor.StaticRefreshPlayerList();
  }

  private void EnableFPC()
  {
    ((Behaviour) ((Component) this).GetComponent<FirstPersonController>()).set_enabled(true);
  }

  public void RefreshPlyModel(int classID = -1)
  {
    ((AnimationController) ((Component) this).GetComponent<AnimationController>()).OnChangeClass();
    if (Object.op_Inequality((Object) this.myModel, (Object) null))
      Object.Destroy((Object) this.myModel);
    Class @class = this.klasy[classID >= 0 ? classID : this.curClass];
    if (@class.team != Team.RIP)
    {
      GameObject gameObject = (GameObject) Object.Instantiate<GameObject>((M0) @class.model_player);
      gameObject.get_transform().SetParent(((Component) this).get_gameObject().get_transform());
      gameObject.get_transform().set_localPosition(@class.model_offset.position);
      gameObject.get_transform().set_localRotation(Quaternion.Euler(@class.model_offset.rotation));
      gameObject.get_transform().set_localScale(@class.model_offset.scale);
      this.myModel = gameObject;
      if (Object.op_Inequality((Object) this.myModel.GetComponent<Animator>(), (Object) null))
        ((AnimationController) ((Component) this).GetComponent<AnimationController>()).animator = (Animator) this.myModel.GetComponent<Animator>();
      if (this.get_isLocalPlayer())
      {
        if (Object.op_Inequality((Object) this.myModel.GetComponent<Renderer>(), (Object) null))
          ((Renderer) this.myModel.GetComponent<Renderer>()).set_enabled(false);
        foreach (Renderer componentsInChild in (Renderer[]) this.myModel.GetComponentsInChildren<Renderer>())
          componentsInChild.set_enabled(false);
        foreach (Collider componentsInChild in (Collider[]) this.myModel.GetComponentsInChildren<Collider>())
        {
          if (((Object) componentsInChild).get_name() != "LookingTarget")
            componentsInChild.set_enabled(false);
        }
      }
    }
    ((Collider) ((Component) this).GetComponent<CapsuleCollider>()).set_enabled(@class.team != Team.RIP);
    if (!Object.op_Inequality((Object) this.myModel, (Object) null))
      return;
    ((WeaponManager) ((Component) this).GetComponent<WeaponManager>()).hitboxes = (HitboxIdentity[]) this.myModel.GetComponentsInChildren<HitboxIdentity>(true);
  }

  public void SetClassID(int id)
  {
    if (!this.IsVerified && id != 2)
      return;
    if (((ServerRoles) ((Component) this).GetComponent<ServerRoles>()).OverwatchEnabled && id != 2)
    {
      if (this.curClass == 2)
        return;
      id = 2;
    }
    this.NetworkcurClass = id;
    if (id == 2 && !this.get_isLocalPlayer())
      return;
    this._aliveTime = 0.0f;
    this.ApplyProperties();
  }

  public void InstantiateRagdoll(int id)
  {
    if (id < 0)
      return;
    Class @class = this.klasy[this.curClass];
    GameObject gameObject = (GameObject) Object.Instantiate<GameObject>((M0) @class.model_ragdoll);
    gameObject.get_transform().set_position(Vector3.op_Addition(((Component) this).get_transform().get_position(), @class.ragdoll_offset.position));
    Transform transform = gameObject.get_transform();
    Quaternion rotation = ((Component) this).get_transform().get_rotation();
    Quaternion quaternion = Quaternion.Euler(Vector3.op_Addition(((Quaternion) ref rotation).get_eulerAngles(), @class.ragdoll_offset.rotation));
    transform.set_rotation(quaternion);
    gameObject.get_transform().set_localScale(@class.ragdoll_offset.scale);
  }

  public void SetRandomRoles()
  {
    MTFRespawn component1 = (MTFRespawn) ((Component) this).GetComponent<MTFRespawn>();
    if (this.get_isLocalPlayer() && this.get_isServer())
    {
      GameObject[] array = this.GetShuffledPlayerList().ToArray();
      RoundSummary component2 = (RoundSummary) ((Component) this).GetComponent<RoundSummary>();
      bool flag = (double) Random.Range(0, 100) < (double) this.ciPercentage;
      RoundSummary.SumInfo_ClassList info = new RoundSummary.SumInfo_ClassList();
      for (int index = 0; index < array.Length; ++index)
      {
        int classid = this.forceClass != -1 ? this.forceClass : this.Find_Random_ID_Using_Defined_Team(this.classTeamQueue[index]);
        switch (this.klasy[classid].team)
        {
          case Team.SCP:
            ++info.scps_except_zombies;
            break;
          case Team.MTF:
            ++info.mtf_and_guards;
            break;
          case Team.CHI:
            ++info.chaos_insurgents;
            break;
          case Team.RSC:
            ++info.scientists;
            break;
          case Team.CDP:
            ++info.class_ds;
            break;
        }
        if (TutorialManager.status)
          this.SetPlayersClass(14, ((Component) this).get_gameObject());
        else
          this.SetPlayersClass(classid, array[index]);
      }
      info.time = (int) Time.get_realtimeSinceStartup();
      info.warhead_kills = -1;
      ((RoundSummary) Object.FindObjectOfType<RoundSummary>()).SetStartClassList(info);
      if (ConfigFile.ServerConfig.GetBool("smart_class_picker", true))
        this.RunSmartClassPicker();
    }
    if (!NetworkServer.get_active())
      return;
    Timing.RunCoroutine(this.MakeSureToSetHP(), (Segment) 0);
  }

  private List<GameObject> GetShuffledPlayerList()
  {
    List<GameObject> gameObjectList1 = new List<GameObject>((IEnumerable<GameObject>) PlayerManager.singleton.players);
    List<GameObject> gameObjectList2 = new List<GameObject>();
    while (gameObjectList1.Count > 0)
    {
      int index = Random.Range(0, gameObjectList1.Count);
      gameObjectList2.Add(gameObjectList1[index]);
      gameObjectList1.RemoveAt(index);
    }
    return gameObjectList2;
  }

  [Command]
  private void CmdRequestDeathScreen()
  {
    this.CallTargetDeathScreen(this.get_connectionToClient(), ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).lastHitInfo);
  }

  [TargetRpc]
  private void TargetDeathScreen(NetworkConnection conn, PlayerStats.HitInfo hitinfo)
  {
    ((YouWereKilled) Object.FindObjectOfType<YouWereKilled>()).Play(hitinfo);
  }

  private void RunSmartClassPicker()
  {
    string str = "Before Starting";
    try
    {
      str = "Setting Initial Value";
      if (ConfigFile.smBalancedPicker == null)
        ConfigFile.smBalancedPicker = new Dictionary<string, int[]>();
      str = "Valid Players List Error";
      List<GameObject> shuffledPlayerList = this.GetShuffledPlayerList();
      str = "Copying Balanced Picker List";
      Dictionary<string, int[]> dictionary = new Dictionary<string, int[]>((IDictionary<string, int[]>) ConfigFile.smBalancedPicker);
      str = "Clearing Balanced Picker List";
      ConfigFile.smBalancedPicker.Clear();
      str = "Re-building Balanced Picker List";
      using (List<GameObject>.Enumerator enumerator = shuffledPlayerList.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          GameObject current = enumerator.Current;
          if (!Object.op_Equality((Object) current, (Object) null))
          {
            NetworkConnection component1 = (NetworkConnection) current.GetComponent<NetworkConnection>();
            CharacterClassManager component2 = (CharacterClassManager) current.GetComponent<CharacterClassManager>();
            str = "Getting Player ID";
            if (component1 == null && Object.op_Equality((Object) component2, (Object) null))
            {
              shuffledPlayerList.Remove(current);
              break;
            }
            string key = (component1 == null ? string.Empty : (string) component1.address) + (!Object.op_Inequality((Object) component2, (Object) null) ? string.Empty : component2.SteamId);
            str = "Setting up Player \"" + key + "\"";
            if (!dictionary.ContainsKey(key))
            {
              str = "Adding Player \"" + key + "\" to smBalancedPicker";
              int[] numArray = new int[this.klasy.Length];
              for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = ConfigFile.ServerConfig.GetInt("smart_cp_starting_weight", 6);
              ConfigFile.smBalancedPicker.Add(key, numArray);
            }
            else
            {
              str = "Updating Player \"" + key + "\" in smBalancedPicker";
              int[] numArray;
              if (dictionary.TryGetValue(key, out numArray))
                ConfigFile.smBalancedPicker.Add(key, numArray);
            }
          }
        }
      }
      str = "Clearing Copied Balanced Picker List";
      dictionary.Clear();
      List<int> availableClasses = new List<int>();
      str = "Getting Available Roles";
      if (shuffledPlayerList.Contains((GameObject) null))
        shuffledPlayerList.Remove((GameObject) null);
      using (List<GameObject>.Enumerator enumerator = shuffledPlayerList.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          GameObject current = enumerator.Current;
          if (!Object.op_Equality((Object) current, (Object) null))
          {
            CharacterClassManager component = (CharacterClassManager) current.GetComponent<CharacterClassManager>();
            if (Object.op_Inequality((Object) component, (Object) null))
              availableClasses.Add(component.curClass);
            else
              shuffledPlayerList.Remove(current);
          }
        }
      }
      List<GameObject> gameObjectList = new List<GameObject>();
      str = "Setting Roles";
      using (List<GameObject>.Enumerator enumerator = shuffledPlayerList.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          GameObject current = enumerator.Current;
          if (!Object.op_Equality((Object) current, (Object) null))
          {
            NetworkConnection component1 = (NetworkConnection) current.GetComponent<NetworkConnection>();
            CharacterClassManager component2 = (CharacterClassManager) current.GetComponent<CharacterClassManager>();
            if (component1 == null && Object.op_Equality((Object) component2, (Object) null))
            {
              shuffledPlayerList.Remove(current);
              break;
            }
            string playerUuid = (component1 == null ? string.Empty : (string) component1.address) + (!Object.op_Inequality((Object) component2, (Object) null) ? string.Empty : component2.SteamId);
            str = "Setting Player \"" + playerUuid + "\"'s Class";
            int mostLikelyClass = this.GetMostLikelyClass(playerUuid, availableClasses);
            if (mostLikelyClass != -1)
            {
              this.SetPlayersClass(mostLikelyClass, current);
              availableClasses.Remove(mostLikelyClass);
            }
            else
              gameObjectList.Add(current);
          }
        }
      }
      str = "Reversing Additional Classes List";
      availableClasses.Reverse();
      str = "Setting Unknown Players Classes";
      using (List<GameObject>.Enumerator enumerator = gameObjectList.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          GameObject current = enumerator.Current;
          if (!Object.op_Equality((Object) current, (Object) null))
          {
            if (availableClasses.Count > 0)
            {
              int classid = availableClasses[0];
              this.SetPlayersClass(classid, current);
              availableClasses.Remove(classid);
            }
            else
              this.SetPlayersClass(2, current);
          }
        }
      }
      str = "Clearing Unknown Players List";
      gameObjectList.Clear();
      str = "Clearing Available Classes List";
      availableClasses.Clear();
    }
    catch
    {
      GameConsole.Console.singleton.AddLog("Smart Class Picker Failed: " + str, new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
    }
  }

  private int GetMostLikelyClass(string playerUuid, List<int> availableClasses)
  {
    int[] classChances = (int[]) null;
    int classChoice = -1;
    if (availableClasses.Count <= 0 || !ConfigFile.smBalancedPicker.TryGetValue(playerUuid, out classChances) || (classChances == null || classChances.Length != this.klasy.Length) || !this.ContainsPossibleClass(classChances, availableClasses))
      return classChoice;
    int num1 = 0;
    int[] numArray = (int[]) classChances.Clone();
    for (int index = 0; index < numArray.Length; ++index)
    {
      num1 += numArray[index];
      numArray[index] = num1;
    }
    while (!availableClasses.Contains(classChoice))
    {
      int num2 = Random.Range(0, num1);
      for (int index = 0; index < numArray.Length; ++index)
      {
        if (num2 < numArray[index])
        {
          classChoice = index;
          break;
        }
      }
    }
    if (classChoice < 0 || classChoice >= this.klasy.Length)
      return -1;
    this.UpdateClassChances(classChoice, classChances);
    return classChoice;
  }

  private bool ContainsPossibleClass(int[] classChances, List<int> availableClasses)
  {
    foreach (int availableClass in availableClasses)
    {
      if (availableClass >= 0 && availableClass < classChances.Length && classChances[availableClass] > 0)
        return true;
    }
    return false;
  }

  private void UpdateClassChances(int classChoice, int[] classChances)
  {
    int num1 = ConfigFile.ServerConfig.GetInt("smart_cp_weight_min", 1);
    int num2 = num1 >= 0 ? num1 : 1;
    int num3 = ConfigFile.ServerConfig.GetInt("smart_cp_weight_max", 11);
    int num4 = num3 >= num2 ? num3 : num2 + 10;
    for (int index = 0; index < classChances.Length; ++index)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (ConfigFile.ServerConfig.GetInt("smart_cp_team_" + (object) this.klasy[index].team + "_weight_decrease", -99) != -99 && this.klasy[index].team == this.klasy[classChoice].team)
      {
        classChances[index] -= ConfigFile.ServerConfig.GetInt("smart_cp_team_" + (object) this.klasy[index].team + "_weight_decrease", 0);
        flag2 = true;
      }
      else if (ConfigFile.ServerConfig.GetInt("smart_cp_team_" + (object) this.klasy[index].team + "_weight_increase", -99) != -99 && this.klasy[index].team != this.klasy[classChoice].team)
      {
        classChances[index] += ConfigFile.ServerConfig.GetInt("smart_cp_team_" + (object) this.klasy[index].team + "_weight_increase", 0);
        flag1 = true;
      }
      if (ConfigFile.ServerConfig.GetInt("smart_cp_class_" + (object) index + "_weight_decrease", -99) != -99 && index == classChoice && !flag1)
        classChances[index] -= ConfigFile.ServerConfig.GetInt("smart_cp_class_" + (object) index + "_weight_decrease", 3);
      else if (ConfigFile.ServerConfig.GetInt("smart_cp_class_" + (object) index + "_weight_increase", -99) != -99 && index != classChoice && !flag2)
        classChances[index] += ConfigFile.ServerConfig.GetInt("smart_cp_class_" + (object) index + "_weight_increase", 1);
      else if (!flag1 && !flag2)
      {
        if (this.klasy[classChoice].team == Team.MTF && this.klasy[classChoice].team == this.klasy[index].team)
        {
          classChances[index] -= 2;
          if (index == classChoice)
            classChances[index] -= 2;
        }
        else if (this.klasy[classChoice].team == Team.CDP && this.klasy[classChoice].team == this.klasy[index].team)
          classChances[index] -= 3;
        else if (this.klasy[classChoice].team == Team.SCP && this.klasy[classChoice].team == this.klasy[index].team)
        {
          classChances[index] -= 2;
          if (index == classChoice)
            --classChances[index];
        }
        else if (index == classChoice)
          classChances[index] -= 2;
        else
          ++classChances[index];
      }
      classChances[index] = Mathf.Clamp(classChances[index], num2, num4);
    }
  }

  private void SetRoundStart(bool b)
  {
    this.NetworkroundStarted = b;
  }

  [ServerCallback]
  private void CmdStartRound()
  {
    if (!NetworkServer.get_active())
      return;
    if (!TutorialManager.status)
    {
      try
      {
        ((Door) GameObject.Find("MeshDoor173").GetComponentInChildren<Door>()).ForceCooldown(25f);
        ((ChopperAutostart) Object.FindObjectOfType<ChopperAutostart>()).SetState(false);
      }
      catch
      {
      }
    }
    this.SetRoundStart(true);
  }

  [ServerCallback]
  public void SetPlayersClass(int classid, GameObject ply)
  {
    if (!NetworkServer.get_active() || !((CharacterClassManager) ply.GetComponent<CharacterClassManager>()).IsVerified)
      return;
    ((CharacterClassManager) ply.GetComponent<CharacterClassManager>()).SetClassID(classid);
    Inventory component = (Inventory) ply.GetComponent<Inventory>();
    ((AmmoBox) ply.GetComponent<AmmoBox>()).SetAmmoAmount();
    ((SyncList<Inventory.SyncItemInfo>) component.items).Clear();
    foreach (int startItem in this.klasy[Mathf.Clamp(classid, 0, this.klasy.Length - 1)].startItems)
      component.AddNewItem(startItem, -4.656647E+11f);
    ((PlayerStats) ply.GetComponent<PlayerStats>()).SetHPAmount(this.klasy[classid].maxHP);
  }

  [DebuggerHidden]
  private IEnumerator<float> MakeSureToSetHP()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new CharacterClassManager.\u003CMakeSureToSetHP\u003Ec__Iterator2()
    {
      \u0024this = this
    };
  }

  private int Find_Random_ID_Using_Defined_Team(Team team)
  {
    List<int> intList = new List<int>();
    for (int index = 0; index < this.klasy.Length; ++index)
    {
      if (this.klasy[index].team == team && !this.klasy[index].banClass)
        intList.Add(index);
    }
    if (intList.Count == 0)
      return 1;
    int index1 = Random.Range(0, intList.Count);
    if (this.klasy[intList[index1]].team == Team.SCP)
      this.klasy[intList[index1]].banClass = true;
    return intList[index1];
  }

  public bool SpawnProtection()
  {
    return (double) this._aliveTime < 2.0;
  }

  private void UNetVersion()
  {
  }

  public int NetworkcurClass
  {
    get
    {
      return this.curClass;
    }
    [param: In] set
    {
      int num1 = value;
      ref int local = ref this.curClass;
      int num2 = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetClassID(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<int>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public Vector3 NetworkdeathPosition
  {
    get
    {
      return this.deathPosition;
    }
    [param: In] set
    {
      Vector3 vector3 = value;
      ref Vector3 local = ref this.deathPosition;
      int num = 2;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SyncDeathPos(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<Vector3>((M0) vector3, (M0&) ref local, (uint) num);
    }
  }

  public int NetworkntfUnit
  {
    get
    {
      return this.ntfUnit;
    }
    [param: In] set
    {
      int num1 = value;
      ref int local = ref this.ntfUnit;
      int num2 = 4;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetUnit(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<int>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public bool NetworkroundStarted
  {
    get
    {
      return this.roundStarted;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.roundStarted;
      int num2 = 8;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetRoundStart(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<bool>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public bool NetworkIsVerified
  {
    get
    {
      return this.IsVerified;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.IsVerified;
      int num2 = 16;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetVerification(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<bool>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public string NetworkSteamId
  {
    get
    {
      return this.SteamId;
    }
    [param: In] set
    {
      string str = value;
      ref string local = ref this.SteamId;
      int num = 32;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetSteamId(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<string>((M0) str, (M0&) ref local, (uint) num);
    }
  }

  protected static void InvokeCmdCmdSendToken(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSendToken called on client.");
    else
      ((CharacterClassManager) obj).CmdSendToken(reader.ReadString());
  }

  protected static void InvokeCmdCmdRequestContactEmail(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdRequestContactEmail called on client.");
    else
      ((CharacterClassManager) obj).CmdRequestContactEmail();
  }

  protected static void InvokeCmdCmdRequestServerConfig(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdRequestServerConfig called on client.");
    else
      ((CharacterClassManager) obj).CmdRequestServerConfig();
  }

  protected static void InvokeCmdCmdRequestServerGroups(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdRequestServerGroups called on client.");
    else
      ((CharacterClassManager) obj).CmdRequestServerGroups();
  }

  protected static void InvokeCmdCmdRequestHideTag(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdRequestHideTag called on client.");
    else
      ((CharacterClassManager) obj).CmdRequestHideTag();
  }

  protected static void InvokeCmdCmdRequestShowTag(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdRequestShowTag called on client.");
    else
      ((CharacterClassManager) obj).CmdRequestShowTag(reader.ReadBoolean());
  }

  protected static void InvokeCmdCmdSuicide(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSuicide called on client.");
    else
      ((CharacterClassManager) obj).CmdSuicide(GeneratedNetworkCode._ReadHitInfo_PlayerStats(reader));
  }

  protected static void InvokeCmdCmdConfirmDisconnect(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdConfirmDisconnect called on client.");
    else
      ((CharacterClassManager) obj).CmdConfirmDisconnect();
  }

  protected static void InvokeCmdCmdRegisterEscape(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdRegisterEscape called on client.");
    else
      ((CharacterClassManager) obj).CmdRegisterEscape();
  }

  protected static void InvokeCmdCmdRequestDeathScreen(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdRequestDeathScreen called on client.");
    else
      ((CharacterClassManager) obj).CmdRequestDeathScreen();
  }

  public void CallCmdSendToken(string token)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSendToken called on server.");
    else if (this.get_isServer())
    {
      this.CmdSendToken(token);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) CharacterClassManager.kCmdCmdSendToken);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(token);
      this.SendCommandInternal(networkWriter, 2, "CmdSendToken");
    }
  }

  public void CallCmdRequestContactEmail()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdRequestContactEmail called on server.");
    else if (this.get_isServer())
    {
      this.CmdRequestContactEmail();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) CharacterClassManager.kCmdCmdRequestContactEmail);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 2, "CmdRequestContactEmail");
    }
  }

  public void CallCmdRequestServerConfig()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdRequestServerConfig called on server.");
    else if (this.get_isServer())
    {
      this.CmdRequestServerConfig();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) CharacterClassManager.kCmdCmdRequestServerConfig);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 2, "CmdRequestServerConfig");
    }
  }

  public void CallCmdRequestServerGroups()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdRequestServerGroups called on server.");
    else if (this.get_isServer())
    {
      this.CmdRequestServerGroups();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) CharacterClassManager.kCmdCmdRequestServerGroups);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 2, "CmdRequestServerGroups");
    }
  }

  public void CallCmdRequestHideTag()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdRequestHideTag called on server.");
    else if (this.get_isServer())
    {
      this.CmdRequestHideTag();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) CharacterClassManager.kCmdCmdRequestHideTag);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 2, "CmdRequestHideTag");
    }
  }

  public void CallCmdRequestShowTag(bool global)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdRequestShowTag called on server.");
    else if (this.get_isServer())
    {
      this.CmdRequestShowTag(global);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) CharacterClassManager.kCmdCmdRequestShowTag);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(global);
      this.SendCommandInternal(networkWriter, 2, "CmdRequestShowTag");
    }
  }

  public void CallCmdSuicide(PlayerStats.HitInfo hitInfo)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSuicide called on server.");
    else if (this.get_isServer())
    {
      this.CmdSuicide(hitInfo);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) CharacterClassManager.kCmdCmdSuicide);
      writer.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      GeneratedNetworkCode._WriteHitInfo_PlayerStats(writer, hitInfo);
      this.SendCommandInternal(writer, 0, "CmdSuicide");
    }
  }

  public void CallCmdConfirmDisconnect()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdConfirmDisconnect called on server.");
    else if (this.get_isServer())
    {
      this.CmdConfirmDisconnect();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) CharacterClassManager.kCmdCmdConfirmDisconnect);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 2, "CmdConfirmDisconnect");
    }
  }

  public void CallCmdRegisterEscape()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdRegisterEscape called on server.");
    else if (this.get_isServer())
    {
      this.CmdRegisterEscape();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) CharacterClassManager.kCmdCmdRegisterEscape);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 2, "CmdRegisterEscape");
    }
  }

  public void CallCmdRequestDeathScreen()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdRequestDeathScreen called on server.");
    else if (this.get_isServer())
    {
      this.CmdRequestDeathScreen();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) CharacterClassManager.kCmdCmdRequestDeathScreen);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 0, "CmdRequestDeathScreen");
    }
  }

  protected static void InvokeRpcRpcPlaceBlood(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcPlaceBlood called on server.");
    else
      ((CharacterClassManager) obj).RpcPlaceBlood((Vector3) reader.ReadVector3(), (int) reader.ReadPackedUInt32(), reader.ReadSingle());
  }

  protected static void InvokeRpcTargetConsolePrint(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetConsolePrint called on server.");
    else
      ((CharacterClassManager) obj).TargetConsolePrint(ClientScene.get_readyConnection(), reader.ReadString(), reader.ReadString());
  }

  protected static void InvokeRpcTargetSetDisconnectError(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetSetDisconnectError called on server.");
    else
      ((CharacterClassManager) obj).TargetSetDisconnectError(ClientScene.get_readyConnection(), reader.ReadString());
  }

  protected static void InvokeRpcTargetDeathScreen(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetDeathScreen called on server.");
    else
      ((CharacterClassManager) obj).TargetDeathScreen(ClientScene.get_readyConnection(), GeneratedNetworkCode._ReadHitInfo_PlayerStats(reader));
  }

  public void CallRpcPlaceBlood(Vector3 pos, int type, float f)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcPlaceBlood called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) CharacterClassManager.kRpcRpcPlaceBlood);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((Vector3) pos);
      networkWriter.WritePackedUInt32((uint) type);
      networkWriter.Write(f);
      this.SendRPCInternal(networkWriter, 0, "RpcPlaceBlood");
    }
  }

  public void CallTargetConsolePrint(NetworkConnection connection, string text, string color)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetConsolePrint called on client.");
    else if (connection is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetConsolePrint called on connection to server");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) CharacterClassManager.kTargetRpcTargetConsolePrint);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(text);
      networkWriter.Write(color);
      this.SendTargetRPCInternal(connection, networkWriter, 2, "TargetConsolePrint");
    }
  }

  public void CallTargetSetDisconnectError(NetworkConnection conn, string message)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetSetDisconnectError called on client.");
    else if (conn is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetSetDisconnectError called on connection to server");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) CharacterClassManager.kTargetRpcTargetSetDisconnectError);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(message);
      this.SendTargetRPCInternal(conn, networkWriter, 2, "TargetSetDisconnectError");
    }
  }

  public void CallTargetDeathScreen(NetworkConnection conn, PlayerStats.HitInfo hitinfo)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetDeathScreen called on client.");
    else if (conn is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetDeathScreen called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) CharacterClassManager.kTargetRpcTargetDeathScreen);
      writer.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      GeneratedNetworkCode._WriteHitInfo_PlayerStats(writer, hitinfo);
      this.SendTargetRPCInternal(conn, writer, 0, "TargetDeathScreen");
    }
  }

  static CharacterClassManager()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (CharacterClassManager), CharacterClassManager.kCmdCmdSendToken, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSendToken)));
    CharacterClassManager.kCmdCmdRequestContactEmail = 2054299309;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (CharacterClassManager), CharacterClassManager.kCmdCmdRequestContactEmail, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdRequestContactEmail)));
    CharacterClassManager.kCmdCmdRequestServerConfig = -2046741578;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (CharacterClassManager), CharacterClassManager.kCmdCmdRequestServerConfig, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdRequestServerConfig)));
    CharacterClassManager.kCmdCmdRequestServerGroups = -1929409976;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (CharacterClassManager), CharacterClassManager.kCmdCmdRequestServerGroups, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdRequestServerGroups)));
    CharacterClassManager.kCmdCmdRequestHideTag = -1886885625;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (CharacterClassManager), CharacterClassManager.kCmdCmdRequestHideTag, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdRequestHideTag)));
    CharacterClassManager.kCmdCmdRequestShowTag = -732213908;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (CharacterClassManager), CharacterClassManager.kCmdCmdRequestShowTag, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdRequestShowTag)));
    CharacterClassManager.kCmdCmdSuicide = -1051695024;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (CharacterClassManager), CharacterClassManager.kCmdCmdSuicide, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSuicide)));
    CharacterClassManager.kCmdCmdConfirmDisconnect = -1987348706;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (CharacterClassManager), CharacterClassManager.kCmdCmdConfirmDisconnect, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdConfirmDisconnect)));
    CharacterClassManager.kCmdCmdRegisterEscape = -1826587486;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (CharacterClassManager), CharacterClassManager.kCmdCmdRegisterEscape, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdRegisterEscape)));
    CharacterClassManager.kCmdCmdRequestDeathScreen = -1840245105;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (CharacterClassManager), CharacterClassManager.kCmdCmdRequestDeathScreen, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdRequestDeathScreen)));
    CharacterClassManager.kRpcRpcPlaceBlood = 1372291111;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (CharacterClassManager), CharacterClassManager.kRpcRpcPlaceBlood, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcPlaceBlood)));
    CharacterClassManager.kTargetRpcTargetConsolePrint = -558403607;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (CharacterClassManager), CharacterClassManager.kTargetRpcTargetConsolePrint, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetConsolePrint)));
    CharacterClassManager.kTargetRpcTargetSetDisconnectError = -2047672291;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (CharacterClassManager), CharacterClassManager.kTargetRpcTargetSetDisconnectError, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetSetDisconnectError)));
    CharacterClassManager.kTargetRpcTargetDeathScreen = -520196787;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (CharacterClassManager), CharacterClassManager.kTargetRpcTargetDeathScreen, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetDeathScreen)));
    NetworkCRC.RegisterBehaviour(nameof (CharacterClassManager), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.WritePackedUInt32((uint) this.curClass);
      writer.Write((Vector3) this.deathPosition);
      writer.WritePackedUInt32((uint) this.ntfUnit);
      writer.Write(this.roundStarted);
      writer.Write(this.IsVerified);
      writer.Write(this.SteamId);
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
      writer.WritePackedUInt32((uint) this.curClass);
    }
    if (((int) this.get_syncVarDirtyBits() & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write((Vector3) this.deathPosition);
    }
    if (((int) this.get_syncVarDirtyBits() & 4) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.ntfUnit);
    }
    if (((int) this.get_syncVarDirtyBits() & 8) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.roundStarted);
    }
    if (((int) this.get_syncVarDirtyBits() & 16) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.IsVerified);
    }
    if (((int) this.get_syncVarDirtyBits() & 32) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.SteamId);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.curClass = (int) reader.ReadPackedUInt32();
      this.deathPosition = (Vector3) reader.ReadVector3();
      this.ntfUnit = (int) reader.ReadPackedUInt32();
      this.roundStarted = reader.ReadBoolean();
      this.IsVerified = reader.ReadBoolean();
      this.SteamId = reader.ReadString();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.SetClassID((int) reader.ReadPackedUInt32());
      if ((num & 2) != 0)
        this.SyncDeathPos((Vector3) reader.ReadVector3());
      if ((num & 4) != 0)
        this.SetUnit((int) reader.ReadPackedUInt32());
      if ((num & 8) != 0)
        this.SetRoundStart(reader.ReadBoolean());
      if ((num & 16) != 0)
        this.SetVerification(reader.ReadBoolean());
      if ((num & 32) == 0)
        return;
      this.SetSteamId(reader.ReadString());
    }
  }
}
