// Decompiled with JetBrains decompiler
// Type: AchievementManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Steamworks;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
  public AchievementManager()
  {
    base.\u002Ector();
  }

  public static void Achieve(string key)
  {
    if (!SteamManager.Initialized || ServerStatic.IsDedicated)
      return;
    SteamUserStats.SetAchievement(key);
    Debug.Log((object) ("Achievement get! " + key));
    SteamUserStats.RequestCurrentStats();
  }

  public static void StatsProgress(string key, string completeAchievement, int maxValue)
  {
    if (!SteamManager.Initialized || ServerStatic.IsDedicated)
      return;
    int num;
    SteamUserStats.GetStat(key, ref num);
    ++num;
    num = Mathf.Clamp(num, 0, maxValue);
    SteamUserStats.SetStat(key, num);
    SteamUserStats.IndicateAchievementProgress(completeAchievement, (uint) num, (uint) maxValue);
    Debug.Log((object) ("Stats Progress! " + key + " " + (object) num + "/" + (object) maxValue));
  }
}
