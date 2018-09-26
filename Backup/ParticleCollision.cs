// Decompiled with JetBrains decompiler
// Type: ParticleCollision
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
  private List<ParticleCollisionEvent> m_CollisionEvents;
  private ParticleSystem m_ParticleSystem;

  public ParticleCollision()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.m_ParticleSystem = (ParticleSystem) ((Component) this).GetComponent<ParticleSystem>();
  }

  private void OnParticleCollision(GameObject other)
  {
    int collisionEvents = ParticlePhysicsExtensions.GetCollisionEvents(this.m_ParticleSystem, other, this.m_CollisionEvents);
    for (int index = 0; index < collisionEvents; ++index)
    {
      ParticleCollisionEvent collisionEvent = this.m_CollisionEvents[index];
      ExtinguishableFire component = (ExtinguishableFire) ((ParticleCollisionEvent) ref collisionEvent).get_colliderComponent().GetComponent<ExtinguishableFire>();
      if (Object.op_Inequality((Object) component, (Object) null))
        component.Extinguish();
    }
  }
}
