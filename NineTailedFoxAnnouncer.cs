// Decompiled with JetBrains decompiler
// Type: NineTailedFoxAnnouncer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class NineTailedFoxAnnouncer : MonoBehaviour
{
  public float natoNumberLengthMultiplier = 0.9f;
  public float natoLetterLengthMultiplier = 0.9f;
  public List<NineTailedFoxAnnouncer.VoiceLine> queue = new List<NineTailedFoxAnnouncer.VoiceLine>();
  public bool isFree = true;
  public NineTailedFoxAnnouncer.VoiceLine[] voiceLines;
  public AudioSource speakerSource;
  public AudioSource backgroundSource;
  public static NineTailedFoxAnnouncer singleton;

  public void AnnounceNtfEntrance(int _scpsLeft, int _mtfNumber, char _mtfLetter)
  {
    string empty = string.Empty;
    int num = Mathf.Clamp(_scpsLeft, 0, 3);
    string[] strArray = new string[2]{ _mtfNumber.ToString("00")[0].ToString(), _mtfNumber.ToString("00")[1].ToString() };
    this.AddPhraseToQueue(empty + "BG_MTF1 BREAK_PREANNC MTFUNIT EPSILON NATO_11 DESIGNATED " + "NATO_" + (object) _mtfLetter + " " + "NATO_" + strArray[0] + " NATO_" + strArray[1] + " " + "ENTERED REMAINING " + "SCP" + (object) num);
  }

  public void AnnounceScpKill(string scpNumber, CharacterClassManager executioner)
  {
    string tts = string.Empty;
    try
    {
      tts += "BG_MTF2 BREAK_PREANNC SCP ";
      if (scpNumber.Contains("-"))
      {
        string str = scpNumber;
        char[] chArray = new char[1]{ '-' };
        foreach (char ch in str.Split(chArray)[1])
          tts = tts + "NATO_" + (object) ch + " ";
      }
      tts += "CONTAINEDSUCCESSFULLY CONTAINMENTUNIT ";
      if ((Object) executioner == (Object) null || executioner.curClass < 0 || executioner.klasy[executioner.curClass].team != Team.MTF)
      {
        tts += "UNKNOWN";
      }
      else
      {
        string str1 = NineTailedFoxUnits.host.list[executioner.ntfUnit];
        char ch = str1[0];
        string str2 = int.Parse(str1.Split('-')[1]).ToString("00");
        tts = tts + "NATO_" + (object) ch + " NATO_" + (object) str2[0] + " NATO_" + (object) str2[1];
      }
    }
    catch
    {
      Debug.Log((object) ("Error: " + tts));
    }
    this.AddPhraseToQueue(tts);
  }

  public void AddPhraseToQueue(string tts)
  {
    string str1 = tts;
    char[] chArray = new char[1]{ ' ' };
    foreach (string str2 in str1.Split(chArray))
    {
      foreach (NineTailedFoxAnnouncer.VoiceLine voiceLine in this.voiceLines)
      {
        if (str2.ToUpper() == voiceLine.apiName.ToUpper())
          this.queue.Add(new NineTailedFoxAnnouncer.VoiceLine()
          {
            apiName = voiceLine.apiName,
            clip = voiceLine.clip,
            length = voiceLine.length
          });
      }
    }
    this.queue.Add(new NineTailedFoxAnnouncer.VoiceLine()
    {
      apiName = "END_OF_MESSAGE"
    });
  }

  [DebuggerHidden]
  private IEnumerator Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new NineTailedFoxAnnouncer.\u003CStart\u003Ec__Iterator0() { \u0024this = this };
  }

  [Serializable]
  public class VoiceLine
  {
    public string apiName;
    public AudioClip clip;
    public float length;
  }
}
