// Decompiled with JetBrains decompiler
// Type: ServerInfo
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ServerInfo : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
  private Canvas canvas;
  public GameObject root;
  public TextMeshProUGUI text;

  public ServerInfo()
  {
    base.\u002Ector();
  }

  public static void ShowInfo(string id)
  {
    Timing.RunCoroutine(((ServerInfo) Object.FindObjectOfType<ServerInfo>())._Show(id), (Segment) 1);
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    int intersectingLink = TMP_TextUtilities.FindIntersectingLink((TMP_Text) this.text, Input.get_mousePosition(), (Camera) null);
    if (intersectingLink == -1)
      return;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    TMP_LinkInfo tmpLinkInfo = ^(TMP_LinkInfo&) ref ((TMP_Text) this.text).get_textInfo().linkInfo[intersectingLink];
    Application.OpenURL(((TMP_LinkInfo) ref tmpLinkInfo).GetLinkID());
  }

  [DebuggerHidden]
  public IEnumerator<float> _Show(string id)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ServerInfo.\u003C_Show\u003Ec__Iterator0()
    {
      id = id,
      \u0024this = this
    };
  }

  public void Close()
  {
    ((ServerInfo) Object.FindObjectOfType<ServerInfo>()).root.SetActive(false);
  }
}
