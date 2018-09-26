// Decompiled with JetBrains decompiler
// Type: PlayerPrefsSL
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerPrefsSL : MonoBehaviour
{
  private static string floatheader = "float_";
  private static string intheader = "int_";
  private static string stringheader = "string_";
  private static string boolheader = "bool_";
  private static string path = "registry.txt";
  private static bool isRegistryAllowed;
  private static string[] registry;

  private void Awake()
  {
    PlayerPrefsSL.isRegistryAllowed = SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows && (!File.Exists("useregistry.txt") ? 0 : (File.ReadAllText("useregistry.txt").Trim().ToLower() == "useregistry: false".Trim().ToLower() ? 1 : 0)) == 0;
    PlayerPrefsSL.Refresh();
  }

  private static bool Refresh()
  {
    if (PlayerPrefsSL.isRegistryAllowed)
      return false;
    if (!File.Exists(PlayerPrefsSL.path))
      File.Create(PlayerPrefsSL.path).Close();
    PlayerPrefsSL.registry = File.ReadAllLines(PlayerPrefsSL.path);
    return true;
  }

  private static string[] RemoveElement(string element, string[] myArray)
  {
    return ((IEnumerable<string>) myArray).Where<string>((Func<string, bool>) (w => !w.StartsWith(element))).ToArray<string>();
  }

  public static void SetFloat(string key, float value)
  {
    if (PlayerPrefsSL.Refresh())
    {
      bool flag = false;
      for (int index = 0; index < PlayerPrefsSL.registry.Length - 1; ++index)
      {
        if (PlayerPrefsSL.registry[index].StartsWith(PlayerPrefsSL.floatheader + key + ":"))
        {
          PlayerPrefsSL.registry[index] = PlayerPrefsSL.floatheader + key + ": " + (object) value;
          flag = true;
        }
      }
      if (!flag)
      {
        using (StreamWriter streamWriter = File.AppendText(PlayerPrefsSL.path))
          streamWriter.WriteLine(PlayerPrefsSL.floatheader + key + ": " + (object) value);
      }
      else
        File.WriteAllLines(PlayerPrefsSL.path, PlayerPrefsSL.registry);
    }
    else
      PlayerPrefs.SetFloat(PlayerPrefsSL.floatheader + key, value);
  }

  public static void SetInt(string key, int value)
  {
    if (PlayerPrefsSL.Refresh())
    {
      bool flag = false;
      for (int index = 0; index < PlayerPrefsSL.registry.Length - 1; ++index)
      {
        if (PlayerPrefsSL.registry[index].StartsWith(PlayerPrefsSL.intheader + key + ":"))
        {
          PlayerPrefsSL.registry[index] = PlayerPrefsSL.intheader + key + ": " + (object) value;
          flag = true;
        }
      }
      if (!flag)
      {
        using (StreamWriter streamWriter = File.AppendText(PlayerPrefsSL.path))
          streamWriter.WriteLine(PlayerPrefsSL.intheader + key + ": " + (object) value);
      }
      else
        File.WriteAllLines(PlayerPrefsSL.path, PlayerPrefsSL.registry);
    }
    else
      PlayerPrefs.SetInt(PlayerPrefsSL.intheader + key, value);
  }

  public static void SetString(string key, string value)
  {
    if (PlayerPrefsSL.Refresh())
    {
      bool flag = false;
      for (int index = 0; index < PlayerPrefsSL.registry.Length - 1; ++index)
      {
        if (PlayerPrefsSL.registry[index].StartsWith(PlayerPrefsSL.stringheader + key + ":"))
        {
          PlayerPrefsSL.registry[index] = PlayerPrefsSL.stringheader + key + ": " + value;
          flag = true;
        }
      }
      if (!flag)
      {
        using (StreamWriter streamWriter = File.AppendText(PlayerPrefsSL.path))
          streamWriter.WriteLine(PlayerPrefsSL.stringheader + key + ": " + value);
      }
      else
        File.WriteAllLines(PlayerPrefsSL.path, PlayerPrefsSL.registry);
    }
    else
      PlayerPrefs.SetString(PlayerPrefsSL.stringheader + key, value);
  }

  public static void SetBool(string key, bool value)
  {
    if (PlayerPrefsSL.Refresh())
    {
      bool flag = false;
      for (int index = 0; index < PlayerPrefsSL.registry.Length - 1; ++index)
      {
        if (PlayerPrefsSL.registry[index].StartsWith(PlayerPrefsSL.boolheader + key + ":"))
        {
          PlayerPrefsSL.registry[index] = PlayerPrefsSL.boolheader + key + ": " + (object) value;
          flag = true;
        }
      }
      if (!flag)
      {
        using (StreamWriter streamWriter = File.AppendText(PlayerPrefsSL.path))
          streamWriter.WriteLine(PlayerPrefsSL.boolheader + key + ": " + (object) value);
      }
      else
        File.WriteAllLines(PlayerPrefsSL.path, PlayerPrefsSL.registry);
    }
    else
      PlayerPrefs.SetInt(PlayerPrefsSL.boolheader + key, !value ? 0 : 1);
  }

  public static void DeleteKey(string key, PlayerPrefsSL.DataTypes type)
  {
    string str = key;
    switch (type)
    {
      case PlayerPrefsSL.DataTypes.Float:
        str = PlayerPrefsSL.floatheader + key;
        break;
      case PlayerPrefsSL.DataTypes.Int:
        str = PlayerPrefsSL.intheader + key;
        break;
      case PlayerPrefsSL.DataTypes.String:
        str = PlayerPrefsSL.stringheader + key;
        break;
      case PlayerPrefsSL.DataTypes.Bool:
        str = PlayerPrefsSL.boolheader + key;
        break;
    }
    if (PlayerPrefsSL.Refresh())
    {
      string[] myArray = PlayerPrefsSL.RemoveElement(key, PlayerPrefsSL.registry);
      string[] contents = PlayerPrefsSL.RemoveElement(str, myArray);
      File.WriteAllLines(PlayerPrefsSL.path, contents);
    }
    else
    {
      PlayerPrefs.DeleteKey(key);
      PlayerPrefs.DeleteKey(str);
    }
  }

  public static void DeleteKey(string key)
  {
    if (PlayerPrefsSL.Refresh())
    {
      string[] myArray1 = PlayerPrefsSL.RemoveElement(key, PlayerPrefsSL.registry);
      string[] myArray2 = PlayerPrefsSL.RemoveElement(PlayerPrefsSL.floatheader + key, myArray1);
      string[] myArray3 = PlayerPrefsSL.RemoveElement(PlayerPrefsSL.intheader + key, myArray2);
      string[] myArray4 = PlayerPrefsSL.RemoveElement(PlayerPrefsSL.boolheader + key, myArray3);
      string[] contents = PlayerPrefsSL.RemoveElement(PlayerPrefsSL.stringheader + key, myArray4);
      File.WriteAllLines(PlayerPrefsSL.path, contents);
    }
    else
    {
      PlayerPrefs.DeleteKey(key);
      PlayerPrefs.DeleteKey(PlayerPrefsSL.floatheader + key);
      PlayerPrefs.DeleteKey(PlayerPrefsSL.intheader + key);
      PlayerPrefs.DeleteKey(PlayerPrefsSL.boolheader + key);
      PlayerPrefs.DeleteKey(PlayerPrefsSL.stringheader + key);
    }
  }

  public static void DeleteAll()
  {
    if (PlayerPrefsSL.Refresh())
      File.WriteAllText(PlayerPrefsSL.path, string.Empty);
    else
      PlayerPrefs.DeleteAll();
  }

  public static float GetFloat(string key, float defaultValue, bool forcedefault = false)
  {
    try
    {
      if (forcedefault)
        return defaultValue;
      if (!PlayerPrefsSL.Refresh())
        return PlayerPrefs.GetFloat(key, defaultValue);
      foreach (string str in PlayerPrefsSL.registry)
      {
        float result;
        if (str.StartsWith(PlayerPrefsSL.intheader + key + ":") && float.TryParse(str.Replace(PlayerPrefsSL.intheader + key + ":", string.Empty).Trim(), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          return result;
      }
      foreach (string str in PlayerPrefsSL.registry)
      {
        float result;
        if (str.StartsWith(key + ":") && float.TryParse(str.Replace(key + ":", string.Empty).Trim(), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          return result;
      }
      return defaultValue;
    }
    catch
    {
      return defaultValue;
    }
  }

  public static int GetInt(string key, int defaultValue, bool forcedefault = false)
  {
    try
    {
      if (forcedefault)
        return defaultValue;
      if (!PlayerPrefsSL.Refresh())
        return PlayerPrefs.GetInt(key, defaultValue);
      foreach (string str in PlayerPrefsSL.registry)
      {
        int result;
        if (str.StartsWith(PlayerPrefsSL.intheader + key + ":") && int.TryParse(str.Replace(PlayerPrefsSL.intheader + key + ":", string.Empty).Trim(), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          return result;
      }
      foreach (string str in PlayerPrefsSL.registry)
      {
        int result;
        if (str.StartsWith(key + ":") && int.TryParse(str.Replace(key + ":", string.Empty).Trim(), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          return result;
      }
      return defaultValue;
    }
    catch
    {
      return defaultValue;
    }
  }

  public static string GetString(string key, string defaultValue, bool forcedefault = false)
  {
    try
    {
      if (forcedefault)
        return defaultValue;
      if (!PlayerPrefsSL.Refresh())
        return PlayerPrefs.GetString(key, defaultValue);
      foreach (string str in PlayerPrefsSL.registry)
      {
        if (str.StartsWith(PlayerPrefsSL.stringheader + key + ":"))
          return str.Replace(PlayerPrefsSL.stringheader + key + ":", string.Empty).Trim();
      }
      foreach (string str in PlayerPrefsSL.registry)
      {
        if (str.StartsWith(key + ":"))
          return str.Replace(key + ":", string.Empty).Trim();
      }
      return defaultValue;
    }
    catch
    {
      return defaultValue;
    }
  }

  public static bool GetBool(string key, bool defaultValue, bool forcedefault = false)
  {
    try
    {
      if (forcedefault)
        return defaultValue;
      if (!PlayerPrefsSL.Refresh())
        return PlayerPrefs.GetInt(key, !defaultValue ? 0 : 1) == 1;
      foreach (string str in PlayerPrefsSL.registry)
      {
        if (str.StartsWith(PlayerPrefsSL.boolheader + key + ":"))
          return str.Replace(PlayerPrefsSL.boolheader + key + ":", string.Empty).Trim() == "true";
      }
      foreach (string str in PlayerPrefsSL.registry)
      {
        if (str.StartsWith(key + ":"))
          return str.Replace(key + ":", string.Empty).Trim() == "true";
      }
      return defaultValue;
    }
    catch
    {
      return defaultValue;
    }
  }

  public static bool HasKey(string key, PlayerPrefsSL.DataTypes type)
  {
    string key1 = key;
    switch (type)
    {
      case PlayerPrefsSL.DataTypes.Float:
        key1 = PlayerPrefsSL.floatheader + key;
        break;
      case PlayerPrefsSL.DataTypes.Int:
        key1 = PlayerPrefsSL.intheader + key;
        break;
      case PlayerPrefsSL.DataTypes.String:
        key1 = PlayerPrefsSL.stringheader + key;
        break;
      case PlayerPrefsSL.DataTypes.Bool:
        key1 = PlayerPrefsSL.boolheader + key;
        break;
    }
    if (PlayerPrefsSL.Refresh())
    {
      foreach (string str in PlayerPrefsSL.registry)
      {
        if (str.StartsWith(key1 + ":") || str.StartsWith(key + ":"))
          return true;
      }
      return false;
    }
    if (!PlayerPrefs.HasKey(key1))
      return PlayerPrefs.HasKey(key);
    return true;
  }

  public static bool HasKey(string key)
  {
    if (PlayerPrefsSL.Refresh())
    {
      foreach (string str in PlayerPrefsSL.registry)
      {
        if (str.StartsWith(key + ":") || str.StartsWith(PlayerPrefsSL.floatheader + key + ":") || (str.StartsWith(PlayerPrefsSL.intheader + key + ":") || str.StartsWith(PlayerPrefsSL.boolheader + key + ":")) || str.StartsWith(PlayerPrefsSL.stringheader + key + ":"))
          return true;
      }
      return false;
    }
    if (!PlayerPrefs.HasKey(key) && !PlayerPrefs.HasKey(PlayerPrefsSL.floatheader + key) && (!PlayerPrefs.HasKey(PlayerPrefsSL.intheader + key) && !PlayerPrefs.HasKey(PlayerPrefsSL.boolheader + key)))
      return PlayerPrefs.HasKey(PlayerPrefsSL.stringheader + key);
    return true;
  }

  public static bool HasKeyWithName(string key)
  {
    if (!PlayerPrefsSL.Refresh())
      return PlayerPrefs.HasKey(key);
    foreach (string str in PlayerPrefsSL.registry)
    {
      if (str.StartsWith(key + ":"))
        return true;
    }
    return false;
  }

  public enum DataTypes
  {
    Float,
    Int,
    String,
    Bool,
  }
}
