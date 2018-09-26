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
    private string label01;
    private string label02;
    private TMP_Text m_textMeshPro;

    public TeleType()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      this.m_textMeshPro = (TMP_Text) ((Component) this).GetComponent<TMP_Text>();
      this.m_textMeshPro.set_text(this.label01);
      this.m_textMeshPro.set_enableWordWrapping(true);
      this.m_textMeshPro.set_alignment((TextAlignmentOptions) 258);
    }

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TeleType.\u003CStart\u003Ec__Iterator0() { \u0024this = this };
    }
  }
}
