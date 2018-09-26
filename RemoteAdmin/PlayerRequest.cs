// Decompiled with JetBrains decompiler
// Type: RemoteAdmin.PlayerRequest
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
  public class PlayerRequest : MonoBehaviour
  {
    private readonly List<GameObject> _spawns = new List<GameObject>();
    public Transform parent;
    public GameObject template;
    public static PlayerRequest singleton;

    private void Awake()
    {
      PlayerRequest.singleton = this;
    }

    public void ResponsePlayerList(string data, bool isSuccess, bool showClasses)
    {
      if (!isSuccess)
        return;
      List<string> stringList = new List<string>();
      foreach (PlayerRecord record in PlayerRecord.records)
      {
        if (record.isSelected)
          stringList.Add(record.playerId);
      }
      PlayerRecord.records = new List<PlayerRecord>();
      foreach (Object spawn in this._spawns)
        Object.Destroy(spawn);
      string str1 = data;
      string[] separator = new string[1]{ "\n" };
      int num = 0;
      foreach (string str2 in str1.Split(separator, (StringSplitOptions) num))
      {
        if (!string.IsNullOrEmpty(str2))
        {
          bool flag = str2.Contains("<OVRM>");
          GameObject gameObject = Object.Instantiate<GameObject>(this.template, this.parent);
          PlayerRecord componentInChildren = gameObject.GetComponentInChildren<PlayerRecord>();
          gameObject.transform.localScale = Vector3.one;
          gameObject.GetComponentInChildren<Text>().text = str2.Replace("<OVRM>", string.Empty);
          this._spawns.Add(gameObject);
          componentInChildren.Setup(Color.white);
          string str3 = str2.Replace("<OVRM>", string.Empty);
          string str4 = str3.Remove(0, str3.IndexOf("(", StringComparison.Ordinal) + 1);
          string str5 = str4.Remove(str4.IndexOf(")", StringComparison.Ordinal));
          componentInChildren.playerId = str5;
          if (stringList.Contains(str5))
            componentInChildren.Toggle();
          if (flag)
            componentInChildren.Setup(new Color(0.0f, 128f, 128f));
          else if (showClasses)
          {
            foreach (GameObject player in PlayerManager.singleton.players)
            {
              if (!(player.GetComponent<QueryProcessor>().PlayerId.ToString() != str5))
              {
                CharacterClassManager component = player.GetComponent<CharacterClassManager>();
                componentInChildren.Setup(component.curClass != 15 ? (component.curClass >= 0 ? component.klasy[component.curClass].classColor : Color.white) : new Color(0.7f, 0.7f, 0.7f));
              }
            }
          }
        }
      }
    }

    public void ResponsePlayerSpecific(string data, bool isSuccess)
    {
      if (!isSuccess)
        data = "<color=red>" + data + "</color>";
      this.GetComponent<DisplayDataOnScreen>().Show(1, data);
    }
  }
}
