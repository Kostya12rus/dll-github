// Decompiled with JetBrains decompiler
// Type: RadioInitializator
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance;
using Dissonance.Audio.Playback;
using Dissonance.Integrations.UNet_HLAPI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RadioInitializator : NetworkBehaviour
{
  private static RadioInitializator.VoiceIndicatorManager voiceIndicators = new RadioInitializator.VoiceIndicatorManager();
  public float multipl = 3f;
  private PlayerManager pm;
  public ServerRoles serverRoles;
  public Radio radio;
  public HlapiPlayer hlapiPlayer;
  public NicknameSync nicknameSync;
  private Transform parent;
  public GameObject prefab;
  private VoiceBroadcastTrigger bct;
  public AnimationCurve noiseOverLoudness;
  public float curAmplitude;

  private void Start()
  {
    this.bct = Object.FindObjectOfType<VoiceBroadcastTrigger>();
    this.pm = PlayerManager.singleton;
    try
    {
      this.parent = GameObject.Find("VoicechatPopups").transform;
    }
    catch
    {
    }
  }

  private void LateUpdate()
  {
    if (!this.isLocalPlayer)
      return;
    foreach (GameObject player in this.pm.players)
    {
      try
      {
        if ((Object) player != (Object) this.gameObject)
        {
          RadioInitializator component1 = player.GetComponent<RadioInitializator>();
          component1.radio.SetRelationship();
          string playerId = component1.hlapiPlayer.PlayerId;
          if ((Object) component1.radio.mySource != (Object) null)
          {
            VoicePlayback component2 = component1.radio.mySource.GetComponent<VoicePlayback>();
            bool flag = (double) component1.radio.mySource.spatialBlend == 0.0 && component2.Priority != ChannelPriority.None && (component1.radio.ShouldBeVisible(this.gameObject) || (Object) Intercom.host.speaker == (Object) player);
            this.curAmplitude = component2.Amplitude * this.multipl;
            if (NetworkServer.active)
              player.GetComponent<Scp939_VisionController>().MakeNoise(this.noiseOverLoudness.Evaluate(this.curAmplitude));
            if (RadioInitializator.voiceIndicators.ContainsId(playerId))
            {
              if (!flag)
              {
                RadioInitializator.voiceIndicators.RemoveId(playerId);
              }
              else
              {
                RadioInitializator.VoiceIndicator fromId = RadioInitializator.voiceIndicators.GetFromId(playerId);
                if (fromId != null)
                {
                  if ((Object) fromId.indicator != (Object) null)
                  {
                    fromId.indicator.GetComponent<Image>().color = component1.serverRoles.GetGradient()[0].Evaluate(component2.Amplitude * 3f);
                    fromId.indicator.GetComponent<Outline>().effectColor = component1.serverRoles.GetGradient()[1].Evaluate(component2.Amplitude * 3f);
                  }
                }
              }
            }
            else if (flag)
            {
              GameObject indicator = Object.Instantiate<GameObject>(this.prefab, this.parent);
              indicator.transform.localScale = Vector3.one;
              indicator.GetComponentInChildren<Text>().text = component1.nicknameSync.myNick;
              RadioInitializator.voiceIndicators.Add(new RadioInitializator.VoiceIndicator(indicator, playerId));
            }
          }
        }
      }
      catch
      {
      }
    }
  }

  private void OnDestroy()
  {
    try
    {
      RadioInitializator.voiceIndicators.RemoveId(this.GetComponent<HlapiPlayer>().PlayerId);
    }
    catch
    {
    }
  }

  private void UNetVersion()
  {
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }

  private class VoiceIndicator
  {
    public GameObject indicator;
    public string id;

    public VoiceIndicator(GameObject indicator, string id)
    {
      this.indicator = indicator;
      this.id = id;
    }
  }

  private class VoiceIndicatorManager
  {
    private List<RadioInitializator.VoiceIndicator> voices = new List<RadioInitializator.VoiceIndicator>();

    public bool ContainsId(string id)
    {
      if (string.IsNullOrEmpty(id))
        return false;
      foreach (RadioInitializator.VoiceIndicator voice in this.voices)
      {
        if (voice != null && voice.id != null && voice.id == id)
          return true;
      }
      return false;
    }

    public RadioInitializator.VoiceIndicator GetFromId(string id)
    {
      if (string.IsNullOrEmpty(id))
        return (RadioInitializator.VoiceIndicator) null;
      foreach (RadioInitializator.VoiceIndicator voice in this.voices)
      {
        if (voice != null && voice.id != null && voice.id == id)
          return voice;
      }
      return (RadioInitializator.VoiceIndicator) null;
    }

    public void RemoveId(string id)
    {
      if (string.IsNullOrEmpty(id))
        return;
      foreach (RadioInitializator.VoiceIndicator voice in this.voices)
      {
        if (voice.id == id)
          this.Remove(voice);
      }
    }

    public void Add(RadioInitializator.VoiceIndicator voiceObject)
    {
      if (voiceObject == null)
        return;
      this.voices.Add(voiceObject);
    }

    public void Remove(RadioInitializator.VoiceIndicator voiceObject)
    {
      if (voiceObject == null)
        return;
      if ((Object) voiceObject.indicator != (Object) null)
        Object.Destroy((Object) voiceObject.indicator);
      this.voices.Remove(voiceObject);
    }
  }
}
