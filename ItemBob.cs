// Decompiled with JetBrains decompiler
// Type: ItemBob
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class ItemBob : MonoBehaviour
{
  public float speedScale = 1f;
  private FirstPersonController fpc;
  private Animator anim;
  private float lerp;

  private void Start()
  {
    this.anim = this.GetComponent<Animator>();
    this.fpc = this.GetComponentInParent<FirstPersonController>();
  }

  private void Update()
  {
    this.lerp = (this.fpc.m_MoveDir + Vector3.up * 10f).magnitude * this.speedScale;
    this.anim.SetFloat("speed", this.lerp);
  }
}
