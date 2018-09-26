// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.DemoCameraMovement
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

namespace AmplifyBloom
{
  public class DemoCameraMovement : MonoBehaviour
  {
    private const string X_AXIS_KEYBOARD = "Mouse X";
    private const string Y_AXIS_KEYBOARD = "Mouse Y";
    private const string X_AXIS_GAMEPAD = "Horizontal";
    private const string Y_AXIS_GAMEPAD = "Vertical";
    private bool m_gamePadMode;
    public float moveSpeed;
    public float yawSpeed;
    public float pitchSpeed;
    private float _yaw;
    private float _pitch;
    private Transform _transform;

    public DemoCameraMovement()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      this._transform = ((Component) this).get_transform();
      this._pitch = (float) this._transform.get_localEulerAngles().x;
      this._yaw = (float) this._transform.get_localEulerAngles().y;
      if (Input.GetJoystickNames().Length <= 0)
        return;
      this.m_gamePadMode = true;
    }

    private void Update()
    {
      if (this.m_gamePadMode)
      {
        this.ChangeYaw(Input.GetAxisRaw("Horizontal") * this.yawSpeed);
        this.ChangePitch(-Input.GetAxisRaw("Vertical") * this.pitchSpeed);
      }
      else
      {
        if (!Input.GetMouseButton(0) || EventSystem.get_current().IsPointerOverGameObject())
          return;
        this.ChangeYaw(Input.GetAxisRaw("Mouse X") * this.yawSpeed);
        this.ChangePitch(-Input.GetAxisRaw("Mouse Y") * this.pitchSpeed);
      }
    }

    private void MoveForwards(float delta)
    {
      Transform transform = this._transform;
      transform.set_position(Vector3.op_Addition(transform.get_position(), Vector3.op_Multiply(delta, this._transform.get_forward())));
    }

    private void Strafe(float delta)
    {
      Transform transform = this._transform;
      transform.set_position(Vector3.op_Addition(transform.get_position(), Vector3.op_Multiply(delta, this._transform.get_right())));
    }

    private void ChangeYaw(float delta)
    {
      this._yaw += delta;
      this.WrapAngle(ref this._yaw);
      this._transform.set_localEulerAngles(new Vector3(this._pitch, this._yaw, 0.0f));
    }

    private void ChangePitch(float delta)
    {
      this._pitch += delta;
      this.WrapAngle(ref this._pitch);
      this._transform.set_localEulerAngles(new Vector3(this._pitch, this._yaw, 0.0f));
    }

    public void WrapAngle(ref float angle)
    {
      if ((double) angle < 0.0)
        angle = 360f + angle;
      if ((double) angle <= 360.0)
        return;
      angle -= 360f;
    }

    public bool GamePadMode
    {
      get
      {
        return this.m_gamePadMode;
      }
    }
  }
}
