// Decompiled with JetBrains decompiler
// Type: FileManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class FileManager
{
  public static string AppFolder
  {
    get
    {
      if (ConfigFile.HosterPolicy != null && ConfigFile.HosterPolicy.GetBool("gamedir_for_configs", false))
        return "AppData/";
      return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/";
    }
  }

  public static string[] ReadAllLines(string path)
  {
    return File.ReadAllLines(path, Encoding.UTF8);
  }

  public static void WriteToFile(string[] data, string path)
  {
    File.WriteAllLines(path, data, Encoding.UTF8);
  }

  public static void WriteStringToFile(string data, string path)
  {
    File.WriteAllText(path, data, Encoding.UTF8);
  }

  public static void AppendFile(string data, string path, bool newLine = true)
  {
    string[] strArray = FileManager.ReadAllLines(path);
    if (!newLine || strArray.Length == 0 || (strArray[strArray.Length - 1].EndsWith(Environment.NewLine) || strArray[strArray.Length - 1].EndsWith("\n")))
      File.AppendAllText(path, data, Encoding.UTF8);
    else
      File.AppendAllText(path, Environment.NewLine + data, Encoding.UTF8);
  }

  public static void RenameFile(string path, string newpath)
  {
    File.Move(path, newpath);
  }

  public static void DeleteFile(string path)
  {
    File.Delete(path);
  }

  public static void ReplaceLine(int line, string text, string path)
  {
    string[] data = FileManager.ReadAllLines(path);
    data[line] = text;
    FileManager.WriteToFile(data, path);
  }

  public static void RemoveEmptyLines(string path)
  {
    FileManager.WriteToFile(((IEnumerable<string>) FileManager.ReadAllLines(path)).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s.Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty)))).ToArray<string>(), path);
  }
}
