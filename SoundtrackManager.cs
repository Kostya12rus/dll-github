// Decompiled with JetBrains decompiler
// Type: SoundtrackManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SoundtrackManager : MonoBehaviour
{
  public SoundtrackManager.Track[] overlayTracks;
  public SoundtrackManager.Track[] mainTracks;
  public int overlayIndex;
  public int mainIndex;
  public bool overlayPlaying;
  public GameObject player;
  public LayerMask mask;
  private float nooneSawTime;
  private bool seeSomeone;
  public static SoundtrackManager singleton;

  private void FixedUpdate()
  {
    bool flag = false;
    if ((Object) AlphaWarheadController.host != (Object) null)
      flag = AlphaWarheadController.host.inProgress;
    if ((double) this.nooneSawTime > 140.0 && !this.overlayPlaying)
    {
      for (int index = 0; index < this.mainTracks.Length; ++index)
      {
        this.mainTracks[index].playing = index == 3 && !flag;
        this.mainTracks[index].Update(1);
      }
      for (int index = 0; index < this.overlayTracks.Length; ++index)
      {
        this.overlayTracks[index].playing = this.overlayPlaying && index == this.overlayIndex && !flag;
        this.overlayTracks[index].Update(1);
      }
    }
    else
    {
      for (int index = 0; index < this.overlayTracks.Length; ++index)
      {
        this.overlayTracks[index].playing = this.overlayPlaying && index == this.overlayIndex && !flag;
        this.overlayTracks[index].Update(1);
      }
      for (int index = 0; index < this.mainTracks.Length; ++index)
      {
        this.mainTracks[index].playing = !this.overlayPlaying && index == this.mainIndex && !flag;
        this.mainTracks[index].Update(1);
      }
    }
  }

  private void Update()
  {
    if ((Object) this.player == (Object) null)
      return;
    if (this.seeSomeone)
      this.nooneSawTime = 0.0f;
    else
      this.nooneSawTime += Time.deltaTime;
  }

  private void Start()
  {
    Timing.RunCoroutine(this._Start(), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  private IEnumerator<float> _Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new SoundtrackManager.\u003C_Start\u003Ec__Iterator0() { \u0024this = this };
  }

  public void PlayOverlay(int id)
  {
    if (id == this.overlayIndex && this.overlayPlaying)
      return;
    this.overlayPlaying = true;
    this.overlayIndex = id;
    if (!this.overlayTracks[id].restartOnPlay)
      return;
    this.overlayTracks[id].source.Stop();
    this.overlayTracks[id].source.Play();
  }

  public void StopOverlay(int id)
  {
    if (this.overlayIndex != id)
      return;
    this.overlayPlaying = false;
  }

  private void Awake()
  {
    SoundtrackManager.singleton = this;
  }

  [Serializable]
  public class Track
  {
    public string name;
    public AudioSource source;
    public bool playing;
    public bool restartOnPlay;
    public float enterFadeDuration;
    public float exitFadeDuration;
    public float maxVolume;

    public void Update(int speed = 1)
    {
      if (this.restartOnPlay && (double) this.source.volume == 0.0 && this.playing)
      {
        this.source.Stop();
        this.source.Play();
      }
      this.source.volume += (float) (0.0199999995529652 * (double) speed * (!this.playing ? -1.0 / (double) this.exitFadeDuration : 1.0 / (double) this.enterFadeDuration)) * this.maxVolume;
      this.source.volume = Mathf.Clamp(this.source.volume, 0.0f, this.maxVolume);
    }
  }
}
