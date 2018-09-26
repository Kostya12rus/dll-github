// Decompiled with JetBrains decompiler
// Type: EnvMapAnimator
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class EnvMapAnimator : MonoBehaviour
{
  public Vector3 RotationSpeeds;
  private TMP_Text m_textMeshPro;
  private Material m_material;

  public EnvMapAnimator()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    this.m_textMeshPro = (TMP_Text) ((Component) this).GetComponent<TMP_Text>();
    this.m_material = this.m_textMeshPro.get_fontSharedMaterial();
  }

  [DebuggerHidden]
  private IEnumerator Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new EnvMapAnimator.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
