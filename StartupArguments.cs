// Decompiled with JetBrains decompiler
// Type: StartupArguments
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;

public class StartupArguments
{
  public static bool IsSetShort(string param)
  {
    return ((IEnumerable<string>) Environment.GetCommandLineArgs()).Any<string>((Func<string, bool>) (x =>
    {
      if (x.StartsWith("-") && !x.StartsWith("--"))
        return x.Contains(param);
      return false;
    }));
  }

  public static bool IsSetBool(string param, string alias = "")
  {
    if (((IEnumerable<string>) Environment.GetCommandLineArgs()).Contains<string>("--" + param))
      return true;
    if (!string.IsNullOrEmpty(alias))
      return StartupArguments.IsSetShort(alias);
    return false;
  }

  public static string GetArgument(string param, string alias = "", string def = "")
  {
    string[] commandLineArgs = Environment.GetCommandLineArgs();
    bool flag = false;
    foreach (string str in commandLineArgs)
    {
      if (flag && !str.StartsWith("-"))
        return str;
      flag = str == "--" + param || !string.IsNullOrEmpty(alias) && str.StartsWith("-") && !str.StartsWith("--") && str.EndsWith(alias);
    }
    return def;
  }
}
