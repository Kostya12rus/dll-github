// Decompiled with JetBrains decompiler
// Type: DebugScreenController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Diagnostics;
using UnityEngine;

public class DebugScreenController : MonoBehaviour
{
  public GameObject GUI;
  public static int asserts;
  public static int errors;
  public static int exceptions;

  private void Start()
  {
    Object.DontDestroyOnLoad((Object) this.gameObject);
    Application.logMessageReceived += new Application.LogCallback(this.LogMessage);
    if (Process.GetProcessesByName("SharpMonoInjector").Length <= 0)
      return;
    Application.Quit();
  }

  private void LogMessage(string condition, string stackTrace, LogType type)
  {
    switch (type)
    {
      case LogType.Error:
        ++DebugScreenController.errors;
        break;
      case LogType.Assert:
        ++DebugScreenController.asserts;
        break;
      case LogType.Exception:
        ++DebugScreenController.exceptions;
        break;
    }
  }

  private void Update()
  {
    if (!Input.GetKeyDown(KeyCode.F3))
      return;
    this.GUI.SetActive(!this.GUI.activeSelf);
  }
}
