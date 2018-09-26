// Decompiled with JetBrains decompiler
// Type: GrenadeSettings
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class GrenadeSettings
{
  public string apiName;
  public int inventoryID;
  public float throwAnimationDuration;
  public float timeUnitilDetonation;
  public Vector3 startPointOffset;
  public Vector3 startRotation;
  public Vector3 angularVelocity;
  public float throwForce;
  public GameObject grenadeInstance;

  public Vector3 GetStartPos(GameObject ply)
  {
    return ply.GetComponent<Scp049PlayerScript>().plyCam.transform.TransformPoint(this.startPointOffset);
  }
}
