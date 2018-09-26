// Decompiled with JetBrains decompiler
// Type: CentralAuthInterface
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using GameConsole;
using UnityEngine;

public class CentralAuthInterface : ICentralAuth
{
  private readonly CharacterClassManager _s;
  private readonly bool _is;

  public CentralAuthInterface(CharacterClassManager sync, bool server)
  {
    this._s = sync;
    this._is = server;
  }

  public CharacterClassManager GetCcm()
  {
    return this._s;
  }

  public void TokenGenerated(string token)
  {
    Console.singleton.AddLog("Authentication token obtained from central server.", Color32.op_Implicit(Color.get_green()), false);
    this._s.CallCmdSendToken(token);
  }

  public void RequestBadge(string token)
  {
    ((ServerRoles) ((Component) this._s).GetComponent<ServerRoles>()).RequestBadge(token);
  }

  public void Fail()
  {
    if (this._is)
    {
      ServerConsole.AddLog("Failed to validate authentication token.");
      ServerConsole.Disconnect(this._s.get_connectionToClient(), "Failed to validate authentication token.");
    }
    else
    {
      Console.singleton.AddLog("Failed to obtain authentication token from central server.", Color32.op_Implicit(Color.get_red()), false);
      this._s.get_connectionToServer().Disconnect();
      this._s.get_connectionToServer().Dispose();
    }
  }

  public void Ok(string steamId, string nickname, string ban, string steamban, string server, bool bypass)
  {
    ServerConsole.AddLog("Accepted authentication token of user " + steamId + " with global ban status " + ban + " signed by " + server + " server.");
    this._s.CallTargetConsolePrint(this._s.get_connectionToClient(), "Accepted your authentication token (your steam id " + steamId + ") with global ban status " + ban + " signed by " + server + " server.", "green");
    if ((!bypass || !ServerStatic.GetPermissionsHandler().IsVerified) && BanHandler.QueryBan(steamId, (string) null).Key != null)
    {
      this._s.CallTargetConsolePrint(this._s.get_connectionToClient(), "You are banned from this server.", "red");
      ServerConsole.AddLog("Player kicked due to local SteamID ban.");
      ServerConsole.Disconnect(this._s.get_connectionToClient(), "You are banned from this server.");
    }
    else if ((!bypass || !ServerStatic.GetPermissionsHandler().IsVerified) && !WhiteList.IsWhitelisted(steamId))
    {
      this._s.CallTargetConsolePrint(this._s.get_connectionToClient(), "You are not on the whitelist!", "red");
      ServerConsole.AddLog("Player kicked due to whitelist enabled.");
      ServerConsole.Disconnect(this._s.get_connectionToClient(), "You are not on the whitelist for this server.");
    }
    else if ((ConfigFile.ServerConfig.GetBool("use_vac", true) || ServerStatic.PermissionsHandler.IsVerified) && steamban != "0")
    {
      this._s.CallTargetConsolePrint(this._s.get_connectionToClient(), "You have active steam ban (" + steamban + " ban).", "red");
      ServerConsole.AddLog("Player kicked due to steam ban (" + steamban + " ban).");
      ServerConsole.Disconnect(this._s.get_connectionToClient(), "You have active steam ban (" + steamban + " ban).");
    }
    else if ((ConfigFile.ServerConfig.GetBool("global_bans_cheating", true) || ServerStatic.PermissionsHandler.IsVerified) && ban == "1")
    {
      this._s.CallTargetConsolePrint(this._s.get_connectionToClient(), "You have been globally banned for cheating.", "red");
      ServerConsole.AddLog("Player kicked due to global ban for cheating.");
      ServerConsole.Disconnect(this._s.get_connectionToClient(), "You have been globally banned for cheating.");
    }
    else if ((ConfigFile.ServerConfig.GetBool("global_bans_griefing", true) || ServerStatic.PermissionsHandler.IsVerified) && ban == "2")
    {
      this._s.CallTargetConsolePrint(this._s.get_connectionToClient(), "You have been globally banned for griefing.", "red");
      ServerConsole.AddLog("Player kicked due to global ban for griefing.");
      ServerConsole.Disconnect(this._s.get_connectionToClient(), "You have been globally banned for griefing.");
    }
    else
    {
      ServerRoles component = (ServerRoles) ((Component) this._s).GetComponent<ServerRoles>();
      component.BypassStaff = component.BypassStaff || bypass;
      if (component.BypassStaff)
        ((CharacterClassManager) ((Component) component).GetComponent<CharacterClassManager>()).CallTargetConsolePrint(this._s.get_connectionToClient(), "Your have the ban bypass flag, so you can't be banned from this server.", "cyan");
      component.StartServerChallenge(0);
    }
  }

  public void FailToken(string reason)
  {
    this._s.CallTargetConsolePrint(this._s.get_connectionToClient(), "Your authentication token is invalid - " + reason + ".", "red");
    ServerConsole.AddLog("Rejected invalid authentication token.");
    ServerConsole.Disconnect(this._s.get_connectionToClient(), reason);
  }
}
