// Decompiled with JetBrains decompiler
// Type: CameraFocuser
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class CameraFocuser : MonoBehaviour
{
  public float targetFovScale = 1f;
  public Transform lookTarget;
  public float minimumAngle;

  private void OnTriggerStay(Collider other)
  {
    Scp049PlayerScript componentInParent = other.GetComponentInParent<Scp049PlayerScript>();
    if (!((Object) componentInParent != (Object) null) || !componentInParent.isLocalPlayer)
      return;
    componentInParent.GetComponent<FirstPersonController>();
    this.transform.LookAt(this.lookTarget);
    Mathf.Clamp(Quaternion.Angle(componentInParent.plyCam.transform.rotation, this.transform.rotation), this.minimumAngle, 70f);
  }
}
