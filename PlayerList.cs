// Decompiled with JetBrains decompiler
// Type: PlayerList
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerList : MonoBehaviour
{
  public static List<PlayerList.Instance> instances = new List<PlayerList.Instance>();
  public Transform parent;
  public Transform template;
  public GameObject panel;
  private static Transform s_parent;
  private static Transform s_template;
  private KeyCode openKey;

  public PlayerList()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    if (Input.GetKeyDown(this.openKey))
    {
      if (this.panel.get_activeSelf())
        this.panel.SetActive(false);
      else if (!Cursor.get_visible())
        this.panel.SetActive(true);
      CursorManager.plOp = this.panel.get_activeSelf();
    }
    if (!Input.GetKeyDown((KeyCode) 27))
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
    GameObject gameObject = (GameObject) Object.Instantiate<GameObject>((M0) ((Component) PlayerList.s_template).get_gameObject(), PlayerList.s_parent);
    gameObject.get_transform().set_localScale(Vector3.get_one());
    ((TMP_Text) gameObject.GetComponentInChildren<TextMeshProUGUI>()).set_text(((NicknameSync) instance.GetComponent<NicknameSync>()).myNick);
    ((PlayerListElement) gameObject.GetComponent<PlayerListElement>()).instance = instance;
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
      if (!Object.op_Inequality((Object) instance, (Object) instance1.owner))
      {
        ((Graphic) instance1.text.GetComponentInChildren<TextMeshProUGUI>()).set_color(((ServerRoles) instance.GetComponent<ServerRoles>()).GetColor());
        ((TMP_Text) instance1.text.GetComponentInChildren<TextMeshProUGUI>()).set_text(((NicknameSync) instance.GetComponent<NicknameSync>()).myNick + " <size=12>" + ((ServerRoles) instance.GetComponent<ServerRoles>()).GetColoredRoleString(false) + "</size>");
      }
    }
  }

  public static void DestroyPlayer(GameObject instance)
  {
    foreach (PlayerList.Instance instance1 in PlayerList.instances)
    {
      if (!Object.op_Inequality((Object) instance1.owner, (Object) instance))
      {
        Object.Destroy((Object) instance1.text.get_gameObject());
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
