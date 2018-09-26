// Decompiled with JetBrains decompiler
// Type: BanPlayer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Networking;

public class BanPlayer : NetworkBehaviour
{
  public BanPlayer()
  {
    base.\u002Ector();
  }

  public string BanUser(GameObject user, int duration, string reason, string issuer)
  {
    string str1 = "good";
    string str2 = "nothing";
    string str3 = "nothing";
    try
    {
      if (duration > 0)
      {
        str1 = "Setting nick";
        string nick = ((NicknameSync) user.GetComponent<NicknameSync>()).myNick;
        str1 = "Online ban";
        if (ConfigFile.ServerConfig.GetBool("online_mode", false))
          str2 = BanHandler.IssueBan(new BanDetails()
          {
            OriginalName = nick,
            Id = ((CharacterClassManager) user.GetComponent<CharacterClassManager>()).SteamId,
            IssuanceTime = TimeBehaviour.CurrentTimestamp(),
            Expires = DateTime.UtcNow.AddMinutes((double) duration).Ticks,
            Reason = reason,
            Issuer = issuer
          }, 0);
        else
          str2 = "good";
        str1 = "IP ban";
        if (ConfigFile.ServerConfig.GetBool("ip_banning", false))
          str3 = BanHandler.IssueBan(new BanDetails()
          {
            OriginalName = nick,
            Id = (string) ((NetworkIdentity) user.GetComponent<NetworkIdentity>()).get_connectionToClient().address,
            IssuanceTime = TimeBehaviour.CurrentTimestamp(),
            Expires = DateTime.UtcNow.AddMinutes((double) duration).Ticks,
            Reason = reason,
            Issuer = issuer
          }, 1);
        else
          str3 = "good";
      }
      else
      {
        str2 = "good";
        str3 = "good";
      }
      str1 = "good";
    }
    catch
    {
    }
    string str4 = !(str2 == "good") || !(str3 == "good") ? "Online ban: " + str2 + ", IP ban: " + str3 : "good";
    ServerConsole.Disconnect(user, duration <= 0 ? "You have been kicked." : "You have been banned.");
    return str4;
  }

  private void UNetVersion()
  {
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
