// Decompiled with JetBrains decompiler
// Type: WhiteList
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class WhiteList : MonoBehaviour
{
  public static List<string> SteamIDs;

  private void Start()
  {
    WhiteList.ReloadWhitelist();
  }

  public static void ReloadWhitelist()
  {
    string path = FileManager.AppFolder + "SteamIDWhitelist.txt";
    if (!Directory.Exists(FileManager.AppFolder))
      Directory.CreateDirectory(FileManager.AppFolder);
    if (!File.Exists(path))
      File.Create(path).Close();
    WhiteList.SteamIDs = ((IEnumerable<string>) FileManager.ReadAllLines(path)).Where<string>((Func<string, bool>) (id => !string.IsNullOrEmpty(id))).ToList<string>();
  }

  public static bool IsWhitelisted(string steamId)
  {
    if (!WhiteList.SteamIDs.Contains(steamId) && ConfigFile.ServerConfig.GetBool("enable_whitelist", false))
      return !ConfigFile.ServerConfig.GetBool("online_mode", true);
    return true;
  }
}
