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
  private List<GameObject> lines = new List<GameObject>();
  public ScrollRect scroll;
  public GameObject parent;
  public GameObject prefab;

  private void OnEnable()
  {
    try
    {
      string url;
      switch (SystemInfo.operatingSystemFamily)
      {
        case OperatingSystemFamily.Windows:
          url = "file://c:/Users/" + Environment.UserName + "/AppData/LocalLow/Hubert Moszka/SCP_ Secret Laboratory/output_log.txt";
          break;
        case OperatingSystemFamily.Linux:
          url = "file://" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/unity3d/Hubert Moszka/SCP_ Secret Laboratory/Player.log";
          break;
        default:
          return;
      }
      if (File.Exists(url.Replace("file://", string.Empty)))
      {
        using (WWW www = new WWW(url))
        {
          string[] strArray = www.text.Trim().Split(Environment.NewLine.ToCharArray());
          foreach (Object line in this.lines)
            Object.Destroy(line);
          this.lines.Clear();
          foreach (string str in strArray)
          {
            if (!string.IsNullOrEmpty(str))
            {
              GameObject gameObject = Object.Instantiate<GameObject>(this.prefab, this.parent.transform);
              gameObject.GetComponent<Text>().text = str.Trim();
              this.lines.Add(gameObject);
            }
          }
        }
      }
      this.scroll.velocity = new Vector2(0.0f, 99999f);
    }
    catch
    {
    }
  }
}
