// Decompiled with JetBrains decompiler
// Type: DiscordController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DiscordController : MonoBehaviour
{
  public DiscordRpc.RichPresence presence;
  public string applicationId;
  public string optionalSteamId;
  public int callbackCalls;
  public DiscordRpc.JoinRequest joinRequest;
  public UnityEvent onConnect;
  public UnityEvent onDisconnect;
  public UnityEvent hasResponded;
  public DiscordJoinEvent onJoin;
  public DiscordJoinEvent onSpectate;
  public DiscordJoinRequestEvent onJoinRequest;
  private GameConsole.Console console;
  public TextMeshProUGUI joinText;
  public Animator joinAnimator;
  private DiscordRpc.EventHandlers handlers;

  public void RequestRespondYes()
  {
    this.joinAnimator.SetBool("Requested", false);
    this.console.AddLog("Discord: Accepted join request.", new Color32((byte) 114, (byte) 137, (byte) 218, byte.MaxValue), false);
    DiscordRpc.Respond(this.joinRequest.userId, DiscordRpc.Reply.Yes);
    this.hasResponded.Invoke();
  }

  public void RequestRespondNo()
  {
    this.joinAnimator.SetBool("Requested", false);
    this.console.AddLog("Discord: Join request rejected.", new Color32((byte) 114, (byte) 137, (byte) 218, byte.MaxValue), false);
    DiscordRpc.Respond(this.joinRequest.userId, DiscordRpc.Reply.No);
    this.hasResponded.Invoke();
  }

  public void ReadyCallback()
  {
    ++this.callbackCalls;
    this.console.AddLog("Discord: ready!", new Color32((byte) 114, (byte) 137, (byte) 218, byte.MaxValue), false);
    this.onConnect.Invoke();
  }

  public void DisconnectedCallback(int errorCode, string message)
  {
    ++this.callbackCalls;
    Debug.Log((object) string.Format("Discord: disconnected - {0} ({1})", (object) errorCode, (object) message));
    this.onDisconnect.Invoke();
  }

  public void ErrorCallback(int errorCode, string message)
  {
    ++this.callbackCalls;
    this.console.AddLog(string.Format("Discord: error - {0} ({1})", (object) errorCode, (object) message), new Color32((byte) 114, (byte) 137, (byte) 218, byte.MaxValue), false);
  }

  public void JoinCallback(string secret)
  {
    ++this.callbackCalls;
    string str = Encoding.UTF8.GetString(Convert.FromBase64String(secret));
    try
    {
      CustomNetworkManager component = this.GetComponent<CustomNetworkManager>();
      string[] ipAndPort = str.Split(':');
      int result = 0;
      if (!int.TryParse(ipAndPort[1], out result))
        throw new Exception();
      component.networkAddress = ipAndPort[0];
      CustomNetworkManager.ConnectionIp = ipAndPort[0];
      component.networkPort = result;
      if (((IEnumerable<string>) component.CompatibleVersions).Any<string>((Func<string, bool>) (item => item == ipAndPort[2])))
      {
        component.ShowLog(13);
        component.StartClient();
        return;
      }
      this.console.AddLog("Discord: Could not join the server - version mismatch.", new Color32((byte) 114, (byte) 137, (byte) 218, byte.MaxValue), false);
    }
    catch
    {
      this.console.AddLog("Discord: Could not join the server - incorrect IP address - " + str, new Color32((byte) 114, (byte) 137, (byte) 218, byte.MaxValue), false);
    }
    this.onJoin.Invoke(secret);
  }

  public void SpectateCallback(string secret)
  {
    ++this.callbackCalls;
    this.console.AddLog("Discord: SpectateCallback fired.", new Color32((byte) 114, (byte) 137, (byte) 218, byte.MaxValue), false);
    this.onSpectate.Invoke(secret);
  }

  public void RequestCallback(ref DiscordRpc.JoinRequest request)
  {
    ++this.callbackCalls;
    this.joinAnimator.SetBool("Requested", true);
    this.joinText.text = string.Format("<b><color=#7289DA>{0}<color=#99AAB5>#</color>{1}</color></b> would like to join your match!", (object) request.username, (object) request.discriminator);
    this.console.AddLog(string.Format("Discord: join request {0}#{1}: {2}", (object) request.username, (object) request.discriminator, (object) request.userId), new Color32((byte) 114, (byte) 137, (byte) 218, byte.MaxValue), false);
    this.joinRequest = request;
    this.onJoinRequest.Invoke(request);
  }

  private void Start()
  {
    DiscordRpc.UpdatePresence(ref this.presence);
    this.presence.startTimestamp = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    this.console = GameConsole.Console.singleton;
  }

  private void Update()
  {
    DiscordRpc.RunCallbacks();
  }

  private void OnEnable()
  {
    this.callbackCalls = 0;
    this.handlers = new DiscordRpc.EventHandlers()
    {
      readyCallback = new DiscordRpc.ReadyCallback(this.ReadyCallback)
    };
    this.handlers.disconnectedCallback += new DiscordRpc.DisconnectedCallback(this.DisconnectedCallback);
    this.handlers.errorCallback += new DiscordRpc.ErrorCallback(this.ErrorCallback);
    this.handlers.joinCallback += new DiscordRpc.JoinCallback(this.JoinCallback);
    this.handlers.spectateCallback += new DiscordRpc.SpectateCallback(this.SpectateCallback);
    this.handlers.requestCallback += new DiscordRpc.RequestCallback(this.RequestCallback);
    DiscordRpc.Initialize(this.applicationId, ref this.handlers, true, this.optionalSteamId);
  }

  private void OnApplicationQuit()
  {
    DiscordRpc.Shutdown();
  }

  private void OnDestroy()
  {
  }
}
