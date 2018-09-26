// Decompiled with JetBrains decompiler
// Type: RemoteAdmin.QueryProcessor
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Cryptography;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

namespace RemoteAdmin
{
  public class QueryProcessor : NetworkBehaviour
  {
    private static int kCmdCmdRequestSalt = -780447461;
    public static QueryProcessor Localplayer;
    private ServerRoles _roles;
    public static bool Lockdown;
    private int _toBanType;
    private static int _idIterator;
    public const int HashIterations = 250;
    internal int PasswordTries;
    internal int SignaturesCounter;
    private int _signaturesCounter;
    internal byte[] Key;
    internal byte[] Salt;
    internal byte[] ClientSalt;
    private byte[] _key;
    private byte[] _salt;
    private byte[] _clientSalt;
    private float _lastPlayerlistRequest;
    private string _toBan;
    private string _toBanNick;
    private string _toBanSteamId;
    [SyncVar(hook = "SetId")]
    public int PlayerId;
    [SyncVar(hook = "SetOverridePasswordEnabled")]
    public bool OverridePasswordEnabled;
    [SyncVar]
    public bool GameplayData;
    private string ipAddress;
    private NetworkConnection conns;
    private static int kTargetRpcTargetSaltGenerated;
    private static int kCmdCmdSendPassword;
    private static int kTargetRpcTargetReplyPassword;
    private static int kTargetRpcTargetReply;
    private static int kCmdCmdSendQuery;
    private static int kTargetRpcTargetStaffPlayerListResponse;
    private static int kTargetRpcTargetStaffAuthTokenResponse;

    private void Start()
    {
      this._roles = this.GetComponent<ServerRoles>();
      this.SignaturesCounter = 0;
      this._signaturesCounter = 0;
      if (NetworkServer.active)
      {
        this.conns = this.connectionToClient;
        this.ipAddress = this.conns.address;
        this.NetworkOverridePasswordEnabled = ServerStatic.PermissionsHandler.OverrideEnabled;
        ++QueryProcessor._idIterator;
        this.SetId(QueryProcessor._idIterator);
      }
      if (!this.isLocalPlayer)
        return;
      QueryProcessor.Localplayer = this;
      this.InvokeRepeating("RefreshPlayerList", 2f, 2f);
    }

    private void SetOverridePasswordEnabled(bool b)
    {
      this.NetworkOverridePasswordEnabled = b;
    }

    private void SetId(int id)
    {
      this.NetworkPlayerId = id;
    }

    public void RefreshPlayerList()
    {
      if (!this.isLocalPlayer || !this._roles.RemoteAdmin || ((double) this._lastPlayerlistRequest <= 0.200000002980232 || !UIController.singleton.opened))
        return;
      this._lastPlayerlistRequest = 0.0f;
      this.CmdSendQuery("REQUEST_DATA PLAYER_LIST SILENT");
    }

    public static void StaticRefreshPlayerList()
    {
      if (!((Object) QueryProcessor.Localplayer != (Object) null))
        return;
      QueryProcessor.Localplayer.RefreshPlayerList();
    }

    private void Update()
    {
      if (!this.isLocalPlayer || (double) this._lastPlayerlistRequest >= 1.0)
        return;
      this._lastPlayerlistRequest += Time.deltaTime;
    }

    [Command(channel = 2)]
    public void CmdRequestSalt(byte[] clSalt)
    {
      if (!ServerStatic.PermissionsHandler.OverrideEnabled)
      {
        this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Password authentication is disabled on this server!", "magenta");
      }
      else
      {
        if (this._clientSalt == null)
        {
          if (clSalt == null)
          {
            this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Please generate and send your salt!", "red");
            return;
          }
          if (clSalt.Length < 16)
          {
            this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Generated salt is too short. Please generate longer salt and try again!", "red");
            return;
          }
          this._clientSalt = clSalt;
          if (this._key == null && this._salt != null)
            this._key = ServerStatic.PermissionsHandler.DerivePassword(this._salt, this._clientSalt);
          this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Your salt " + Convert.ToBase64String(clSalt) + " has been accepted by the server.", "cyan");
        }
        if (this._salt != null)
        {
          this.CallTargetSaltGenerated(this.connectionToClient, this._salt);
        }
        else
        {
          RandomNumberGenerator randomNumberGenerator = (RandomNumberGenerator) new RNGCryptoServiceProvider();
          byte[] data = new byte[16];
          randomNumberGenerator.GetBytes(data);
          this._salt = data;
          this._key = ServerStatic.PermissionsHandler.DerivePassword(this._salt, this._clientSalt);
          this.CallTargetSaltGenerated(this.connectionToClient, this._salt);
        }
      }
    }

    [TargetRpc(channel = 2)]
    public void TargetSaltGenerated(NetworkConnection conn, byte[] salt)
    {
      if (salt.Length < 16)
      {
        GameConsole.Console.singleton.AddLog("Rejected salt " + (object) salt + " because it's too short!", (Color32) Color.red, false);
      }
      else
      {
        GameConsole.Console.singleton.AddLog("Obtained server's salt " + Convert.ToBase64String(salt) + " from server.", (Color32) Color.cyan, false);
        this.Salt = salt;
      }
    }

    [Command(channel = 15)]
    public void CmdSendPassword(byte[] authSignature)
    {
      bool b = false;
      if (this._roles.RemoteAdmin)
      {
        b = true;
        this.PasswordTries = 0;
      }
      else
      {
        if (this._salt == null || this._clientSalt == null)
        {
          this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Can't verify your remote admin password - please generate salt first!", "red");
          return;
        }
        if (this._clientSalt.Length < 16)
        {
          this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Generated salt is too short. Please rejoin the server and try again!", "red");
          return;
        }
        if (this.VerifyHmacSignature("Login", -1, authSignature, false))
        {
          this.PasswordTries = 0;
          UserGroup overrideGroup = ServerStatic.PermissionsHandler.OverrideGroup;
          if (overrideGroup != null)
          {
            ServerConsole.AddLog("Assigned group " + overrideGroup.BadgeText + " to " + this.GetComponent<NicknameSync>().myNick + " - override password.");
            this._roles.SetGroup(overrideGroup, true, false);
            b = true;
          }
          else
            this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Non-existing group is assigned for override password!", "red");
        }
        else
        {
          ++this.PasswordTries;
          ServerConsole.AddLog("Rejected override password sent by " + this.GetComponent<NicknameSync>().myNick + ".");
        }
      }
      if (this.PasswordTries >= 3)
        ServerConsole.Disconnect(this.connectionToClient, "You have been kicked for too many Remote Admin login attempts.");
      else
        this.CallTargetReplyPassword(this.connectionToClient, b);
    }

    [TargetRpc(channel = 14)]
    private void TargetReplyPassword(NetworkConnection conn, bool b)
    {
      Object.FindObjectOfType<UIController>().awaitingLogin = !b ? 0 : 2;
    }

    [TargetRpc(channel = 15)]
    private void TargetReply(NetworkConnection conn, string content, bool isSuccess, bool logInConsole, string overrideDisplay)
    {
      string str = content.Remove(content.IndexOf("#", StringComparison.Ordinal));
      content = content.Remove(0, content.IndexOf("#", StringComparison.Ordinal) + 1);
      if (logInConsole)
        TextBasedRemoteAdmin.AddLog((!isSuccess ? "<color=orange>" : "<color=white>") + "(" + str + ") " + content + "</color>");
      if (overrideDisplay == string.Empty)
      {
        if (str != null)
        {
          if (!(str == "HELP"))
          {
            if (!(str == "REQUEST_DATA:PLAYER_LIST"))
            {
              if (!(str == "REQUEST_DATA:PLAYER"))
              {
                if (str == "LOGOUT")
                {
                  UIController objectOfType = Object.FindObjectOfType<UIController>();
                  if (objectOfType.root_root.activeSelf)
                    objectOfType.ChangeConsoleStage();
                  objectOfType.loggedIn = false;
                  return;
                }
              }
              else
              {
                PlayerRequest.singleton.ResponsePlayerSpecific(content, isSuccess);
                return;
              }
            }
            else
            {
              PlayerRequest.singleton.ResponsePlayerList(content, isSuccess, this.GameplayData);
              return;
            }
          }
          else
          {
            Application.OpenURL("https://docs.google.com/document/d/1nj6fNULwc7Kx3fNnt5Gh2YTIqg8jS5d_Z0fDXpTimAw/edit?usp=sharing");
            return;
          }
        }
        int menuId = 0;
        foreach (SubmenuSelector.SubMenu menu in SubmenuSelector.singleton.menus)
        {
          if (menu.commandTemplate.StartsWith(str))
            DisplayDataOnScreen.singleton.Show(menuId, (!isSuccess ? "<color=red>" : "<color=green>") + content + "</color>");
          ++menuId;
        }
      }
      else
      {
        int menuId = 0;
        foreach (SubmenuSelector.SubMenu menu in SubmenuSelector.singleton.menus)
        {
          if (menu.commandTemplate == overrideDisplay)
            DisplayDataOnScreen.singleton.Show(menuId, (!isSuccess ? "<color=red>" : "<color=green>") + content + "</color>");
          ++menuId;
        }
      }
    }

    [Client]
    public void CmdSendQuery(string query)
    {
      if (!NetworkClient.active)
      {
        Debug.LogWarning((object) "[Client] function 'System.Void RemoteAdmin.QueryProcessor::CmdSendQuery(System.String)' called on server");
      }
      else
      {
        ++this.SignaturesCounter;
        this.CallCmdSendQuery(query, this.SignaturesCounter, this.SignRequest(query, -2));
      }
    }

    [Command(channel = 15)]
    public void CmdSendQuery(string query, int counter, byte[] signature)
    {
      if (this._roles.RemoteAdmin)
      {
        if (this.VerifyRequestSignature(query, counter, signature, true))
          this.ProcessQuery(query);
        else
          this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "Signature verification of request \"" + query + "\" failed!", "magenta");
      }
      else
        this.GetComponent<CharacterClassManager>().CallTargetConsolePrint(this.connectionToClient, "You are not logged in to remote admin panel!", "red");
    }

    [ServerCallback]
    private void ProcessQuery(string q)
    {
      if (!NetworkServer.active)
        return;
      if (!q.Contains("SILENT"))
        TextBasedRemoteAdmin.AddLog("<color=purple>(USER-INPUT) " + q + "</color>");
      string[] strArray = q.Split(' ');
      string nick = this.GetComponent<NicknameSync>().myNick;
      string upper1 = strArray[0].ToUpper();
      if (upper1 != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (QueryProcessor.\u003C\u003Ef__switch\u0024map2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          QueryProcessor.\u003C\u003Ef__switch\u0024map2 = new Dictionary<string, int>(40)
          {
            {
              "HELLO",
              0
            },
            {
              "HELP",
              1
            },
            {
              "BAN",
              2
            },
            {
              "SETGROUP",
              3
            },
            {
              "UNBAN",
              4
            },
            {
              "GROUPS",
              5
            },
            {
              "FORCECLASS",
              6
            },
            {
              "HEAL",
              7
            },
            {
              "HP",
              8
            },
            {
              "SETHP",
              8
            },
            {
              "OVR",
              9
            },
            {
              "OVERWATCH",
              9
            },
            {
              "GOD",
              10
            },
            {
              "BM",
              11
            },
            {
              "BYPASS",
              11
            },
            {
              "LD",
              12
            },
            {
              "LOCKDOWN",
              12
            },
            {
              "O",
              13
            },
            {
              "OPEN",
              13
            },
            {
              "C",
              14
            },
            {
              "CLOSE",
              14
            },
            {
              "L",
              15
            },
            {
              "LOCK",
              15
            },
            {
              "UL",
              16
            },
            {
              "UNLOCK",
              16
            },
            {
              "DESTROY",
              17
            },
            {
              "DL",
              18
            },
            {
              "DOORS",
              18
            },
            {
              "DOORLIST",
              18
            },
            {
              "GIVE",
              19
            },
            {
              "REQUEST_DATA",
              20
            },
            {
              "CONTACT",
              21
            },
            {
              "LOGOUT",
              22
            },
            {
              "SERVER_EVENT",
              23
            },
            {
              "HIDETAG",
              24
            },
            {
              "SHOWTAG",
              25
            },
            {
              "GTAG",
              26
            },
            {
              "GLOBALTAG",
              26
            },
            {
              "SRVCFG",
              27
            },
            {
              "PERM",
              28
            }
          };
        }
        int num;
        // ISSUE: reference to a compiler-generated field
        if (QueryProcessor.\u003C\u003Ef__switch\u0024map2.TryGetValue(upper1, out num))
        {
          switch (num)
          {
            case 0:
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Hello World!", true, true, string.Empty);
              return;
            case 1:
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#This should be useful!", true, true, string.Empty);
              return;
            case 2:
              if (strArray.Length >= 3)
              {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, nick + " ran the ban command (duration: " + strArray[2] + " min) on " + strArray[1] + " players.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                int failures;
                int successes;
                string error;
                bool replySent;
                this.StandardizedQueryModel1(strArray[0], strArray[1], strArray[2], out failures, out successes, out error, out replySent);
                if (replySent)
                  return;
                switch (failures)
                {
                  case -2:
                    this.CallTargetReply(this.connectionToClient, strArray[0] + "#You can't ban global staff from verified server.", false, true, string.Empty);
                    return;
                  case 0:
                    string str = "Banned";
                    int result;
                    if (int.TryParse(strArray[2], out result))
                      str = result <= 0 ? "Kicked" : "Banned";
                    this.CallTargetReply(this.connectionToClient, strArray[0] + "#Done! " + str + " " + (object) successes + " player(s)!", true, true, string.Empty);
                    return;
                  default:
                    this.CallTargetReply(this.connectionToClient, strArray[0] + "#The proccess has occured an issue! Failures: " + (object) failures + "\nLast error log:\n" + error, false, true, string.Empty);
                    return;
                }
              }
              else
              {
                this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#To run this program, type at least 3 arguments! (some parameters are missing)", false, true, string.Empty);
                return;
              }
            case 3:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.SetGroup, true))
                return;
              if (strArray.Length >= 3)
              {
                ServerLogs.AddLog(ServerLogs.Modules.Permissions, nick + " ran the setgroup command (new group: " + strArray[2] + " min) on " + strArray[1] + " players.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                int failures;
                int successes;
                string error;
                bool replySent;
                this.StandardizedQueryModel1(strArray[0], strArray[1], strArray[2], out failures, out successes, out error, out replySent);
                if (replySent)
                  return;
                if (failures == 0)
                {
                  this.CallTargetReply(this.connectionToClient, strArray[0] + "#Done! The request affedted " + (object) successes + " player(s)!", true, true, string.Empty);
                  return;
                }
                this.CallTargetReply(this.connectionToClient, strArray[0] + "#The proccess has occured an issue! Failures: " + (object) failures + "\nLast error log:\n" + error, false, true, string.Empty);
                return;
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#To run this program, type at least 3 arguments! (some parameters are missing)", false, true, string.Empty);
              return;
            case 4:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.LongTermBanning, true))
                return;
              if (strArray.Length == 3)
              {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, nick + " ran the unban " + strArray[1] + " command on " + strArray[2] + ".", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
                if (strArray[1].ToLower() == "id" || strArray[1].ToLower() == "steamid")
                {
                  BanHandler.RemoveBan(strArray[2], 0);
                  this.CallTargetReply(this.connectionToClient, strArray[0] + "#Done!", true, true, string.Empty);
                  return;
                }
                if (strArray[1].ToLower() == "ip" || strArray[1].ToLower() == "address")
                {
                  BanHandler.RemoveBan(strArray[2], 1);
                  this.CallTargetReply(this.connectionToClient, strArray[0] + "#Done!", true, true, string.Empty);
                  return;
                }
                this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Correct syntax is: unban id SteamIdHere OR unban ip IpAddressHere", false, true, string.Empty);
                return;
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Correct syntax is: unban id SteamIdHere OR unban ip IpAddressHere", false, true, string.Empty);
              return;
            case 5:
              string str1 = "Groups defined on this server:";
              Dictionary<string, UserGroup> allGroups = ServerStatic.PermissionsHandler.GetAllGroups();
              ServerRoles.NamedColor[] namedColors = this.GetComponent<ServerRoles>().NamedColors;
              foreach (KeyValuePair<string, UserGroup> keyValuePair in allGroups)
              {
                KeyValuePair<string, UserGroup> permentry = keyValuePair;
                try
                {
                  str1 = str1 + "\n" + permentry.Key + " (" + (object) permentry.Value.Permissions + ") - <color=#" + ((IEnumerable<ServerRoles.NamedColor>) namedColors).FirstOrDefault<ServerRoles.NamedColor>((Func<ServerRoles.NamedColor, bool>) (x => x.Name == permentry.Value.BadgeColor)).ColorHex + ">" + permentry.Value.BadgeText + "</color> in color " + permentry.Value.BadgeColor;
                }
                catch
                {
                  str1 = str1 + "\n" + permentry.Key + " (" + (object) permentry.Value.Permissions + ") - " + permentry.Value.BadgeText + " in color " + permentry.Value.BadgeColor;
                }
                if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.KickingAndShortTermBanning))
                  str1 += " K";
                if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.BanningUpToDay))
                  str1 += " B1";
                if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.LongTermBanning))
                  str1 += " B2";
                if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ForceclassSelf))
                  str1 += " FSE";
                if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ForceclassToSpectator))
                  str1 += " FSP";
                if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ForceclassWithoutRestrictions))
                  str1 += " FC";
                if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.GivingItems))
                  str1 += " G";
                if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.WarheadEvents))
                  str1 += " EW";
                if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.RespawnEvents))
                  str1 += " ERS";
                if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.RoundEvents))
                  str1 += " ERD";
                if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.SetGroup))
                  str1 += " SG";
                if (ServerStatic.PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.FacilityManagement))
                  str1 += " FM";
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#" + str1, true, true, string.Empty);
              return;
            case 6:
              if (strArray.Length >= 3)
              {
                if (this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.ForceclassWithoutRestrictions, false) || strArray[2] == "2" && this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.ForceclassToSpectator, false) || strArray[1] == this.PlayerId.ToString() + "." && this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.ForceclassSelf, false))
                {
                  ServerLogs.AddLog(ServerLogs.Modules.ClassChange, nick + " ran the forceclass command (ID:" + strArray[2] + ") on " + strArray[1] + " players.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                  int failures;
                  int successes;
                  string error;
                  bool replySent;
                  this.StandardizedQueryModel1(strArray[0], strArray[1], strArray[2], out failures, out successes, out error, out replySent);
                  if (replySent)
                    return;
                  if (failures == 0)
                  {
                    this.CallTargetReply(this.connectionToClient, strArray[0] + "#Done! The request affected " + (object) successes + " player(s)!", true, true, string.Empty);
                    return;
                  }
                  this.CallTargetReply(this.connectionToClient, strArray[0] + "#The proccess has occured an issue! Failures: " + (object) failures + "\nLast error log:\n" + error, false, true, string.Empty);
                  return;
                }
                this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#You don't have permissions to execute this command.", false, true, string.Empty);
                return;
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#To run this program, type at least 3 arguments! (some parameters are missing)", false, true, string.Empty);
              return;
            case 7:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.PlayersManagement, true))
                return;
              if (strArray.Length >= 2)
              {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, nick + " ran the heal command on " + strArray[1] + " players.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                int failures;
                int successes;
                string error;
                bool replySent;
                this.StandardizedQueryModel1(strArray[0], strArray[1], (string) null, out failures, out successes, out error, out replySent);
                if (replySent)
                  return;
                if (failures == 0)
                {
                  this.CallTargetReply(this.connectionToClient, strArray[0] + "#Done! The request affected " + (object) successes + " player(s)!", true, true, string.Empty);
                  return;
                }
                this.CallTargetReply(this.connectionToClient, strArray[0] + "#The proccess has occured an issue! Failures: " + (object) failures + "\nLast error log:\n" + error, false, true, "AdminTools");
                return;
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#To run this program, type at least 2 arguments! (some parameters are missing)", false, true, string.Empty);
              return;
            case 8:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.PlayersManagement, true))
                return;
              if (strArray.Length >= 3)
              {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, nick + " ran the sethp command on " + strArray[1] + " players.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                int failures;
                int successes;
                string error;
                bool replySent;
                this.StandardizedQueryModel1(strArray[0], strArray[1], strArray[2], out failures, out successes, out error, out replySent);
                if (replySent)
                  return;
                if (failures == 0)
                {
                  this.CallTargetReply(this.connectionToClient, strArray[0] + "#Done! The request affected " + (object) successes + " player(s)!", true, true, string.Empty);
                  return;
                }
                this.CallTargetReply(this.connectionToClient, strArray[0] + "#The proccess has occured an issue! Failures: " + (object) failures + "\nLast error log:\n" + error, false, true, string.Empty);
                return;
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#To run this program, type at least 3 arguments! (some parameters are missing)", false, true, string.Empty);
              return;
            case 9:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.Overwatch, true))
                return;
              if (strArray.Length >= 2)
              {
                if (strArray.Length == 2)
                  strArray = new string[3]
                  {
                    strArray[0],
                    strArray[1],
                    string.Empty
                  };
                ServerLogs.AddLog(ServerLogs.Modules.ClassChange, nick + " ran the overwatch command (new status: " + (!(strArray[2] == string.Empty) ? strArray[2] : "TOGGLE") + ") on " + strArray[1] + " players.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                int failures;
                int successes;
                string error;
                bool replySent;
                this.StandardizedQueryModel1("OVERWATCH", strArray[1], strArray[2], out failures, out successes, out error, out replySent);
                if (replySent)
                  return;
                if (failures == 0)
                {
                  this.CallTargetReply(this.connectionToClient, "OVERWATCH#Done! The request affected " + (object) successes + " player(s)!", true, true, "AdminTools");
                  return;
                }
                this.CallTargetReply(this.connectionToClient, "OVERWATCH#The proccess has occured an issue! Failures: " + (object) failures + "\nLast error log:\n" + error, false, true, "AdminTools");
                return;
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#To run this program, type at least 2 arguments! (some parameters are missing)", false, true, "AdminTools");
              return;
            case 10:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.Overwatch, true))
                return;
              if (strArray.Length >= 2)
              {
                if (strArray.Length == 2)
                  strArray = new string[3]
                  {
                    strArray[0],
                    strArray[1],
                    string.Empty
                  };
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, nick + " ran the god command (new status: " + (!(strArray[2] == string.Empty) ? strArray[2] : "TOGGLE") + ") on " + strArray[1] + " players.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                int failures;
                int successes;
                string error;
                bool replySent;
                this.StandardizedQueryModel1("GOD", strArray[1], strArray[2], out failures, out successes, out error, out replySent);
                if (replySent)
                  return;
                if (failures == 0)
                {
                  this.CallTargetReply(this.connectionToClient, "OVERWATCH#Done! The request affected " + (object) successes + " player(s)!", true, true, "AdminTools");
                  return;
                }
                this.CallTargetReply(this.connectionToClient, "OVERWATCH#The proccess has occured an issue! Failures: " + (object) failures + "\nLast error log:\n" + error, false, true, "AdminTools");
                return;
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#To run this program, type at least 2 arguments! (some parameters are missing)", false, true, "AdminTools");
              return;
            case 11:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.FacilityManagement, true))
                return;
              if (strArray.Length >= 2)
              {
                if (strArray.Length == 2)
                  strArray = new string[3]
                  {
                    strArray[0],
                    strArray[1],
                    string.Empty
                  };
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, nick + " ran the bypass mode command (new status: " + (!(strArray[2] == string.Empty) ? strArray[2] : "TOGGLE") + ") on " + strArray[1] + " players.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                int failures;
                int successes;
                string error;
                bool replySent;
                this.StandardizedQueryModel1("BYPASS", strArray[1], strArray[2], out failures, out successes, out error, out replySent);
                if (replySent)
                  return;
                if (failures == 0)
                {
                  this.CallTargetReply(this.connectionToClient, "BYPASS#Done! The request affected " + (object) successes + " player(s)!", true, true, "AdminTools");
                  return;
                }
                this.CallTargetReply(this.connectionToClient, "BYPASS#The proccess has occured an issue! Failures: " + (object) failures + "\nLast error log:\n" + error, false, true, "AdminTools");
                return;
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#To run this program, type at least 2 arguments! (some parameters are missing)", false, true, "AdminTools");
              return;
            case 12:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.FacilityManagement, true))
                return;
              if (!QueryProcessor.Lockdown)
              {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, nick + " enabled the lockdown.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                foreach (Door door in Object.FindObjectsOfType<Door>())
                {
                  if (!door.locked)
                  {
                    door.lockdown = true;
                    door.UpdateLock();
                  }
                }
                QueryProcessor.Lockdown = true;
                this.CallTargetReply(this.connectionToClient, strArray[0] + "#Lockdown enabled!", true, true, "AdminTools");
                return;
              }
              ServerLogs.AddLog(ServerLogs.Modules.Administrative, nick + " disabled the lockdown.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
              foreach (Door door in Object.FindObjectsOfType<Door>())
              {
                if (door.lockdown)
                {
                  door.lockdown = false;
                  door.UpdateLock();
                }
              }
              QueryProcessor.Lockdown = false;
              this.CallTargetReply(this.connectionToClient, strArray[0] + "#Lockdown disabled!", true, true, "AdminTools");
              return;
            case 13:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.FacilityManagement, true))
                return;
              if (strArray.Length != 2)
              {
                this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Syntax of this program: " + strArray[0].ToUpper() + " DoorName", false, true, string.Empty);
                return;
              }
              this.ProcessDoorQuery("OPEN", strArray[1]);
              return;
            case 14:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.FacilityManagement, true))
                return;
              if (strArray.Length != 2)
              {
                this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Syntax of this program: " + strArray[0].ToUpper() + " DoorName", false, true, string.Empty);
                return;
              }
              this.ProcessDoorQuery("CLOSE", strArray[1]);
              return;
            case 15:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.FacilityManagement, true))
                return;
              if (strArray.Length != 2)
              {
                this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Syntax of this program: " + strArray[0].ToUpper() + " DoorName", false, true, string.Empty);
                return;
              }
              this.ProcessDoorQuery("LOCK", strArray[1]);
              return;
            case 16:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.FacilityManagement, true))
                return;
              if (strArray.Length != 2)
              {
                this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Syntax of this program: " + strArray[0].ToUpper() + " DoorName", false, true, string.Empty);
                return;
              }
              this.ProcessDoorQuery("UNLOCK", strArray[1]);
              return;
            case 17:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.FacilityManagement, true))
                return;
              if (strArray.Length != 2)
              {
                this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Syntax of this program: " + strArray[0].ToUpper() + " DoorName", false, true, string.Empty);
                return;
              }
              this.ProcessDoorQuery("DESTROY", strArray[1]);
              return;
            case 18:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.FacilityManagement, true))
                return;
              string str2 = "List of named doors in the facility:\n";
              List<string> list = ((IEnumerable<Door>) Object.FindObjectsOfType<Door>()).Where<Door>((Func<Door, bool>) (item => !string.IsNullOrEmpty(item.DoorName))).Select<Door, string>((Func<Door, string>) (item => item.DoorName + " - " + (!item.isOpen ? "<color=orange>CLOSED</color>" : "<color=green>OPENED</color>") + (!item.locked ? string.Empty : " <color=red>[LOCKED]</color>") + (!string.IsNullOrEmpty(item.permissionLevel) ? " <color=blue>[CARD REQUIRED]</color>" : string.Empty))).ToList<string>();
              list.Sort();
              string str3 = str2 + list.Aggregate<string>((Func<string, string, string>) ((current, adding) => current + "\n" + adding));
              this.CallTargetReply(this.connectionToClient, strArray[0] + "#" + str3, true, true, string.Empty);
              return;
            case 19:
              if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.GivingItems, true))
                return;
              if (strArray.Length >= 3)
              {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, nick + " ran the give command (ID:" + strArray[2] + ") on " + strArray[1] + " players.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                int failures;
                int successes;
                string error;
                bool replySent;
                this.StandardizedQueryModel1(strArray[0], strArray[1], strArray[2], out failures, out successes, out error, out replySent);
                if (replySent)
                  return;
                if (failures == 0)
                {
                  this.CallTargetReply(this.connectionToClient, strArray[0] + "#Done! The request affected " + (object) successes + " player(s)!", true, true, string.Empty);
                  return;
                }
                this.CallTargetReply(this.connectionToClient, strArray[0] + "#The proccess has occured an issue! Failures: " + (object) failures + "\nLast error log:\n" + error, false, true, string.Empty);
                return;
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#To run this program, type at least 3 arguments! (some parameters are missing)", false, true, string.Empty);
              return;
            case 20:
              if (strArray.Length >= 2)
              {
                if (strArray[1].ToUpper() == "PLAYER_LIST")
                {
                  try
                  {
                    string data = "\n";
                    this.NetworkGameplayData = this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.GameplayData, false);
                    foreach (NetworkConnection connection in NetworkServer.connections)
                    {
                      GameObject connectedRoot = GameConsole.Console.FindConnectedRoot(connection);
                      if ((Object) connectedRoot != (Object) null)
                      {
                        if (!q.ToUpper().Contains("STAFF"))
                        {
                          string str4 = string.Empty;
                          try
                          {
                            str4 = !connectedRoot.GetComponent<ServerRoles>().RaEverywhere ? (!connectedRoot.GetComponent<ServerRoles>().Staff ? (!connectedRoot.GetComponent<ServerRoles>().RemoteAdmin ? string.Empty : "[RA] ") : "[@] ") : "[~] ";
                          }
                          catch
                          {
                          }
                          data = data + str4 + "(" + (object) connectedRoot.GetComponent<QueryProcessor>().PlayerId + ") " + connectedRoot.GetComponent<NicknameSync>().myNick.Replace("\n", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty) + (!connectedRoot.GetComponent<ServerRoles>().OverwatchEnabled ? (object) string.Empty : (object) "<OVRM>");
                        }
                        else
                          data = data + (object) connectedRoot.GetComponent<QueryProcessor>().PlayerId + ";" + connectedRoot.GetComponent<NicknameSync>().myNick;
                      }
                      data += "\n";
                    }
                    if (!q.ToUpper().Contains("STAFF"))
                    {
                      this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + ":PLAYER_LIST#" + data, true, strArray.Length < 3 || strArray[2].ToUpper() != "SILENT", string.Empty);
                      return;
                    }
                    this.CallTargetStaffPlayerListResponse(this.connectionToClient, data);
                    return;
                  }
                  catch (Exception ex)
                  {
                    this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + ":PLAYER_LIST#An unexpected problem has occurred!\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace + "\nAt: " + ex.Source, false, true, string.Empty);
                    throw;
                  }
                }
                else if (strArray[1].ToUpper() == "PLAYER")
                {
                  if (strArray.Length >= 3)
                  {
                    try
                    {
                      GameObject gameObject = (GameObject) null;
                      NetworkConnection networkConnection = (NetworkConnection) null;
                      foreach (NetworkConnection connection in NetworkServer.connections)
                      {
                        GameObject connectedRoot = GameConsole.Console.FindConnectedRoot(connection);
                        if (strArray[2].Contains("."))
                          strArray[2] = strArray[2].Split('.')[0];
                        if ((Object) connectedRoot != (Object) null && connectedRoot.GetComponent<QueryProcessor>().PlayerId.ToString() == strArray[2])
                        {
                          gameObject = connectedRoot;
                          networkConnection = connection;
                        }
                      }
                      if ((Object) gameObject == (Object) null)
                      {
                        this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + ":PLAYER#Player with id " + (!string.IsNullOrEmpty(strArray[2]) ? strArray[2] : "[null]") + " not found!", false, true, string.Empty);
                        return;
                      }
                      bool flag = this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.GameplayData, false);
                      CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
                      string str4 = string.Empty + "Nickname: " + gameObject.GetComponent<NicknameSync>().myNick + "\nPlayer ID: " + (object) gameObject.GetComponent<QueryProcessor>().PlayerId + "\nIP: " + (networkConnection == null ? "null" : networkConnection.address) + "\nSteam ID: " + (!string.IsNullOrEmpty(component.SteamId) ? component.SteamId : "(none)") + "\nServer role: " + gameObject.GetComponent<ServerRoles>().GetColoredRoleString(false);
                      if (!string.IsNullOrEmpty(gameObject.GetComponent<ServerRoles>().HiddenBadge))
                        str4 = str4 + "\n<color=#DC143C>Hidden role: </color>" + gameObject.GetComponent<ServerRoles>().HiddenBadge;
                      if (gameObject.GetComponent<ServerRoles>().RaEverywhere)
                        str4 += "\nActive flag: <color=#BCC6CC>Studio GLOBAL Staff (management or security team)</color>";
                      else if (gameObject.GetComponent<ServerRoles>().Staff)
                        str4 += "\nActive flag: Studio Staff";
                      if (gameObject.GetComponent<CharacterClassManager>().GodMode)
                        str4 += "\nActive flag: <color=#659EC7>GOD MODE</color>";
                      if (gameObject.GetComponent<ServerRoles>().BypassMode)
                        str4 += "\nActive flag: <color=#BFFF00>BYPASS MODE</color>";
                      if (gameObject.GetComponent<ServerRoles>().RemoteAdmin)
                        str4 += "\nActive flag: <color=#43C6DB>REMOTE ADMIN AUTHENTICATED</color>";
                      string str5;
                      if (gameObject.GetComponent<ServerRoles>().OverwatchEnabled)
                      {
                        str5 = str4 + "\nActive flag: <color=#008080>OVERWATCH MODE</color>";
                      }
                      else
                      {
                        str5 = str4 + "\nClass: " + (!flag ? "<color=#D4AF37>INSUFFICIENT PERMISSIONS</color>" : (component.curClass < 0 || component.curClass >= component.klasy.Length ? "None" : component.klasy[component.curClass].fullName)) + "\nHP: " + (!flag ? "<color=#D4AF37>INSUFFICIENT PERMISSIONS</color>" : gameObject.GetComponent<PlayerStats>().HealthToString());
                        if (!flag)
                          str5 += "\n<color=#D4AF37>* GameplayData permission required</color>";
                      }
                      this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + ":PLAYER#" + str5, true, true, string.Empty);
                      return;
                    }
                    catch (Exception ex)
                    {
                      this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#An unexpected problem has occurred!\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace + "\nAt: " + ex.Source, false, true, string.Empty);
                      throw;
                    }
                  }
                  else
                  {
                    this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + ":PLAYER#Please specify the PlayerId!", false, true, string.Empty);
                    return;
                  }
                }
                else if (strArray[1].ToUpper() == "AUTH")
                {
                  if (!this.GetComponent<ServerRoles>().Staff && !this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.LongTermBanning, true))
                    return;
                  if (strArray.Length >= 3)
                  {
                    try
                    {
                      GameObject gameObject = (GameObject) null;
                      foreach (NetworkConnection connection in NetworkServer.connections)
                      {
                        GameObject connectedRoot = GameConsole.Console.FindConnectedRoot(connection);
                        if (strArray[2].Contains("."))
                          strArray[2] = strArray[2].Split('.')[0];
                        if ((Object) connectedRoot != (Object) null && connectedRoot.GetComponent<QueryProcessor>().PlayerId.ToString() == strArray[2])
                          gameObject = connectedRoot;
                      }
                      if ((Object) gameObject == (Object) null)
                      {
                        this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + ":PLAYER#Player with id " + (!string.IsNullOrEmpty(strArray[2]) ? strArray[2] : "[null]") + " not found!", false, true, string.Empty);
                        return;
                      }
                      if (!q.ToUpper().Contains("STAFF"))
                      {
                        string str4 = "Authentication token of player " + gameObject.GetComponent<NicknameSync>().myNick + "(" + (object) gameObject.GetComponent<QueryProcessor>().PlayerId + "):\n" + gameObject.GetComponent<CharacterClassManager>().AuthToken;
                        this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + ":PLAYER#" + str4, true, true, string.Empty);
                        return;
                      }
                      this.CallTargetStaffAuthTokenResponse(this.connectionToClient, gameObject.GetComponent<CharacterClassManager>().AuthToken);
                      return;
                    }
                    catch (Exception ex)
                    {
                      this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#An unexpected problem has occurred!\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace + "\nAt: " + ex.Source, false, true, string.Empty);
                      throw;
                    }
                  }
                  else
                  {
                    this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + ":PLAYER#Please specify the PlayerId!", false, true, string.Empty);
                    return;
                  }
                }
                else
                {
                  this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Unknown parameter, type HELP to open the documentation.", false, true, string.Empty);
                  return;
                }
              }
              else
              {
                this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#To run this program, type at least 2 arguments! (some parameters are missing)", false, true, string.Empty);
                return;
              }
            case 21:
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Contact email address: " + ConfigFile.ServerConfig.GetString("contact_email", string.Empty), false, true, string.Empty);
              return;
            case 22:
              if (this._roles.RemoteAdminMode == ServerRoles.AccessMode.PasswordOverride)
              {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, nick + " logged out from the Remote Admin.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
                this._roles.NetworkRemoteAdmin = false;
                if (!this._roles.GlobalSet)
                {
                  this._roles.SetText(string.Empty);
                  this._roles.SetColor("default");
                }
                this.PasswordTries = 0;
                this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Logged out!", true, true, string.Empty);
                return;
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#You can't log out, when you are not using override password!", true, true, string.Empty);
              return;
            case 23:
              if (strArray.Length >= 2)
              {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, nick + " forced a server event: " + strArray[1].ToUpper(), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                GameObject gameObject1 = GameObject.Find("Host");
                bool flag = true;
                string upper2 = strArray[1].ToUpper();
                if (upper2 != null)
                {
                  // ISSUE: reference to a compiler-generated field
                  if (QueryProcessor.\u003C\u003Ef__switch\u0024map1 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    QueryProcessor.\u003C\u003Ef__switch\u0024map1 = new Dictionary<string, int>(7)
                    {
                      {
                        "FORCE_CI_RESPAWN",
                        0
                      },
                      {
                        "FORCE_MTF_RESPAWN",
                        1
                      },
                      {
                        "DETONATION_START",
                        2
                      },
                      {
                        "DETONATION_CANCEL",
                        3
                      },
                      {
                        "DETONATION_INSTANT",
                        4
                      },
                      {
                        "TERMINATE_UNCONN",
                        5
                      },
                      {
                        "ROUND_RESTART",
                        6
                      }
                    };
                  }
                  // ISSUE: reference to a compiler-generated field
                  if (QueryProcessor.\u003C\u003Ef__switch\u0024map1.TryGetValue(upper2, out num))
                  {
                    switch (num)
                    {
                      case 0:
                        if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.RespawnEvents, true))
                          return;
                        gameObject1.GetComponent<MTFRespawn>().nextWaveIsCI = true;
                        gameObject1.GetComponent<MTFRespawn>().timeToNextRespawn = 0.1f;
                        goto label_301;
                      case 1:
                        if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.RespawnEvents, true))
                          return;
                        gameObject1.GetComponent<MTFRespawn>().nextWaveIsCI = false;
                        gameObject1.GetComponent<MTFRespawn>().timeToNextRespawn = 0.1f;
                        goto label_301;
                      case 2:
                        if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.WarheadEvents, true))
                          return;
                        gameObject1.GetComponent<AlphaWarheadController>().InstantPrepare();
                        gameObject1.GetComponent<AlphaWarheadController>().StartDetonation();
                        goto label_301;
                      case 3:
                        if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.WarheadEvents, true))
                          return;
                        gameObject1.GetComponent<AlphaWarheadController>().CancelDetonation((GameObject) null);
                        goto label_301;
                      case 4:
                        if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.WarheadEvents, true))
                          return;
                        gameObject1.GetComponent<AlphaWarheadController>().InstantPrepare();
                        gameObject1.GetComponent<AlphaWarheadController>().StartDetonation();
                        gameObject1.GetComponent<AlphaWarheadController>().NetworktimeToDetonation = 5f;
                        goto label_301;
                      case 5:
                        if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.RoundEvents, true))
                          return;
                        using (IEnumerator<NetworkConnection> enumerator = NetworkServer.connections.GetEnumerator())
                        {
                          while (enumerator.MoveNext())
                          {
                            NetworkConnection current = enumerator.Current;
                            if ((Object) GameConsole.Console.FindConnectedRoot(current) == (Object) null)
                            {
                              current.Disconnect();
                              current.Dispose();
                            }
                          }
                          goto label_301;
                        }
                      case 6:
                        if (!this.CheckPermissions(strArray[0].ToUpper(), PlayerPermissions.RoundEvents, true))
                          return;
                        foreach (GameObject gameObject2 in GameObject.FindGameObjectsWithTag("Player"))
                        {
                          PlayerStats component = gameObject2.GetComponent<PlayerStats>();
                          if (component.isLocalPlayer && component.isServer)
                            component.Roundrestart();
                        }
                        goto label_301;
                    }
                  }
                }
                flag = false;
label_301:
                if (flag)
                {
                  this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Started event: " + strArray[1].ToUpper(), true, true, "ServerEvents");
                  return;
                }
                this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Incorrect event! (Doesn't exist)", false, true, "ServerEvents");
                return;
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#To run this program, type at least 2 arguments! (some parameters are missing)", false, true, string.Empty);
              return;
            case 24:
              this._roles.HiddenBadge = this._roles.MyText;
              this._roles.SetBadgeUpdate(string.Empty);
              this._roles.SetText(string.Empty);
              this._roles.SetColor("default");
              this._roles.NetworkGlobalSet = false;
              this._roles.RefreshHiddenTag();
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Tag hidden!", true, true, string.Empty);
              this.PasswordTries = 0;
              return;
            case 25:
              this._roles.HiddenBadge = string.Empty;
              this._roles.CallRpcResetFixed();
              this._roles.RefreshPermissions();
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Local tag refreshed!", true, true, string.Empty);
              return;
            case 26:
              if (string.IsNullOrEmpty(this._roles.PrevBadge))
              {
                this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#You don't have global tag.", false, true, string.Empty);
                return;
              }
              this._roles.HiddenBadge = string.Empty;
              this._roles.CallRpcResetFixed();
              this._roles.SetBadgeUpdate(this._roles.PrevBadge);
              this._roles.NetworkGlobalSet = true;
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Global tag refreshed!", true, true, string.Empty);
              return;
            case 27:
              YamlConfig serverConfig = ConfigFile.ServerConfig;
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#Server name: " + serverConfig.GetString("server_name", string.Empty) + "\nServer IP: " + serverConfig.GetString("server_ip", string.Empty) + "\nCurrent Server IP:: " + CustomNetworkManager.Ip + "\nServer pastebin ID: " + serverConfig.GetString("serverinfo_pastebin_id", string.Empty) + "\nServer max players: " + (object) serverConfig.GetInt("max_players", 0) + "\nOnline mode: " + (object) serverConfig.GetBool("online_mode", false) + "\nIP banning: " + (object) serverConfig.GetBool("ip_banning", false) + "\nWhitelist: " + (object) serverConfig.GetBool("enable_whitelist", false) + "\nQuery status: " + (object) serverConfig.GetBool("enable_query", false) + " with port shift " + (object) serverConfig.GetInt("query_port_shift", 0) + "\nFriendly fire: " + (object) serverConfig.GetBool("friendly_fire", false) + "\nMap seed: " + (object) serverConfig.GetInt("map_seed", 0), true, true, string.Empty);
              return;
            case 28:
              ulong permissions = this.GetComponent<ServerRoles>().Permissions;
              string str6 = "Your permissions:";
              foreach (string allPermission in ServerStatic.PermissionsHandler.GetAllPermissions())
              {
                string str4 = !ServerStatic.PermissionsHandler.IsRaPermitted(ServerStatic.PermissionsHandler.GetPermissionValue(allPermission)) ? string.Empty : "*";
                str6 = str6 + "\n" + allPermission + str4 + " (" + (object) ServerStatic.PermissionsHandler.GetPermissionValue(allPermission) + "): " + (!ServerStatic.PermissionsHandler.IsPermitted(permissions, allPermission) ? (object) "NO" : (object) "YES");
              }
              this.CallTargetReply(this.connectionToClient, strArray[0].ToUpper() + "#" + str6, true, true, string.Empty);
              return;
          }
        }
      }
      this.CallTargetReply(this.connectionToClient, "SYSTEM#Unknown command!", false, true, string.Empty);
    }

    private void ProcessDoorQuery(string command, string door)
    {
      if (!this.CheckPermissions(command.ToUpper(), PlayerPermissions.FacilityManagement, true))
        return;
      if (string.IsNullOrEmpty(door))
      {
        this.CallTargetReply(this.connectionToClient, command + "#Please select door first.", false, true, "DoorsManagement");
      }
      else
      {
        bool flag = false;
        door = door.ToUpper();
        int num1;
        if (command != null)
        {
          if (!(command == "OPEN"))
          {
            if (!(command == "LOCK"))
            {
              if (!(command == "UNLOCK"))
              {
                if (command == "DESTROY")
                {
                  num1 = 4;
                  goto label_14;
                }
              }
              else
              {
                num1 = 3;
                goto label_14;
              }
            }
            else
            {
              num1 = 2;
              goto label_14;
            }
          }
          else
          {
            num1 = 1;
            goto label_14;
          }
        }
        num1 = 0;
label_14:
        foreach (Door door1 in Object.FindObjectsOfType<Door>())
        {
          if (!(door1.DoorName.ToUpper() != door) || !(door != "**") && !(door1.permissionLevel == "UNACCESSIBLE") || !(door != "!*") && string.IsNullOrEmpty(door1.DoorName) || !(door != "*") && !string.IsNullOrEmpty(door1.DoorName) && !(door1.permissionLevel == "UNACCESSIBLE"))
          {
            switch (num1)
            {
              case 0:
                door1.SetStateWithSound(false);
                break;
              case 1:
                door1.SetStateWithSound(true);
                break;
              case 2:
                door1.commandlock = true;
                door1.UpdateLock();
                break;
              case 3:
                door1.commandlock = false;
                door1.UpdateLock();
                break;
              case 4:
                door1.DestroyDoor(true);
                break;
            }
            flag = true;
          }
        }
        NetworkConnection connectionToClient = this.connectionToClient;
        string str1 = command;
        string str2 = "#";
        string str3;
        if (flag)
          str3 = "Door " + door + " " + command.ToLower() + "ed.";
        else
          str3 = "Can't find door " + door + ".";
        string content = str1 + str2 + str3;
        int num2 = flag ? 1 : 0;
        int num3 = 1;
        string overrideDisplay = "DoorsManagement";
        this.CallTargetReply(connectionToClient, content, num2 != 0, num3 != 0, overrideDisplay);
        if (!flag)
          return;
        ServerLogs.AddLog(ServerLogs.Modules.Administrative, this.GetComponent<NicknameSync>().myNick + " " + command.ToLower() + "ed door " + door + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
      }
    }

    private void StandardizedQueryModel1(string programName, string playerIds, string xValue, out int failures, out int successes, out string error, out bool replySent)
    {
      error = string.Empty;
      failures = 0;
      successes = 0;
      replySent = false;
      programName = programName.ToUpper();
      int result;
      if (int.TryParse(xValue, out result) || programName == "SETGROUP" || (programName == "OVERWATCH" || programName == "BYPASS") || (programName == "HEAL" || programName == "GOD"))
      {
        List<int> intList1 = new List<int>();
        try
        {
          string[] strArray = playerIds.Split('.');
          List<int> intList2 = intList1;
          IEnumerable<string> source = ((IEnumerable<string>) strArray).Where<string>((Func<string, bool>) (item => !string.IsNullOrEmpty(item)));
          // ISSUE: reference to a compiler-generated field
          if (QueryProcessor.\u003C\u003Ef__mg\u0024cache0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            QueryProcessor.\u003C\u003Ef__mg\u0024cache0 = new Func<string, int>(int.Parse);
          }
          // ISSUE: reference to a compiler-generated field
          Func<string, int> fMgCache0 = QueryProcessor.\u003C\u003Ef__mg\u0024cache0;
          IEnumerable<int> collection = source.Select<string, int>(fMgCache0);
          intList2.AddRange(collection);
          UserGroup group = (UserGroup) null;
          if (programName == "BAN")
          {
            replySent = true;
            if (result <= 60 && !this.CheckPermissions(programName, PlayerPermissions.KickingAndShortTermBanning, true) || result > 60 && result <= 1440 && !this.CheckPermissions(programName, PlayerPermissions.BanningUpToDay, true) || result > 1440 && !this.CheckPermissions(programName, PlayerPermissions.LongTermBanning, true))
              return;
            replySent = false;
          }
          else if (programName == "SETGROUP")
          {
            group = ServerStatic.PermissionsHandler.GetGroup(xValue);
            if (group == null)
            {
              replySent = true;
              this.CallTargetReply(this.connectionToClient, programName + "#Requested group doesn't exist!", false, true, string.Empty);
              return;
            }
          }
          bool isVerified = ServerStatic.PermissionsHandler.IsVerified;
          string nick = this.GetComponent<NicknameSync>().myNick;
          foreach (int num1 in intList1)
          {
            try
            {
              foreach (GameObject player in PlayerManager.singleton.players)
              {
                if (num1 == player.GetComponent<QueryProcessor>().PlayerId)
                {
                  if (programName != null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    if (QueryProcessor.\u003C\u003Ef__switch\u0024map3 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      QueryProcessor.\u003C\u003Ef__switch\u0024map3 = new Dictionary<string, int>(10)
                      {
                        {
                          "BAN",
                          0
                        },
                        {
                          "FORCECLASS",
                          1
                        },
                        {
                          "GIVE",
                          2
                        },
                        {
                          "SETGROUP",
                          3
                        },
                        {
                          "HEAL",
                          4
                        },
                        {
                          "HP",
                          5
                        },
                        {
                          "SETHP",
                          5
                        },
                        {
                          "OVERWATCH",
                          6
                        },
                        {
                          "GOD",
                          7
                        },
                        {
                          "BYPASS",
                          8
                        }
                      };
                    }
                    int num2;
                    // ISSUE: reference to a compiler-generated field
                    if (QueryProcessor.\u003C\u003Ef__switch\u0024map3.TryGetValue(programName, out num2))
                    {
                      switch (num2)
                      {
                        case 0:
                          if (isVerified && result > 0 && player.GetComponent<ServerRoles>().BypassStaff)
                          {
                            failures = -2;
                            continue;
                          }
                          error = "Error Code: " + this.GetComponent<BanPlayer>().BanUser(player, result, string.Empty, nick);
                          if (error != "Error Code: good" && failures != -2)
                          {
                            ++failures;
                            break;
                          }
                          break;
                        case 1:
                          this.GetComponent<CharacterClassManager>().SetPlayersClass(result, player);
                          break;
                        case 2:
                          player.GetComponent<Inventory>().AddNewItem(result, -4.656647E+11f);
                          break;
                        case 3:
                          player.GetComponent<ServerRoles>().SetGroup(group, false, true);
                          break;
                        case 4:
                          PlayerStats component = player.GetComponent<PlayerStats>();
                          component.SetHPAmount(component.ccm.klasy[component.ccm.curClass].maxHP);
                          break;
                        case 5:
                          player.GetComponent<PlayerStats>().SetHPAmount(result);
                          break;
                        case 6:
                          if (string.IsNullOrEmpty(xValue))
                          {
                            player.GetComponent<ServerRoles>().CallCmdToggleOverwatch();
                            break;
                          }
                          if (xValue == "1" || xValue.ToLower() == "true" || (xValue.ToLower() == "enable" || xValue.ToLower() == "on"))
                          {
                            player.GetComponent<ServerRoles>().CallCmdSetOverwatchStatus(true);
                            break;
                          }
                          if (xValue == "0" || xValue.ToLower() == "false" || (xValue.ToLower() == "disable" || xValue.ToLower() == "off"))
                          {
                            player.GetComponent<ServerRoles>().CallCmdSetOverwatchStatus(false);
                            break;
                          }
                          replySent = true;
                          this.CallTargetReply(this.connectionToClient, programName + "#Invalid option " + xValue + " - leave null for toggle or use 1/0, true/false, enable/disable or on/off.", false, true, "AdminTools");
                          return;
                        case 7:
                          if (string.IsNullOrEmpty(xValue))
                          {
                            player.GetComponent<CharacterClassManager>().GodMode = !player.GetComponent<CharacterClassManager>().GodMode;
                            break;
                          }
                          if (xValue == "1" || xValue.ToLower() == "true" || (xValue.ToLower() == "enable" || xValue.ToLower() == "on"))
                          {
                            player.GetComponent<CharacterClassManager>().GodMode = true;
                            break;
                          }
                          if (xValue == "0" || xValue.ToLower() == "false" || (xValue.ToLower() == "disable" || xValue.ToLower() == "off"))
                          {
                            player.GetComponent<CharacterClassManager>().GodMode = false;
                            break;
                          }
                          replySent = true;
                          this.CallTargetReply(this.connectionToClient, programName + "#Invalid option " + xValue + " - leave null for toggle or use 1/0, true/false, enable/disable or on/off.", false, true, "AdminTools");
                          return;
                        case 8:
                          if (string.IsNullOrEmpty(xValue))
                            player.GetComponent<ServerRoles>().BypassMode = !player.GetComponent<ServerRoles>().BypassMode;
                          else if (xValue == "1" || xValue.ToLower() == "true" || (xValue.ToLower() == "enable" || xValue.ToLower() == "on"))
                            player.GetComponent<ServerRoles>().BypassMode = true;
                          else if (xValue == "0" || xValue.ToLower() == "false" || (xValue.ToLower() == "disable" || xValue.ToLower() == "off"))
                          {
                            player.GetComponent<ServerRoles>().BypassMode = false;
                          }
                          else
                          {
                            replySent = true;
                            this.CallTargetReply(this.connectionToClient, programName + "#Invalid option " + xValue + " - leave null for toggle or use 1/0, true/false, enable/disable or on/off.", false, true, "AdminTools");
                            return;
                          }
                          if (player.GetComponent<ServerRoles>().BypassMode)
                          {
                            player.GetComponent<Intercom>().remainingCooldown = 0.0f;
                            break;
                          }
                          break;
                      }
                    }
                  }
                  ++successes;
                }
              }
            }
            catch (Exception ex)
            {
              ++failures;
              error = ex.Message + "\nStackTrace:\n" + ex.StackTrace;
            }
          }
        }
        catch (Exception ex)
        {
          replySent = true;
          this.CallTargetReply(this.connectionToClient, programName + "#An unexpected problem has occurred!\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace + "\nAt: " + ex.Source + "\nMost likely the PlayerId array was not in the correct format.", false, true, string.Empty);
          throw;
        }
      }
      else
      {
        replySent = true;
        this.CallTargetReply(this.connectionToClient, programName + "#The third parameter has to be an integer!", false, true, string.Empty);
      }
    }

    internal bool CheckPermissions(string queryZero, PlayerPermissions perm, bool reply = true)
    {
      if (ServerStatic.PermissionsHandler.IsPermitted(this.GetComponent<ServerRoles>().Permissions, perm))
        return true;
      if (reply)
        this.CallTargetReply(this.connectionToClient, queryZero + "#You don't have permissions to execute this command.\nMissing permission: " + (object) perm, false, true, string.Empty);
      return false;
    }

    public bool VerifyRequestSignature(string message, int counter, byte[] signature, bool validateCounter = true)
    {
      if (this.GetComponent<ServerRoles>().RemoteAdminMode == ServerRoles.AccessMode.PasswordOverride)
        return this.VerifyHmacSignature(message, counter, signature, validateCounter);
      return this.VerifyEcdsaSignature(message, counter, signature, validateCounter);
    }

    public byte[] SignRequest(string message, int counter = -2)
    {
      if (this.GetComponent<ServerRoles>().RemoteAdminMode == ServerRoles.AccessMode.PasswordOverride)
        return this.HmacSign(message, counter);
      return this.EcdsaSign(message, counter);
    }

    public bool VerifyHmacSignature(string message, int counter, byte[] signature, bool validateCounter = true)
    {
      if (counter <= this._signaturesCounter)
      {
        if (validateCounter)
          return false;
      }
      else
        this._signaturesCounter = counter;
      if (this.OverridePasswordEnabled)
        return ((IEnumerable<byte>) Sha.Sha512Hmac(Utf8.GetBytes(message + ":[:COUNTER:]:" + (object) counter), this._key)).SequenceEqual<byte>((IEnumerable<byte>) signature);
      return false;
    }

    public bool VerifyEcdsaSignature(string message, int counter, byte[] signature, bool validateCounter = true)
    {
      if (counter <= this._signaturesCounter)
      {
        if (validateCounter)
          return false;
      }
      else
        this._signaturesCounter = counter;
      return ECDSA.VerifyBytes(message + ":[:COUNTER:]:" + (object) counter, signature, this.GetComponent<ServerRoles>().PublicKey);
    }

    public byte[] EcdsaSign(string message, int counter = -2)
    {
      if (counter == -2)
        counter = this.SignaturesCounter;
      return ECDSA.SignBytes(message + ":[:COUNTER:]:" + (object) counter, GameConsole.Console.SessionKeys.Private);
    }

    public byte[] HmacSign(string message, int counter = -2)
    {
      if (counter == -2)
        counter = this.SignaturesCounter;
      return Sha.Sha512Hmac(Utf8.GetBytes(message + ":[:COUNTER:]:" + (object) counter), this.Key);
    }

    public static byte[] DerivePassword(string password, byte[] serversalt, byte[] clientsalt)
    {
      byte[] salt = Sha.Sha512(Convert.ToBase64String(serversalt) + Convert.ToBase64String(clientsalt));
      return PBKDF2.Pbkdf2HashBytes(password, salt, 250, 512);
    }

    internal void RequestGlobalBan(string key, int keytype)
    {
      this._toBan = key;
      this._toBanType = keytype;
      this.CmdSendQuery("REQUEST_DATA PLAYER_LIST STAFF");
    }

    [TargetRpc(channel = 2)]
    internal void TargetStaffPlayerListResponse(NetworkConnection conn, string data)
    {
      if (string.IsNullOrEmpty(this._toBan) || !string.IsNullOrEmpty(this._toBanNick))
        return;
      string[] strArray = data.Split('\n');
      string str1 = "-1";
      string str2 = string.Empty;
      foreach (string str3 in strArray)
      {
        try
        {
          int length = str3.IndexOf(";", StringComparison.Ordinal);
          if (length != -1)
          {
            string str4 = str3.Substring(0, length);
            string a = str3.Substring(length + 1);
            if (this._toBanType == 0 && str4 == this._toBan)
            {
              str1 = str4;
              str2 = a;
              break;
            }
            if (this._toBanType == 1)
            {
              if (string.Equals(a, this._toBan, StringComparison.CurrentCultureIgnoreCase))
              {
                str1 = str4;
                str2 = a;
                break;
              }
            }
          }
        }
        catch (Exception ex)
        {
          GameConsole.Console.singleton.AddLog("Error while processing online list for global banning: " + ex.GetType().FullName, (Color32) Color.red, false);
        }
      }
      if (str1 == "-1")
      {
        GameConsole.Console.singleton.AddLog("Requested player can't be found!", (Color32) Color.red, false);
      }
      else
      {
        GameConsole.Console.singleton.AddLog("Requesting authentication token of player " + str2 + "(" + str1 + ").", (Color32) Color.cyan, false);
        this._toBan = str1;
        this._toBanNick = str2;
        this.CmdSendQuery("REQUEST_DATA AUTH " + str1 + " STAFF");
      }
      this._toBanType = 0;
    }

    [TargetRpc(channel = 2)]
    internal void TargetStaffAuthTokenResponse(NetworkConnection conn, string auth)
    {
      if (string.IsNullOrEmpty(this._toBan) || string.IsNullOrEmpty(this._toBanNick))
        return;
      string str = CentralAuth.ValidateForGlobalBanning(auth, this._toBanNick);
      if (str == "-1")
      {
        GameConsole.Console.singleton.AddLog("Aborting global banning....", (Color32) Color.red, false);
        this._toBan = string.Empty;
        this._toBanNick = string.Empty;
        this._toBanSteamId = string.Empty;
        this._toBanType = 0;
      }
      else
      {
        this._toBanSteamId = str;
        GameConsole.Console.singleton.AddLog("==== GLOBAL BANNING FINAL STEP ====", (Color32) Color.cyan, false);
        GameConsole.Console.singleton.AddLog("Nick: " + this._toBanNick, (Color32) Color.cyan, false);
        GameConsole.Console.singleton.AddLog("ID on this server: " + this._toBan, (Color32) Color.cyan, false);
        GameConsole.Console.singleton.AddLog("SteamID64: " + this._toBanSteamId, (Color32) Color.cyan, false);
        GameConsole.Console.singleton.AddLog(string.Empty, (Color32) Color.cyan, false);
        GameConsole.Console.singleton.AddLog("To confirm ban please execute \"CONFIRM\" command.", (Color32) Color.cyan, false);
        GameConsole.Console.singleton.AddLog("==== GLOBAL BANNING FINAL STEP ====", (Color32) Color.cyan, false);
        this._toBanNick = string.Empty;
      }
    }

    internal void ConfirmGlobalBanning()
    {
      this.StartCoroutine(this.IssueGlobalBan());
    }

    [DebuggerHidden]
    private IEnumerator IssueGlobalBan()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new QueryProcessor.\u003CIssueGlobalBan\u003Ec__Iterator0() { \u0024this = this };
    }

    private void OnDestroy()
    {
      if (!NetworkServer.active)
        return;
      CustomNetworkManager.PlayerDisconnect(this.conns);
      if (!((Object) ServerLogs.singleton != (Object) null))
        return;
      ServerLogs.AddLog(ServerLogs.Modules.Networking, "Player ID " + this.PlayerId.ToString() + " disconnected from IP " + this.ipAddress + " with SteamID " + (!string.IsNullOrEmpty(this.GetComponent<CharacterClassManager>().SteamId) ? this.GetComponent<CharacterClassManager>().SteamId : "(unavailable)") + " and nickname " + this.GetComponent<NicknameSync>().myNick + ". His last class was " + this.GetComponent<CharacterClassManager>().curClass.ToString(), ServerLogs.ServerLogType.ConnectionUpdate);
    }

    private void UNetVersion()
    {
    }

    public int NetworkPlayerId
    {
      get
      {
        return this.PlayerId;
      }
      [param: In] set
      {
        int num1 = value;
        ref int local = ref this.PlayerId;
        int num2 = 1;
        if (NetworkServer.localClientActive && !this.syncVarHookGuard)
        {
          this.syncVarHookGuard = true;
          this.SetId(value);
          this.syncVarHookGuard = false;
        }
        this.SetSyncVar<int>(num1, ref local, (uint) num2);
      }
    }

    public bool NetworkOverridePasswordEnabled
    {
      get
      {
        return this.OverridePasswordEnabled;
      }
      [param: In] set
      {
        int num1 = value ? 1 : 0;
        ref bool local = ref this.OverridePasswordEnabled;
        int num2 = 2;
        if (NetworkServer.localClientActive && !this.syncVarHookGuard)
        {
          this.syncVarHookGuard = true;
          this.SetOverridePasswordEnabled(value);
          this.syncVarHookGuard = false;
        }
        this.SetSyncVar<bool>(num1 != 0, ref local, (uint) num2);
      }
    }

    public bool NetworkGameplayData
    {
      get
      {
        return this.GameplayData;
      }
      [param: In] set
      {
        this.SetSyncVar<bool>(value, ref this.GameplayData, 4U);
      }
    }

    protected static void InvokeCmdCmdRequestSalt(NetworkBehaviour obj, NetworkReader reader)
    {
      if (!NetworkServer.active)
        Debug.LogError((object) "Command CmdRequestSalt called on client.");
      else
        ((QueryProcessor) obj).CmdRequestSalt(reader.ReadBytesAndSize());
    }

    protected static void InvokeCmdCmdSendPassword(NetworkBehaviour obj, NetworkReader reader)
    {
      if (!NetworkServer.active)
        Debug.LogError((object) "Command CmdSendPassword called on client.");
      else
        ((QueryProcessor) obj).CmdSendPassword(reader.ReadBytesAndSize());
    }

    protected static void InvokeCmdCmdSendQuery(NetworkBehaviour obj, NetworkReader reader)
    {
      if (!NetworkServer.active)
        Debug.LogError((object) "Command CmdSendQuery called on client.");
      else
        ((QueryProcessor) obj).CmdSendQuery(reader.ReadString(), (int) reader.ReadPackedUInt32(), reader.ReadBytesAndSize());
    }

    public void CallCmdRequestSalt(byte[] clSalt)
    {
      if (!NetworkClient.active)
        Debug.LogError((object) "Command function CmdRequestSalt called on server.");
      else if (this.isServer)
      {
        this.CmdRequestSalt(clSalt);
      }
      else
      {
        NetworkWriter writer = new NetworkWriter();
        writer.Write((short) 0);
        writer.Write((short) 5);
        writer.WritePackedUInt32((uint) QueryProcessor.kCmdCmdRequestSalt);
        writer.Write(this.GetComponent<NetworkIdentity>().netId);
        writer.WriteBytesFull(clSalt);
        this.SendCommandInternal(writer, 2, "CmdRequestSalt");
      }
    }

    public void CallCmdSendPassword(byte[] authSignature)
    {
      if (!NetworkClient.active)
        Debug.LogError((object) "Command function CmdSendPassword called on server.");
      else if (this.isServer)
      {
        this.CmdSendPassword(authSignature);
      }
      else
      {
        NetworkWriter writer = new NetworkWriter();
        writer.Write((short) 0);
        writer.Write((short) 5);
        writer.WritePackedUInt32((uint) QueryProcessor.kCmdCmdSendPassword);
        writer.Write(this.GetComponent<NetworkIdentity>().netId);
        writer.WriteBytesFull(authSignature);
        this.SendCommandInternal(writer, 15, "CmdSendPassword");
      }
    }

    public void CallCmdSendQuery(string query, int counter, byte[] signature)
    {
      if (!NetworkClient.active)
        Debug.LogError((object) "Command function CmdSendQuery called on server.");
      else if (this.isServer)
      {
        this.CmdSendQuery(query, counter, signature);
      }
      else
      {
        NetworkWriter writer = new NetworkWriter();
        writer.Write((short) 0);
        writer.Write((short) 5);
        writer.WritePackedUInt32((uint) QueryProcessor.kCmdCmdSendQuery);
        writer.Write(this.GetComponent<NetworkIdentity>().netId);
        writer.Write(query);
        writer.WritePackedUInt32((uint) counter);
        writer.WriteBytesFull(signature);
        this.SendCommandInternal(writer, 15, "CmdSendQuery");
      }
    }

    protected static void InvokeRpcTargetSaltGenerated(NetworkBehaviour obj, NetworkReader reader)
    {
      if (!NetworkClient.active)
        Debug.LogError((object) "TargetRPC TargetSaltGenerated called on server.");
      else
        ((QueryProcessor) obj).TargetSaltGenerated(ClientScene.readyConnection, reader.ReadBytesAndSize());
    }

    protected static void InvokeRpcTargetReplyPassword(NetworkBehaviour obj, NetworkReader reader)
    {
      if (!NetworkClient.active)
        Debug.LogError((object) "TargetRPC TargetReplyPassword called on server.");
      else
        ((QueryProcessor) obj).TargetReplyPassword(ClientScene.readyConnection, reader.ReadBoolean());
    }

    protected static void InvokeRpcTargetReply(NetworkBehaviour obj, NetworkReader reader)
    {
      if (!NetworkClient.active)
        Debug.LogError((object) "TargetRPC TargetReply called on server.");
      else
        ((QueryProcessor) obj).TargetReply(ClientScene.readyConnection, reader.ReadString(), reader.ReadBoolean(), reader.ReadBoolean(), reader.ReadString());
    }

    protected static void InvokeRpcTargetStaffPlayerListResponse(NetworkBehaviour obj, NetworkReader reader)
    {
      if (!NetworkClient.active)
        Debug.LogError((object) "TargetRPC TargetStaffPlayerListResponse called on server.");
      else
        ((QueryProcessor) obj).TargetStaffPlayerListResponse(ClientScene.readyConnection, reader.ReadString());
    }

    protected static void InvokeRpcTargetStaffAuthTokenResponse(NetworkBehaviour obj, NetworkReader reader)
    {
      if (!NetworkClient.active)
        Debug.LogError((object) "TargetRPC TargetStaffAuthTokenResponse called on server.");
      else
        ((QueryProcessor) obj).TargetStaffAuthTokenResponse(ClientScene.readyConnection, reader.ReadString());
    }

    public void CallTargetSaltGenerated(NetworkConnection conn, byte[] salt)
    {
      if (!NetworkServer.active)
        Debug.LogError((object) "TargetRPC Function TargetSaltGenerated called on client.");
      else if (conn is ULocalConnectionToServer)
      {
        Debug.LogError((object) "TargetRPC Function TargetSaltGenerated called on connection to server");
      }
      else
      {
        NetworkWriter writer = new NetworkWriter();
        writer.Write((short) 0);
        writer.Write((short) 2);
        writer.WritePackedUInt32((uint) QueryProcessor.kTargetRpcTargetSaltGenerated);
        writer.Write(this.GetComponent<NetworkIdentity>().netId);
        writer.WriteBytesFull(salt);
        this.SendTargetRPCInternal(conn, writer, 2, "TargetSaltGenerated");
      }
    }

    public void CallTargetReplyPassword(NetworkConnection conn, bool b)
    {
      if (!NetworkServer.active)
        Debug.LogError((object) "TargetRPC Function TargetReplyPassword called on client.");
      else if (conn is ULocalConnectionToServer)
      {
        Debug.LogError((object) "TargetRPC Function TargetReplyPassword called on connection to server");
      }
      else
      {
        NetworkWriter writer = new NetworkWriter();
        writer.Write((short) 0);
        writer.Write((short) 2);
        writer.WritePackedUInt32((uint) QueryProcessor.kTargetRpcTargetReplyPassword);
        writer.Write(this.GetComponent<NetworkIdentity>().netId);
        writer.Write(b);
        this.SendTargetRPCInternal(conn, writer, 14, "TargetReplyPassword");
      }
    }

    public void CallTargetReply(NetworkConnection conn, string content, bool isSuccess, bool logInConsole, string overrideDisplay)
    {
      if (!NetworkServer.active)
        Debug.LogError((object) "TargetRPC Function TargetReply called on client.");
      else if (conn is ULocalConnectionToServer)
      {
        Debug.LogError((object) "TargetRPC Function TargetReply called on connection to server");
      }
      else
      {
        NetworkWriter writer = new NetworkWriter();
        writer.Write((short) 0);
        writer.Write((short) 2);
        writer.WritePackedUInt32((uint) QueryProcessor.kTargetRpcTargetReply);
        writer.Write(this.GetComponent<NetworkIdentity>().netId);
        writer.Write(content);
        writer.Write(isSuccess);
        writer.Write(logInConsole);
        writer.Write(overrideDisplay);
        this.SendTargetRPCInternal(conn, writer, 15, "TargetReply");
      }
    }

    public void CallTargetStaffPlayerListResponse(NetworkConnection conn, string data)
    {
      if (!NetworkServer.active)
        Debug.LogError((object) "TargetRPC Function TargetStaffPlayerListResponse called on client.");
      else if (conn is ULocalConnectionToServer)
      {
        Debug.LogError((object) "TargetRPC Function TargetStaffPlayerListResponse called on connection to server");
      }
      else
      {
        NetworkWriter writer = new NetworkWriter();
        writer.Write((short) 0);
        writer.Write((short) 2);
        writer.WritePackedUInt32((uint) QueryProcessor.kTargetRpcTargetStaffPlayerListResponse);
        writer.Write(this.GetComponent<NetworkIdentity>().netId);
        writer.Write(data);
        this.SendTargetRPCInternal(conn, writer, 2, "TargetStaffPlayerListResponse");
      }
    }

    public void CallTargetStaffAuthTokenResponse(NetworkConnection conn, string auth)
    {
      if (!NetworkServer.active)
        Debug.LogError((object) "TargetRPC Function TargetStaffAuthTokenResponse called on client.");
      else if (conn is ULocalConnectionToServer)
      {
        Debug.LogError((object) "TargetRPC Function TargetStaffAuthTokenResponse called on connection to server");
      }
      else
      {
        NetworkWriter writer = new NetworkWriter();
        writer.Write((short) 0);
        writer.Write((short) 2);
        writer.WritePackedUInt32((uint) QueryProcessor.kTargetRpcTargetStaffAuthTokenResponse);
        writer.Write(this.GetComponent<NetworkIdentity>().netId);
        writer.Write(auth);
        this.SendTargetRPCInternal(conn, writer, 2, "TargetStaffAuthTokenResponse");
      }
    }

    static QueryProcessor()
    {
      NetworkBehaviour.RegisterCommandDelegate(typeof (QueryProcessor), QueryProcessor.kCmdCmdRequestSalt, new NetworkBehaviour.CmdDelegate(QueryProcessor.InvokeCmdCmdRequestSalt));
      QueryProcessor.kCmdCmdSendPassword = 1923616621;
      NetworkBehaviour.RegisterCommandDelegate(typeof (QueryProcessor), QueryProcessor.kCmdCmdSendPassword, new NetworkBehaviour.CmdDelegate(QueryProcessor.InvokeCmdCmdSendPassword));
      QueryProcessor.kCmdCmdSendQuery = -1744616138;
      NetworkBehaviour.RegisterCommandDelegate(typeof (QueryProcessor), QueryProcessor.kCmdCmdSendQuery, new NetworkBehaviour.CmdDelegate(QueryProcessor.InvokeCmdCmdSendQuery));
      QueryProcessor.kTargetRpcTargetSaltGenerated = -59915534;
      NetworkBehaviour.RegisterRpcDelegate(typeof (QueryProcessor), QueryProcessor.kTargetRpcTargetSaltGenerated, new NetworkBehaviour.CmdDelegate(QueryProcessor.InvokeRpcTargetSaltGenerated));
      QueryProcessor.kTargetRpcTargetReplyPassword = -1238863682;
      NetworkBehaviour.RegisterRpcDelegate(typeof (QueryProcessor), QueryProcessor.kTargetRpcTargetReplyPassword, new NetworkBehaviour.CmdDelegate(QueryProcessor.InvokeRpcTargetReplyPassword));
      QueryProcessor.kTargetRpcTargetReply = -489945853;
      NetworkBehaviour.RegisterRpcDelegate(typeof (QueryProcessor), QueryProcessor.kTargetRpcTargetReply, new NetworkBehaviour.CmdDelegate(QueryProcessor.InvokeRpcTargetReply));
      QueryProcessor.kTargetRpcTargetStaffPlayerListResponse = -1316694695;
      NetworkBehaviour.RegisterRpcDelegate(typeof (QueryProcessor), QueryProcessor.kTargetRpcTargetStaffPlayerListResponse, new NetworkBehaviour.CmdDelegate(QueryProcessor.InvokeRpcTargetStaffPlayerListResponse));
      QueryProcessor.kTargetRpcTargetStaffAuthTokenResponse = -454891367;
      NetworkBehaviour.RegisterRpcDelegate(typeof (QueryProcessor), QueryProcessor.kTargetRpcTargetStaffAuthTokenResponse, new NetworkBehaviour.CmdDelegate(QueryProcessor.InvokeRpcTargetStaffAuthTokenResponse));
      NetworkCRC.RegisterBehaviour(nameof (QueryProcessor), 0);
    }

    public override bool OnSerialize(NetworkWriter writer, bool forceAll)
    {
      if (forceAll)
      {
        writer.WritePackedUInt32((uint) this.PlayerId);
        writer.Write(this.OverridePasswordEnabled);
        writer.Write(this.GameplayData);
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
        writer.WritePackedUInt32((uint) this.PlayerId);
      }
      if (((int) this.syncVarDirtyBits & 2) != 0)
      {
        if (!flag)
        {
          writer.WritePackedUInt32(this.syncVarDirtyBits);
          flag = true;
        }
        writer.Write(this.OverridePasswordEnabled);
      }
      if (((int) this.syncVarDirtyBits & 4) != 0)
      {
        if (!flag)
        {
          writer.WritePackedUInt32(this.syncVarDirtyBits);
          flag = true;
        }
        writer.Write(this.GameplayData);
      }
      if (!flag)
        writer.WritePackedUInt32(this.syncVarDirtyBits);
      return flag;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
      if (initialState)
      {
        this.PlayerId = (int) reader.ReadPackedUInt32();
        this.OverridePasswordEnabled = reader.ReadBoolean();
        this.GameplayData = reader.ReadBoolean();
      }
      else
      {
        int num = (int) reader.ReadPackedUInt32();
        if ((num & 1) != 0)
          this.SetId((int) reader.ReadPackedUInt32());
        if ((num & 2) != 0)
          this.SetOverridePasswordEnabled(reader.ReadBoolean());
        if ((num & 4) == 0)
          return;
        this.GameplayData = reader.ReadBoolean();
      }
    }
  }
}
