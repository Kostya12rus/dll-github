// Decompiled with JetBrains decompiler
// Type: CustomNetworkManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance.Integrations.UNet_HLAPI;
using MEC;
using Mono.Nat;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomNetworkManager : NetworkManager
{
  public static string Ip = string.Empty;
  public GameObject popup;
  public GameObject createpop;
  public RectTransform contSize;
  public Text content;
  private GameConsole.Console _console;
  private static QueryServer _queryserver;
  private List<INatDevice> _mappedDevices;
  public CustomNetworkManager.DisconnectLog[] logs;
  private int _curLogId;
  private int _queryPort;
  public bool reconnect;
  private bool _queryEnabled;
  private bool _activated;
  public string disconnectMessage;
  public static string ConnectionIp;
  [Space(20f)]
  public string[] CompatibleVersions;

  public CustomNetworkManager()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    if (!this.popup.get_activeSelf() || !Input.GetKey((KeyCode) 27))
      return;
    this.ClickButton();
  }

  public virtual void OnClientDisconnect(NetworkConnection conn)
  {
    this.ShowLog((int) conn.get_lastError());
  }

  public virtual void OnClientError(NetworkConnection conn, int errorCode)
  {
    this.ShowLog(errorCode);
  }

  public virtual void OnStartClient(NetworkClient client)
  {
    base.OnStartClient(client);
    ((MonoBehaviour) this).StartCoroutine((IEnumerator) this._ConnectToServer(client));
  }

  [DebuggerHidden]
  private IEnumerator<float> _ConnectToServer(NetworkClient client)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new CustomNetworkManager.\u003C_ConnectToServer\u003Ec__Iterator0()
    {
      client = client,
      \u0024this = this
    };
  }

  public virtual void OnServerConnect(NetworkConnection conn)
  {
    base.OnServerConnect(conn);
    if (BanHandler.QueryBan((string) null, (string) conn.address).Value != null)
    {
      ServerConsole.AddLog("Player tried to connect from banned IP address " + (string) conn.address + ".");
      ServerConsole.Disconnect(conn, "You are banned from this server.");
    }
    else
      ServerConsole.AddLog("Player joined from IP address " + (string) conn.address + ".");
  }

  public virtual void OnServerDisconnect(NetworkConnection conn)
  {
    base.OnServerDisconnect(conn);
    HlapiServer.OnServerDisconnect(conn);
  }

  public static void PlayerDisconnect(NetworkConnection conn)
  {
    HlapiServer.OnServerDisconnect(conn);
  }

  private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
  {
    if (!this._activated && ((Scene) ref scene).get_name().ToLower().Contains("menu"))
    {
      this._activated = true;
      this.set_networkAddress("none");
      this.StartClient();
      this.set_networkAddress("localhost");
      this.StopClient();
    }
    if (!this.reconnect)
      return;
    this.ShowLog(14);
    ((MonoBehaviour) this).Invoke("Reconnect", 3f);
  }

  public virtual void OnClientSceneChanged(NetworkConnection conn)
  {
    base.OnClientSceneChanged(conn);
    if (this.reconnect || !this.logs[this._curLogId].autoHideOnSceneLoad)
      return;
    this.popup.SetActive(false);
  }

  private void Reconnect()
  {
    if (!this.reconnect)
      return;
    this.reconnect = false;
    this.StartClient();
  }

  public void StopReconnecting()
  {
    this.reconnect = false;
  }

  public void ShowLog(int id)
  {
    this._curLogId = id;
    this.popup.SetActive(true);
    this.content.set_text(TranslationReader.Get("Connection_Errors", id));
    if (!string.IsNullOrEmpty(this.disconnectMessage))
    {
      string[] strArray = this.content.get_text().Split(new string[1]
      {
        Environment.NewLine
      }, StringSplitOptions.None);
      if (strArray.Length > 0)
        this.content.set_text(strArray[0] + Environment.NewLine + this.disconnectMessage);
      this.disconnectMessage = string.Empty;
    }
    ((Graphic) this.content).get_rectTransform().set_sizeDelta(Vector2.op_Implicit(Vector3.get_zero()));
  }

  public void ClickButton()
  {
    foreach (ConnInfoButton action in this.logs[this._curLogId].button.actions)
      action.UseButton();
  }

  public virtual void OnClientConnect(NetworkConnection conn)
  {
    base.OnClientConnect(conn);
  }

  private void Start()
  {
    ConfigFile.HosterPolicy = !File.Exists("hoster_policy.txt") ? (!File.Exists(FileManager.AppFolder + "hoster_policy.txt") ? new YamlConfig() : new YamlConfig(FileManager.AppFolder + "hoster_policy.txt")) : new YamlConfig("hoster_policy.txt");
    if (ServerStatic.IsDedicated)
      return;
    ServerConsole.AddLog("Loading config...");
    ConfigFile.ServerConfig = ConfigFile.ReloadGameConfig(FileManager.AppFolder + "config_gameplay.txt", false);
    ServerConsole.AddLog("Config file loaded!");
    this._console = GameConsole.Console.singleton;
    if (!SteamAPI.Init())
    {
      this._console.AddLog("Failed to init SteamAPI.", new Color32((byte) 128, (byte) 128, (byte) 128, byte.MaxValue), false);
    }
    else
    {
      if (Directory.Exists("SCPSL_Data\\Managed"))
      {
        if (!File.Exists("SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml"))
        {
          this.CreateVersionFile(false);
        }
        else
        {
          string[] strArray1 = FileManager.ReadAllLines("SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml");
          if (strArray1.Length < 1 || !ServerRoles.Base64Decode(strArray1[0].Replace("UI Build GUID: ", string.Empty).Replace("-", string.Empty)).Contains(";"))
          {
            this.CreateVersionFile(false);
          }
          else
          {
            string[] strArray2 = ServerRoles.Base64Decode(strArray1[0].Replace("UI Build GUID: ", string.Empty).Replace("-", string.Empty)).Split(';');
            if (strArray2.Length != 3 || strArray2[0] != this.CompatibleVersions[0])
              this.CreateVersionFile(false);
            else if (strArray2[2] != SteamUser.GetSteamID().ToString())
            {
              try
              {
                string plainText = strArray2[0] + ";" + strArray2[1] + ";" + (object) SteamUser.GetSteamID();
                File.Delete("SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml");
                File.Create("SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml").Close();
                FileManager.AppendFile("UI Build GUID: " + this.GUIDSplit(ServerRoles.Base64Encode(plainText)), "SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml", true);
              }
              catch (Exception ex)
              {
                GameConsole.Console.singleton.AddLog("IO startup error 2.1", Color32.op_Implicit(Color.get_red()), false);
              }
            }
          }
        }
      }
      if (Directory.Exists("PrivateBeta") && Directory.Exists("PrivateBeta\\SCPSL_Data\\Managed"))
      {
        if (!File.Exists("PrivateBeta\\SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml"))
        {
          this.CreateVersionFile(true);
        }
        else
        {
          string[] strArray1 = FileManager.ReadAllLines("PrivateBeta\\SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml");
          if (strArray1.Length < 1 || !ServerRoles.Base64Decode(strArray1[0].Replace("UI Build GUID: ", string.Empty).Replace("-", string.Empty)).Contains(";"))
          {
            this.CreateVersionFile(true);
          }
          else
          {
            string[] strArray2 = ServerRoles.Base64Decode(strArray1[0].Replace("UI Build GUID: ", string.Empty).Replace("-", string.Empty)).Split(';');
            if (strArray2.Length != 3 || strArray2[0] != this.CompatibleVersions[0])
              this.CreateVersionFile(true);
            else if (strArray2[2] != SteamUser.GetSteamID().ToString())
            {
              try
              {
                string plainText = strArray2[0] + ";" + strArray2[1] + ";" + (object) SteamUser.GetSteamID();
                File.Delete("PrivateBeta\\SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml");
                File.Create("PrivateBeta\\SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml").Close();
                FileManager.AppendFile("UI Build GUID: " + this.GUIDSplit(ServerRoles.Base64Encode(plainText)), "PrivateBeta\\SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml", true);
              }
              catch (Exception ex)
              {
                GameConsole.Console.singleton.AddLog("IO startup error 2.2", Color32.op_Implicit(Color.get_red()), false);
              }
            }
          }
        }
      }
    }
    // ISSUE: method pointer
    SceneManager.add_sceneLoaded(new UnityAction<Scene, LoadSceneMode>((object) this, __methodptr(OnLevelFinishedLoading)));
    this.get_connectionConfig().set_MaxSentMessageQueueSize((ushort) 300);
  }

  private string GUIDSplit(string GUID)
  {
    string str = string.Empty;
    for (; GUID.Length > 5; GUID = GUID.Substring(5))
      str = str + GUID.Substring(0, 5) + "-";
    return str + GUID;
  }

  private void CreateVersionFile(bool privbeta)
  {
    if (!privbeta)
    {
      try
      {
        if (File.Exists("SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml"))
          File.Delete("SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml");
        File.Create("SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml").Close();
        FileManager.AppendFile("UI Build GUID: " + this.GUIDSplit(ServerRoles.Base64Encode(this.CompatibleVersions[0] + ";" + (object) SteamUser.GetSteamID() + ";-")), "SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml", true);
      }
      catch
      {
        GameConsole.Console.singleton.AddLog("IO startup error 1.1", Color32.op_Implicit(Color.get_red()), false);
      }
    }
    else
    {
      if (!Directory.Exists("PrivateBeta"))
        return;
      try
      {
        if (File.Exists("PrivateBeta\\SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml"))
          File.Delete("PrivateBeta\\SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml");
        File.Create("PrivateBeta\\SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml").Close();
        FileManager.AppendFile("UI Build GUID: " + this.GUIDSplit(ServerRoles.Base64Encode(this.CompatibleVersions[0] + ";" + (object) SteamUser.GetSteamID() + ";-")), "PrivateBeta\\SCPSL_Data\\Managed\\UnityEngine.UIVersion.xml", true);
      }
      catch
      {
        GameConsole.Console.singleton.AddLog("IO startup error 1.2", Color32.op_Implicit(Color.get_red()), false);
      }
    }
  }

  public void CreateMatch()
  {
    this.ShowLog(13);
    this.createpop.SetActive(false);
    NetworkServer.Reset();
    this.set_networkPort(this.GetFreePort());
    this.set_maxConnections(ConfigFile.ServerConfig.GetInt("max_players", 20));
    ServerConsole.Port = this.get_networkPort();
    ServerConsole.AddLog("Config file loaded: " + ConfigFile.ConfigPath);
    this._queryEnabled = ConfigFile.ServerConfig.GetBool("enable_query", true);
    if (ConfigFile.ServerConfig.GetBool("forward_ports", true))
      this.UpnpStart();
    string str = FileManager.AppFolder + "config_remoteadmin.txt";
    if (!File.Exists(str))
      File.Copy("MiscData/remoteadmin_template.txt", str);
    ServerStatic.RolesConfigPath = str;
    ServerStatic.RolesConfig = new YamlConfig(str);
    ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig);
    Timing.RunCoroutine(this._CreateLobby());
    if (ServerStatic.IsDedicated)
      return;
    this.NonsteamHost();
  }

  [DebuggerHidden]
  private IEnumerator<float> _CreateLobby()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new CustomNetworkManager.\u003C_CreateLobby\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  private void NonsteamHost()
  {
    this.set_onlineScene("Facility");
    this.set_maxConnections(20);
    this.StartHostWithPort();
  }

  public void StartHostWithPort()
  {
    if (ConfigFile.ServerConfig.GetString("bind_ip", "ANY").ToUpper() == "ANY")
    {
      ServerConsole.AddLog("Server starting at all IP addresses and port " + (object) this.get_networkPort());
      this.set_serverBindToIP(false);
      this.StartHost();
    }
    else
    {
      ServerConsole.AddLog("Server starting at IP " + ConfigFile.ServerConfig.GetString("bind_ip", "ANY") + " and  port " + (object) this.get_networkPort());
      this.set_serverBindAddress(ConfigFile.ServerConfig.GetString("bind_ip", "ANY"));
      this.set_serverBindToIP(true);
      this.StartHost();
    }
  }

  public int GetFreePort()
  {
    ServerConsole.AddLog("Loading config...");
    ConfigFile.ServerConfig = ConfigFile.ReloadGameConfig(FileManager.AppFolder + "config_gameplay.txt", false);
    string q = string.Empty;
    try
    {
      q = "Failed to split ports.";
      int[] numArray = ConfigFile.ServerConfig.GetIntList("port_queue").ToArray();
      if (numArray.Length == 0)
        numArray = new int[8]
        {
          7777,
          7778,
          7779,
          7780,
          7781,
          7782,
          7783,
          7784
        };
      string str = string.Join(", ", new List<int>((IEnumerable<int>) numArray).ConvertAll<string>((Converter<int, string>) (i => i.ToString())).ToArray());
      if (numArray.Length == 0)
      {
        q = "Failed to detect ports.";
        throw new Exception();
      }
      ServerConsole.AddLog("Port queue loaded: " + str);
      foreach (int num in numArray)
      {
        ServerConsole.AddLog("Trying to init port: " + (object) num + "...");
        if (NetworkServer.Listen(num))
        {
          NetworkServer.Reset();
          ServerConsole.AddLog("Done!LOGTYPE-10");
          return num;
        }
        ServerConsole.AddLog("...failed.LOGTYPE-6");
      }
    }
    catch
    {
      ServerConsole.AddLog(q);
    }
    return 7777;
  }

  private void UpnpStart()
  {
    if (this._mappedDevices == null)
    {
      ServerConsole.AddLog("Automatic port forwarding using UPnP enabled!LOGTYPE-9");
      this._mappedDevices = new List<INatDevice>();
    }
    NatUtility.add_DeviceFound(new EventHandler<DeviceEventArgs>(this.DeviceFound));
    NatUtility.add_DeviceLost(new EventHandler<DeviceEventArgs>(this.DeviceLost));
    NatUtility.StartDiscovery();
  }

  private void UpnpStop()
  {
    NatUtility.StopDiscovery();
    using (List<INatDevice>.Enumerator enumerator = this._mappedDevices.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        INatDevice current = enumerator.Current;
        try
        {
          current.DeletePortMap(new Mapping((Protocol) 1, this.get_networkPort(), this.get_networkPort()));
          if (this._mappedDevices.Contains(current))
            this._mappedDevices.Remove(current);
          ServerConsole.AddLog("Removed forwarding rule on port " + (object) this.get_networkPort() + " from " + (object) current.GetExternalIP() + " to this device.LOGTYPE-10");
        }
        catch
        {
          ServerConsole.AddLog("Can't remove forwarding rule on port " + (object) this.get_networkPort() + " UDP from " + (object) current.GetExternalIP() + " to this device.LOGTYPE-12");
        }
        if (this._queryEnabled)
        {
          try
          {
            current.DeletePortMap(new Mapping((Protocol) 0, this._queryPort, this._queryPort));
            ServerConsole.AddLog("Removed forwarding rule on query port " + (object) this._queryPort + " from " + (object) current.GetExternalIP() + " to this device.LOGTYPE-10");
          }
          catch
          {
            ServerConsole.AddLog("Can't remove forwarding rule on query port " + (object) this._queryPort + " UDP from " + (object) current.GetExternalIP() + " to this device.LOGTYPE-12");
          }
        }
      }
    }
  }

  private void DeviceFound(object sender, DeviceEventArgs args)
  {
    INatDevice device = args.get_Device();
    try
    {
      device = args.get_Device();
      this._mappedDevices.Add(device);
      device.CreatePortMap(new Mapping((Protocol) 1, this.get_networkPort(), this.get_networkPort()));
      ServerConsole.AddLog("Forwarded port " + (object) this.get_networkPort() + " UDP (game port) from " + (object) device.GetExternalIP() + " to this device.LOGTYPE-10");
    }
    catch (Exception ex)
    {
      ServerConsole.AddLog("Can't forward port " + (object) this.get_networkPort() + " UDP from " + (object) device.GetExternalIP() + " to this device. Error: " + ex.Message + "LOGTYPE-12");
    }
    if (!this._queryEnabled)
      return;
    try
    {
      if (!this._queryEnabled)
        return;
      device.CreatePortMap(new Mapping((Protocol) 0, this._queryPort, this._queryPort));
      ServerConsole.AddLog("Forwarded port " + (object) this._queryPort + " TCP (query port) from " + (object) device.GetExternalIP() + " to this device.LOGTYPE-10");
    }
    catch (Exception ex)
    {
      ServerConsole.AddLog("Can't forward query port " + (object) this._queryPort + " TCP from " + (object) device.GetExternalIP() + " to this device. Error: " + ex.Message + "LOGTYPE-12");
    }
  }

  private void DeviceLost(object sender, DeviceEventArgs args)
  {
    INatDevice device = args.get_Device();
    try
    {
      device.DeletePortMap(new Mapping((Protocol) 1, this.get_networkPort(), this.get_networkPort()));
      if (this._mappedDevices.Contains(device))
        this._mappedDevices.Remove(device);
      ServerConsole.AddLog("Removed forwarding rule on port " + (object) this.get_networkPort() + " from " + (object) device.GetExternalIP() + " to this device.LOGTYPE-10");
    }
    catch
    {
      ServerConsole.AddLog("Can't remove forwarding rule on port " + (object) this.get_networkPort() + " UDP from " + (object) device.GetExternalIP() + " to this device.LOGTYPE-12");
    }
    if (!this._queryEnabled)
      return;
    try
    {
      device.DeletePortMap(new Mapping((Protocol) 0, this._queryPort, this._queryPort));
      ServerConsole.AddLog("Removed forwarding rule on query port " + (object) this._queryPort + " from " + (object) device.GetExternalIP() + " to this device.LOGTYPE-10");
    }
    catch
    {
      ServerConsole.AddLog("Can't remove forwarding rule on query port " + (object) this._queryPort + " UDP from " + (object) device.GetExternalIP() + " to this device.LOGTYPE-12");
    }
  }

  [Serializable]
  public class DisconnectLog
  {
    [Multiline]
    public string msg_en;
    public CustomNetworkManager.DisconnectLog.LogButton button;
    public bool autoHideOnSceneLoad;

    [Serializable]
    public class LogButton
    {
      public ConnInfoButton[] actions;
    }
  }
}
