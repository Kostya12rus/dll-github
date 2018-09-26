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
    public float moveSpeed = 1f;
    public float yawSpeed = 3f;
    public float pitchSpeed = 3f;
    private const string X_AXIS_KEYBOARD = "Mouse X";
    private const string Y_AXIS_KEYBOARD = "Mouse Y";
    private const string X_AXIS_GAMEPAD = "Horizontal";
    private const string Y_AXIS_GAMEPAD = "Vertical";
    private bool m_gamePadMode;
    private float _yaw;
    private float _pitch;
    private Transform _transform;

    private void Start()
    {
      this._transform = this.transform;
      this._pitch = this._transform.localEulerAngles.x;
      this._yaw = this._transform.localEulerAngles.y;
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
        if (!Input.GetMouseButton(0) || EventSystem.current.IsPointerOverGameObject())
          return;
        this.ChangeYaw(Input.GetAxisRaw("Mouse X") * this.yawSpeed);
        this.ChangePitch(-Input.GetAxisRaw("Mouse Y") * this.pitchSpeed);
      }
    }

    private void MoveForwards(float delta)
    {
      this._transform.position += delta * this._transform.forward;
    }

    private void Strafe(float delta)
    {
      this._transform.position += delta * this._transform.right;
    }

    private void ChangeYaw(float delta)
    {
      this._yaw += delta;
      this.WrapAngle(ref this._yaw);
      this._transform.localEulerAngles = new Vector3(this._pitch, this._yaw, 0.0f);
    }

    private void ChangePitch(float delta)
    {
      this._pitch += delta;
      this.WrapAngle(ref this._pitch);
      this._transform.localEulerAngles = new Vector3(this._pitch, this._yaw, 0.0f);
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
