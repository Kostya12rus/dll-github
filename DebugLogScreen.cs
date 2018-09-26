// Decompiled with JetBrains decompiler
// Type: DebugLogScreen
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class DebugLogScreen : MonoBehaviour
{
  public GameObject log;
  public GameObject info;

  public DebugLogScreen()
  {
    base.\u002Ector();
  }

  private void OnEnable()
  {
    this.info.SetActive(true);
    CursorManager.debuglogopen = this.log.get_activeSelf();
  }

  private void OnDisable()
  {
    this.info.SetActive(false);
    CursorManager.debuglogopen = false;
  }

  private void Update()
  {
    if (!Input.GetKeyDown((KeyCode) 285))
      return;
    this.log.SetActive(!this.log.get_activeSelf());
    CursorManager.debuglogopen = this.log.get_activeSelf();
  }
}
