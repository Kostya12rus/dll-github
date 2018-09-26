// Decompiled with JetBrains decompiler
// Type: WeaponLaser
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class WeaponLaser : MonoBehaviour
{
  public GameObject forwardDirection;
  public Light light;
  public AnimationCurve sizeOverDistance;
  private Quaternion localRot;
  public float speedLerp;
  public float maxAngle;
  private Vector3 rotCam;
  private Vector3 rotBar;
  private Vector3 hitPoint;
  public LayerMask raycastMask;
  private RaycastHit hit;

  public WeaponLaser()
  {
    base.\u002Ector();
  }

  private void LateUpdate()
  {
    if (Object.op_Equality((Object) this.forwardDirection, (Object) null))
    {
      ((Behaviour) this.light).set_enabled(false);
    }
    else
    {
      ((Behaviour) this.light).set_enabled(true);
      float num = Vector3.Angle(((Component) this).get_transform().get_forward(), this.forwardDirection.get_transform().get_forward());
      Quaternion rotation1 = ((Component) this).get_transform().get_rotation();
      this.rotCam = ((Quaternion) ref rotation1).get_eulerAngles();
      Quaternion rotation2 = this.forwardDirection.get_transform().get_rotation();
      this.rotBar = ((Quaternion) ref rotation2).get_eulerAngles();
      this.rotBar.z = (__Null) 0.0;
      this.rotCam.z = (__Null) 0.0;
      Quaternion quaternion = Quaternion.Euler(Vector3.op_Multiply(Vector3.op_Subtraction(this.rotBar, this.rotCam), 4f));
      this.localRot = (double) num <= (double) this.maxAngle ? Quaternion.Euler(Vector3.get_zero()) : quaternion;
      Physics.Raycast(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_forward(), ref this.hit, 1000f, LayerMask.op_Implicit(this.raycastMask));
      this.hitPoint = ((RaycastHit) ref this.hit).get_point();
      this.light.set_spotAngle(this.sizeOverDistance.Evaluate(((RaycastHit) ref this.hit).get_distance()));
      ((Component) this.light).get_transform().set_localPosition(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_forward(), ((RaycastHit) ref this.hit).get_distance()), 0.75f));
      ((Component) this.light).get_transform().set_localRotation(Quaternion.Lerp(((Component) this.light).get_transform().get_localRotation(), this.localRot, Time.get_deltaTime() * this.speedLerp));
    }
  }

  private void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_cyan());
    Gizmos.DrawSphere(((RaycastHit) ref this.hit).get_point(), 0.5f);
  }
}
