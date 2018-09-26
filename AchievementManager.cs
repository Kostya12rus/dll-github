// Decompiled with JetBrains decompiler
// Type: AchievementManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Steamworks;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
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
    int pData;
    SteamUserStats.GetStat(key, out pData);
    ++pData;
    pData = Mathf.Clamp(pData, 0, maxValue);
    SteamUserStats.SetStat(key, pData);
    SteamUserStats.IndicateAchievementProgress(completeAchievement, (uint) pData, (uint) maxValue);
    Debug.Log((object) ("Stats Progress! " + key + " " + (object) pData + "/" + (object) maxValue));
  }
}
