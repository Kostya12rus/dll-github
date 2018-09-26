// Decompiled with JetBrains decompiler
// Type: SECTR_FPSController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("SECTR/Demos/SECTR Character Controller")]
[RequireComponent(typeof (SECTR_CharacterMotor))]
public class SECTR_FPSController : SECTR_FPController
{
  private SECTR_CharacterMotor cachedMotor;

  private void Awake()
  {
    this.cachedMotor = this.GetComponent<SECTR_CharacterMotor>();
  }

  protected override void Update()
  {
    base.Update();
    Vector3 vector3_1 = !Input.multiTouchEnabled || Application.isEditor ? new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f) : (Vector3) this.GetScreenJoystick(false);
    if (vector3_1 != Vector3.zero)
    {
      float magnitude = vector3_1.magnitude;
      Vector3 vector3_2 = vector3_1 / magnitude;
      float num1 = Mathf.Min(1f, magnitude);
      float num2 = num1 * num1;
      vector3_1 = vector3_2 * num2;
    }
    this.cachedMotor.inputMoveDirection = Quaternion.FromToRotation(-this.transform.forward, this.transform.up) * (this.transform.rotation * vector3_1);
    this.cachedMotor.inputJump = Input.GetKey(NewInput.GetKey("Jump"));
  }

  private Vector3 ProjectOntoPlane(Vector3 v, Vector3 normal)
  {
    return v - Vector3.Project(v, normal);
  }

  private Vector3 ConstantSlerp(Vector3 from, Vector3 to, float angle)
  {
    float t = Mathf.Min(1f, angle / Vector3.Angle(from, to));
    return Vector3.Slerp(from, to, t);
  }
}
