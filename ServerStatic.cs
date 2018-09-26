// Decompiled with JetBrains decompiler
// Type: ServerStatic
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ServerStatic : MonoBehaviour
{
  public static bool IsDedicated;
  public bool Simulate;
  internal static YamlConfig RolesConfig;
  internal static string RolesConfigPath;
  internal static PermissionsHandler PermissionsHandler;

  private void Awake()
  {
    foreach (string commandLineArg in Environment.GetCommandLineArgs())
    {
      if (commandLineArg == "-nographics" && !this.Simulate)
        this.Simulate = true;
      if (commandLineArg.Contains("-key"))
        ServerConsole.Session = commandLineArg.Remove(0, 4);
      if (commandLineArg.Contains("-id"))
      {
        foreach (Process process in Process.GetProcesses())
        {
          if (process.Id.ToString() == commandLineArg.Remove(0, 3))
            ServerConsole.ConsoleId = process;
        }
      }
    }
    if (this.Simulate)
    {
      ServerStatic.IsDedicated = true;
      AudioListener.volume = 0.0f;
      ServerConsole.AddLog("SCP Secret Laboratory process started. Creating match... LOGTYPE02");
    }
    SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.OnSceneWasLoaded);
  }

  private void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
  {
    if (!ServerStatic.IsDedicated || scene.buildIndex != 1 && scene.buildIndex != 3)
      return;
    this.GetComponent<CustomNetworkManager>().CreateMatch();
  }

  public static PermissionsHandler GetPermissionsHandler()
  {
    return ServerStatic.PermissionsHandler;
  }
}
