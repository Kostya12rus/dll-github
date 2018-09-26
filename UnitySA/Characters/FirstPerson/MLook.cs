// Decompiled with JetBrains decompiler
// Type: UnitySA.Characters.FirstPerson.MLook
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace UnitySA.Characters.FirstPerson
{
  [Serializable]
  public class MLook
  {
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90f;
    public float MaximumX = 90f;
    public float smoothTime = 5f;
    public bool lockCursor = true;
    private bool m_cursorIsLocked = true;
    public bool smooth;
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    public void Init(Transform character, Transform camera)
    {
      this.m_CharacterTargetRot = character.get_localRotation();
      this.m_CameraTargetRot = camera.get_localRotation();
    }

    public void LookRotation(Transform character, Transform camera)
    {
      float num1 = Input.GetAxis("Mouse X") * this.XSensitivity;
      float num2 = Input.GetAxis("Mouse Y") * this.YSensitivity;
      MLook mlook1 = this;
      mlook1.m_CharacterTargetRot = Quaternion.op_Multiply(mlook1.m_CharacterTargetRot, Quaternion.Euler(0.0f, num1, 0.0f));
      MLook mlook2 = this;
      mlook2.m_CameraTargetRot = Quaternion.op_Multiply(mlook2.m_CameraTargetRot, Quaternion.Euler(-num2, 0.0f, 0.0f));
      if (this.clampVerticalRotation)
        this.m_CameraTargetRot = this.ClampRotationAroundXAxis(this.m_CameraTargetRot);
      if (this.smooth)
      {
        character.set_localRotation(Quaternion.Slerp(character.get_localRotation(), this.m_CharacterTargetRot, this.smoothTime * Time.get_deltaTime()));
        camera.set_localRotation(Quaternion.Slerp(camera.get_localRotation(), this.m_CameraTargetRot, this.smoothTime * Time.get_deltaTime()));
      }
      else
      {
        character.set_localRotation(this.m_CharacterTargetRot);
        camera.set_localRotation(this.m_CameraTargetRot);
      }
      this.UpdateCursorLock();
    }

    public void SetCursorLock(bool value)
    {
      this.lockCursor = value;
      if (this.lockCursor)
        return;
      Cursor.set_lockState((CursorLockMode) 0);
      Cursor.set_visible(true);
    }

    public void UpdateCursorLock()
    {
      if (!this.lockCursor)
        return;
      this.InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {
      if (Input.GetKeyUp((KeyCode) 27))
        this.m_cursorIsLocked = false;
      else if (Input.GetMouseButtonUp(0))
        this.m_cursorIsLocked = true;
      if (this.m_cursorIsLocked)
      {
        Cursor.set_lockState((CursorLockMode) 1);
        Cursor.set_visible(false);
      }
      else
      {
        if (this.m_cursorIsLocked)
          return;
        Cursor.set_lockState((CursorLockMode) 0);
        Cursor.set_visible(true);
      }
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
      ref Quaternion local1 = ref q;
      local1.x = local1.x / q.w;
      ref Quaternion local2 = ref q;
      local2.y = local2.y / q.w;
      ref Quaternion local3 = ref q;
      local3.z = local3.z / q.w;
      q.w = (__Null) 1.0;
      float num = Mathf.Clamp(114.5916f * Mathf.Atan((float) q.x), this.MinimumX, this.MaximumX);
      q.x = (__Null) (double) Mathf.Tan((float) Math.PI / 360f * num);
      return q;
    }
  }
}
