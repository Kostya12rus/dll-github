// Decompiled with JetBrains decompiler
// Type: SimpleMenu
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SimpleMenu : MonoBehaviour
{
  public static string targetSceneName;
  private static bool server;

  public SimpleMenu()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    foreach (string commandLineArg in Environment.GetCommandLineArgs())
    {
      if (commandLineArg == "-fastmenu")
        PlayerPrefs.SetInt("fastmenu", 1);
      else if (commandLineArg == "-nographics")
        SimpleMenu.server = true;
    }
    this.Refresh();
  }

  public void ChangeMode()
  {
    PlayerPrefs.SetInt("fastmenu", PlayerPrefs.GetInt("fastmenu", 0) != 0 ? 0 : 1);
    this.Refresh();
    SceneManager.LoadScene("Loader");
  }

  private void Refresh()
  {
    SimpleMenu.targetSceneName = !SimpleMenu.server ? (PlayerPrefs.GetInt("fastmenu", 0) != 0 ? "FastMenu" : "MainMenuRemastered") : "FastMenu";
    ((NetworkManager) Object.FindObjectOfType<CustomNetworkManager>()).set_offlineScene(SimpleMenu.targetSceneName);
  }

  public static void LoadCorrectScene()
  {
    SceneManager.LoadScene(SimpleMenu.targetSceneName);
  }
}
