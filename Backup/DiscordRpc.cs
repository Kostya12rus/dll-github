// Decompiled with JetBrains decompiler
// Type: DiscordRpc
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

public class DiscordRpc
{
  [DllImport("discord-rpc", EntryPoint = "Discord_Initialize", CallingConvention = CallingConvention.Cdecl)]
  public static extern void Initialize(string applicationId, ref DiscordRpc.EventHandlers handlers, bool autoRegister, string optionalSteamId);

  [DllImport("discord-rpc", EntryPoint = "Discord_Shutdown", CallingConvention = CallingConvention.Cdecl)]
  public static extern void Shutdown();

  [DllImport("discord-rpc", EntryPoint = "Discord_RunCallbacks", CallingConvention = CallingConvention.Cdecl)]
  public static extern void RunCallbacks();

  [DllImport("discord-rpc", EntryPoint = "Discord_UpdatePresence", CallingConvention = CallingConvention.Cdecl)]
  public static extern void UpdatePresence(ref DiscordRpc.RichPresence presence);

  [DllImport("discord-rpc", EntryPoint = "Discord_ClearPresence", CallingConvention = CallingConvention.Cdecl)]
  public static extern void ClearPresence();

  [DllImport("discord-rpc", EntryPoint = "Discord_Respond", CallingConvention = CallingConvention.Cdecl)]
  public static extern void Respond(string userId, DiscordRpc.Reply reply);

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void ReadyCallback();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void DisconnectedCallback(int errorCode, string message);

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void ErrorCallback(int errorCode, string message);

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void JoinCallback(string secret);

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void SpectateCallback(string secret);

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void RequestCallback(ref DiscordRpc.JoinRequest request);

  public struct EventHandlers
  {
    public DiscordRpc.ReadyCallback readyCallback;
    public DiscordRpc.DisconnectedCallback disconnectedCallback;
    public DiscordRpc.ErrorCallback errorCallback;
    public DiscordRpc.JoinCallback joinCallback;
    public DiscordRpc.SpectateCallback spectateCallback;
    public DiscordRpc.RequestCallback requestCallback;
  }

  [Serializable]
  public struct RichPresence
  {
    public string state;
    public string details;
    public long startTimestamp;
    public long endTimestamp;
    public string largeImageKey;
    public string largeImageText;
    public string smallImageKey;
    public string smallImageText;
    public string partyId;
    public int partySize;
    public int partyMax;
    public string matchSecret;
    public string joinSecret;
    public string spectateSecret;
    public bool instance;
  }

  [Serializable]
  public struct JoinRequest
  {
    public string userId;
    public string username;
    public string discriminator;
    public string avatar;
  }

  public enum Reply
  {
    No,
    Yes,
    Ignore,
  }
}
