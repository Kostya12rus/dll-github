// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TeleType
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TMPro.Examples
{
  public class TeleType : MonoBehaviour
  {
    private string label01 = "Example <sprite=2> of using <sprite=7> <#ffa000>Graphics Inline</color> <sprite=5> with Text in <font=\"Bangers SDF\" material=\"Bangers SDF - Drop Shadow\">TextMesh<#40a0ff>Pro</color></font><sprite=0> and Unity<sprite=1>";
    private string label02 = "Example <sprite=2> of using <sprite=7> <#ffa000>Graphics Inline</color> <sprite=5> with Text in <font=\"Bangers SDF\" material=\"Bangers SDF - Drop Shadow\">TextMesh<#40a0ff>Pro</color></font><sprite=0> and Unity<sprite=2>";
    private TMP_Text m_textMeshPro;

    private void Awake()
    {
      this.m_textMeshPro = this.GetComponent<TMP_Text>();
      this.m_textMeshPro.text = this.label01;
      this.m_textMeshPro.enableWordWrapping = true;
      this.m_textMeshPro.alignment = TextAlignmentOptions.Top;
    }

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TeleType.\u003CStart\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
