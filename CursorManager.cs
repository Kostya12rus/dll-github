// Decompiled with JetBrains decompiler
// Type: CursorManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
  public static bool eqOpen;
  public static bool pauseOpen;
  public static bool isServerOnly;
  public static bool consoleOpen;
  public static bool is079;
  public static bool scp106;
  public static bool roundStarted;
  public static bool raOp;
  public static bool plOp;
  public static bool debuglogopen;
  public static bool isNotFacility;
  public static bool isApplicationNotFocused;

  private void LateUpdate()
  {
    bool flag = CursorManager.eqOpen | CursorManager.pauseOpen | CursorManager.isServerOnly | CursorManager.consoleOpen | CursorManager.is079 | CursorManager.scp106 | CursorManager.roundStarted | CursorManager.raOp | CursorManager.plOp | CursorManager.isNotFacility | CursorManager.isApplicationNotFocused;
    Cursor.lockState = !flag ? CursorLockMode.Locked : CursorLockMode.None;
    Cursor.visible = flag;
  }

  private void OnEnable()
  {
    SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.OnSceneWasLoaded);
  }

  private void OnDisable()
  {
    SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.OnSceneWasLoaded);
  }

  private void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
  {
    CursorManager.UnsetAll();
    CursorManager.isNotFacility = SceneManager.GetActiveScene().name != "Facility";
  }

  private void OnApplicationFocus(bool focus)
  {
    CursorManager.isApplicationNotFocused = !focus;
  }

  public static void UnsetAll()
  {
    CursorManager.eqOpen = false;
    CursorManager.pauseOpen = false;
    CursorManager.isServerOnly = false;
    CursorManager.consoleOpen = false;
    CursorManager.is079 = false;
    CursorManager.scp106 = false;
    CursorManager.roundStarted = false;
    CursorManager.raOp = false;
    CursorManager.plOp = false;
    CursorManager.debuglogopen = false;
  }
}
