// Decompiled with JetBrains decompiler
// Type: FragGrenade
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class FragGrenade : Grenade
{
  public float triggerOtherNadesDistance = 12f;
  public GameObject explosionEffects;
  public AnimationCurve shakeOverDistance;
  public AnimationCurve damageOverDistance;
  public LayerMask layerMask;
  public LayerMask triggerMask;
  private static int thrownFrags;

  public override void ClientsideExplosion(int pId)
  {
    Object.Destroy((Object) Object.Instantiate<GameObject>(this.explosionEffects, this.transform.position, this.explosionEffects.transform.rotation), 10f);
    GrenadeManager.grenadesOnScene.Remove((Grenade) this);
    ExplosionCameraShake.singleton.Shake(this.shakeOverDistance.Evaluate(Vector3.Distance(this.transform.position, PlayerManager.localPlayer.transform.position)));
    Object.Destroy((Object) this.gameObject);
  }

  public override void ServersideExplosion(GameObject thrower)
  {
    Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.triggerOtherNadesDistance, (int) this.triggerMask);
    int num1 = 0;
    if ((Object) thrower != (Object) null)
      num1 = thrower.GetComponent<QueryProcessor>().PlayerId;
    if (NetworkServer.active)
    {
      foreach (Collider collider in colliderArray)
      {
        Pickup componentInChildren = collider.GetComponentInChildren<Pickup>();
        if ((Object) componentInChildren != (Object) null && componentInChildren.info.itemId == 25)
        {
          ++FragGrenade.thrownFrags;
          PlayerManager.localPlayer.GetComponent<GrenadeManager>().ChangeIntoGrenade(componentInChildren, 0, num1, FragGrenade.thrownFrags, ((componentInChildren.transform.position - this.transform.position).normalized + Vector3.up / 3f).normalized * 16f, componentInChildren.transform.position);
        }
        BreakableWindow component1 = collider.GetComponent<BreakableWindow>();
        if ((Object) component1 != (Object) null)
        {
          if ((double) Vector3.Distance(component1.transform.position, this.transform.position) < (double) this.triggerOtherNadesDistance / 2.0)
            component1.ServerDamageWindow(500f);
        }
        else
        {
          Door d = collider.GetComponentInParent<Door>();
          if (!((Object) d == (Object) null) && (double) Vector3.Distance(d.transform.position, this.transform.position) < (double) this.triggerOtherNadesDistance / 2.0 && !d.locked)
          {
            if (string.IsNullOrEmpty(d.permissionLevel))
              d.DestroyDoor(true);
            else if ((Object) thrower != (Object) null)
            {
              Inventory component2 = thrower.GetComponent<Inventory>();
              foreach (Inventory.SyncItemInfo syncItemInfo in (SyncList<Inventory.SyncItemInfo>) component2.items)
              {
                if (((IEnumerable<string>) component2.availableItems[syncItemInfo.id].permissions).Any<string>((Func<string, bool>) (perm => perm == d.permissionLevel)))
                {
                  if ((Object) d.destroyedPrefab != (Object) null)
                  {
                    foreach (Collider componentsInChild in d.GetComponentsInChildren<Collider>())
                      componentsInChild.enabled = false;
                  }
                  d.DestroyDoor(true);
                }
              }
            }
          }
        }
      }
    }
    bool flag = ConfigFile.ServerConfig.GetBool("friendly_fire", false);
    foreach (GameObject player in PlayerManager.singleton.players)
    {
      PlayerStats component = player.GetComponent<PlayerStats>();
      if (!((Object) component == (Object) null) && component.ccm.curClass != 2)
      {
        float num2 = this.damageOverDistance.Evaluate(Vector3.Distance(this.transform.position, component.transform.position));
        float amnt = !component.ccm.IsHuman() ? num2 * ConfigFile.ServerConfig.GetFloat("scp_grenade_multiplier", 1f) : num2 * ConfigFile.ServerConfig.GetFloat("human_grenade_multiplier", 0.7f);
        if ((double) amnt > 5.0 && (flag || !((Object) player != (Object) thrower) || !((Object) thrower == (Object) null) && thrower.GetComponent<WeaponManager>().GetShootPermission(component.ccm, false)))
        {
          foreach (Transform grenadePoint in component.grenadePoints)
          {
            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(this.transform.position, (grenadePoint.position - this.transform.position).normalized), out hitInfo, 100f, (int) this.layerMask) && (Object) hitInfo.collider.GetComponentInParent<PlayerStats>() == (Object) component)
            {
              component.HurtPlayer(new PlayerStats.HitInfo(amnt, !((Object) thrower != (Object) null) ? "(UNKNOWN)" : thrower.GetComponent<CharacterClassManager>().SteamId + " (" + thrower.GetComponent<NicknameSync>().myNick + ")", "GRENADE", num1), player);
              break;
            }
          }
        }
      }
    }
  }
}
