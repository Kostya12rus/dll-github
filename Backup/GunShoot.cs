// Decompiled with JetBrains decompiler
// Type: GunShoot
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour
{
  public float fireRate;
  public float weaponRange;
  public Transform gunEnd;
  public ParticleSystem muzzleFlash;
  public ParticleSystem cartridgeEjection;
  public GameObject metalHitEffect;
  public GameObject sandHitEffect;
  public GameObject stoneHitEffect;
  public GameObject waterLeakEffect;
  public GameObject waterLeakExtinguishEffect;
  public GameObject[] fleshHitEffects;
  public GameObject woodHitEffect;
  private float nextFire;
  private Animator anim;
  private GunAim gunAim;

  public GunShoot()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.anim = (Animator) ((Component) this).GetComponent<Animator>();
    this.gunAim = (GunAim) ((Component) this).GetComponentInParent<GunAim>();
  }

  private void Update()
  {
    if (!Input.GetKeyDown(NewInput.GetKey("Fire1")) || (double) Time.get_time() <= (double) this.nextFire || this.gunAim.GetIsOutOfBounds())
      return;
    this.nextFire = Time.get_time() + this.fireRate;
    this.muzzleFlash.Play();
    this.cartridgeEjection.Play();
    this.anim.SetTrigger("Fire");
    RaycastHit hit;
    if (!Physics.Raycast(this.gunEnd.get_position(), this.gunEnd.get_forward(), ref hit, this.weaponRange))
      return;
    this.HandleHit(hit);
  }

  private void HandleHit(RaycastHit hit)
  {
    if (!Object.op_Inequality((Object) ((RaycastHit) ref hit).get_collider().get_sharedMaterial(), (Object) null))
      return;
    string name = ((Object) ((RaycastHit) ref hit).get_collider().get_sharedMaterial()).get_name();
    if (name == null)
      return;
    // ISSUE: reference to a compiler-generated field
    if (GunShoot.\u003C\u003Ef__switch\u0024map6 == null)
    {
      // ISSUE: reference to a compiler-generated field
      GunShoot.\u003C\u003Ef__switch\u0024map6 = new Dictionary<string, int>(8)
      {
        {
          "Metal",
          0
        },
        {
          "Sand",
          1
        },
        {
          "Stone",
          2
        },
        {
          "WaterFilled",
          3
        },
        {
          "Wood",
          4
        },
        {
          "Meat",
          5
        },
        {
          "Character",
          6
        },
        {
          "WaterFilledExtinguish",
          7
        }
      };
    }
    int num;
    // ISSUE: reference to a compiler-generated field
    if (!GunShoot.\u003C\u003Ef__switch\u0024map6.TryGetValue(name, out num))
      return;
    switch (num)
    {
      case 0:
        this.SpawnDecal(hit, this.metalHitEffect);
        break;
      case 1:
        this.SpawnDecal(hit, this.sandHitEffect);
        break;
      case 2:
        this.SpawnDecal(hit, this.stoneHitEffect);
        break;
      case 3:
        this.SpawnDecal(hit, this.waterLeakEffect);
        this.SpawnDecal(hit, this.metalHitEffect);
        break;
      case 4:
        this.SpawnDecal(hit, this.woodHitEffect);
        break;
      case 5:
        this.SpawnDecal(hit, this.fleshHitEffects[Random.Range(0, this.fleshHitEffects.Length)]);
        break;
      case 6:
        this.SpawnDecal(hit, this.fleshHitEffects[Random.Range(0, this.fleshHitEffects.Length)]);
        break;
      case 7:
        this.SpawnDecal(hit, this.waterLeakExtinguishEffect);
        this.SpawnDecal(hit, this.metalHitEffect);
        break;
    }
  }

  private void SpawnDecal(RaycastHit hit, GameObject prefab)
  {
    ((GameObject) Object.Instantiate<GameObject>((M0) prefab, ((RaycastHit) ref hit).get_point(), Quaternion.LookRotation(((RaycastHit) ref hit).get_normal()))).get_transform().SetParent(((Component) ((RaycastHit) ref hit).get_collider()).get_transform());
  }
}
