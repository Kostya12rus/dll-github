// Decompiled with JetBrains decompiler
// Type: SECTR_Culler
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (SECTR_Member))]
[ExecuteInEditMode]
[AddComponentMenu("")]
public class SECTR_Culler : MonoBehaviour
{
  private SECTR_Member cachedMember;
  [SECTR_ToolTip("Overrides the culling information on Member.")]
  public bool CullEachChild;

  private void OnEnable()
  {
    this.cachedMember = this.GetComponent<SECTR_Member>();
    this.cachedMember.ChildCulling = !this.CullEachChild ? SECTR_Member.ChildCullModes.Group : SECTR_Member.ChildCullModes.Individual;
  }

  private void OnDisable()
  {
  }
}
