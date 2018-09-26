// Decompiled with JetBrains decompiler
// Type: ServerListManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public class ServerListManager : MonoBehaviour
{
  private List<GameObject> spawns = new List<GameObject>();
  public RectTransform contentParent;
  public RectTransform element;
  public UnityEngine.UI.Text loadingText;
  public static ServerListManager singleton;
  private ServerFilters filters;

  private void Awake()
  {
    this.filters = this.GetComponent<ServerFilters>();
    ServerListManager.singleton = this;
  }

  public GameObject AddRecord()
  {
    RectTransform rectTransform = Object.Instantiate<RectTransform>(this.element);
    rectTransform.SetParent((Transform) this.contentParent);
    rectTransform.localScale = Vector3.one;
    rectTransform.localPosition = Vector3.zero;
    this.spawns.Add(rectTransform.gameObject);
    this.contentParent.sizeDelta = Vector2.up * 150f * (float) this.spawns.Count;
    return rectTransform.gameObject;
  }

  private void OnEnable()
  {
    this.Refresh();
  }

  public void ApplyNameFilter(string nameFilter)
  {
    this.filters.nameFilter = nameFilter;
    this.Refresh();
  }

  public void Refresh()
  {
    Timing.RunCoroutine(this._DownloadList(), Segment.FixedUpdate);
  }

  [DebuggerHidden]
  public IEnumerator<float> _DownloadList()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new ServerListManager.\u003C_DownloadList\u003Ec__Iterator0() { \u0024this = this };
  }

  private string Base64Decode(string t)
  {
    return Encoding.UTF8.GetString(Convert.FromBase64String(t));
  }
}
