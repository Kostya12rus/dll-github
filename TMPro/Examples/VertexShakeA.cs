// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.VertexShakeA
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
  public class VertexShakeA : MonoBehaviour
  {
    public float AngleMultiplier = 1f;
    public float SpeedMultiplier = 1f;
    public float ScaleMultiplier = 1f;
    public float RotationMultiplier = 1f;
    private TMP_Text m_TextComponent;
    private bool hasTextChanged;

    private void Awake()
    {
      this.m_TextComponent = this.GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
      TMPro_EventManager.TEXT_CHANGED_EVENT.Add(new Action<Object>(this.ON_TEXT_CHANGED));
    }

    private void OnDisable()
    {
      TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(new Action<Object>(this.ON_TEXT_CHANGED));
    }

    private void Start()
    {
      this.StartCoroutine(this.AnimateVertexColors());
    }

    private void ON_TEXT_CHANGED(Object obj)
    {
      if (!(bool) (obj = (Object) this.m_TextComponent))
        return;
      this.hasTextChanged = true;
    }

    [DebuggerHidden]
    private IEnumerator AnimateVertexColors()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new VertexShakeA.\u003CAnimateVertexColors\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
