// Decompiled with JetBrains decompiler
// Type: SpeakerIcon
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

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

  public SpeakerIcon()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.img = (RawImage) ((Component) this).GetComponentInChildren<RawImage>();
    this.ccm = (CharacterClassManager) ((Component) this).GetComponentInParent<CharacterClassManager>();
    this.nid = (NetworkIdentity) ((Component) this).GetComponentInParent<NetworkIdentity>();
    this.r = (Radio) ((Component) this).GetComponentInParent<Radio>();
    this.cr = (CanvasRenderer) ((Component) this).GetComponent<CanvasRenderer>();
  }

  public void SetAlpha(float a)
  {
    this.cr.SetAlpha(Mathf.Clamp01(a));
  }

  private void Update()
  {
    if (this.nid.get_isLocalPlayer())
      return;
    if (Object.op_Equality((Object) this.vp, (Object) null))
    {
      if (!Object.op_Inequality((Object) this.r.mySource, (Object) null))
        return;
      this.vp = (VoicePlayback) ((Component) this.r.mySource).GetComponent<VoicePlayback>();
    }
    else
    {
      if (this.vp.get_Priority() == -2)
        this.id = 0;
      this.img.set_texture(this.sprites[this.id]);
    }
  }

  private void LateUpdate()
  {
    Class @class = (Class) null;
    if (this.ccm.curClass >= 0)
    {
      @class = this.ccm.klasy[this.ccm.curClass];
      if (this.nid.get_isLocalPlayer())
        SpeakerIcon.iAmHuman = @class.team != Team.SCP;
    }
    if (Object.op_Equality((Object) this.cam, (Object) null))
    {
      this.cam = GameObject.Find("SpectatorCamera").get_transform();
    }
    else
    {
      if (@class == null)
        return;
      ((Component) this).get_transform().set_localPosition(Vector3.op_Addition(Vector3.op_Multiply(Vector3.get_up(), 1.42f), Vector3.op_Multiply(Vector3.get_up(), @class.iconHeightOffset)));
      ((Component) this).get_transform().LookAt(this.cam);
    }
  }
}
