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
    this.cachedMotor = (SECTR_CharacterMotor) ((Component) this).GetComponent<SECTR_CharacterMotor>();
  }

  protected override void Update()
  {
    base.Update();
    Vector3 vector3_1;
    if (Input.get_multiTouchEnabled() && !Application.get_isEditor())
      vector3_1 = Vector2.op_Implicit(this.GetScreenJoystick(false));
    else
      ((Vector3) ref vector3_1).\u002Ector(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
    if (Vector3.op_Inequality(vector3_1, Vector3.get_zero()))
    {
      float magnitude = ((Vector3) ref vector3_1).get_magnitude();
      Vector3 vector3_2 = Vector3.op_Division(vector3_1, magnitude);
      float num1 = Mathf.Min(1f, magnitude);
      float num2 = num1 * num1;
      vector3_1 = Vector3.op_Multiply(vector3_2, num2);
    }
    this.cachedMotor.inputMoveDirection = Quaternion.op_Multiply(Quaternion.FromToRotation(Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward()), ((Component) this).get_transform().get_up()), Quaternion.op_Multiply(((Component) this).get_transform().get_rotation(), vector3_1));
    this.cachedMotor.inputJump = Input.GetKey(NewInput.GetKey("Jump"));
  }

  private Vector3 ProjectOntoPlane(Vector3 v, Vector3 normal)
  {
    return Vector3.op_Subtraction(v, Vector3.Project(v, normal));
  }

  private Vector3 ConstantSlerp(Vector3 from, Vector3 to, float angle)
  {
    float num = Mathf.Min(1f, angle / Vector3.Angle(from, to));
    return Vector3.Slerp(from, to, num);
  }
}
