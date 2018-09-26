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
  public VoiceBroadcastTrigger dissonanceSetup;
  public Image background;
  public Image volume;
  public float amplitudeFactor;
  public float increaseLerp;
  public float decreaseLerp;
  private VoicePlayerState lplaystate;

  public MyMicrophoneIndicator()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    if (this.lplaystate == null && Object.op_Inequality((Object) PlayerManager.localPlayer, (Object) null))
      this.lplaystate = ((DissonanceComms) ((Component) this.dissonanceSetup).GetComponent<DissonanceComms>()).FindPlayer(((HlapiPlayer) PlayerManager.localPlayer.GetComponent<HlapiPlayer>()).PlayerId);
    float num1 = 0.0f;
    if (this.lplaystate != null)
      num1 = this.lplaystate.get_Amplitude();
    ((Behaviour) this.volume).set_enabled(this.dissonanceSetup.get_IsTransmitting());
    ((Behaviour) this.background).set_enabled(this.dissonanceSetup.get_IsTransmitting());
    float num2 = num1 * this.amplitudeFactor;
    float fillAmount = this.volume.get_fillAmount();
    this.volume.set_fillAmount(Mathf.Lerp(fillAmount, num2, Time.get_deltaTime() * ((double) fillAmount >= (double) num2 ? this.decreaseLerp : this.increaseLerp)));
  }
}
