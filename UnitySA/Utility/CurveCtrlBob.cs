// Decompiled with JetBrains decompiler
// Type: UnitySA.Utility.CurveCtrlBob
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace UnitySA.Utility
{
  [Serializable]
  public class CurveCtrlBob
  {
    public float HorizontalBobRange = 0.33f;
    public float VerticalBobRange = 0.33f;
    public AnimationCurve Bobcurve = new AnimationCurve(new Keyframe[5]{ new Keyframe(0.0f, 0.0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0.0f), new Keyframe(1.5f, -1f), new Keyframe(2f, 0.0f) });
    public float VerticaltoHorizontalRatio = 1f;
    private float m_CyclePositionX;
    private float m_CyclePositionY;
    private float m_BobBaseInterval;
    private Vector3 m_OriginalCameraPosition;
    private float m_Time;

    public void Setup(Camera camera, float bobBaseInterval)
    {
      this.m_BobBaseInterval = bobBaseInterval;
      this.m_OriginalCameraPosition = camera.transform.localPosition;
      this.m_Time = this.Bobcurve[this.Bobcurve.length - 1].time;
    }

    public Vector3 DoHeadBob(float speed)
    {
      float x = this.m_OriginalCameraPosition.x + this.Bobcurve.Evaluate(this.m_CyclePositionX) * this.HorizontalBobRange;
      float y = this.m_OriginalCameraPosition.y + this.Bobcurve.Evaluate(this.m_CyclePositionY) * this.VerticalBobRange;
      this.m_CyclePositionX += speed * Time.deltaTime / this.m_BobBaseInterval;
      this.m_CyclePositionY += speed * Time.deltaTime / this.m_BobBaseInterval * this.VerticaltoHorizontalRatio;
      if ((double) this.m_CyclePositionX > (double) this.m_Time)
        this.m_CyclePositionX -= this.m_Time;
      if ((double) this.m_CyclePositionY > (double) this.m_Time)
        this.m_CyclePositionY -= this.m_Time;
      return new Vector3(x, y, 0.0f);
    }
  }
}
