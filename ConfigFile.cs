// Decompiled with JetBrains decompiler
// Type: ConfigFile
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigFile : MonoBehaviour
{
  public static YamlConfig ServerConfig;
  public static YamlConfig HosterPolicy;
  internal static string ConfigPath;
  public static Dictionary<string, int[]> smBalancedPicker;

  private void Start()
  {
    if (!Directory.Exists(FileManager.AppFolder))
      Directory.CreateDirectory(FileManager.AppFolder);
    if (!File.Exists(FileManager.AppFolder + "config.txt") || File.Exists(FileManager.AppFolder + "LEGANCY CONFIG BACKUP - NOT WORKING.txt"))
      return;
    File.Move(FileManager.AppFolder + "config.txt", FileManager.AppFolder + "LEGANCY CONFIG BACKUP - NOT WORKING.txt");
  }

  public static void ReloadGameConfig()
  {
    if (ConfigFile.ServerConfig == null)
      throw new IOException("Please use ReloadGameConfig() with arguments first!");
    ConfigFile.ServerConfig = ConfigFile.ReloadGameConfig(ConfigFile.ConfigPath, true);
  }

  public static YamlConfig ReloadGameConfig(string path, bool notSet = false)
  {
    if (!notSet)
      ConfigFile.ConfigPath = path;
    if (!Directory.Exists(FileManager.AppFolder))
      Directory.CreateDirectory(FileManager.AppFolder);
    if (File.Exists(path))
      return new YamlConfig(path);
    try
    {
      File.Copy("MiscData/gameconfig_template.txt", path);
    }
    catch
    {
      ServerConsole.AddLog("Error during copying config file!");
      return (YamlConfig) null;
    }
    return new YamlConfig(path);
  }
}
