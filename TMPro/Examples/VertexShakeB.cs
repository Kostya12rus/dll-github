// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.VertexShakeB
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
  public class VertexShakeB : MonoBehaviour
  {
    public float AngleMultiplier;
    public float SpeedMultiplier;
    public float CurveScale;
    private TMP_Text m_TextComponent;
    private bool hasTextChanged;

    public VertexShakeB()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      this.m_TextComponent = (TMP_Text) ((Component) this).GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
      ((FastAction<Object>) TMPro_EventManager.TEXT_CHANGED_EVENT).Add(new Action<Object>(this.ON_TEXT_CHANGED));
    }

    private void OnDisable()
    {
      ((FastAction<Object>) TMPro_EventManager.TEXT_CHANGED_EVENT).Remove(new Action<Object>(this.ON_TEXT_CHANGED));
    }

    private void Start()
    {
      this.StartCoroutine(this.AnimateVertexColors());
    }

    private void ON_TEXT_CHANGED(Object obj)
    {
      if (!Object.op_Implicit(obj = (Object) this.m_TextComponent))
        return;
      this.hasTextChanged = true;
    }

    [DebuggerHidden]
    private IEnumerator AnimateVertexColors()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new VertexShakeB.\u003CAnimateVertexColors\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
