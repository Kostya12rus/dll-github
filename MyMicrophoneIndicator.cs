// Decompiled with JetBrains decompiler
// Type: MyMicrophoneIndicator
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance;
using Dissonance.Integrations.UNet_HLAPI;
using UnityEngine;
using UnityEngine.UI;

public class MyMicrophoneIndicator : MonoBehaviour
{
  public float amplitudeFactor = 3.4f;
  public float increaseLerp = 5f;
  public float decreaseLerp = 2f;
  public VoiceBroadcastTrigger dissonanceSetup;
  public Image background;
  public Image volume;
  private VoicePlayerState lplaystate;

  private void Update()
  {
    if (this.lplaystate == null && (Object) PlayerManager.localPlayer != (Object) null)
      this.lplaystate = this.dissonanceSetup.GetComponent<DissonanceComms>().FindPlayer(PlayerManager.localPlayer.GetComponent<HlapiPlayer>().PlayerId);
    float num = 0.0f;
    if (this.lplaystate != null)
      num = this.lplaystate.Amplitude;
    this.volume.enabled = this.dissonanceSetup.IsTransmitting;
    this.background.enabled = this.dissonanceSetup.IsTransmitting;
    float b = num * this.amplitudeFactor;
    float fillAmount = this.volume.fillAmount;
    this.volume.fillAmount = Mathf.Lerp(fillAmount, b, Time.deltaTime * ((double) fillAmount >= (double) b ? this.decreaseLerp : this.increaseLerp));
  }
}
