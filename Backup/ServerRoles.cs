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
using System.Collections;
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

  public ServerRoles()
  {
    base.\u002Ector();
  }

  [TargetRpc(channel = 2)]
  public void TargetSetHiddenRole(NetworkConnection connection, string role)
  {
    if (this.get_isServer())
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
    Timing.RunCoroutine(this._RequestRoleFromServer(token), (Segment) 1);
  }

  [ServerCallback]
  public void RefreshPermissions()
  {
    if (!NetworkServer.get_active())
      return;
    this.SetGroup(ServerStatic.PermissionsHandler.GetUserGroup(((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).SteamId), false, false);
  }

  [ServerCallback]
  public void SetGroup(UserGroup group, bool ovr, bool byAdmin = false)
  {
    if (!NetworkServer.get_active() || group == null)
      return;
    ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), byAdmin ? "Updating your group on server (set by server administrator)..." : "Updating your group on server (local permissions)...", "cyan");
    if (!this.OverwatchPermitted && ServerStatic.PermissionsHandler.IsPermitted(group.Permissions, PlayerPermissions.Overwatch))
      this.OverwatchPermitted = true;
    if (group.Permissions > 0UL && (long) this.Permissions != (long) ServerStatic.PermissionsHandler.FullPerm && ServerStatic.PermissionsHandler.IsRaPermitted(group.Permissions))
    {
      this.NetworkRemoteAdmin = true;
      this.Permissions = group.Permissions;
      this.NetworkRemoteAdminMode = !ovr ? ServerRoles.AccessMode.LocalAccess : ServerRoles.AccessMode.PasswordOverride;
      ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PasswordTries = 0;
      this.CallTargetOpenRemoteAdmin(this.get_connectionToClient());
      ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), byAdmin ? "Your remote admin access has been granted (set by server administrator)." : "Your remote admin access has been granted (local permissions).", "cyan");
      foreach (GameObject player in PlayerManager.singleton.players)
      {
        ServerRoles component = (ServerRoles) player.GetComponent<ServerRoles>();
        if (!string.IsNullOrEmpty(component.HiddenBadge))
          component.CallTargetSetHiddenRole(this.get_connectionToClient(), component.HiddenBadge);
      }
      ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "Hidden badges have been displayed for you (if there are any).", "gray");
    }
    ServerLogs.AddLog(ServerLogs.Modules.Permissions, "User with nickname " + ((NicknameSync) ((Component) this).GetComponent<NicknameSync>()).myNick + " and SteamID " + ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).SteamId + " has been assigned to group " + group.BadgeText + " (local permissions).", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
    if (group.BadgeColor == "hidden")
      return;
    this.NetworkMyText = group.BadgeText;
    this.NetworkMyColor = group.BadgeColor;
    if (!byAdmin)
      ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "Your role \"" + group.BadgeText + "\" with color " + group.BadgeColor + " has been granted to you (local permissions).", "cyan");
    else
      ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "Your role \"" + group.BadgeText + "\" with color " + group.BadgeColor + " has been granted to you (set by server administrator).", "cyan");
  }

  [ServerCallback]
  public void RefreshHiddenTag()
  {
    if (!NetworkServer.get_active())
      return;
    using (IEnumerator<GameObject> enumerator = ((IEnumerable<GameObject>) PlayerManager.singleton.players).Where<GameObject>((Func<GameObject, bool>) (player =>
    {
      if (!((ServerRoles) player.GetComponent<ServerRoles>()).RemoteAdmin)
        return ((ServerRoles) player.GetComponent<ServerRoles>()).Staff;
      return true;
    })).GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
        this.CallTargetSetHiddenRole(((NetworkBehaviour) enumerator.Current.GetComponent<ServerRoles>()).get_connectionToClient(), this.HiddenBadge);
    }
  }

  [DebuggerHidden]
  private IEnumerator<float> _RequestRoleFromServer(string token)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ServerRoles.\u003C_RequestRoleFromServer\u003Ec__Iterator0()
    {
      token = token,
      \u0024this = this
    };
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
      return Color.get_white();
    ServerRoles.NamedColor namedColor = ((IEnumerable<ServerRoles.NamedColor>) this.NamedColors).FirstOrDefault<ServerRoles.NamedColor>((Func<ServerRoles.NamedColor, bool>) (row => row.Name == this.MyColor));
    if (namedColor == null)
      return Color.get_white();
    return namedColor.SpeakingColorIn.Evaluate(1f);
  }

  public Gradient[] GetGradient()
  {
    ServerRoles.NamedColor namedColor = ((IEnumerable<ServerRoles.NamedColor>) this.NamedColors).FirstOrDefault<ServerRoles.NamedColor>((Func<ServerRoles.NamedColor, bool>) (row => row.Name == this.MyColor));
    return new Gradient[2]
    {
      namedColor.SpeakingColorIn,
      namedColor.SpeakingColorOut
    };
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
        GameConsole.Console.singleton.AddLog("Validating global badge of user " + ((NicknameSync) ((Component) this).GetComponent<NicknameSync>()).myNick, Color32.op_Implicit(Color.get_gray()), false);
        Dictionary<string, string> dictionary = CentralAuth.ValidateBadgeRequest(this.GlobalBadge, ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).SteamId, ((NicknameSync) ((Component) this).GetComponent<NicknameSync>()).myNick);
        if (dictionary == null)
        {
          GameConsole.Console.singleton.AddLog("Validation of global badge of user " + ((NicknameSync) ((Component) this).GetComponent<NicknameSync>()).myNick + " failed - invalid digital signature.", Color32.op_Implicit(Color.get_red()), false);
          this._bgc = string.Empty;
          this._bgt = string.Empty;
          this.AuthroizeBadge = false;
          this._prevColor += ".";
          this._prevText += ".";
          return;
        }
        GameConsole.Console.singleton.AddLog("Validation of global badge of user " + ((NicknameSync) ((Component) this).GetComponent<NicknameSync>()).myNick + " complete - badge signed by central server " + dictionary["Issued by"] + ".", Color32.op_Implicit(Color.get_grey()), false);
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
        GameConsole.Console.singleton.AddLog("TAG FAIL 1 - " + this.MyText + " - " + this._bgt + " /-/ " + this.MyColor + " - " + this._bgc, Color32.op_Implicit(Color.get_gray()), false);
        this.AuthroizeBadge = false;
        this.NetworkMyColor = string.Empty;
        this.NetworkMyText = string.Empty;
        this._prevColor = string.Empty;
        this._prevText = string.Empty;
        PlayerList.UpdatePlayerRole(((Component) this).get_gameObject());
      }
      else if (this.MyText != this._bgt && (this.MyText.Contains("[") || this.MyText.Contains("]")) || (this.MyText.Contains("<") || this.MyText.Contains(">")))
      {
        GameConsole.Console.singleton.AddLog("TAG FAIL 2 - " + this.MyText + " - " + this._bgt + " /-/ " + this.MyColor + " - " + this._bgc, Color32.op_Implicit(Color.get_gray()), false);
        this.AuthroizeBadge = false;
        this.NetworkMyColor = string.Empty;
        this.NetworkMyText = string.Empty;
        this._prevColor = string.Empty;
        this._prevText = string.Empty;
        PlayerList.UpdatePlayerRole(((Component) this).get_gameObject());
      }
      else
      {
        this._prevColor = this.MyColor;
        this._prevText = this.MyText;
        this._prevBadge = this.GlobalBadge;
        PlayerList.UpdatePlayerRole(((Component) this).get_gameObject());
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
    if (!NetworkServer.get_active() || selector == 0 && !string.IsNullOrEmpty(this._authChallenge) || (selector == 1 && !string.IsNullOrEmpty(this._badgeChallenge) || (selector > 1 || selector < 0)))
      return;
    RandomNumberGenerator randomNumberGenerator = (RandomNumberGenerator) new RNGCryptoServiceProvider();
    byte[] numArray = new byte[32];
    randomNumberGenerator.GetBytes(numArray);
    string base64String = Convert.ToBase64String(numArray);
    if (selector == 0)
    {
      this._authChallenge = "auth-" + base64String;
      this.CallTargetSignServerChallenge(this.get_connectionToClient(), this._authChallenge);
    }
    else
    {
      this._badgeChallenge = "badge-server-" + base64String;
      this.CallTargetSignServerChallenge(this.get_connectionToClient(), this._badgeChallenge);
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
    string response = ECDSA.Sign(challenge, GameConsole.Console.SessionKeys.get_Private());
    GameConsole.Console.singleton.AddLog("Signed " + challenge + " for server.", Color32.op_Implicit(Color.get_cyan()), false);
    this.CallCmdServerSignatureComplete(challenge, response, ECDSA.KeyToString(GameConsole.Console.SessionKeys.get_Public()));
  }

  [Command(channel = 2)]
  public void CmdServerSignatureComplete(string challenge, string response, string publickey)
  {
    if (this.FirstVerResult == null)
      this.FirstVerResult = CentralAuth.ValidateBadgeRequest(this._globalBadgeUnconfirmed, ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).SteamId, ((NicknameSync) ((Component) this).GetComponent<NicknameSync>()).myNick);
    if (this.FirstVerResult == null)
      return;
    if (this.FirstVerResult["Public key"] != ServerRoles.Base64Encode(Sha.HashToString(Sha.Sha256(publickey))))
    {
      GameConsole.Console.singleton.AddLog("Rejected signature of challenge " + challenge + " due to public key hash mismatch.", Color32.op_Implicit(Color.get_red()), false);
      ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "Challenge signature rejected due to public key mismatch.", "red");
    }
    else
    {
      if (this.PublicKey == null)
        this.PublicKey = ECDSA.PublicKeyFromString(publickey);
      if (!ECDSA.Verify(challenge, response, this.PublicKey))
      {
        GameConsole.Console.singleton.AddLog("Rejected signature of challenge " + challenge + " due to signature mismatch.", Color32.op_Implicit(Color.get_red()), false);
        ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "Challenge signature rejected due to signature mismatch.", "red");
      }
      else if (challenge.StartsWith("auth-") && challenge == this._authChallenge)
      {
        ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).NetworkSteamId = this.FirstVerResult["Steam ID"];
        ((NicknameSync) ((Component) this).GetComponent<NicknameSync>()).UpdateNickname(ServerRoles.Base64Decode(this.FirstVerResult["Nickname"]));
        ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "Hi " + ServerRoles.Base64Decode(this.FirstVerResult["Nickname"]) + "! Your challenge signature has been accepted.", "green");
        this.RefreshPermissions();
        this._authChallenge = string.Empty;
      }
      else
      {
        if (!challenge.StartsWith("badge-server-") || !(challenge == this._badgeChallenge))
          return;
        Dictionary<string, string> dictionary = CentralAuth.ValidateBadgeRequest(this._globalBadgeUnconfirmed, ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).SteamId, ((NicknameSync) ((Component) this).GetComponent<NicknameSync>()).myNick);
        if (dictionary == null)
        {
          ServerConsole.AddLog("Rejected signature of challenge " + challenge + " due to signature mismatch.");
          ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "Challenge signature rejected due to signature mismatch.", "red");
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
            ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PasswordTries = 0;
            this.CallTargetOpenRemoteAdmin(this.get_connectionToClient());
            ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "Your remote admin access has been granted (global permissions - staff).", "cyan");
          }
          else if (dictionary["Management"] == "YES" && ServerStatic.PermissionsHandler.ManagersAccess)
          {
            this.NetworkRemoteAdmin = true;
            this.Permissions = ServerStatic.PermissionsHandler.FullPerm;
            this.NetworkRemoteAdminMode = ServerRoles.AccessMode.GlobalAccess;
            ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PasswordTries = 0;
            this.CallTargetOpenRemoteAdmin(this.get_connectionToClient());
            ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "Your remote admin access has been granted (global permissions - management).", "cyan");
          }
          else if (dictionary["Global banning"] == "YES" && ServerStatic.PermissionsHandler.BanningTeamAccess)
          {
            this.NetworkRemoteAdmin = true;
            this.Permissions = ServerStatic.PermissionsHandler.FullPerm;
            this.NetworkRemoteAdminMode = ServerRoles.AccessMode.GlobalAccess;
            ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PasswordTries = 0;
            this.CallTargetOpenRemoteAdmin(this.get_connectionToClient());
            ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "Your remote admin access has been granted (global permissions - banning team).", "cyan");
          }
          ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "Your global badge has been granted.", "cyan");
          this._badgeChallenge = string.Empty;
          if (!this.Staff)
            return;
          foreach (GameObject player in PlayerManager.singleton.players)
          {
            ServerRoles component = (ServerRoles) player.GetComponent<ServerRoles>();
            if (!string.IsNullOrEmpty(component.HiddenBadge))
              component.CallTargetSetHiddenRole(this.get_connectionToClient(), component.HiddenBadge);
          }
          ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "Hidden badges have been displayed for you (if there are any).", "gray");
        }
      }
    }
  }

  [TargetRpc]
  private void TargetOpenRemoteAdmin(NetworkConnection connection)
  {
    ((UIController) Object.FindObjectOfType<UIController>()).ActivateRemoteAdmin();
  }

  [Command(channel = 2)]
  public void CmdSetOverwatchStatus(bool status)
  {
    if (!this.OverwatchPermitted && status)
      ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "You don't have permissions to enable overwatch mode!", "red");
    else
      this.SetOverwatchStatus(status);
  }

  [Command(channel = 2)]
  public void CmdToggleOverwatch()
  {
    if (!this.OverwatchPermitted && !this.OverwatchEnabled)
      ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this.get_connectionToClient(), "You don't have permissions to enable overwatch mode!", "red");
    else
      this.SetOverwatchStatus(!this.OverwatchEnabled);
  }

  public void SetOverwatchStatus(bool status)
  {
    this.OverwatchEnabled = status;
    CharacterClassManager component = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    if (status && component.curClass != 2 && component.curClass != -1)
      component.SetClassID(2);
    this.CallTargetSetOverwatch(this.get_connectionToClient(), this.OverwatchEnabled);
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
    GameConsole.Console.singleton.AddLog("Overwatch status: " + (!s ? "DISABLED" : "ENABLED"), Color32.op_Implicit(Color.get_green()), false);
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
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetColor(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<string>((M0) str, (M0&) ref local, (uint) num);
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
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetText(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<string>((M0) str, (M0&) ref local, (uint) num);
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
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetBadgeUpdate(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<string>((M0) str, (M0&) ref local, (uint) num);
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
      this.SetSyncVar<bool>((M0) (value ? 1 : 0), (M0&) ref this.GlobalSet, 8U);
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
      this.SetSyncVar<bool>((M0) (value ? 1 : 0), (M0&) ref this.RemoteAdmin, 16U);
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
      this.SetSyncVar<ServerRoles.AccessMode>((M0) value, (M0&) ref this.RemoteAdminMode, 32U);
    }
  }

  protected static void InvokeCmdCmdRequestBadge(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdRequestBadge called on client.");
    else
      ((ServerRoles) obj).CmdRequestBadge(reader.ReadString());
  }

  protected static void InvokeCmdCmdServerSignatureComplete(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdServerSignatureComplete called on client.");
    else
      ((ServerRoles) obj).CmdServerSignatureComplete(reader.ReadString(), reader.ReadString(), reader.ReadString());
  }

  protected static void InvokeCmdCmdSetOverwatchStatus(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSetOverwatchStatus called on client.");
    else
      ((ServerRoles) obj).CmdSetOverwatchStatus(reader.ReadBoolean());
  }

  protected static void InvokeCmdCmdToggleOverwatch(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdToggleOverwatch called on client.");
    else
      ((ServerRoles) obj).CmdToggleOverwatch();
  }

  public void CallCmdRequestBadge(string token)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdRequestBadge called on server.");
    else if (this.get_isServer())
    {
      this.CmdRequestBadge(token);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) ServerRoles.kCmdCmdRequestBadge);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(token);
      this.SendCommandInternal(networkWriter, 2, "CmdRequestBadge");
    }
  }

  public void CallCmdServerSignatureComplete(string challenge, string response, string publickey)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdServerSignatureComplete called on server.");
    else if (this.get_isServer())
    {
      this.CmdServerSignatureComplete(challenge, response, publickey);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) ServerRoles.kCmdCmdServerSignatureComplete);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(challenge);
      networkWriter.Write(response);
      networkWriter.Write(publickey);
      this.SendCommandInternal(networkWriter, 2, "CmdServerSignatureComplete");
    }
  }

  public void CallCmdSetOverwatchStatus(bool status)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSetOverwatchStatus called on server.");
    else if (this.get_isServer())
    {
      this.CmdSetOverwatchStatus(status);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) ServerRoles.kCmdCmdSetOverwatchStatus);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(status);
      this.SendCommandInternal(networkWriter, 2, "CmdSetOverwatchStatus");
    }
  }

  public void CallCmdToggleOverwatch()
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdToggleOverwatch called on server.");
    else if (this.get_isServer())
    {
      this.CmdToggleOverwatch();
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) ServerRoles.kCmdCmdToggleOverwatch);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendCommandInternal(networkWriter, 2, "CmdToggleOverwatch");
    }
  }

  protected static void InvokeRpcRpcResetFixed(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcResetFixed called on server.");
    else
      ((ServerRoles) obj).RpcResetFixed();
  }

  protected static void InvokeRpcTargetSetHiddenRole(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetSetHiddenRole called on server.");
    else
      ((ServerRoles) obj).TargetSetHiddenRole(ClientScene.get_readyConnection(), reader.ReadString());
  }

  protected static void InvokeRpcTargetSignServerChallenge(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetSignServerChallenge called on server.");
    else
      ((ServerRoles) obj).TargetSignServerChallenge(ClientScene.get_readyConnection(), reader.ReadString());
  }

  protected static void InvokeRpcTargetOpenRemoteAdmin(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetOpenRemoteAdmin called on server.");
    else
      ((ServerRoles) obj).TargetOpenRemoteAdmin(ClientScene.get_readyConnection());
  }

  protected static void InvokeRpcTargetSetOverwatch(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "TargetRPC TargetSetOverwatch called on server.");
    else
      ((ServerRoles) obj).TargetSetOverwatch(ClientScene.get_readyConnection(), reader.ReadBoolean());
  }

  public void CallRpcResetFixed()
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcResetFixed called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) ServerRoles.kRpcRpcResetFixed);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendRPCInternal(networkWriter, 2, "RpcResetFixed");
    }
  }

  public void CallTargetSetHiddenRole(NetworkConnection connection, string role)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetSetHiddenRole called on client.");
    else if (connection is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetSetHiddenRole called on connection to server");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) ServerRoles.kTargetRpcTargetSetHiddenRole);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(role);
      this.SendTargetRPCInternal(connection, networkWriter, 2, "TargetSetHiddenRole");
    }
  }

  public void CallTargetSignServerChallenge(NetworkConnection target, string challenge)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetSignServerChallenge called on client.");
    else if (target is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetSignServerChallenge called on connection to server");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) ServerRoles.kTargetRpcTargetSignServerChallenge);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(challenge);
      this.SendTargetRPCInternal(target, networkWriter, 2, "TargetSignServerChallenge");
    }
  }

  public void CallTargetOpenRemoteAdmin(NetworkConnection connection)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetOpenRemoteAdmin called on client.");
    else if (connection is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetOpenRemoteAdmin called on connection to server");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) ServerRoles.kTargetRpcTargetOpenRemoteAdmin);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      this.SendTargetRPCInternal(connection, networkWriter, 0, "TargetOpenRemoteAdmin");
    }
  }

  public void CallTargetSetOverwatch(NetworkConnection conn, bool s)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "TargetRPC Function TargetSetOverwatch called on client.");
    else if (conn is ULocalConnectionToServer)
    {
      Debug.LogError((object) "TargetRPC Function TargetSetOverwatch called on connection to server");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) ServerRoles.kTargetRpcTargetSetOverwatch);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(s);
      this.SendTargetRPCInternal(conn, networkWriter, 2, "TargetSetOverwatch");
    }
  }

  static ServerRoles()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (ServerRoles), ServerRoles.kCmdCmdRequestBadge, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdRequestBadge)));
    ServerRoles.kCmdCmdServerSignatureComplete = -834487468;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (ServerRoles), ServerRoles.kCmdCmdServerSignatureComplete, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdServerSignatureComplete)));
    ServerRoles.kCmdCmdSetOverwatchStatus = 200610181;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (ServerRoles), ServerRoles.kCmdCmdSetOverwatchStatus, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSetOverwatchStatus)));
    ServerRoles.kCmdCmdToggleOverwatch = -571630643;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (ServerRoles), ServerRoles.kCmdCmdToggleOverwatch, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdToggleOverwatch)));
    ServerRoles.kRpcRpcResetFixed = -1154333771;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (ServerRoles), ServerRoles.kRpcRpcResetFixed, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcResetFixed)));
    ServerRoles.kTargetRpcTargetSetHiddenRole = -948979541;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (ServerRoles), ServerRoles.kTargetRpcTargetSetHiddenRole, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetSetHiddenRole)));
    ServerRoles.kTargetRpcTargetSignServerChallenge = 1367769996;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (ServerRoles), ServerRoles.kTargetRpcTargetSignServerChallenge, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetSignServerChallenge)));
    ServerRoles.kTargetRpcTargetOpenRemoteAdmin = 1449538856;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (ServerRoles), ServerRoles.kTargetRpcTargetOpenRemoteAdmin, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetOpenRemoteAdmin)));
    ServerRoles.kTargetRpcTargetSetOverwatch = -1052391504;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (ServerRoles), ServerRoles.kTargetRpcTargetSetOverwatch, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcTargetSetOverwatch)));
    NetworkCRC.RegisterBehaviour(nameof (ServerRoles), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
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
    if (((int) this.get_syncVarDirtyBits() & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.MyColor);
    }
    if (((int) this.get_syncVarDirtyBits() & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.MyText);
    }
    if (((int) this.get_syncVarDirtyBits() & 4) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.GlobalBadge);
    }
    if (((int) this.get_syncVarDirtyBits() & 8) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.GlobalSet);
    }
    if (((int) this.get_syncVarDirtyBits() & 16) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.RemoteAdmin);
    }
    if (((int) this.get_syncVarDirtyBits() & 32) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write((int) this.RemoteAdminMode);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
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
