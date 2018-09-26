// Decompiled with JetBrains decompiler
// Type: SECTR_FPController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Camera))]
public abstract class SECTR_FPController : MonoBehaviour
{
  private Vector2 _mouseAbsolute;
  private Vector2 _smoothMouse;
  private Vector2 _clampInDegrees;
  private Vector2 _targetDirection;
  private bool focused;
  protected Dictionary<int, SECTR_FPController.TrackedTouch> _touches;
  [SECTR_ToolTip("Whether to lock the cursor when this camera is active.")]
  public bool LockCursor;
  [SECTR_ToolTip("Scalar for mouse sensitivity.")]
  public Vector2 Sensitivity;
  [SECTR_ToolTip("Scalar for mouse smoothing.")]
  public Vector2 Smoothing;
  [SECTR_ToolTip("Adjusts the size of the virtual joystick.")]
  public float TouchScreenLookScale;

  protected SECTR_FPController()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    Quaternion localRotation = ((Component) this).get_transform().get_localRotation();
    this._targetDirection = Vector2.op_Implicit(((Quaternion) ref localRotation).get_eulerAngles());
  }

  private void OnApplicationFocus(bool focused)
  {
    this.focused = focused;
  }

  protected virtual void Update()
  {
    if (!this.focused)
      return;
    Cursor.set_lockState((CursorLockMode) 1);
    Cursor.set_visible(false);
    Quaternion quaternion1 = Quaternion.Euler(Vector2.op_Implicit(this._targetDirection));
    Vector2 screenJoystick;
    if (Input.get_multiTouchEnabled() && !Application.get_isEditor())
    {
      this._UpdateTouches();
      screenJoystick = this.GetScreenJoystick(true);
    }
    else
    {
      screenJoystick.x = (__Null) (double) Input.GetAxisRaw("Mouse X");
      screenJoystick.y = (__Null) (double) Input.GetAxisRaw("Mouse Y");
    }
    Vector2 vector2 = Vector2.Scale(screenJoystick, new Vector2((float) (this.Sensitivity.x * this.Smoothing.x), (float) (this.Sensitivity.y * this.Smoothing.y)));
    if (Input.get_multiTouchEnabled())
    {
      this._smoothMouse = vector2;
    }
    else
    {
      this._smoothMouse.x = (__Null) (double) Mathf.Lerp((float) this._smoothMouse.x, (float) vector2.x, (float) (1.0 / this.Smoothing.x));
      this._smoothMouse.y = (__Null) (double) Mathf.Lerp((float) this._smoothMouse.y, (float) vector2.y, (float) (1.0 / this.Smoothing.y));
    }
    SECTR_FPController sectrFpController = this;
    sectrFpController._mouseAbsolute = Vector2.op_Addition(sectrFpController._mouseAbsolute, this._smoothMouse);
    if (this._clampInDegrees.x < 360.0)
      this._mouseAbsolute.x = (__Null) (double) Mathf.Clamp((float) this._mouseAbsolute.x, (float) (-this._clampInDegrees.x * 0.5), (float) (this._clampInDegrees.x * 0.5));
    ((Component) this).get_transform().set_localRotation(Quaternion.AngleAxis((float) -this._mouseAbsolute.y, Quaternion.op_Multiply(quaternion1, Vector3.get_right())));
    if (this._clampInDegrees.y < 360.0)
      this._mouseAbsolute.y = (__Null) (double) Mathf.Clamp((float) this._mouseAbsolute.y, (float) (-this._clampInDegrees.y * 0.5), (float) (this._clampInDegrees.y * 0.5));
    Transform transform1 = ((Component) this).get_transform();
    transform1.set_localRotation(Quaternion.op_Multiply(transform1.get_localRotation(), quaternion1));
    Quaternion quaternion2 = Quaternion.AngleAxis((float) this._mouseAbsolute.x, ((Component) this).get_transform().InverseTransformDirection(Vector3.get_up()));
    Transform transform2 = ((Component) this).get_transform();
    transform2.set_localRotation(Quaternion.op_Multiply(transform2.get_localRotation(), quaternion2));
  }

  protected Vector2 GetScreenJoystick(bool left)
  {
    foreach (SECTR_FPController.TrackedTouch trackedTouch in this._touches.Values)
    {
      float num = (float) Screen.get_width() * 0.5f;
      if (left && trackedTouch.startPos.x < (double) num || !left && trackedTouch.startPos.x > (double) num)
      {
        Vector2 vector2 = Vector2.op_Subtraction(trackedTouch.currentPos, trackedTouch.startPos);
        vector2.x = (__Null) (double) Mathf.Clamp((float) (vector2.x / ((double) num * 0.5 * (double) this.TouchScreenLookScale)), -1f, 1f);
        vector2.y = (__Null) (double) Mathf.Clamp((float) (vector2.y / ((double) Screen.get_height() * 0.5 * (double) this.TouchScreenLookScale)), -1f, 1f);
        return vector2;
      }
    }
    return Vector2.get_zero();
  }

  private void _UpdateTouches()
  {
    int touchCount = Input.get_touchCount();
    for (int index = 0; index < touchCount; ++index)
    {
      Touch touch = Input.get_touches()[index];
      if (((Touch) ref touch).get_phase() == null)
      {
        Debug.Log((object) ("Touch " + (object) ((Touch) ref touch).get_fingerId() + "Started at : " + (object) ((Touch) ref touch).get_position()));
        this._touches.Add(((Touch) ref touch).get_fingerId(), new SECTR_FPController.TrackedTouch()
        {
          startPos = ((Touch) ref touch).get_position(),
          currentPos = ((Touch) ref touch).get_position()
        });
      }
      else if (((Touch) ref touch).get_phase() == 4 || ((Touch) ref touch).get_phase() == 3)
      {
        Debug.Log((object) ("Touch " + (object) ((Touch) ref touch).get_fingerId() + "Ended at : " + (object) ((Touch) ref touch).get_position()));
        this._touches.Remove(((Touch) ref touch).get_fingerId());
      }
      else
      {
        SECTR_FPController.TrackedTouch trackedTouch;
        if (this._touches.TryGetValue(((Touch) ref touch).get_fingerId(), out trackedTouch))
          trackedTouch.currentPos = ((Touch) ref touch).get_position();
      }
    }
  }

  protected class TrackedTouch
  {
    public Vector2 startPos;
    public Vector2 currentPos;
  }
}
