// Decompiled with JetBrains decompiler
// Type: YouWereKilled
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class YouWereKilled : MonoBehaviour
{
  public Texture[] texturesRelatedToClass;
  [Space]
  public GameObject _root;
  public Text _info;
  public RawImage _imageClass;

  public void Play(PlayerStats.HitInfo hitInfo)
  {
    Timing.RunCoroutine(this._Play(hitInfo), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  private IEnumerator<float> _Play(PlayerStats.HitInfo hitInfo)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new YouWereKilled.\u003C_Play\u003Ec__Iterator0() { hitInfo = hitInfo, \u0024this = this };
  }

  private string GetNick(PlayerStats.HitInfo hitInfo)
  {
    if ((Object) hitInfo.GetPlayerObject() == (Object) null)
      return string.Empty;
    return hitInfo.GetPlayerObject().GetComponent<NicknameSync>().myNick;
  }

  private int GetClass(PlayerStats.HitInfo hitInfo)
  {
    return hitInfo.GetPlayerObject().GetComponent<CharacterClassManager>().curClass;
  }
}
