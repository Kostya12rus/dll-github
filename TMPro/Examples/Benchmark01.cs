// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.Benchmark01
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
  public class Benchmark01 : MonoBehaviour
  {
    public int BenchmarkType;
    public TMP_FontAsset TMProFont;
    public Font TextMeshFont;
    private TextMeshPro m_textMeshPro;
    private TextContainer m_textContainer;
    private TextMesh m_textMesh;
    private const string label01 = "The <#0050FF>count is: </color>{0}";
    private const string label02 = "The <color=#0050FF>count is: </color>";
    private Material m_material01;
    private Material m_material02;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Benchmark01.\u003CStart\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
