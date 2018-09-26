// Decompiled with JetBrains decompiler
// Type: RemoteAdmin.PlayerRecord
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
  public class PlayerRecord : MonoBehaviour
  {
    public static List<PlayerRecord> records = new List<PlayerRecord>();
    public Text myText;
    public bool isSelected;
    public string playerId;

    public void Toggle()
    {
      bool flag = !this.isSelected;
      if (!Input.GetKey(KeyCode.LeftControl))
      {
        foreach (PlayerRecord record in PlayerRecord.records)
        {
          if (record.isSelected)
          {
            record.isSelected = false;
            flag = true;
          }
        }
      }
      this.isSelected = flag;
      if (!(this.playerId == "unconnected"))
        return;
      this.isSelected = false;
    }

    private void LateUpdate()
    {
      this.myText.GetComponent<Outline>().enabled = this.isSelected;
    }

    private void Start()
    {
      PlayerRecord.records.Add(this);
    }

    public void Setup(Color c)
    {
      this.myText.color = c;
    }
  }
}
