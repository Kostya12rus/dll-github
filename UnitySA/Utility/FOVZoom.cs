// Decompiled with JetBrains decompiler
// Type: UnitySA.Utility.FOVZoom
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace UnitySA.Utility
{
  [Serializable]
  public class FOVZoom
  {
    public float FOVIncrease = 3f;
    public float TimeToIncrease = 1f;
    public float TimeToDecrease = 1f;
    public Camera Camera;
    [HideInInspector]
    public float originalFov;
    public AnimationCurve IncreaseCurve;

    public void Setup(Camera camera)
    {
      this.CheckStatus(camera);
      this.Camera = camera;
      this.originalFov = camera.fieldOfView;
    }

    private void CheckStatus(Camera camera)
    {
      if ((Object) camera == (Object) null)
        throw new Exception("FOVKick camera is null, please supply the camera to the constructor");
      if (this.IncreaseCurve == null)
        throw new Exception("FOVKick Increase curve is null, please define the curve for the field of view kicks");
    }

    public void ChangeCamera(Camera camera)
    {
      this.Camera = camera;
    }

    [DebuggerHidden]
    public IEnumerator FOVKickUp()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FOVZoom.\u003CFOVKickUp\u003Ec__Iterator0() { \u0024this = this };
    }

    [DebuggerHidden]
    public IEnumerator FOVKickDown()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FOVZoom.\u003CFOVKickDown\u003Ec__Iterator1() { \u0024this = this };
    }
  }
}
