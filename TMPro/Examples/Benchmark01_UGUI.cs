// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.Benchmark01_UGUI
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro.Examples
{
  public class Benchmark01_UGUI : MonoBehaviour
  {
    public int BenchmarkType;
    public Canvas canvas;
    public TMP_FontAsset TMProFont;
    public Font TextMeshFont;
    private TextMeshProUGUI m_textMeshPro;
    private Text m_textMesh;
    private const string label01 = "The <#0050FF>count is: </color>";
    private const string label02 = "The <color=#0050FF>count is: </color>";
    private Material m_material01;
    private Material m_material02;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Benchmark01_UGUI.\u003CStart\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
