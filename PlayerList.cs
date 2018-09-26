// Decompiled with JetBrains decompiler
// Type: PlayerList
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
  public static List<PlayerList.Instance> instances = new List<PlayerList.Instance>();
  public Transform parent;
  public Transform template;
  public GameObject panel;
  private static Transform s_parent;
  private static Transform s_template;
  private KeyCode openKey;

  private void Update()
  {
    if (Input.GetKeyDown(this.openKey))
    {
      if (this.panel.activeSelf)
        this.panel.SetActive(false);
      else if (!Cursor.visible)
        this.panel.SetActive(true);
      CursorManager.plOp = this.panel.activeSelf;
    }
    if (!Input.GetKeyDown(KeyCode.Escape))
      return;
    CursorManager.plOp = false;
    this.panel.SetActive(false);
  }

  private void Start()
  {
    this.openKey = NewInput.GetKey("Player List");
    PlayerList.s_parent = this.parent;
    PlayerList.s_template = this.template;
  }

  public static void AddPlayer(GameObject instance)
  {
    GameObject gameObject = Object.Instantiate<GameObject>(PlayerList.s_template.gameObject, PlayerList.s_parent);
    gameObject.transform.localScale = Vector3.one;
    gameObject.GetComponentInChildren<TextMeshProUGUI>().text = instance.GetComponent<NicknameSync>().myNick;
    gameObject.GetComponent<PlayerListElement>().instance = instance;
    PlayerList.instances.Add(new PlayerList.Instance()
    {
      owner = instance,
      text = gameObject
    });
    PlayerList.UpdatePlayerRole(instance);
  }

  public static void UpdatePlayerRole(GameObject instance)
  {
    foreach (PlayerList.Instance instance1 in PlayerList.instances)
    {
      if (!((Object) instance != (Object) instance1.owner))
      {
        instance1.text.GetComponentInChildren<TextMeshProUGUI>().color = instance.GetComponent<ServerRoles>().GetColor();
        instance1.text.GetComponentInChildren<TextMeshProUGUI>().text = instance.GetComponent<NicknameSync>().myNick + " <size=12>" + instance.GetComponent<ServerRoles>().GetColoredRoleString(false) + "</size>";
      }
    }
  }

  public static void DestroyPlayer(GameObject instance)
  {
    foreach (PlayerList.Instance instance1 in PlayerList.instances)
    {
      if (!((Object) instance1.owner != (Object) instance))
      {
        Object.Destroy((Object) instance1.text.gameObject);
        PlayerList.instances.Remove(instance1);
        break;
      }
    }
  }

  [Serializable]
  public class Instance
  {
    public GameObject text;
    public GameObject owner;
  }
}
