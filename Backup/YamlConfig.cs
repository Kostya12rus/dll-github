// Decompiled with JetBrains decompiler
// Type: YamlConfig
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class YamlConfig
{
  public string[] RawData;

  public YamlConfig()
  {
    this.RawData = new string[0];
  }

  public YamlConfig(string path)
  {
    this.LoadConfigFile(path);
  }

  public void LoadConfigFile(string path)
  {
    this.RawData = FileManager.ReadAllLines(path);
  }

  public string GetString(string key, string def = "")
  {
    foreach (string str in this.RawData)
    {
      if (str.StartsWith(key + ": "))
        return str.Substring(key.Length + 2);
    }
    return def;
  }

  public int GetInt(string key, int def = 0)
  {
    foreach (string str in this.RawData)
    {
      if (str.StartsWith(key + ": "))
      {
        try
        {
          return Convert.ToInt32(str.Substring(key.Length + 2));
        }
        catch
        {
          return 0;
        }
      }
    }
    return def;
  }

  public float GetFloat(string key, float def = 0.0f)
  {
    string str = this.GetString(key, string.Empty);
    float result;
    if (str == string.Empty || !float.TryParse(str.Replace(',', '.'), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      return def;
    return result;
  }

  public bool GetBool(string key, bool def = false)
  {
    foreach (string str in this.RawData)
    {
      if (str.StartsWith(key + ": "))
        return str.Substring(key.Length + 2) == "true";
    }
    return def;
  }

  public List<string> GetStringList(string key)
  {
    bool flag = false;
    List<string> stringList = new List<string>();
    foreach (string str in this.RawData)
    {
      if (str.StartsWith(key + ":"))
        flag = true;
      else if (flag)
      {
        if (str.StartsWith(" - "))
          stringList.Add(str.Substring(3));
        else if (!str.StartsWith("#"))
          break;
      }
    }
    return stringList;
  }

  public List<int> GetIntList(string key)
  {
    List<string> stringList = this.GetStringList(key);
    // ISSUE: reference to a compiler-generated field
    if (YamlConfig.\u003C\u003Ef__mg\u0024cache0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      YamlConfig.\u003C\u003Ef__mg\u0024cache0 = new Func<string, int>(Convert.ToInt32);
    }
    // ISSUE: reference to a compiler-generated field
    Func<string, int> fMgCache0 = YamlConfig.\u003C\u003Ef__mg\u0024cache0;
    return stringList.Select<string, int>(fMgCache0).ToList<int>();
  }

  public Dictionary<string, string> GetStringDictionary(string key)
  {
    List<string> stringList = this.GetStringList(key);
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    foreach (string str in stringList)
    {
      int length = str.IndexOf(": ", StringComparison.Ordinal);
      dictionary.Add(str.Substring(0, length), str.Substring(length + 2));
    }
    return dictionary;
  }

  public static string[] ParseCommaSeparatedString(string data)
  {
    if (!data.StartsWith("[") || !data.EndsWith("]"))
      return (string[]) null;
    data = data.Substring(1, data.Length - 2);
    return data.Split(new string[1]{ ", " }, StringSplitOptions.None);
  }
}
