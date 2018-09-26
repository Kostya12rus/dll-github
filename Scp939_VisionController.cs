// Decompiled with JetBrains decompiler
// Type: Scp939_VisionController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Scp939_VisionController : NetworkBehaviour
{
  public float minimumSilenceTime = 2.5f;
  public float minimumNoiseLevel = 2f;
  public List<Scp939_VisionController.Scp939_Vision> seeingSCPs = new List<Scp939_VisionController.Scp939_Vision>();
  public float noise;
  private CharacterClassManager ccm;

  private void Start()
  {
    this.ccm = this.GetComponent<CharacterClassManager>();
  }

  public bool CanSee(Scp939PlayerScript scp939)
  {
    if (!scp939.iAm939)
      return false;
    foreach (Scp939_VisionController.Scp939_Vision seeingScP in this.seeingSCPs)
    {
      if ((Object) seeingScP.scp == (Object) scp939)
        return true;
    }
    return false;
  }

  private void FixedUpdate()
  {
    if (!NetworkServer.active)
      return;
    foreach (Scp939PlayerScript instance in Scp939PlayerScript.instances)
    {
      if ((double) Vector3.Distance(this.transform.position, instance.transform.position) < (double) this.noise)
        this.AddVision(instance);
    }
    this.noise = this.minimumNoiseLevel;
    this.UpdateVisions();
  }

  private void AddVision(Scp939PlayerScript scp939)
  {
    for (int index = 0; index < this.seeingSCPs.Count; ++index)
    {
      if ((Object) this.seeingSCPs[index].scp == (Object) scp939)
      {
        this.seeingSCPs[index].remainingTime = this.minimumSilenceTime;
        return;
      }
    }
    this.seeingSCPs.Add(new Scp939_VisionController.Scp939_Vision()
    {
      scp = scp939,
      remainingTime = this.minimumSilenceTime
    });
  }

  private void UpdateVisions()
  {
    for (int index = 0; index < this.seeingSCPs.Count; ++index)
    {
      this.seeingSCPs[index].remainingTime -= 0.02f;
      if ((Object) this.seeingSCPs[index].scp == (Object) null || !this.seeingSCPs[index].scp.iAm939 || (double) this.seeingSCPs[index].remainingTime <= 0.0)
      {
        this.seeingSCPs.RemoveAt(index);
        break;
      }
    }
  }

  [Server]
  public void MakeNoise(float distanceIntensity)
  {
    if (!NetworkServer.active)
    {
      Debug.LogWarning((object) "[Server] function 'System.Void Scp939_VisionController::MakeNoise(System.Single)' called on client");
    }
    else
    {
      if (!this.ccm.IsHuman() || (double) this.noise >= (double) distanceIntensity)
        return;
      this.noise = distanceIntensity;
    }
  }

  private void UNetVersion()
  {
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }

  [Serializable]
  public class Scp939_Vision
  {
    public Scp939PlayerScript scp;
    public float remainingTime;
  }
}
