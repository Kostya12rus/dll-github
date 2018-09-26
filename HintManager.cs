// Decompiled with JetBrains decompiler
// Type: HintManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (AudioSource))]
public class HintManager : MonoBehaviour
{
  public List<HintManager.Hint> hintQueue = new List<HintManager.Hint>();
  public static HintManager singleton;
  [SerializeField]
  private Image box;
  public HintManager.Hint[] hints;

  private void Awake()
  {
    this.box.canvasRenderer.SetAlpha(0.0f);
    HintManager.singleton = this;
    for (int v = 0; v < this.hints.Length; ++v)
      this.hints[v].content_en = TranslationReader.Get("Hints", v);
  }

  private void Start()
  {
    this.box.canvasRenderer.SetAlpha(0.0f);
  }

  [DebuggerHidden]
  private IEnumerator<float> _ShowHints()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    HintManager.\u003C_ShowHints\u003Ec__Iterator0 showHintsCIterator0 = new HintManager.\u003C_ShowHints\u003Ec__Iterator0();
    return (IEnumerator<float>) showHintsCIterator0;
  }

  public void AddHint(int hintID)
  {
    if (TutorialManager.status || PlayerPrefs.GetInt(this.hints[hintID].keyName, 0) != 0)
      return;
    this.hintQueue.Add(this.hints[hintID]);
    PlayerPrefs.SetInt(this.hints[hintID].keyName, 1);
  }

  [Serializable]
  public class Hint
  {
    [Multiline]
    public string content_en;
    public string keyName;
    public float duration;
  }
}
