// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.VertexColorCycler
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
  public class VertexColorCycler : MonoBehaviour
  {
    private TMP_Text m_TextComponent;

    private void Awake()
    {
      this.m_TextComponent = this.GetComponent<TMP_Text>();
    }

    private void Start()
    {
      this.StartCoroutine(this.AnimateVertexColors());
    }

    [DebuggerHidden]
    private IEnumerator AnimateVertexColors()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new VertexColorCycler.\u003CAnimateVertexColors\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
