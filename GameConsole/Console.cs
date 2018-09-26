// Decompiled with JetBrains decompiler
// Type: GameConsole.Console
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
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameConsole
{
  public class Console : MonoBehaviour
  {
    private readonly List<Console.Log> _logs = new List<Console.Log>();
    private readonly List<Console.Value> _values = new List<Console.Value>();
    private string _response = string.Empty;
    public static AsymmetricKeyParameter Publickey;
    public Console.CommandHint[] hints;
    public static Console singleton;
    public Text txt;
    public InputField cmdField;
    public GameObject console;
    internal static AsymmetricCipherKeyPair SessionKeys;
    private int scrollup;
    private int previous_scrlup;
    private string loadedLevel;
    private string _content;
    private bool allwaysRefreshing;
    private bool _change;

    private void Start()
    {
      this.AddLog("Hi there! Initializing console...", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
      this.AddLog("Done! Type 'help' to print the list of available commands.", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
      Timing.RunCoroutine(this._RefreshPublicKey(), Segment.FixedUpdate);
      Timing.RunCoroutine(this._RefreshCentralServers(), Segment.FixedUpdate);
      this.AddLog("Generatig session keys...", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
      Console.SessionKeys = ECDSA.GenerateKeys(384);
      this.AddLog("Session keys generated!", (Color32) Color.green, false);
    }

    private void Update()
    {
      if (!this._change)
        return;
      this.txt.text = this._content;
      this._change = false;
    }

    private void LateUpdate()
    {
      if (Input.GetKeyDown(KeyCode.Return))
        this.ProceedButton();
      else if (Input.GetKeyDown(KeyCode.BackQuote))
        this.ToggleConsole();
      else if (Input.GetKey(KeyCode.Escape) && this.console.activeSelf)
        this.ToggleConsole();
      this.scrollup += Mathf.RoundToInt(Input.GetAxisRaw("Mouse ScrollWheel") * 10f);
      this.scrollup = this._logs.Count <= 0 ? 0 : Mathf.Clamp(this.scrollup, 0, this._logs.Count - 1);
      if (this.previous_scrlup != this.scrollup)
      {
        this.previous_scrlup = this.scrollup;
        this.RefreshConsoleScreen();
      }
      Scene activeScene = SceneManager.GetActiveScene();
      if (activeScene.name != this.loadedLevel)
      {
        this.loadedLevel = activeScene.name;
        this.AddLog("Scene Manager: Loaded scene '" + activeScene.name + "' [" + activeScene.path + "]", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
        this.RefreshConsoleScreen();
      }
      if (!this.allwaysRefreshing)
        return;
      this.RefreshConsoleScreen();
    }

    public List<Console.Log> GetAllLogs()
    {
      return this._logs;
    }

    private void Awake()
    {
      Object.DontDestroyOnLoad((Object) this.gameObject);
      if ((Object) Console.singleton == (Object) null)
        Console.singleton = this;
      else
        Object.DestroyImmediate((Object) this.gameObject);
    }

    private void RefreshConsoleScreen()
    {
      this._change = true;
      bool flag = false;
      if (this.txt.text.Length > 15000)
      {
        this._logs.RemoveAt(0);
        flag = true;
      }
      if ((Object) this.txt == (Object) null)
        return;
      this._content = string.Empty;
      if (this._logs.Count > 0)
      {
        for (int index = 0; index < this._logs.Count - this.scrollup; ++index)
        {
          string str1 = (!this._logs[index].nospace ? "\n\n" : "\n") + "<color=" + this.ColorToHex(this._logs[index].color) + ">" + this._logs[index].text + "</color>";
          if (str1.Contains("@#{["))
          {
            string str2 = str1.Remove(str1.IndexOf("@#{[", StringComparison.Ordinal));
            string str3 = str1.Remove(0, str1.IndexOf("@#{[", StringComparison.Ordinal) + 4);
            string str4 = str3.Remove(str3.Length - 12);
            foreach (Console.Value obj in this._values)
            {
              if (obj.key == str4)
                str1 = str2 + obj.value + "</color>";
            }
          }
          this._content += str1;
        }
      }
      if (!flag)
        return;
      this.RefreshConsoleScreen();
    }

    public void AddLog(string text, Color32 c, bool nospace = false)
    {
      Console console = this;
      console._response = console._response + text + Environment.NewLine;
      if (!nospace)
        this._response += Environment.NewLine;
      this.scrollup = 0;
      this._logs.Add(new Console.Log(text, c, nospace));
      this.RefreshConsoleScreen();
    }

    private string ColorToHex(Color32 color)
    {
      return "#" + (color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2"));
    }

    public static GameObject FindConnectedRoot(NetworkConnection conn)
    {
      try
      {
        foreach (PlayerController playerController in conn.playerControllers)
        {
          if (playerController.gameObject.tag == "Player")
            return playerController.gameObject;
        }
      }
      catch
      {
        return (GameObject) null;
      }
      return (GameObject) null;
    }

    public string TypeCommand(string cmd)
    {
      this._response = string.Empty;
      string[] strArray = cmd.ToUpper().Split(' ');
      cmd = strArray[0];
      if (cmd == "HELLO")
        this.AddLog("Hello World!", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
      else if (cmd == "LENNY")
        this.AddLog("<size=450>( ͡° ͜ʖ ͡°)</size>\n\n", new Color32(byte.MaxValue, (byte) 180, (byte) 180, byte.MaxValue), false);
      else if (cmd == "CONTACT")
      {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
          {
            this.AddLog("Requesting contact email to server owner...", (Color32) Color.yellow, false);
            gameObject.GetComponent<CharacterClassManager>().CallCmdRequestContactEmail();
          }
        }
      }
      else if (cmd == "SRVCFG")
      {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
          {
            this.AddLog("Requesting server config...", (Color32) Color.yellow, false);
            gameObject.GetComponent<CharacterClassManager>().CallCmdRequestServerConfig();
          }
        }
      }
      else if (cmd == "GROUPS")
      {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
          {
            this.AddLog("Requesting server groups...", (Color32) Color.yellow, false);
            gameObject.GetComponent<CharacterClassManager>().CallCmdRequestServerGroups();
          }
        }
      }
      else if (cmd == "HIDETAG")
      {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
          {
            this.AddLog("Hidding your tag...", (Color32) Color.yellow, false);
            gameObject.GetComponent<CharacterClassManager>().CallCmdRequestHideTag();
          }
        }
      }
      else if (cmd == "SHOWTAG" || cmd == "TAG")
      {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
          {
            this.AddLog("Requesting your local tag...", (Color32) Color.yellow, false);
            gameObject.GetComponent<CharacterClassManager>().CallCmdRequestShowTag(false);
          }
        }
      }
      else if (cmd == "GLOBALTAG" || cmd == "GTAG")
      {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
          {
            this.AddLog("Requesting your global tag...", (Color32) Color.yellow, false);
            gameObject.GetComponent<CharacterClassManager>().CallCmdRequestShowTag(true);
          }
        }
      }
      else if (cmd == "GLOBALBAN" || cmd == "GBAN" || cmd == "SUPERBAN")
      {
        if (strArray.Length < 3 || strArray[1].ToLower() != "nick" && strArray[1].ToLower() != "id")
          this.AddLog("Syntax: globalban <nick/id> <player to ban>", (Color32) Color.red, false);
        else if (!File.Exists(FileManager.AppFolder + "StaffAPI.txt"))
        {
          this.AddLog("Staff API token not found on your computer!", (Color32) Color.red, false);
        }
        else
        {
          foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
          {
            if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
              this.AddLog("Requesting your global ban...", (Color32) Color.yellow, false);
              gameObject.GetComponent<QueryProcessor>().RequestGlobalBan(strArray[2], !(strArray[1].ToLower() == "id") ? 1 : 0);
            }
          }
        }
      }
      else if (cmd == "CONFIRM")
      {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            gameObject.GetComponent<QueryProcessor>().ConfirmGlobalBanning();
        }
      }
      else if (cmd == "OVERWATCH" || cmd == "OVR")
      {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
          {
            if (strArray.Length == 1)
              gameObject.GetComponent<ServerRoles>().CallCmdToggleOverwatch();
            else if (strArray[1] == "1" || strArray[1].ToLower() == "true" || (strArray[1].ToLower() == "enable" || strArray[1].ToLower() == "on"))
              gameObject.GetComponent<ServerRoles>().CallCmdSetOverwatchStatus(true);
            else if (strArray[1] == "0" || strArray[1].ToLower() == "false" || (strArray[1].ToLower() == "disable" || strArray[1].ToLower() == "off"))
              gameObject.GetComponent<ServerRoles>().CallCmdSetOverwatchStatus(false);
            else
              this.AddLog("Unknown status: " + strArray[1], (Color32) Color.red, false);
          }
        }
      }
      else if (cmd == "GIVE")
      {
        if (!((IEnumerable<GameObject>) GameObject.FindGameObjectsWithTag("Player")).Select<GameObject, PlayerStats>((Func<GameObject, PlayerStats>) (player => player.GetComponent<PlayerStats>())).Any<PlayerStats>((Func<PlayerStats, bool>) (nid =>
        {
          if (nid.isLocalPlayer)
            return nid.isServer;
          return false;
        })))
        {
          this.AddLog("You're not owner of this server!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
        }
        else
        {
          int result = 0;
          if (strArray.Length >= 2 && int.TryParse(strArray[1], out result))
          {
            string str = "offline";
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
            {
              if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
              {
                str = "online";
                Inventory component = gameObject.GetComponent<Inventory>();
                if (!((Object) component == (Object) null))
                {
                  if (component.availableItems.Length > result)
                  {
                    component.AddNewItem(result, -4.656647E+11f);
                    str = "none";
                  }
                  else
                    this.AddLog("Failed to add ITEM#" + result.ToString("000") + " - item does not exist!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
                }
              }
            }
            if (str == "offline" || str == "online")
              this.AddLog(!(str == "offline") ? "Player inventory script couldn't be find!" : "You cannot use that command if you are not playing on any server!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
            else
              this.AddLog("ITEM#" + result.ToString("000") + " has been added!", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
          }
          else
            this.AddLog("Second argument has to be a number!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
        }
      }
      else if (cmd == "ROUNDRESTART")
      {
        bool flag = false;
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          PlayerStats component = gameObject.GetComponent<PlayerStats>();
          if (component.isLocalPlayer && component.isServer)
          {
            flag = true;
            this.AddLog("The round is about to restart! Please wait..", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
            component.Roundrestart();
          }
        }
        if (!flag)
          this.AddLog("You're not owner of this server!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
      }
      else if (cmd == "ITEMLIST")
      {
        string str = "offline";
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          int result = 1;
          if (strArray.Length >= 2 && !int.TryParse(strArray[1], out result))
          {
            this.AddLog("Please enter correct page number!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
            return this._response;
          }
          if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
          {
            str = "online";
            Inventory component = gameObject.GetComponent<Inventory>();
            if (!((Object) component == (Object) null))
            {
              str = "none";
              if (result < 1)
              {
                this.AddLog("Page '" + (object) result + "' does not exist!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
                this.RefreshConsoleScreen();
                return this._response;
              }
              Item[] availableItems = component.availableItems;
              for (int index = 10 * (result - 1); index < 10 * result; ++index)
              {
                if (10 * (result - 1) > availableItems.Length)
                {
                  this.AddLog("Page '" + (object) result + "' does not exist!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
                  break;
                }
                if (index < availableItems.Length)
                  this.AddLog("ITEM#" + index.ToString("000") + " : " + availableItems[index].label, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
                else
                  break;
              }
            }
          }
        }
        if (str != "none")
          this.AddLog(!(str == "offline") ? "Player inventory script couldn't be find!" : "You cannot use that command if you are not playing on any server!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
      }
      else if (cmd == "BAN")
      {
        if (!GameObject.Find("Host").GetComponent<NetworkIdentity>().isLocalPlayer)
          return this._response;
        if (strArray.Length < 3)
        {
          this.AddLog("Syntax: BAN [player kick / ip] [minutes]", new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue), false);
          foreach (NetworkConnection connection in NetworkServer.connections)
          {
            string str = string.Empty;
            GameObject connectedRoot = Console.FindConnectedRoot(connection);
            if ((Object) connectedRoot != (Object) null)
              str = connectedRoot.GetComponent<NicknameSync>().myNick;
            if (str == string.Empty)
              this.AddLog("Player :: " + connection.address, new Color32((byte) 160, (byte) 128, (byte) 128, byte.MaxValue), true);
            else
              this.AddLog("Player :: " + str + " :: " + connection.address, new Color32((byte) 128, (byte) 160, (byte) 128, byte.MaxValue), true);
          }
        }
        else
        {
          int result = 0;
          if (int.TryParse(strArray[2], out result))
          {
            bool flag = false;
            foreach (NetworkConnection connection in NetworkServer.connections)
            {
              GameObject connectedRoot = Console.FindConnectedRoot(connection);
              if (connection.address.ToUpper().Contains(strArray[1]) || !((Object) connectedRoot == (Object) null) && connectedRoot.GetComponent<NicknameSync>().myNick.ToUpper().Contains(strArray[1]))
              {
                flag = true;
                PlayerManager.localPlayer.GetComponent<BanPlayer>().BanUser(connectedRoot, result, string.Empty, "Administrator");
                this.AddLog("Player banned.", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
              }
            }
            if (!flag)
              this.AddLog("Player not found.", new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue), false);
          }
          else
            this.AddLog("Parse error: [minutes] - has to be an integer.", new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue), false);
        }
      }
      else if (cmd == "CLS" || cmd == "CLEAR")
      {
        this._logs.Clear();
        this.RefreshConsoleScreen();
      }
      else if (cmd == "QUIT" || cmd == "EXIT")
      {
        this._logs.Clear();
        this.RefreshConsoleScreen();
        this.AddLog("<size=50>GOODBYE!</size>", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
        this.RefreshConsoleScreen();
        this.Invoke("QuitGame", 1f);
      }
      else if (cmd == "HELP")
      {
        if (strArray.Length > 1)
        {
          string str = strArray[1];
          foreach (Console.CommandHint hint in this.hints)
          {
            if (!(hint.name != str))
            {
              this.AddLog(hint.name + " - " + hint.fullDesc, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
              this.RefreshConsoleScreen();
              return this._response;
            }
          }
          this.AddLog("Help for command '" + strArray[1] + "' does not exist!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
          this.RefreshConsoleScreen();
          return this._response;
        }
        this.AddLog("List of available commands:\n", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
        foreach (Console.CommandHint hint in this.hints)
          this.AddLog(hint.name + " - " + hint.shortDesc, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), true);
        this.AddLog("Type 'HELP [COMMAND]' to print a full description of the chosen command.", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
        this.RefreshConsoleScreen();
      }
      else if (cmd == "REFRESHFIX")
      {
        this.allwaysRefreshing = !this.allwaysRefreshing;
        this.AddLog("Console log refresh mode: " + (!this.allwaysRefreshing ? "OPTIMIZED" : "FIXED"), new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
      }
      else if (cmd == "COLOR" || cmd == "COLORS")
      {
        bool flag1 = strArray.Length > 1 && strArray[1].ToUpper() == "LIST";
        bool flag2 = strArray.Length > 1 && strArray[1].ToUpper() == "ALL" || strArray.Length > 2 && strArray[2].ToUpper() == "ALL";
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          ServerRoles component = gameObject.GetComponent<ServerRoles>();
          if (component.isLocalPlayer)
          {
            this.AddLog("Available colors:", (Color32) Color.gray, false);
            string text = string.Empty;
            foreach (ServerRoles.NamedColor namedColor in component.NamedColors)
            {
              if (!namedColor.Restricted || flag2)
              {
                if (flag1)
                  this.AddLog("<color=#" + namedColor.ColorHex + ">" + namedColor.Name + " - #" + namedColor.ColorHex + "</color>", (Color32) Color.white, false);
                else
                  text = text + "<color=#" + namedColor.ColorHex + ">" + namedColor.Name + "</color>    ";
              }
            }
            if (!flag1)
              this.AddLog(text, (Color32) Color.white, false);
          }
        }
      }
      else if (cmd == "VALUE")
      {
        if (strArray.Length < 2)
        {
          this.AddLog("The second argument cannot be <i>null</i>!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
        }
        else
        {
          bool flag = false;
          string str = strArray[1];
          foreach (Console.Value obj in this._values)
          {
            if (!(obj.key != str))
            {
              flag = true;
              this.AddLog("The value of " + str + " is: @#{[" + str + "}]#@", new Color32((byte) 50, (byte) 70, (byte) 100, byte.MaxValue), false);
            }
          }
          if (!flag)
            this.AddLog("Key " + str + " not found!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
        }
      }
      else if (cmd == "SEED")
      {
        GameObject gameObject = GameObject.Find("Host");
        this.AddLog("Map seed is: <b>" + (!((Object) gameObject == (Object) null) ? gameObject.GetComponent<RandomSeedSync>().seed.ToString() : "NONE") + "</b>", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
      }
      else if (cmd == "SHOWRIDS")
      {
        GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("RoomID");
        foreach (GameObject gameObject in gameObjectsWithTag)
        {
          gameObject.GetComponentsInChildren<MeshRenderer>()[0].enabled = !gameObject.GetComponentsInChildren<MeshRenderer>()[0].enabled;
          gameObject.GetComponentsInChildren<MeshRenderer>()[1].enabled = !gameObject.GetComponentsInChildren<MeshRenderer>()[1].enabled;
        }
        if (gameObjectsWithTag.Length > 0)
          this.AddLog("Show RIDS: " + (object) gameObjectsWithTag[0].GetComponentInChildren<MeshRenderer>().enabled, new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
        else
          this.AddLog("There are no RIDS!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
      }
      else if (cmd == "CLASSLIST")
      {
        string str = "offline";
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          int result = 1;
          if (strArray.Length >= 2 && !int.TryParse(strArray[1], out result))
          {
            this.AddLog("Please enter correct page number!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
            return this._response;
          }
          if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
          {
            str = "online";
            CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
            if (!((Object) component == (Object) null))
            {
              str = "none";
              if (result < 1)
              {
                this.AddLog("Page '" + (object) result + "' does not exist!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
                this.RefreshConsoleScreen();
                return this._response;
              }
              Class[] klasy = component.klasy;
              for (int index = 10 * (result - 1); index < 10 * result; ++index)
              {
                if (10 * (result - 1) > klasy.Length)
                {
                  this.AddLog("Page '" + (object) result + "' does not exist!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
                  break;
                }
                if (index < klasy.Length)
                  this.AddLog("CLASS#" + index.ToString("000") + " : " + klasy[index].fullName, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
                else
                  break;
              }
            }
          }
        }
        if (str != "none")
          this.AddLog(!(str == "offline") ? "Player inventory script couldn't be find!" : "You cannot use that command if you are not playing on any server!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
      }
      else if (cmd == "RANGE")
      {
        string str = "offline";
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
          {
            str = "online";
            ShootingRange component = gameObject.GetComponent<ShootingRange>();
            if (!((Object) component == (Object) null))
            {
              str = "none";
              component.isOnRange = true;
            }
          }
        }
        if (str == "offline" || str == "online")
          this.AddLog(!(str == "offline") ? "Player range script couldn't be find!" : "You cannot use that command if you are not playing on any server!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
        else
          this.AddLog("<b>Shooting range</b> is now available!", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
      }
      else if (cmd == "WARHEAD")
      {
        AlphaWarheadController host = AlphaWarheadController.host;
        if (strArray.Length == 1)
        {
          this.AddLog("Synax: warhead (status|detonate|cancel|enable|disable)", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
        }
        else
        {
          string lower = strArray[1].ToLower();
          if (lower == "status")
          {
            if (host.detonated || (double) host.timeToDetonation == 0.0)
              this.AddLog("Warhead has been detonated.", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
            else if (host.inProgress)
              this.AddLog("Detonation is in progress.", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
            else if (!AlphaWarheadOutsitePanel.nukeside.enabled)
              this.AddLog("Warhead is disabled.", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
            else if ((double) host.timeToDetonation > (double) AlphaWarheadController.host.RealDetonationTime())
              this.AddLog("Warhead is restarting.", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
            else
              this.AddLog("Warhead is ready to detonation.", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
          }
          else if (lower == "detonate")
          {
            AlphaWarheadController.host.StartDetonation();
            this.AddLog("Detonation sequence started.", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
          }
          else if (lower == "cancel")
          {
            AlphaWarheadController.host.CancelDetonation((GameObject) null);
            this.AddLog("Detonation has been canceled.", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
          }
          else if (lower == "enable")
          {
            AlphaWarheadOutsitePanel.nukeside.Networkenabled = true;
            this.AddLog("Warhead has been enabled.", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
          }
          else if (lower == "disable")
          {
            AlphaWarheadOutsitePanel.nukeside.Networkenabled = false;
            this.AddLog("Warhead has been disabled.", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
          }
          else
            this.AddLog("WARHEAD: Unknown subcommand.", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
        }
      }
      else if (cmd == "CONFIG")
      {
        if (strArray.Length < 2)
        {
          this.TypeCommand("HELP CONFIG");
        }
        else
        {
          switch (strArray[1])
          {
            case "RELOAD":
            case "R":
            case "RLD":
              ConfigFile.ReloadGameConfig();
              ServerStatic.RolesConfig = new YamlConfig(ServerStatic.RolesConfigPath);
              ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig);
              this.AddLog("Configuration file <b>successfully reloaded</b>. New settings will be applied on <b>your</b> server in <b>next</b> round.", new Color32((byte) 0, byte.MaxValue, (byte) 0, byte.MaxValue), false);
              break;
            case "PATH":
              this.AddLog("Configuration file path: <i>" + ConfigFile.ConfigPath + "</i>", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
              this.AddLog("<i>No visible drive letter means the root game directory.</i>", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
              break;
            case "VALUE":
              if (strArray.Length < 3)
              {
                this.AddLog("Please enter key name in the third argument. (CONFIG VALUE <i>KEYNAME</i>)", new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue), false);
                break;
              }
              this.AddLog("The value of <i>'" + strArray[2] + "'</i> is: " + ConfigFile.ServerConfig.GetString(strArray[2], "<color=ff0>DENIED: Entered key does not exists</color>"), new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), false);
              break;
          }
        }
      }
      else
        this.AddLog("Command " + cmd + " does not exist!", new Color32(byte.MaxValue, (byte) 180, (byte) 0, byte.MaxValue), false);
      return this._response;
    }

    public void ProceedButton()
    {
      if (this.cmdField.text != string.Empty)
        this.TypeCommand(this.cmdField.text);
      this.cmdField.text = string.Empty;
      EventSystem.current.SetSelectedGameObject(this.cmdField.gameObject);
    }

    public void ToggleConsole()
    {
      CursorManager.consoleOpen = !this.console.activeSelf;
      this.cmdField.text = string.Empty;
      this.console.SetActive(!this.console.activeSelf);
      if ((Object) PlayerManager.singleton != (Object) null)
      {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
          if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
          {
            FirstPersonController component = gameObject.GetComponent<FirstPersonController>();
            if ((Object) component != (Object) null)
              component.usingConsole = this.console.activeSelf;
          }
        }
      }
      if (!this.console.activeSelf)
        return;
      EventSystem.current.SetSelectedGameObject(this.cmdField.gameObject);
    }

    [DebuggerHidden]
    private IEnumerator<float> _RefreshCentralServers()
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      Console.\u003C_RefreshCentralServers\u003Ec__Iterator0 serversCIterator0 = new Console.\u003C_RefreshCentralServers\u003Ec__Iterator0();
      return (IEnumerator<float>) serversCIterator0;
    }

    [DebuggerHidden]
    private IEnumerator<float> _RefreshPublicKey()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator<float>) new Console.\u003C_RefreshPublicKey\u003Ec__Iterator1() { \u0024this = this };
    }

    private void QuitGame()
    {
      Application.Quit();
    }

    [Serializable]
    public class CommandHint
    {
      public string name;
      public string shortDesc;
      [Multiline]
      public string fullDesc;
    }

    [Serializable]
    public class Value
    {
      public string key;
      public string value;

      public Value(string k, string v)
      {
        this.key = k;
        this.value = v;
      }
    }

    [Serializable]
    public class Log
    {
      public string text;
      public Color32 color;
      public bool nospace;

      public Log(string t, Color32 c, bool b)
      {
        this.text = t;
        this.color = c;
        this.nospace = b;
      }
    }
  }
}
