// Decompiled with JetBrains decompiler
// Type: CameraFocuser
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class CameraFocuser : MonoBehaviour
{
  public Transform lookTarget;
  public float targetFovScale;
  public float minimumAngle;

  public CameraFocuser()
  {
    base.\u002Ector();
  }

  private void OnTriggerStay(Collider other)
  {
    Scp049PlayerScript componentInParent = (Scp049PlayerScript) ((Component) other).GetComponentInParent<Scp049PlayerScript>();
    if (!Object.op_Inequality((Object) componentInParent, (Object) null) || !componentInParent.get_isLocalPlayer())
      return;
    FirstPersonController component = (FirstPersonController) ((Component) componentInParent).GetComponent<FirstPersonController>();
    ((Component) this).get_transform().LookAt(this.lookTarget);
    Mathf.Clamp(Quaternion.Angle(componentInParent.plyCam.get_transform().get_rotation(), ((Component) this).get_transform().get_rotation()), this.minimumAngle, 70f);
  }
}
