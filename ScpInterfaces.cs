// Decompiled with JetBrains decompiler
// Type: ScpInterfaces
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScpInterfaces : MonoBehaviour
{
  public GameObject Scp106_eq;
  public TextMeshProUGUI Scp106_ability_highlight;
  public Text Scp106_ability_points;
  public GameObject Scp049_eq;
  public Image Scp049_loading;
  public TextMeshProUGUI remainingTargets;

  public ScpInterfaces()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    Timing.RunCoroutine(this._UpdateTargets());
  }

  [DebuggerHidden]
  private IEnumerator<float> _UpdateTargets()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ScpInterfaces.\u003C_UpdateTargets\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private GameObject FindLocalPlayer()
  {
    return PlayerManager.localPlayer;
  }

  public void CreatePortal()
  {
    ((Scp106PlayerScript) this.FindLocalPlayer().GetComponent<Scp106PlayerScript>()).CreatePortalInCurrentPosition();
  }

  public void Update106Highlight(int id)
  {
    ((Scp106PlayerScript) this.FindLocalPlayer().GetComponent<Scp106PlayerScript>()).highlightID = id;
  }

  public void Use106Portal()
  {
    ((Scp106PlayerScript) this.FindLocalPlayer().GetComponent<Scp106PlayerScript>()).UseTeleport();
  }
}
