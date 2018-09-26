// Decompiled with JetBrains decompiler
// Type: SpeakerIcon
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using Dissonance;
using Dissonance.Audio.Playback;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SpeakerIcon : MonoBehaviour
{
  private Transform cam;
  private CharacterClassManager ccm;
  private RawImage img;
  private NetworkIdentity nid;
  private CanvasRenderer cr;
  private Radio r;
  private VoicePlayback vp;
  public Texture[] sprites;
  public static bool iAmHuman;
  public int id;

  private void Start()
  {
    this.img = this.GetComponentInChildren<RawImage>();
    this.ccm = this.GetComponentInParent<CharacterClassManager>();
    this.nid = this.GetComponentInParent<NetworkIdentity>();
    this.r = this.GetComponentInParent<Radio>();
    this.cr = this.GetComponent<CanvasRenderer>();
  }

  public void SetAlpha(float a)
  {
    this.cr.SetAlpha(Mathf.Clamp01(a));
  }

  private void Update()
  {
    if (this.nid.isLocalPlayer)
      return;
    if ((Object) this.vp == (Object) null)
    {
      if (!((Object) this.r.mySource != (Object) null))
        return;
      this.vp = this.r.mySource.GetComponent<VoicePlayback>();
    }
    else
    {
      if (this.vp.Priority == ChannelPriority.None)
        this.id = 0;
      this.img.texture = this.sprites[this.id];
    }
  }

  private void LateUpdate()
  {
    Class @class = (Class) null;
    if (this.ccm.curClass >= 0)
    {
      @class = this.ccm.klasy[this.ccm.curClass];
      if (this.nid.isLocalPlayer)
        SpeakerIcon.iAmHuman = @class.team != Team.SCP;
    }
    if ((Object) this.cam == (Object) null)
    {
      this.cam = GameObject.Find("SpectatorCamera").transform;
    }
    else
    {
      if (@class == null)
        return;
      this.transform.localPosition = Vector3.up * 1.42f + Vector3.up * @class.iconHeightOffset;
      this.transform.LookAt(this.cam);
    }
  }
}
