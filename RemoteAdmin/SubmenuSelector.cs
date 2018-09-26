// Decompiled with JetBrains decompiler
// Type: RemoteAdmin.SubmenuSelector
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
  public class SubmenuSelector : MonoBehaviour
  {
    public Color c_selected;
    public Color c_deselected;
    public SubmenuSelector.SubMenu[] menus;
    private string[] arguments;
    private int currentMenu;
    public static SubmenuSelector singleton;

    private void Start()
    {
      this.menus[0].panel.SetActive(true);
      this.SelectMenu(0);
      foreach (SubmenuSelector.SubMenu menu in this.menus)
        menu.button.interactable = true;
    }

    private void Awake()
    {
      SubmenuSelector.singleton = this;
    }

    public void SetProperty(int field, string value)
    {
      this.arguments[field - 1] = value;
      if ((Object) this.menus[this.currentMenu].submitButton == (Object) null)
        return;
      this.menus[this.currentMenu].submitButton.interactable = true;
      foreach (string str in this.arguments)
      {
        if (string.IsNullOrEmpty(str) && this.arguments.Length > 0)
          this.menus[this.currentMenu].submitButton.interactable = false;
      }
    }

    public void Confirm()
    {
      if ((Object) this.menus[this.currentMenu].optionalDisplay != (Object) null)
        this.menus[this.currentMenu].optionalDisplay.text = string.Empty;
      string str1 = this.menus[this.currentMenu].commandTemplate;
      List<string> stringList = new List<string>();
      string str2 = string.Empty;
      foreach (PlayerRecord record in PlayerRecord.records)
      {
        if (record.isSelected)
          str2 = str2 + record.playerId + ".";
      }
      stringList.Add(str2);
      stringList.AddRange((IEnumerable<string>) this.arguments);
      if (str1.Contains("{0}"))
      {
        try
        {
          str1 = string.Format(str1, (object[]) stringList.ToArray());
        }
        catch
        {
          Debug.Log((object) (str1 + ":" + (object) stringList.Count));
        }
      }
      PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery(str1);
    }

    public void AdminToolsConfirm(string operation)
    {
      string str = string.Empty;
      foreach (PlayerRecord record in PlayerRecord.records)
      {
        if (record.isSelected)
          str = str + record.playerId + ".";
      }
      if (operation == null)
        return;
      // ISSUE: reference to a compiler-generated field
      if (SubmenuSelector.\u003C\u003Ef__switch\u0024map4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        SubmenuSelector.\u003C\u003Ef__switch\u0024map4 = new Dictionary<string, int>(13)
        {
          {
            "OverwatchEnable",
            0
          },
          {
            "OverwatchDisable",
            1
          },
          {
            "BypassEnable",
            2
          },
          {
            "BypassDisable",
            3
          },
          {
            "GodEnable",
            4
          },
          {
            "GodDisable",
            5
          },
          {
            "Heal",
            6
          },
          {
            "Lockdown",
            7
          },
          {
            "Open",
            8
          },
          {
            "Close",
            9
          },
          {
            "Lock",
            10
          },
          {
            "Unlock",
            11
          },
          {
            "Destroy",
            12
          }
        };
      }
      int num;
      // ISSUE: reference to a compiler-generated field
      if (!SubmenuSelector.\u003C\u003Ef__switch\u0024map4.TryGetValue(operation, out num))
        return;
      switch (num)
      {
        case 0:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("overwatch " + str + " 1");
          break;
        case 1:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("overwatch " + str + " 0");
          break;
        case 2:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("bypass " + str + " 1");
          break;
        case 3:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("bypass " + str + " 0");
          break;
        case 4:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("god " + str + " 1");
          break;
        case 5:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("god " + str + " 0");
          break;
        case 6:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("heal " + str);
          break;
        case 7:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("lockdown");
          break;
        case 8:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("open " + DoorPrinter.SelectedDoors);
          break;
        case 9:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("close " + DoorPrinter.SelectedDoors);
          break;
        case 10:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("lock " + DoorPrinter.SelectedDoors);
          break;
        case 11:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("unlock " + DoorPrinter.SelectedDoors);
          break;
        case 12:
          PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery("destroy " + DoorPrinter.SelectedDoors);
          break;
      }
    }

    public void RunCommand(string command)
    {
      PlayerManager.localPlayer.GetComponent<QueryProcessor>().CmdSendQuery(command);
    }

    public void SelectMenu(Button b)
    {
      for (int i = 0; i < this.menus.Length; ++i)
      {
        bool flag = (Object) this.menus[i].button == (Object) b;
        this.menus[i].button.GetComponent<Text>().color = !flag ? this.c_deselected : this.c_selected;
        this.menus[i].panel.SetActive(flag);
        if (flag)
          this.SelectMenu(i);
      }
    }

    public void SelectMenu(int i)
    {
      this.currentMenu = i;
      this.arguments = new string[this.menus[i].argumentsCount];
    }

    [Serializable]
    public class SubMenu
    {
      public Button button;
      public int argumentsCount;
      public string commandTemplate;
      public GameObject panel;
      public TextMeshProUGUI optionalDisplay;
      public Button submitButton;
    }
  }
}
