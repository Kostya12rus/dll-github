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

  public GameMenu()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    if (!Input.GetKeyDown((KeyCode) 27) || CursorManager.eqOpen || CursorManager.consoleOpen)
      return;
    this.ToggleMenu();
  }

  public void ToggleMenu()
  {
    foreach (GameObject minor in this.minors)
    {
      if (minor.get_activeSelf())
        minor.SetActive(false);
    }
    this.background.SetActive(!this.background.get_activeSelf());
    CursorManager.pauseOpen = this.background.get_activeSelf();
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if (((NetworkIdentity) player.GetComponent<NetworkIdentity>()).get_isLocalPlayer())
        ((FirstPersonController) player.GetComponent<FirstPersonController>()).isPaused = (__Null) (this.background.get_activeSelf() ? 1 : 0);
    }
    this.main.SetActive(true);
  }

  public void Disconnect()
  {
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      if (((NetworkIdentity) player.GetComponent<NetworkIdentity>()).get_isLocalPlayer())
      {
        if (((NetworkIdentity) player.GetComponent<NetworkIdentity>()).get_isServer())
          ((NetworkManager) Object.FindObjectOfType<NetworkManager>()).StopHost();
        else
          ((NetworkManager) Object.FindObjectOfType<NetworkManager>()).StopClient();
      }
    }
  }
}
