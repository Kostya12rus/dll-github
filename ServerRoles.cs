// Decompiled with JetBrains decompiler
// Type: ServerRoles
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Cryptography;
using MEC;
using Org.BouncyCastle.Crypto;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ServerRoles : NetworkBehaviour
{
  private static int kCmdCmdRequestBadge = 1417446350;
  public ServerRoles.NamedColor CurrentColor;
  public ServerRoles.NamedColor[] NamedColors;
  public Dictionary<string, string> FirstVerResult;
  internal AsymmetricKeyParameter PublicKey;
  public bool AuthroizeBadge;
  public bool BypassMode;
  internal bool OverwatchPermitted;
  internal bool OverwatchEnabled;
  internal bool AmIInOverwatch;
  private bool _requested;
  private bool _badgeRequested;
  private bool _authRequested;
  internal string PrevBadge;
  private string _globalBadgeUnconfirmed;
  private string _prevColor;
  private string _prevText;
  private string _prevBadge;
  private string _badgeUserChallenge;
  private string _authChallenge;
  private string _badgeChallenge;
  private string _bgc;
  private string _bgt;
  [SyncVar(hook = "SetColor")]
  public string MyColor;
  [SyncVar(hook = "SetText")]
  public string MyText;
  [SyncVar(hook = "SetBadgeUpdate")]
  public string GlobalBadge;
  [SyncVar]
  public bool GlobalSet;
  public string FixedBadge;
  [SyncVar]
  public bool RemoteAdmin;
  public bool Staff;
  public bool BypassStaff;
  public bool RaEverywhere;
  public ulong Permissions;
  public string HiddenBadge;
  [SyncVar]
  public ServerRoles.AccessMode RemoteAdminMode;
  private static int kTargetRpcTargetSetHiddenRole;
  private static int kRpcRpcResetFixed;
  private static int kTargetRpcTargetSignServerChallenge;
  private static int kCmdCmdServerSignatureComplete;
  private static int kTargetRpcTargetOpenRemoteAdmin;
  private static int kCmdCmdSetOverwatchStatus;
  private static int kCmdCmdToggleOverwatch;
  private static int kTargetRpcTargetSetOverwatch;

  [TargetRpc(channel = 2)]
  public void TargetSetHiddenRole(NetworkConnection connection, string role)
  {
    if (this.isServer)
      return;
    if (string.IsNullOrEmpty(role))
    {
      this.NetworkGlobalSet = false;
      this.SetColor("default");
      this.SetText(string.Empty);
      this.FixedBadge = string.Empty;
      this.SetText(string.Empty);
    }
    else
    {
      this.NetworkGlobalSet = true;
      this.SetColor("silver");
      this.FixedBadge = role.Replace("[", string.Empty).Replace("]", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty) + " (hidden)";
      this.SetText(this.FixedBadge);
    }
  }

  [ClientRpc(channel = 2)]
  public void RpcResetFixed()
  {
    this.FixedBadge = string.Empty;
  }

  [Command(channel = 2)]
  public void CmdRequestBadge(string token)
  {
    if (this._requested)
      return;
    this._requested = true;
    Timing.RunCoroutine(this._RequestRoleFromServer(token), Segment.FixedUpdate);
  }

  [ServerCallback]
  public void RefreshPermissions()
  {
    if (!NetworkServer.active)
      return;
    this.SetGroup(ServerStatic.PermissionsHandler.GetUserGroup(this.GetComponent<CharacterClassManager>().SteamId), false, false);
  }

  [ServerCallback]
  public void SetGroup(UserGroup group, bool ovr, bool byAdmin = false)
  {
    if (!NetworkServer.active || group == null)
      return;
    this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, byAdmin ? "Updating your group on server (set by server administrator)..." : "Updating your group on server (local permissions)...", "cyan");
    if (!this.OverwatchPermitted && ServerStatic.PermissionsHandler.IsPermitted(group.Permissions, PlayerPermissions.Overwatch))
      this.OverwatchPermitted = true;
    if (group.Permissions > 0UL && (long) this.Permissions != (long) ServerStatic.PermissionsHandler.FullPerm && ServerStatic.PermissionsHandler.IsRaPermitted(group.Permissions))
    {
      this.NetworkRemoteAdmin = true;
      this.Permissions = group.Permissions;
      this.NetworkRemoteAdminMode = !ovr ? ServerRoles.AccessMode.LocalAccess : ServerRoles.AccessMode.PasswordOverride;
      this.GetComponent<QueryProcessor>().PasswordTries = 0;
      this.CallTargetOpenRemoteAdmin(this.connectionToClient);
      this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, byAdmin ? "Your remote admin access has been granted (set by server administrator)." : "Your remote admin access has been granted (local permissions).", "cyan");
      foreach (GameObject player in PlayerManager.singleton.players)
      {
        ServerRoles component = player.GetComponent<ServerRoles>();
        if (!string.IsNullOrEmpty(component.HiddenBadge))
          component.CallTargetSetHiddenRole(this.connectionToClient, component.HiddenBadge);
      }
      this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Hidden badges have been displayed for you (if there are any).", "gray");
    }
    ServerLogs.AddLog(ServerLogs.Modules.Permissions, "User with nickname " + this.GetComponent<NicknameSync>().myNick + " and SteamID " + this.GetComponent<CharacterClassManager>().SteamId + " has been assigned to group " + group.BadgeText + " (local permissions).", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
    if (group.BadgeColor == "hidden")
      return;
    this.NetworkMyText = group.BadgeText;
    this.NetworkMyColor = group.BadgeColor;
    if (!byAdmin)
      this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Your role \"" + group.BadgeText + "\" with color " + group.BadgeColor + " has been granted to you (local permissions).", "cyan");
    else
      this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Your role \"" + group.BadgeText + "\" with color " + group.BadgeColor + " has been granted to you (set by server administrator).", "cyan");
  }

  [ServerCallback]
  public void RefreshHiddenTag()
  {
    if (!NetworkServer.active)
      return;
    foreach (GameObject gameObject in ((IEnumerable<GameObject>) PlayerManager.singleton.players).Where<GameObject>((Func<GameObject, bool>) (player =>
    {
      if (!player.GetComponent<ServerRoles>().RemoteAdmin)
        return player.GetComponent<ServerRoles>().Staff;
      return true;
    })))
      this.CallTargetSetHiddenRole(gameObject.GetComponent<ServerRoles>().connectionToClient, this.HiddenBadge);
  }

  [DebuggerHidden]
  private IEnumerator<float> _RequestRoleFromServer(string token)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ServerRoles.\u003C_RequestRoleFromServer\u003Ec__Iterator0() { token = token, \u0024this = this };
  }

  public string GetColoredRoleString(bool newLine = false)
  {
    if (string.IsNullOrEmpty(this.MyColor) || string.IsNullOrEmpty(this.MyText) || this.CurrentColor == null || (this.CurrentColor.Restricted || this.MyText.Contains("[") || (this.MyText.Contains("]") || this.MyText.Contains("<")) || this.MyText.Contains(">")) && !this.AuthroizeBadge)
      return string.Empty;
    ServerRoles.NamedColor namedColor = ((IEnumerable<ServerRoles.NamedColor>) this.NamedColors).FirstOrDefault<ServerRoles.NamedColor>((Func<ServerRoles.NamedColor, bool>) (row => row.Name == this.MyColor));
    if (namedColor == null)
      return string.Empty;
    return (!newLine ? string.Empty : "\n") + "<color=#" + namedColor.ColorHex + ">" + this.MyText + "</color>";
  }

  public Color GetColor()
  {
    if (string.IsNullOrEmpty(this.MyColor) || this.MyColor == "default" || this.CurrentColor == null || (this.CurrentColor.Restricted || this.MyText.Contains("[") || (this.MyText.Contains("]") || this.MyText.Contains("<")) || this.MyText.Contains(">")) && !this.AuthroizeBadge)
      return Color.white;
    ServerRoles.NamedColor namedColor = ((IEnumerable<ServerRoles.NamedColor>) this.NamedColors).FirstOrDefault<ServerRoles.NamedColor>((Func<ServerRoles.NamedColor, bool>) (row => row.Name == this.MyColor));
    if (namedColor == null)
      return Color.white;
    return namedColor.SpeakingColorIn.Evaluate(1f);
  }

  public Gradient[] GetGradient()
  {
    ServerRoles.NamedColor namedColor = ((IEnumerable<ServerRoles.NamedColor>) this.NamedColors).FirstOrDefault<ServerRoles.NamedColor>((Func<ServerRoles.NamedColor, bool>) (row => row.Name == this.MyColor));
    return new Gradient[2]{ namedColor.SpeakingColorIn, namedColor.SpeakingColorOut };
  }

  private void Update()
  {
    if (this.CurrentColor == null)
      return;
    if (!string.IsNullOrEmpty(this.FixedBadge) && this.MyText != this.FixedBadge)
    {
      this.SetText(this.FixedBadge);
      this.SetColor("silver");
    }
    else if (!string.IsNullOrEmpty(this.FixedBadge) && this.CurrentColor.Name != "silver")
    {
      this.SetColor("silver");
    }
    else
    {
      if (this.GlobalBadge != this._prevBadge)
      {
        this._prevBadge = this.GlobalBadge;
        if (string.IsNullOrEmpty(this.GlobalBadge))
        {
          this._bgc = string.Empty;
          this._bgt = string.Empty;
          this.AuthroizeBadge = false;
          this._prevColor += ".";
          this._prevText += ".";
          return;
        }
        GameConsole.Console.singleton.AddLog("Validating global badge of user " + this.GetComponent<NicknameSync>().myNick, (Color32) Color.gray, false);
        Dictionary<string, string> dictionary = CentralAuth.ValidateBadgeRequest(this.GlobalBadge, this.GetComponent<CharacterClassManager>().SteamId, this.GetComponent<NicknameSync>().myNick);
        if (dictionary == null)
        {
          GameConsole.Console.singleton.AddLog("Validation of global badge of user " + this.GetComponent<NicknameSync>().myNick + " failed - invalid digital signature.", (Color32) Color.red, false);
          this._bgc = string.Empty;
          this._bgt = string.Empty;
          this.AuthroizeBadge = false;
          this._prevColor += ".";
          this._prevText += ".";
          return;
        }
        GameConsole.Console.singleton.AddLog("Validation of global badge of user " + this.GetComponent<NicknameSync>().myNick + " complete - badge signed by central server " + dictionary["Issued by"] + ".", (Color32) Color.grey, false);
        this._bgc = dictionary["Badge color"];
        this._bgt = dictionary["Badge text"];
        this.NetworkMyColor = dictionary["Badge color"];
        this.NetworkMyText = dictionary["Badge text"];
        this.AuthroizeBadge = true;
      }
      if (this._prevColor == this.MyColor && this._prevText == this.MyText)
        return;
      if (this.CurrentColor.Restricted && (this.MyText != this._bgt || this.MyColor != this._bgc))
      {
        GameConsole.Console.singleton.AddLog("TAG FAIL 1 - " + this.MyText + " - " + this._bgt + " /-/ " + this.MyColor + " - " + this._bgc, (Color32) Color.gray, false);
        this.AuthroizeBadge = false;
        this.NetworkMyColor = string.Empty;
        this.NetworkMyText = string.Empty;
        this._prevColor = string.Empty;
        this._prevText = string.Empty;
        PlayerList.UpdatePlayerRole(this.gameObject);
      }
      else if (this.MyText != this._bgt && (this.MyText.Contains("[") || this.MyText.Contains("]")) || (this.MyText.Contains("<") || this.MyText.Contains(">")))
      {
        GameConsole.Console.singleton.AddLog("TAG FAIL 2 - " + this.MyText + " - " + this._bgt + " /-/ " + this.MyColor + " - " + this._bgc, (Color32) Color.gray, false);
        this.AuthroizeBadge = false;
        this.NetworkMyColor = string.Empty;
        this.NetworkMyText = string.Empty;
        this._prevColor = string.Empty;
        this._prevText = string.Empty;
        PlayerList.UpdatePlayerRole(this.gameObject);
      }
      else
      {
        this._prevColor = this.MyColor;
        this._prevText = this.MyText;
        this._prevBadge = this.GlobalBadge;
        PlayerList.UpdatePlayerRole(this.gameObject);
      }
    }
  }

  public void SetColor(string i)
  {
    this.NetworkMyColor = i;
    ServerRoles.NamedColor namedColor = ((IEnumerable<ServerRoles.NamedColor>) this.NamedColors).FirstOrDefault<ServerRoles.NamedColor>((Func<ServerRoles.NamedColor, bool>) (row => row.Name == this.MyColor));
    if (namedColor == null && i != "default")
      this.SetColor("default");
    else
      this.CurrentColor = namedColor;
  }

  public void SetText(string i)
  {
    this.NetworkMyText = i;
    ServerRoles.NamedColor namedColor = ((IEnumerable<ServerRoles.NamedColor>) this.NamedColors).FirstOrDefault<ServerRoles.NamedColor>((Func<ServerRoles.NamedColor, bool>) (row => row.Name == this.MyColor));
    if (namedColor == null)
      return;
    this.CurrentColor = namedColor;
  }

  public void SetBadgeUpdate(string i)
  {
    this.NetworkGlobalBadge = i;
  }

  [ServerCallback]
  public void StartServerChallenge(int selector)
  {
    if (!NetworkServer.active || selector == 0 && !string.IsNullOrEmpty(this._authChallenge) || (selector == 1 && !string.IsNullOrEmpty(this._badgeChallenge) || (selector > 1 || selector < 0)))
      return;
    RandomNumberGenerator randomNumberGenerator = (RandomNumberGenerator) new RNGCryptoServiceProvider();
    byte[] numArray = new byte[32];
    randomNumberGenerator.GetBytes(numArray);
    string base64String = Convert.ToBase64String(numArray);
    if (selector == 0)
    {
      this._authChallenge = "auth-" + base64String;
      this.CallTargetSignServerChallenge(this.connectionToClient, this._authChallenge);
    }
    else
    {
      this._badgeChallenge = "badge-server-" + base64String;
      this.CallTargetSignServerChallenge(this.connectionToClient, this._badgeChallenge);
    }
  }

  [TargetRpc(channel = 2)]
  public void TargetSignServerChallenge(NetworkConnection target, string challenge)
  {
    if (challenge.StartsWith("auth-"))
    {
      if (this._authRequested)
        return;
      this._authRequested = true;
    }
    else
    {
      if (!challenge.StartsWith("badge-server-") || this._badgeRequested)
        return;
      this._badgeRequested = true;
    }
    string response = ECDSA.Sign(challenge, GameConsole.Console.SessionKeys.Private);
    GameConsole.Console.singleton.AddLog("Signed " + challenge + " for server.", (Color32) Color.cyan, false);
    this.CallCmdServerSignatureComplete(challenge, response, ECDSA.KeyToString(GameConsole.Console.SessionKeys.Public));
  }

  [Command(channel = 2)]
  public void CmdServerSignatureComplete(string challenge, string response, string publickey)
  {
    if (this.FirstVerResult == null)
      this.FirstVerResult = CentralAuth.ValidateBadgeRequest(this._globalBadgeUnconfirmed, this.GetComponent<CharacterClassManager>().SteamId, this.GetComponent<NicknameSync>().myNick);
    if (this.FirstVerResult == null)
      return;
    if (this.FirstVerResult["Public key"] != ServerRoles.Base64Encode(Sha.HashToString(Sha.Sha256(publickey))))
    {
      GameConsole.Console.singleton.AddLog("Rejected signature of challenge " + challenge + " due to public key hash mismatch.", (Color32) Color.red, false);
      this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Challenge signature rejected due to public key mismatch.", "red");
    }
    else
    {
      if (this.PublicKey == null)
        this.PublicKey = ECDSA.PublicKeyFromString(publickey);
      if (!ECDSA.Verify(challenge, response, this.PublicKey))
      {
        GameConsole.Console.singleton.AddLog("Rejected signature of challenge " + challenge + " due to signature mismatch.", (Color32) Color.red, false);
        this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Challenge signature rejected due to signature mismatch.", "red");
      }
      else if (challenge.StartsWith("auth-") && challenge == this._authChallenge)
      {
        this.GetComponent<CharacterClassManager>().NetworkSteamId = this.FirstVerResult["Steam ID"];
        this.GetComponent<NicknameSync>().UpdateNickname(ServerRoles.Base64Decode(this.FirstVerResult["Nickname"]));
        this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Hi " + ServerRoles.Base64Decode(this.FirstVerResult["Nickname"]) + "! Your challenge signature has been accepted.", "green");
        this.RefreshPermissions();
        this._authChallenge = string.Empty;
      }
      else
      {
        if (!challenge.StartsWith("badge-server-") || !(challenge == this._badgeChallenge))
          return;
        Dictionary<string, string> dictionary = CentralAuth.ValidateBadgeRequest(this._globalBadgeUnconfirmed, this.GetComponent<CharacterClassManager>().SteamId, this.GetComponent<NicknameSync>().myNick);
        if (dictionary == null)
        {
          ServerConsole.AddLog("Rejected signature of challenge " + challenge + " due to signature mismatch.");
          this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Challenge signature rejected due to signature mismatch.", "red");
        }
        else
        {
          this.SetBadgeUpdate(this._globalBadgeUnconfirmed);
          this.PrevBadge = this._globalBadgeUnconfirmed;
          this._globalBadgeUnconfirmed = string.Empty;
          if (dictionary["Remote admin"] == "YES" || dictionary["Management"] == "YES" || dictionary["Global banning"] == "YES")
            this.Staff = true;
          if (dictionary["Management"] == "YES" || dictionary["Global banning"] == "YES")
            this.RaEverywhere = true;
          if (dictionary["Overwatch mode"] == "YES")
            this.OverwatchPermitted = true;
          if (dictionary["Remote admin"] == "YES" && ServerStatic.PermissionsHandler.StaffAccess)
          {
            this.NetworkRemoteAdmin = true;
            this.Permissions = ServerStatic.PermissionsHandler.FullPerm;
            this.NetworkRemoteAdminMode = ServerRoles.AccessMode.GlobalAccess;
            this.GetComponent<QueryProcessor>().PasswordTries = 0;
            this.CallTargetOpenRemoteAdmin(this.connectionToClient);
            this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Your remote admin access has been granted (global permissions - staff).", "cyan");
          }
          else if (dictionary["Management"] == "YES" && ServerStatic.PermissionsHandler.ManagersAccess)
          {
            this.NetworkRemoteAdmin = true;
            this.Permissions = ServerStatic.PermissionsHandler.FullPerm;
            this.NetworkRemoteAdminMode = ServerRoles.AccessMode.GlobalAccess;
            this.GetComponent<QueryProcessor>().PasswordTries = 0;
            this.CallTargetOpenRemoteAdmin(this.connectionToClient);
            this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Your remote admin access has been granted (global permissions - management).", "cyan");
          }
          else if (dictionary["Global banning"] == "YES" && ServerStatic.PermissionsHandler.BanningTeamAccess)
          {
            this.NetworkRemoteAdmin = true;
            this.Permissions = ServerStatic.PermissionsHandler.FullPerm;
            this.NetworkRemoteAdminMode = ServerRoles.AccessMode.GlobalAccess;
            this.GetComponent<QueryProcessor>().PasswordTries = 0;
            this.CallTargetOpenRemoteAdmin(this.connectionToClient);
            this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Your remote admin access has been granted (global permissions - banning team).", "cyan");
          }
          this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Your global badge has been granted.", "cyan");
          this._badgeChallenge = string.Empty;
          if (!this.Staff)
            return;
          foreach (GameObject player in PlayerManager.singleton.players)
          {
            ServerRoles component = player.GetComponent<ServerRoles>();
            if (!string.IsNullOrEmpty(component.HiddenBadge))
              component.CallTargetSetHiddenRole(this.connectionToClient, component.HiddenBadge);
          }
          this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Hidden badges have been displayed for you (if there are any).", "gray");
        }
      }
    }
  }

  [TargetRpc]
  private void TargetOpenRemoteAdmin(NetworkConnection connection)
  {
    Object.FindObjectOfType<UIController>().ActivateRemoteAdmin();
  }

  [Command(channel = 2)]
  public void CmdSetOverwatchStatus(bool status)
  {
    if (!this.OverwatchPermitted && status)
      this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "You don't have permissions to enable overwatch mode!", "red");
    else
      this.SetOverwatchStatus(status);
  }

  [Command(channel = 2)]
  public void CmdToggleOverwatch()
  {
    if (!this.OverwatchPermitted && !this.OverwatchEnabled)
      this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "You don't have permissions to enable overwatch mode!", "red");
    else
      this.SetOverwatchStatus(!this.OverwatchEnabled);
  }

  public void SetOverwatchStatus(bool status)
  {
    this.OverwatchEnabled = status;
    CharacterClassManager component = this.GetComponent<CharacterClassManager>();
    if (status && component.curClass != 2 && component.curClass != -1)
      component.SetClassID(2);
    this.CallTargetSetOverwatch(this.connectionToClient, this.OverwatchEnabled);
  }

  public void RequestBadge(string token)
  {
    this.CallCmdRequestBadge(token);
  }

  public static string Base64Encode(string plainText)
  {
    return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
  }

  public static string Base64Decode(string base64EncodedData)
  {
    return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
  }

  [TargetRpc(channel = 2)]
  public void TargetSetOverwatch(NetworkConnection conn, bool s)
  {
    GameConsole.Console.singleton.AddLog("Overwatch status: " + (!s ? "DISABLED" : "ENABLED"), (Color32) Color.green, false);
    this.AmIInOverwatch = s;
  }

  private void UNetVersion()
  {
  }

  public string NetworkMyColor
  {
    get
    {
      return this.MyColor;
    }
    [param: In] set
    {
      string str = value;
      ref string local = ref this.MyColor;
      int num = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetColor(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<string>(str, ref local, (uint) num);
    }
  }

  public string NetworkMyText
  {
    get
    {
      return this.MyText;
    }
    [param: In] set
    {
      string str = value;
      ref string local = ref this.MyText;
      int num = 2;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetText(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<string>(str, ref local, (uint) num);
    }
  }

  public string NetworkGlobalBadge
  {
    get
    {
      return this.GlobalBadge;
    }
    [param: In] set
    {
      string str = value;
      ref string local = ref this.GlobalBadge;
      int num = 4;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetBadgeUpdate(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<string>(str, ref local, (uint) num);
    }
  }

  public bool NetworkGlobalSet
  {
    get
    {
      return this.GlobalSet;
    }
    [param: In] set
    {
      this.SetSyncVar<bool>(value, ref this.GlobalSet, 8U);
    }
  }

  public bool NetworkRemoteAdmin
  {
    get
    {
      return this.RemoteAdmin;
    }
    [param: In] set
    {
      this.SetSyncVar<bool>(value, ref this.RemoteAdmin, 16U);
    }
  }

  public ServerRoles.AccessMode NetworkRemoteAdminMode
  {
    get
    {
      return this.RemoteAdminMode;
    }
    [param: In] set
    {
      this.SetSyncVar<ServerRoles.AccessMode>(value, ref this.RemoteAdminMode, 32U);
    }
  }

  protected static void InvokeCmdCmdRequestBadge(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdRequestBadge called on client.");
    else
      ((ServerRoles) obj).CmdRequestBadge(reader.ReadString());
  }

  protected static void InvokeCmdCmdServerSignatureComplete(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdServerSignatureComplete called on client.");
    else
      ((ServerRoles) obj).CmdServerSignatureComplete(reader.ReadString(), reader.ReadString(), reader.ReadString());
  }

  protected static void InvokeCmdCmdSetOverwatchStatus(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdSetOverwatchStatus called on client.");
    else
      ((ServerRoles) obj).CmdSetOverwatchStatus(reader.ReadBoolean());
  }

  protected static void InvokeCmdCmdToggleOverwatch(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdToggleOverwatch called on client.");
    else
      ((ServerRoles) obj).CmdToggleOverwatch();
  }

  public void CallCmdRequestBadge(string token)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdRequestBadge called on server.");
    else if (this.isServer)
    {
      this.CmdRequestBadge(token);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) ServerRoles.kCmdCmdRequestBadge);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(token);
      this.SendCommandInternal(writer, 2, "CmdRequestBadge");
    }
  }

  public void CallCmdServerSignatureComplete(string challenge, string response, string publickey)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdServerSignatureComplete called on server.");
    else if (this.isServer)
    {
      this.CmdServerSignatureComplete(challenge, response, publickey);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) ServerRoles.kCmdCmdServerSignatureComplete);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(challenge);
      writer.Write(response);
      writer.Write(publickey);
      this.SendCommandInternal(writer, 2, "CmdServerSignatureComplete");
    }
  }

  public void CallCmdSetOverwatchStatus(bool status)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdSetOverwatchStatus called on server.");
    else if (this.isServer)
    {
      this.CmdSetOverwatchStatus(status);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) ServerRoles.kCmdCmdSetOverwatchStatus);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(status);
      this.SendCommandInternal(writer, 2, "CmdSetOverwatchStatus");
    }
  }

  public void CallCmdToggleOverwatch()
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdToggleOverwatch called on server.");
    else if (this.isServer)
    {
      this.CmdToggleOverwatch();
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) ServerRoles.kCmdCmdToggleOverwatch);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendCommandInternal(writer, 2, "CmdToggleOverwatch");
    }
  }

  protected static void InvokeRpcRpcResetFixed(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcResetFixed called on server.");
    else
      ((ServerRoles) obj).RpcResetFixed();
  }

  protected static void InvokeRpcTargetSetHiddenRole(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetSetHiddenRole called on server.");
    else
      ((ServerRoles) obj).TargetSetHiddenRole(ClientScene.readyConnection, reader.ReadString());
  }

  protected static void InvokeRpcTargetSignServerChallenge(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetSignServerChallenge called on server.");
    else
      ((ServerRoles) obj).TargetSignServerChallenge(ClientScene.readyConnection, reader.ReadString());
  }

  protected static void InvokeRpcTargetOpenRemoteAdmin(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetOpenRemoteAdmin called on server.");
    else
      ((ServerRoles) obj).TargetOpenRemoteAdmin(ClientScene.readyConnection);
  }

  protected static void InvokeRpcTargetSetOverwatch(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "TargetRPC TargetSetOverwatch called on server.");
    else
      ((ServerRoles) obj).TargetSetOverwatch(ClientScene.readyConnection, reader.ReadBoolean());
  }

  public void CallRpcResetFixed()
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcResetFixed called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) ServerRoles.kRpcRpcResetFixed);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendRPCInternal(writer, 2, "RpcResetFixed");
    }
  }

  public void CallTargetSetHiddenRole(NetworkConnection connection, string role)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetSetHiddenRole called on client.");
    else if (connection is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetSetHiddenRole called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) ServerRoles.kTargetRpcTargetSetHiddenRole);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(role);
      this.SendTargetRPCInternal(connection, writer, 2, "TargetSetHiddenRole");
    }
  }

  public void CallTargetSignServerChallenge(NetworkConnection target, string challenge)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetSignServerChallenge called on client.");
    else if (target is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetSignServerChallenge called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) ServerRoles.kTargetRpcTargetSignServerChallenge);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(challenge);
      this.SendTargetRPCInternal(target, writer, 2, "TargetSignServerChallenge");
    }
  }

  public void CallTargetOpenRemoteAdmin(NetworkConnection connection)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetOpenRemoteAdmin called on client.");
    else if (connection is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetOpenRemoteAdmin called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) ServerRoles.kTargetRpcTargetOpenRemoteAdmin);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      this.SendTargetRPCInternal(connection, writer, 0, "TargetOpenRemoteAdmin");
    }
  }

  public void CallTargetSetOverwatch(NetworkConnection conn, bool s)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "TargetRPC Function TargetSetOverwatch called on client.");
    else if (conn is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetSetOverwatch called on connection to server");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) ServerRoles.kTargetRpcTargetSetOverwatch);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(s);
      this.SendTargetRPCInternal(conn, writer, 2, "TargetSetOverwatch");
    }
  }

  static ServerRoles()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (ServerRoles), ServerRoles.kCmdCmdRequestBadge, new NetworkBehaviour.CmdDelegate(ServerRoles.InvokeCmdCmdRequestBadge));
    ServerRoles.kCmdCmdServerSignatureComplete = -834487468;
    NetworkBehaviour.RegisterCommandDelegate(typeof (ServerRoles), ServerRoles.kCmdCmdServerSignatureComplete, new NetworkBehaviour.CmdDelegate(ServerRoles.InvokeCmdCmdServerSignatureComplete));
    ServerRoles.kCmdCmdSetOverwatchStatus = 200610181;
    NetworkBehaviour.RegisterCommandDelegate(typeof (ServerRoles), ServerRoles.kCmdCmdSetOverwatchStatus, new NetworkBehaviour.CmdDelegate(ServerRoles.InvokeCmdCmdSetOverwatchStatus));
    ServerRoles.kCmdCmdToggleOverwatch = -571630643;
    NetworkBehaviour.RegisterCommandDelegate(typeof (ServerRoles), ServerRoles.kCmdCmdToggleOverwatch, new NetworkBehaviour.CmdDelegate(ServerRoles.InvokeCmdCmdToggleOverwatch));
    ServerRoles.kRpcRpcResetFixed = -1154333771;
    NetworkBehaviour.RegisterRpcDelegate(typeof (ServerRoles), ServerRoles.kRpcRpcResetFixed, new NetworkBehaviour.CmdDelegate(ServerRoles.InvokeRpcRpcResetFixed));
    ServerRoles.kTargetRpcTargetSetHiddenRole = -948979541;
    NetworkBehaviour.RegisterRpcDelegate(typeof (ServerRoles), ServerRoles.kTargetRpcTargetSetHiddenRole, new NetworkBehaviour.CmdDelegate(ServerRoles.InvokeRpcTargetSetHiddenRole));
    ServerRoles.kTargetRpcTargetSignServerChallenge = 1367769996;
    NetworkBehaviour.RegisterRpcDelegate(typeof (ServerRoles), ServerRoles.kTargetRpcTargetSignServerChallenge, new NetworkBehaviour.CmdDelegate(ServerRoles.InvokeRpcTargetSignServerChallenge));
    ServerRoles.kTargetRpcTargetOpenRemoteAdmin = 1449538856;
    NetworkBehaviour.RegisterRpcDelegate(typeof (ServerRoles), ServerRoles.kTargetRpcTargetOpenRemoteAdmin, new NetworkBehaviour.CmdDelegate(ServerRoles.InvokeRpcTargetOpenRemoteAdmin));
    ServerRoles.kTargetRpcTargetSetOverwatch = -1052391504;
    NetworkBehaviour.RegisterRpcDelegate(typeof (ServerRoles), ServerRoles.kTargetRpcTargetSetOverwatch, new NetworkBehaviour.CmdDelegate(ServerRoles.InvokeRpcTargetSetOverwatch));
    NetworkCRC.RegisterBehaviour(nameof (ServerRoles), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.MyColor);
      writer.Write(this.MyText);
      writer.Write(this.GlobalBadge);
      writer.Write(this.GlobalSet);
      writer.Write(this.RemoteAdmin);
      writer.Write((int) this.RemoteAdminMode);
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
      writer.Write(this.MyColor);
    }
    if (((int) this.syncVarDirtyBits & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.MyText);
    }
    if (((int) this.syncVarDirtyBits & 4) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.GlobalBadge);
    }
    if (((int) this.syncVarDirtyBits & 8) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.GlobalSet);
    }
    if (((int) this.syncVarDirtyBits & 16) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.RemoteAdmin);
    }
    if (((int) this.syncVarDirtyBits & 32) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write((int) this.RemoteAdminMode);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.MyColor = reader.ReadString();
      this.MyText = reader.ReadString();
      this.GlobalBadge = reader.ReadString();
      this.GlobalSet = reader.ReadBoolean();
      this.RemoteAdmin = reader.ReadBoolean();
      this.RemoteAdminMode = (ServerRoles.AccessMode) reader.ReadInt32();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.SetColor(reader.ReadString());
      if ((num & 2) != 0)
        this.SetText(reader.ReadString());
      if ((num & 4) != 0)
        this.SetBadgeUpdate(reader.ReadString());
      if ((num & 8) != 0)
        this.GlobalSet = reader.ReadBoolean();
      if ((num & 16) != 0)
        this.RemoteAdmin = reader.ReadBoolean();
      if ((num & 32) == 0)
        return;
      this.RemoteAdminMode = (ServerRoles.AccessMode) reader.ReadInt32();
    }
  }

  [Serializable]
  public class NamedColor
  {
    public string Name;
    public Gradient SpeakingColorIn;
    public Gradient SpeakingColorOut;
    public string ColorHex;
    public bool Restricted;
  }

  [Serializable]
  public enum AccessMode
  {
    LocalAccess = 1,
    GlobalAccess = 2,
    PasswordOverride = 3,
  }
}
