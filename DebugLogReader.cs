// Decompiled with JetBrains decompiler
// Type: DebugLogReader
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogReader : MonoBehaviour
{
  public ScrollRect scroll;
  public GameObject parent;
  public GameObject prefab;
  private List<GameObject> lines;

  public DebugLogReader()
  {
    base.\u002Ector();
  }

  private void OnEnable()
  {
    try
    {
      string str1;
      if (SystemInfo.get_operatingSystemFamily() == 2)
      {
        str1 = "file://c:/Users/" + Environment.UserName + "/AppData/LocalLow/Hubert Moszka/SCP_ Secret Laboratory/output_log.txt";
      }
      else
      {
        if (SystemInfo.get_operatingSystemFamily() != 3)
          return;
        str1 = "file://" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/unity3d/Hubert Moszka/SCP_ Secret Laboratory/Player.log";
      }
      if (File.Exists(str1.Replace("file://", string.Empty)))
      {
        using (WWW www = new WWW(str1))
        {
          string[] strArray = www.get_text().Trim().Split(Environment.NewLine.ToCharArray());
          using (List<GameObject>.Enumerator enumerator = this.lines.GetEnumerator())
          {
            while (enumerator.MoveNext())
              Object.Destroy((Object) enumerator.Current);
          }
          this.lines.Clear();
          foreach (string str2 in strArray)
          {
            if (!string.IsNullOrEmpty(str2))
            {
              GameObject gameObject = (GameObject) Object.Instantiate<GameObject>((M0) this.prefab, this.parent.get_transform());
              ((Text) gameObject.GetComponent<Text>()).set_text(str2.Trim());
              this.lines.Add(gameObject);
            }
          }
        }
      }
      this.scroll.set_velocity(new Vector2(0.0f, 99999f));
    }
    catch
    {
    }
  }
}
