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

  private void LateUpdate()
  {
    if ((Object) this.forwardDirection == (Object) null)
    {
      this.light.enabled = false;
    }
    else
    {
      this.light.enabled = true;
      float num = Vector3.Angle(this.transform.forward, this.forwardDirection.transform.forward);
      this.rotCam = this.transform.rotation.eulerAngles;
      this.rotBar = this.forwardDirection.transform.rotation.eulerAngles;
      this.rotBar.z = 0.0f;
      this.rotCam.z = 0.0f;
      Quaternion quaternion = Quaternion.Euler((this.rotBar - this.rotCam) * 4f);
      this.localRot = (double) num <= (double) this.maxAngle ? Quaternion.Euler(Vector3.zero) : quaternion;
      Physics.Raycast(this.transform.position, this.transform.forward, out this.hit, 1000f, (int) this.raycastMask);
      this.hitPoint = this.hit.point;
      this.light.spotAngle = this.sizeOverDistance.Evaluate(this.hit.distance);
      this.light.transform.localPosition = Vector3.forward * this.hit.distance * 0.75f;
      this.light.transform.localRotation = Quaternion.Lerp(this.light.transform.localRotation, this.localRot, Time.deltaTime * this.speedLerp);
    }
  }

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawSphere(this.hit.point, 0.5f);
  }
}
