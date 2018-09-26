// Decompiled with JetBrains decompiler
// Type: ParticleCollision
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
  private List<ParticleCollisionEvent> m_CollisionEvents = new List<ParticleCollisionEvent>();
  private ParticleSystem m_ParticleSystem;

  private void Start()
  {
    this.m_ParticleSystem = this.GetComponent<ParticleSystem>();
  }

  private void OnParticleCollision(GameObject other)
  {
    int collisionEvents = this.m_ParticleSystem.GetCollisionEvents(other, this.m_CollisionEvents);
    for (int index = 0; index < collisionEvents; ++index)
    {
      ExtinguishableFire component = this.m_CollisionEvents[index].colliderComponent.GetComponent<ExtinguishableFire>();
      if ((Object) component != (Object) null)
        component.Extinguish();
    }
  }
}
