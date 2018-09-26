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

  public DebugScreenController()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    Object.DontDestroyOnLoad((Object) ((Component) this).get_gameObject());
    // ISSUE: method pointer
    Application.add_logMessageReceived(new Application.LogCallback((object) this, __methodptr(LogMessage)));
    if (Process.GetProcessesByName("SharpMonoInjector").Length <= 0)
      return;
    Application.Quit();
  }

  private void LogMessage(string condition, string stackTrace, LogType type)
  {
    if (type == 1)
      ++DebugScreenController.asserts;
    else if (type == null)
    {
      ++DebugScreenController.errors;
    }
    else
    {
      if (type != 4)
        return;
      ++DebugScreenController.exceptions;
    }
  }

  private void Update()
  {
    if (!Input.GetKeyDown((KeyCode) 284))
      return;
    this.GUI.SetActive(!this.GUI.get_activeSelf());
  }
}
