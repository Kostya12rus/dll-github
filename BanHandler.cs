// Decompiled with JetBrains decompiler
// Type: BanHandler
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BanHandler : MonoBehaviour
{
  public BanHandler()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    try
    {
      if (!Directory.Exists(FileManager.AppFolder))
        Directory.CreateDirectory(FileManager.AppFolder);
      if (!File.Exists(BanHandler.GetPath(0)))
        File.Create(BanHandler.GetPath(0)).Close();
      else
        FileManager.RemoveEmptyLines(BanHandler.GetPath(0));
      if (!File.Exists(BanHandler.GetPath(1)))
        File.Create(BanHandler.GetPath(1)).Close();
      else
        FileManager.RemoveEmptyLines(BanHandler.GetPath(1));
    }
    catch
    {
      ServerConsole.AddLog("Can't create ban files!");
    }
    BanHandler.ValidateBans();
  }

  public static string IssueBan(BanDetails ban, int selector)
  {
    string str = "good";
    try
    {
      str = "1";
      ban.OriginalName = ban.OriginalName.Replace(";", ":").Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty);
      str = "2";
      List<BanDetails> bans = BanHandler.GetBans(selector);
      str = "3";
      bool flag = bans.Where<BanDetails>((Func<BanDetails, bool>) (b => b.Id == ban.Id)).Any<BanDetails>();
      str = "4";
      if (!flag)
      {
        FileManager.AppendFile(ban.ToString(), BanHandler.GetPath(selector), true);
        FileManager.RemoveEmptyLines(BanHandler.GetPath(selector));
      }
      else
      {
        str = "5";
        BanHandler.RemoveBan(ban.Id, selector);
        str = "6";
        BanHandler.IssueBan(ban, selector);
      }
      str = "good";
    }
    catch
    {
    }
    return str;
  }

  public static void ValidateBans()
  {
    BanHandler.ValidateBans(0);
    BanHandler.ValidateBans(1);
  }

  public static void ValidateBans(int selector)
  {
    List<string> list = ((IEnumerable<string>) FileManager.ReadAllLines(BanHandler.GetPath(selector))).ToList<string>();
    List<int> intList = new List<int>();
    for (int index = list.Count - 1; index >= 0; --index)
    {
      string ban = list[index];
      if (BanHandler.ProcessBanItem(ban) == null || !BanHandler.CheckExpiration(BanHandler.ProcessBanItem(ban), -1))
        intList.Add(index);
    }
    List<int> source = new List<int>();
    foreach (int num in intList)
    {
      if (!source.Contains(num))
        source.Add(num);
    }
    foreach (int index in (IEnumerable<int>) source.OrderByDescending<int, int>((Func<int, int>) (index => index)))
      list.RemoveAt(index);
    FileManager.WriteToFile(list.ToArray(), BanHandler.GetPath(selector));
  }

  public static bool CheckExpiration(BanDetails ban, int selector)
  {
    if (ban == null)
      return false;
    if (TimeBehaviour.ValidateTimestamp(ban.Expires, TimeBehaviour.CurrentTimestamp(), 0L))
      return true;
    if (selector >= 0)
      BanHandler.RemoveBan(ban.Id, selector);
    return false;
  }

  public static BanDetails ReturnChecks(BanDetails ban, int selector)
  {
    if (BanHandler.CheckExpiration(ban, selector))
      return ban;
    return (BanDetails) null;
  }

  public static void RemoveBan(string id, int selector)
  {
    id = id.Replace(";", ":").Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty);
    FileManager.WriteToFile(((IEnumerable<string>) FileManager.ReadAllLines(BanHandler.GetPath(selector))).Where<string>((Func<string, bool>) (l =>
    {
      if (BanHandler.ProcessBanItem(l) != null)
        return BanHandler.ProcessBanItem(l).Id != id;
      return false;
    })).ToArray<string>(), BanHandler.GetPath(selector));
  }

  public static List<BanDetails> GetBans(int selector)
  {
    string[] strArray = FileManager.ReadAllLines(BanHandler.GetPath(selector));
    // ISSUE: reference to a compiler-generated field
    if (BanHandler.\u003C\u003Ef__mg\u0024cache0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      BanHandler.\u003C\u003Ef__mg\u0024cache0 = new Func<string, BanDetails>(BanHandler.ProcessBanItem);
    }
    // ISSUE: reference to a compiler-generated field
    Func<string, BanDetails> fMgCache0 = BanHandler.\u003C\u003Ef__mg\u0024cache0;
    return ((IEnumerable<string>) strArray).Select<string, BanDetails>(fMgCache0).Where<BanDetails>((Func<BanDetails, bool>) (b => b != null)).ToList<BanDetails>();
  }

  public static KeyValuePair<BanDetails, BanDetails> QueryBan(string steamId, string ip)
  {
    string ban1 = (string) null;
    string ban2 = (string) null;
    if (!string.IsNullOrEmpty(steamId))
    {
      steamId = steamId.Replace(";", ":").Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty);
      ban1 = ((IEnumerable<string>) FileManager.ReadAllLines(BanHandler.GetPath(0))).Where<string>((Func<string, bool>) (b =>
      {
        if (BanHandler.ProcessBanItem(b) != null)
          return BanHandler.ProcessBanItem(b).Id == steamId;
        return false;
      })).FirstOrDefault<string>();
    }
    if (!string.IsNullOrEmpty(ip))
    {
      ip = ip.Replace(";", ":").Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty);
      ban2 = ((IEnumerable<string>) FileManager.ReadAllLines(BanHandler.GetPath(1))).Where<string>((Func<string, bool>) (b =>
      {
        if (BanHandler.ProcessBanItem(b) != null)
          return BanHandler.ProcessBanItem(b).Id == ip;
        return false;
      })).FirstOrDefault<string>();
    }
    return new KeyValuePair<BanDetails, BanDetails>(BanHandler.ReturnChecks(BanHandler.ProcessBanItem(ban1), 0), BanHandler.ReturnChecks(BanHandler.ProcessBanItem(ban2), 1));
  }

  public static BanDetails ProcessBanItem(string ban)
  {
    if (string.IsNullOrEmpty(ban) || !ban.Contains(";"))
      return (BanDetails) null;
    string[] strArray = ban.Split(';');
    if (strArray.Length != 6)
      return (BanDetails) null;
    return new BanDetails()
    {
      OriginalName = strArray[0],
      Id = strArray[1].Trim(),
      Expires = Convert.ToInt64(strArray[2].Trim()),
      Reason = strArray[3],
      Issuer = strArray[4],
      IssuanceTime = Convert.ToInt64(strArray[5].Trim())
    };
  }

  public static string GetPath(int selector)
  {
    if (selector == 0 || selector != 1)
      return FileManager.AppFolder + "SteamIdBans.txt";
    return FileManager.AppFolder + "IpBans.txt";
  }
}
