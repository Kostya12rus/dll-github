// Decompiled with JetBrains decompiler
// Type: ItemBob
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class ItemBob : MonoBehaviour
{
  private FirstPersonController fpc;
  private Animator anim;
  public float speedScale;
  private float lerp;

  public ItemBob()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.anim = (Animator) ((Component) this).GetComponent<Animator>();
    this.fpc = (FirstPersonController) ((Component) this).GetComponentInParent<FirstPersonController>();
  }

  private void Update()
  {
    Vector3 vector3 = Vector3.op_Addition((Vector3) this.fpc.m_MoveDir, Vector3.op_Multiply(Vector3.get_up(), 10f));
    this.lerp = ((Vector3) ref vector3).get_magnitude() * this.speedScale;
    this.anim.SetFloat("speed", this.lerp);
  }
}
