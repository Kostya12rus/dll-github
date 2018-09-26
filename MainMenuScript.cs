// Decompiled with JetBrains decompiler
// Type: MainMenuScript
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Steamworks;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
  public GameObject[] submenus;
  private CustomNetworkManager _mng;
  public int CurMenu;
  private bool allowQuit;

  public MainMenuScript()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    Scene activeScene = SceneManager.GetActiveScene();
    if (!((Scene) ref activeScene).get_name().ToLower().Contains("menu"))
      return;
    CursorManager.UnsetAll();
    Cursor.set_lockState((CursorLockMode) 0);
    Cursor.set_visible(true);
  }

  public void SetIP(string ip)
  {
    NetworkServer.Reset();
    this._mng.set_networkPort(7777);
    try
    {
      this._mng.set_networkPort(int.Parse(ip.Remove(0, ip.LastIndexOf(":", StringComparison.Ordinal) + 1)));
    }
    catch
    {
    }
    this._mng.set_networkAddress(!ip.Contains(":") ? ip : ip.Remove(ip.IndexOf(":", StringComparison.Ordinal)));
    CustomNetworkManager.ConnectionIp = !ip.Contains(":") ? ip : ip.Remove(ip.IndexOf(":", StringComparison.Ordinal));
  }

  public void ChangeMenu(int id)
  {
    this.CurMenu = id;
    for (int index = 0; index < this.submenus.Length; ++index)
      this.submenus[index].SetActive(index == id);
  }

  public void QuitGame()
  {
    this.allowQuit = true;
    Application.Quit();
  }

  private void Start()
  {
    this._mng = (CustomNetworkManager) Object.FindObjectOfType<CustomNetworkManager>();
    CursorManager.UnsetAll();
    if (SteamManager.Initialized)
      SteamUserStats.SetAchievement("TEST_1");
    this.ChangeMenu(0);
  }

  private void OnApplicationQuit()
  {
    if (this.allowQuit || Input.GetKey((KeyCode) 308))
      return;
    Application.CancelQuit();
  }

  public void StartServer()
  {
    this._mng.set_onlineScene("Facility");
    this._mng.set_maxConnections(20);
    this._mng.createpop.SetActive(true);
  }

  public void StartTutorial(string scene)
  {
    this._mng.set_onlineScene(scene);
    this._mng.set_maxConnections(1);
    this._mng.ShowLog(15);
    this._mng.StartHost();
  }

  public void Connect()
  {
    if (CrashDetector.Show())
      return;
    NetworkServer.Reset();
    this._mng.ShowLog(13);
    this._mng.StartClient();
  }
}
