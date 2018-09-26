// Decompiled with JetBrains decompiler
// Type: CCTV_Camera
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class CCTV_Camera : MonoBehaviour
{
  public Transform cameraTarget;
  public Transform camera079;
  public Transform spriteHolder;
  public MeshRenderer spriteRenderer;
  public Material mat;
  public AnimationCurve transparencyOverDistance;
  public AnimationCurve scaleOverDistance;
  public string liftID;

  public CCTV_Camera()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    ((Renderer) this.spriteRenderer).set_material(new Material(this.mat));
  }

  public void UpdateLOD()
  {
    float num = Vector3.Distance(this.camera079.get_position(), ((Component) this).get_transform().get_position());
    ((Renderer) this.spriteRenderer).get_material().set_color(new Color((float) ((Renderer) this.spriteRenderer).get_material().get_color().r, (float) ((Renderer) this.spriteRenderer).get_material().get_color().g, (float) ((Renderer) this.spriteRenderer).get_material().get_color().b, this.transparencyOverDistance.Evaluate(num)));
    ((Component) this.spriteHolder).get_transform().set_localScale(Vector3.op_Multiply(Vector3.get_one(), this.scaleOverDistance.Evaluate(num)));
    ((Component) this.spriteHolder).get_transform().LookAt(this.camera079);
    ((SphereCollider) ((Component) this).GetComponent<SphereCollider>()).set_radius(Mathf.Clamp((float) (((Component) this.spriteHolder).get_transform().get_localScale().x / 2.0), 0.13f, 10f));
  }
}
