// Decompiled with JetBrains decompiler
// Type: GameMenu
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Networking;

public class GameMenu : MonoBehaviour
{
  public GameObject background;
  public GameObject main;
  public GameObject[] minors;

  private void Update()
  {
    if (!Input.GetKeyDown(KeyCode.Escape) || CursorManager.eqOpen || CursorManager.consoleOpen)
      return;
    this.ToggleMenu();
  }

  public void ToggleMenu()
  {
    foreach (GameObject minor in this.minors)
    {
      if (minor.activeSelf)
        minor.SetActive(false);
    }
    this.background.SetActive(!this.background.activeSelf);
    CursorManager.pauseOpen = this.background.activeSelf;
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
        player.GetComponent<FirstPersonController>().isPaused = this.background.activeSelf;
    }
    this.main.SetActive(true);
  }

  public void Disconnect()
  {
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
      {
        if (player.GetComponent<NetworkIdentity>().isServer)
          Object.FindObjectOfType<NetworkManager>().StopHost();
        else
          Object.FindObjectOfType<NetworkManager>().StopClient();
      }
    }
  }
}
