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

  public static void ShowInfo(string id)
  {
    Timing.RunCoroutine(Object.FindObjectOfType<ServerInfo>()._Show(id), Segment.FixedUpdate);
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    int intersectingLink = TMP_TextUtilities.FindIntersectingLink((TMP_Text) this.text, Input.mousePosition, (Camera) null);
    if (intersectingLink == -1)
      return;
    Application.OpenURL(this.text.textInfo.linkInfo[intersectingLink].GetLinkID());
  }

  [DebuggerHidden]
  public IEnumerator<float> _Show(string id)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ServerInfo.\u003C_Show\u003Ec__Iterator0() { id = id, \u0024this = this };
  }

  public void Close()
  {
    Object.FindObjectOfType<ServerInfo>().root.SetActive(false);
  }
}
