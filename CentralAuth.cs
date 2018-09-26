// Decompiled with JetBrains decompiler
// Type: CentralAuth
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Cryptography;
using MEC;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class CentralAuth : MonoBehaviour
{
  private byte[] m_Ticket;
  private uint m_pcbTicket;
  private string hexticket;
  private string _roleToRequest;
  private HAuthTicket m_HAuthTicket;
  private ICentralAuth _ica;
  private bool _responded;
  private Callback<GetAuthSessionTicketResponse_t> m_GetAuthSessionTicketResponse;
  public static CentralAuth singleton;

  public CentralAuth()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    CentralAuth.singleton = this;
  }

  public void GenerateToken(ICentralAuth icaa)
  {
    if (!SteamManager.Initialized)
      return;
    GameConsole.Console.singleton.AddLog("Obtaining ticket from Steam...", Color32.op_Implicit(Color.get_blue()), false);
    this._ica = icaa;
    if (this.m_GetAuthSessionTicketResponse == null)
    {
      // ISSUE: method pointer
      this.m_GetAuthSessionTicketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(new Callback<GetAuthSessionTicketResponse_t>.DispatchDelegate((object) this, __methodptr(OnGetAuthSessionTicketResponse)));
    }
    this.m_Ticket = new byte[1024];
    this.m_HAuthTicket = SteamUser.GetAuthSessionTicket(this.m_Ticket, 1024, ref this.m_pcbTicket);
  }

  public void OnGetAuthSessionTicketResponse(GetAuthSessionTicketResponse_t pCallback)
  {
    GameConsole.Console.singleton.AddLog("Ticked obtained from steam.", Color32.op_Implicit(Color.get_blue()), false);
    Array.Resize<byte>(ref this.m_Ticket, (int) this.m_pcbTicket);
    this.hexticket = BitConverter.ToString(this.m_Ticket).Replace("-", string.Empty);
    this._responded = true;
  }

  private void Update()
  {
    if (this._responded)
    {
      this._responded = false;
      Timing.RunCoroutine(this._RequestToken(), (Segment) 1);
    }
    if (string.IsNullOrEmpty(this._roleToRequest) || !Object.op_Inequality((Object) PlayerManager.localPlayer, (Object) null) || string.IsNullOrEmpty(((NicknameSync) PlayerManager.localPlayer.GetComponent<NicknameSync>()).myNick))
      return;
    GameConsole.Console.singleton.AddLog("Requesting your global badge...", Color32.op_Implicit(Color.get_yellow()), false);
    this._ica.RequestBadge(this._roleToRequest);
    this._roleToRequest = string.Empty;
  }

  [DebuggerHidden]
  private IEnumerator<float> _RequestToken()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new CentralAuth.\u003C_RequestToken\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public void StartValidateToken(ICentralAuth icaa, string token)
  {
    Timing.RunCoroutine(this._ValidateToken(icaa, token), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _ValidateToken(ICentralAuth icaa, string token)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new CentralAuth.\u003C_ValidateToken\u003Ec__Iterator1()
    {
      token = token,
      icaa = icaa,
      \u0024this = this
    };
  }

  internal static string ValidateForGlobalBanning(string token, string nickname)
  {
    try
    {
      string data = token.Substring(0, token.IndexOf("<br>Signature: ", StringComparison.Ordinal));
      string signature = token.Substring(token.IndexOf("<br>Signature: ", StringComparison.Ordinal) + 15).Replace("<br>", string.Empty);
      if (!ECDSA.Verify(data, signature, ServerConsole.Publickey))
      {
        GameConsole.Console.singleton.AddLog("Authentication token rejected due to signature mismatch.", Color32.op_Implicit(Color.get_red()), false);
        return "-1";
      }
      Dictionary<string, string> dictionary = ((IEnumerable<string>) data.Split(new string[1]
      {
        "<br>"
      }, StringSplitOptions.None)).Select<string, string[]>((Func<string, string[]>) (rwr => rwr.Split(new string[1]
      {
        ": "
      }, StringSplitOptions.None))).ToDictionary<string[], string, string>((Func<string[], string>) (split => split[0]), (Func<string[], string>) (split => split[1]));
      if (dictionary["Usage"] != "Authentication")
      {
        GameConsole.Console.singleton.AddLog("Authentication token rejected due to usage mismatch.", Color32.op_Implicit(Color.get_red()), false);
        return "-1";
      }
      if (dictionary["Test signature"] != "NO")
      {
        GameConsole.Console.singleton.AddLog("Authentication token rejected due to test flag.", Color32.op_Implicit(Color.get_red()), false);
        return "-1";
      }
      if (ServerRoles.Base64Decode(dictionary["Nickname"]) != nickname)
      {
        GameConsole.Console.singleton.AddLog("Authentication token rejected due to nickname mismatch (token issued for " + ServerRoles.Base64Decode(dictionary["Nickname"]) + ").", Color32.op_Implicit(Color.get_red()), false);
        return "-1";
      }
      DateTime exact1 = DateTime.ParseExact(dictionary["Expiration time"], "yyyy-MM-dd HH:mm:ss", (IFormatProvider) null);
      DateTime exact2 = DateTime.ParseExact(dictionary["Issuence time"], "yyyy-MM-dd HH:mm:ss", (IFormatProvider) null);
      if (exact1 < DateTime.UtcNow.AddMinutes(-45.0))
      {
        GameConsole.Console.singleton.AddLog("Authentication token rejected due to expiration date.", Color32.op_Implicit(Color.get_red()), false);
        return "-1";
      }
      if (exact2 > DateTime.UtcNow.AddMinutes(45.0))
      {
        GameConsole.Console.singleton.AddLog("Authentication token rejected due to issuance date.", Color32.op_Implicit(Color.get_red()), false);
        return "-1";
      }
      GameConsole.Console.singleton.AddLog("Accepted verification token of user " + dictionary["Steam ID"] + " - " + ServerRoles.Base64Decode(dictionary["Nickname"]) + " signed by " + dictionary["Issued by"] + ".", Color32.op_Implicit(Color.get_green()), false);
      return dictionary["Steam ID"];
    }
    catch (Exception ex)
    {
      GameConsole.Console.singleton.AddLog("Error during authentication token verification: " + ex.Message, Color32.op_Implicit(Color.get_red()), false);
      return "-1";
    }
  }

  internal static Dictionary<string, string> ValidateBadgeRequest(string token, string steamid, string nickname)
  {
    try
    {
      string data = token.Substring(0, token.IndexOf("<br>Signature: ", StringComparison.Ordinal));
      string signature = token.Substring(token.IndexOf("<br>Signature: ", StringComparison.Ordinal) + 15).Replace("<br>", string.Empty);
      if (!ECDSA.Verify(data, signature, ServerConsole.Publickey))
      {
        ServerConsole.AddLog("Badge request signature mismatch.");
        return (Dictionary<string, string>) null;
      }
      Dictionary<string, string> dictionary = ((IEnumerable<string>) data.Split(new string[1]
      {
        "<br>"
      }, StringSplitOptions.None)).Select<string, string[]>((Func<string, string[]>) (rwr => rwr.Split(new string[1]
      {
        ": "
      }, StringSplitOptions.None))).ToDictionary<string[], string, string>((Func<string[], string>) (split => split[0]), (Func<string[], string>) (split => split[1]));
      if (dictionary["Usage"] != "Badge request")
      {
        ServerConsole.AddLog("Player tried to use token not issued to request a badge.");
        return (Dictionary<string, string>) null;
      }
      if (dictionary["Test signature"] != "NO")
      {
        ServerConsole.AddLog("Player tried to use badge request token issued only for testing. Server: " + dictionary["Issued by"] + ".");
        return (Dictionary<string, string>) null;
      }
      if (dictionary["Steam ID"] != steamid && !string.IsNullOrEmpty(steamid))
      {
        ServerConsole.AddLog("Player tried to use badge request token issued for different user (Steam ID mismatch). Server: " + dictionary["Issued by"] + ".");
        return (Dictionary<string, string>) null;
      }
      if (ServerRoles.Base64Decode(dictionary["Nickname"]) != nickname)
      {
        ServerConsole.AddLog("Player tried to use badge request token issued for different user (nickname mismatch). Server: " + dictionary["Issued by"] + ".");
        return (Dictionary<string, string>) null;
      }
      DateTime exact1 = DateTime.ParseExact(dictionary["Expiration time"], "yyyy-MM-dd HH:mm:ss", (IFormatProvider) null);
      DateTime exact2 = DateTime.ParseExact(dictionary["Issuence time"], "yyyy-MM-dd HH:mm:ss", (IFormatProvider) null);
      if (exact1 < DateTime.UtcNow)
      {
        ServerConsole.AddLog("Player tried to use expired badge request token. Server: " + dictionary["Issued by"] + ".");
        ServerConsole.AddLog("Make sure that time and timezone set on server is correct. We recommend synchronizing the time.");
        return (Dictionary<string, string>) null;
      }
      if (!(exact2 > DateTime.UtcNow.AddMinutes(20.0)))
        return dictionary;
      ServerConsole.AddLog("Player tried to use non-issued badge request token. Server: " + dictionary["Issued by"] + ".");
      ServerConsole.AddLog("Make sure that time and timezone set on server is correct. We recommend synchronizing the time.");
      return (Dictionary<string, string>) null;
    }
    catch (Exception ex)
    {
      ServerConsole.AddLog("Error during badge request token verification: " + ex.Message);
      Debug.Log((object) ("Error during badge request token verification: " + ex.Message + " StackTrace: " + ex.StackTrace));
      return (Dictionary<string, string>) null;
    }
  }
}
