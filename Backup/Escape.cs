﻿// Decompiled with JetBrains decompiler
// Type: Escape
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Escape : NetworkBehaviour
{
  private CharacterClassManager ccm;
  private Text respawnText;
  private bool escaped;
  public Vector3 worldPosition;
  public int radius;

  public Escape()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this.respawnText = (Text) GameObject.Find("Respawn Text").GetComponent<Text>();
  }

  private void Update()
  {
    if (!this.get_isLocalPlayer() || (double) Vector3.Distance(((Component) this).get_transform().get_position(), this.worldPosition) >= (double) this.radius)
      return;
    this.EscapeFromFacility();
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_color(Color.get_green());
    Gizmos.DrawWireSphere(this.worldPosition, (float) this.radius);
  }

  private void EscapeFromFacility()
  {
    if (this.escaped)
      return;
    if (this.ccm.klasy[this.ccm.curClass].team == Team.RSC)
    {
      this.escaped = true;
      this.ccm.RegisterEscape();
      Timing.RunCoroutine(this._EscapeAnim(TranslationReader.Get("Facility", 29)), (Segment) 0);
      AchievementManager.Achieve("forscience");
    }
    if (this.ccm.klasy[this.ccm.curClass].team != Team.CDP)
      return;
    this.escaped = true;
    this.ccm.RegisterEscape();
    Timing.RunCoroutine(this._EscapeAnim(TranslationReader.Get("Facility", 30)), (Segment) 0);
    AchievementManager.Achieve("awayout");
  }

  [DebuggerHidden]
  private IEnumerator<float> _EscapeAnim(string txt)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new Escape.\u003C_EscapeAnim\u003Ec__Iterator0()
    {
      txt = txt,
      \u0024this = this
    };
  }

  private void UNetVersion()
  {
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
