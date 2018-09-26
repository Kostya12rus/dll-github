// Decompiled with JetBrains decompiler
// Type: Grenade
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using RemoteAdmin;
using UnityEngine;
using UnityEngine.Networking;

public class Grenade : MonoBehaviour
{
  public AudioClip[] collisionSounds;
  public float collisionSpeedToSound;
  public string id;

  public Grenade()
  {
    base.\u002Ector();
  }

  public void Explode(int playerID)
  {
    if (NetworkServer.get_active())
    {
      GameObject thrower = (GameObject) null;
      foreach (GameObject player in PlayerManager.singleton.players)
      {
        if (((QueryProcessor) player.GetComponent<QueryProcessor>()).PlayerId == playerID)
          thrower = player;
      }
      this.ServersideExplosion(thrower);
    }
    this.ClientsideExplosion(playerID);
  }

  public virtual void ServersideExplosion(GameObject thrower)
  {
  }

  public virtual void ClientsideExplosion(int grenadeOwnerPlayerID)
  {
  }

  private void OnCollisionEnter(Collision collision)
  {
    Vector3 relativeVelocity = collision.get_relativeVelocity();
    if ((double) ((Vector3) ref relativeVelocity).get_magnitude() <= (double) this.collisionSpeedToSound)
      return;
    ((AudioSource) ((Component) this).GetComponent<AudioSource>()).PlayOneShot(this.collisionSounds[Random.Range(0, this.collisionSounds.Length)]);
  }

  public void SyncMovement(Vector3 pos, Vector3 vel, Quaternion rot, Vector3 angularSpeed)
  {
    if ((double) Vector3.Distance(pos, ((Component) this).get_transform().get_position()) <= 1.0)
      return;
    ((Rigidbody) ((Component) this).GetComponent<Rigidbody>()).set_velocity(vel);
    ((Rigidbody) ((Component) this).GetComponent<Rigidbody>()).set_angularVelocity(angularSpeed);
    ((Component) this).get_transform().set_position(pos);
    ((Component) this).get_transform().set_rotation(rot);
  }
}
