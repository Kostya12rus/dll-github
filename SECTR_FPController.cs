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
  private Vector2 _clampInDegrees = new Vector2(360f, 180f);
  private bool focused = true;
  protected Dictionary<int, SECTR_FPController.TrackedTouch> _touches = new Dictionary<int, SECTR_FPController.TrackedTouch>();
  [SECTR_ToolTip("Whether to lock the cursor when this camera is active.")]
  public bool LockCursor = true;
  [SECTR_ToolTip("Scalar for mouse sensitivity.")]
  public Vector2 Sensitivity = new Vector2(2f, 2f);
  [SECTR_ToolTip("Scalar for mouse smoothing.")]
  public Vector2 Smoothing = new Vector2(3f, 3f);
  [SECTR_ToolTip("Adjusts the size of the virtual joystick.")]
  public float TouchScreenLookScale = 1f;
  private Vector2 _mouseAbsolute;
  private Vector2 _smoothMouse;
  private Vector2 _targetDirection;

  private void Start()
  {
    this._targetDirection = (Vector2) this.transform.localRotation.eulerAngles;
  }

  private void OnApplicationFocus(bool focused)
  {
    this.focused = focused;
  }

  protected virtual void Update()
  {
    if (!this.focused)
      return;
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    Quaternion quaternion = Quaternion.Euler((Vector3) this._targetDirection);
    Vector2 screenJoystick;
    if (Input.multiTouchEnabled && !Application.isEditor)
    {
      this._UpdateTouches();
      screenJoystick = this.GetScreenJoystick(true);
    }
    else
    {
      screenJoystick.x = Input.GetAxisRaw("Mouse X");
      screenJoystick.y = Input.GetAxisRaw("Mouse Y");
    }
    Vector2 vector2 = Vector2.Scale(screenJoystick, new Vector2(this.Sensitivity.x * this.Smoothing.x, this.Sensitivity.y * this.Smoothing.y));
    if (Input.multiTouchEnabled)
    {
      this._smoothMouse = vector2;
    }
    else
    {
      this._smoothMouse.x = Mathf.Lerp(this._smoothMouse.x, vector2.x, 1f / this.Smoothing.x);
      this._smoothMouse.y = Mathf.Lerp(this._smoothMouse.y, vector2.y, 1f / this.Smoothing.y);
    }
    this._mouseAbsolute += this._smoothMouse;
    if ((double) this._clampInDegrees.x < 360.0)
      this._mouseAbsolute.x = Mathf.Clamp(this._mouseAbsolute.x, (float) (-(double) this._clampInDegrees.x * 0.5), this._clampInDegrees.x * 0.5f);
    this.transform.localRotation = Quaternion.AngleAxis(-this._mouseAbsolute.y, quaternion * Vector3.right);
    if ((double) this._clampInDegrees.y < 360.0)
      this._mouseAbsolute.y = Mathf.Clamp(this._mouseAbsolute.y, (float) (-(double) this._clampInDegrees.y * 0.5), this._clampInDegrees.y * 0.5f);
    this.transform.localRotation *= quaternion;
    this.transform.localRotation *= Quaternion.AngleAxis(this._mouseAbsolute.x, this.transform.InverseTransformDirection(Vector3.up));
  }

  protected Vector2 GetScreenJoystick(bool left)
  {
    foreach (SECTR_FPController.TrackedTouch trackedTouch in this._touches.Values)
    {
      float num = (float) Screen.width * 0.5f;
      if (left && (double) trackedTouch.startPos.x < (double) num || !left && (double) trackedTouch.startPos.x > (double) num)
      {
        Vector2 vector2 = trackedTouch.currentPos - trackedTouch.startPos;
        vector2.x = Mathf.Clamp(vector2.x / (num * 0.5f * this.TouchScreenLookScale), -1f, 1f);
        vector2.y = Mathf.Clamp(vector2.y / ((float) Screen.height * 0.5f * this.TouchScreenLookScale), -1f, 1f);
        return vector2;
      }
    }
    return Vector2.zero;
  }

  private void _UpdateTouches()
  {
    int touchCount = Input.touchCount;
    for (int index = 0; index < touchCount; ++index)
    {
      Touch touch = Input.touches[index];
      if (touch.phase == TouchPhase.Began)
      {
        Debug.Log((object) ("Touch " + (object) touch.fingerId + "Started at : " + (object) touch.position));
        this._touches.Add(touch.fingerId, new SECTR_FPController.TrackedTouch()
        {
          startPos = touch.position,
          currentPos = touch.position
        });
      }
      else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
      {
        Debug.Log((object) ("Touch " + (object) touch.fingerId + "Ended at : " + (object) touch.position));
        this._touches.Remove(touch.fingerId);
      }
      else
      {
        SECTR_FPController.TrackedTouch trackedTouch;
        if (this._touches.TryGetValue(touch.fingerId, out trackedTouch))
          trackedTouch.currentPos = touch.position;
      }
    }
  }

  protected class TrackedTouch
  {
    public Vector2 startPos;
    public Vector2 currentPos;
  }
}
