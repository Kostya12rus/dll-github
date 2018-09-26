// Decompiled with JetBrains decompiler
// Type: CentralServerKeyCache
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CentralServerKeyCache
{
  public const string CacheLocation = "internal/KeyCache";
  public const string InternalDir = "internal/";

  public static string ReadCache()
  {
    try
    {
      string path = FileManager.AppFolder + "internal/KeyCache";
      if (!File.Exists(path))
      {
        ServerConsole.AddLog("Central server public key not found in cache.");
        return (string) null;
      }
      string str = ((IEnumerable<string>) FileManager.ReadAllLines(path)).Aggregate<string, string>(string.Empty, (Func<string, string, string>) ((current, line) => current + line + "\n"));
      try
      {
        return str;
      }
      catch (Exception ex)
      {
        if (ServerStatic.IsDedicated)
          ServerConsole.AddLog("Can't load central server public key from cache - " + ex.Message);
        else
          GameConsole.Console.singleton.AddLog("Can't load central server public key from cache - " + ex.Message, (Color32) Color.magenta, false);
        return (string) null;
      }
    }
    catch (Exception ex)
    {
      ServerConsole.AddLog("Can't read public key cache - " + ex.Message);
      return (string) null;
    }
  }

  public static void SaveCache(string key)
  {
    try
    {
      string path = FileManager.AppFolder + "internal/KeyCache";
      if (!Directory.Exists(FileManager.AppFolder + "internal/"))
        Directory.CreateDirectory(FileManager.AppFolder + "internal/");
      if (File.Exists(path))
      {
        if (key == CentralServerKeyCache.ReadCache())
        {
          ServerConsole.AddLog("Key cache is up to date.");
          return;
        }
        File.Delete(path);
      }
      ServerConsole.AddLog("Updating key cache...");
      FileManager.WriteStringToFile(key, path);
      ServerConsole.AddLog("Key cache updated.");
    }
    catch (Exception ex)
    {
      ServerConsole.AddLog("Can't write public key cache - " + ex.Message);
    }
  }
}
