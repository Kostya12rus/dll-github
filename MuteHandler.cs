// Decompiled with JetBrains decompiler
// Type: MuteHandler
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Networking;

public class MuteHandler : NetworkBehaviour
{
  private static string _path;
  private static List<string> mutes;

  private void Start()
  {
    MuteHandler._path = FileManager.AppFolder + "mutes.txt";
    try
    {
      if (!Directory.Exists(FileManager.AppFolder))
        Directory.CreateDirectory(FileManager.AppFolder);
      if (File.Exists(MuteHandler._path))
        return;
      File.Create(MuteHandler._path).Close();
    }
    catch
    {
      ServerConsole.AddLog("Can't create mute file!");
    }
  }

  public static void IssuePersistantMute(string steamId)
  {
    steamId = steamId.Replace(";", ":").Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty);
    if (!((IEnumerable<string>) FileManager.ReadAllLines(MuteHandler._path)).Where<string>((Func<string, bool>) (b => b == steamId)).Any<string>())
    {
      FileManager.AppendFile(steamId, MuteHandler._path, true);
    }
    else
    {
      MuteHandler.RevokePersistantMute(steamId);
      MuteHandler.IssuePersistantMute(steamId);
    }
  }

  public static void RevokePersistantMute(string steamId)
  {
    steamId = steamId.Replace(";", ":").Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty);
    FileManager.WriteToFile(((IEnumerable<string>) FileManager.ReadAllLines(MuteHandler._path)).Where<string>((Func<string, bool>) (l => l != steamId)).ToArray<string>(), MuteHandler._path);
  }

  private void UNetVersion()
  {
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
