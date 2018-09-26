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
    Object.Destroy((Object) Object.Instantiate<GameObject>((M0) this.explosionEffects, ((Component) this).get_transform().get_position(), this.explosionEffects.get_transform().get_rotation()), 10f);
    GrenadeManager.grenadesOnScene.Remove((Grenade) this);
    ExplosionCameraShake.singleton.Shake(this.shakeOverDistance.Evaluate(Vector3.Distance(((Component) this).get_transform().get_position(), PlayerManager.localPlayer.get_transform().get_position())));
    Object.Destroy((Object) ((Component) this).get_gameObject());
  }

  public override void ServersideExplosion(GameObject thrower)
  {
    Collider[] colliderArray = Physics.OverlapSphere(((Component) this).get_transform().get_position(), this.triggerOtherNadesDistance, LayerMask.op_Implicit(this.triggerMask));
    int attackerID = 0;
    if (Object.op_Inequality((Object) thrower, (Object) null))
      attackerID = ((QueryProcessor) thrower.GetComponent<QueryProcessor>()).PlayerId;
    if (NetworkServer.get_active())
    {
      foreach (Collider collider in colliderArray)
      {
        Pickup componentInChildren = (Pickup) ((Component) collider).GetComponentInChildren<Pickup>();
        if (Object.op_Inequality((Object) componentInChildren, (Object) null) && componentInChildren.info.itemId == 25)
        {
          ++FragGrenade.thrownFrags;
          M0 component = PlayerManager.localPlayer.GetComponent<GrenadeManager>();
          Pickup pickup = componentInChildren;
          int id = 0;
          int ti_pid = attackerID;
          int thrownFrags = FragGrenade.thrownFrags;
          Vector3 vector3_1 = Vector3.op_Subtraction(((Component) componentInChildren).get_transform().get_position(), ((Component) this).get_transform().get_position());
          Vector3 vector3_2 = Vector3.op_Addition(((Vector3) ref vector3_1).get_normalized(), Vector3.op_Division(Vector3.get_up(), 3f));
          Vector3 dir = Vector3.op_Multiply(((Vector3) ref vector3_2).get_normalized(), 16f);
          Vector3 position = ((Component) componentInChildren).get_transform().get_position();
          ((GrenadeManager) component).ChangeIntoGrenade(pickup, id, ti_pid, thrownFrags, dir, position);
        }
        BreakableWindow component1 = (BreakableWindow) ((Component) collider).GetComponent<BreakableWindow>();
        if (Object.op_Inequality((Object) component1, (Object) null))
        {
          if ((double) Vector3.Distance(((Component) component1).get_transform().get_position(), ((Component) this).get_transform().get_position()) < (double) this.triggerOtherNadesDistance / 2.0)
            component1.ServerDamageWindow(500f);
        }
        else
        {
          Door d = (Door) ((Component) collider).GetComponentInParent<Door>();
          if (!Object.op_Equality((Object) d, (Object) null) && (double) Vector3.Distance(((Component) d).get_transform().get_position(), ((Component) this).get_transform().get_position()) < (double) this.triggerOtherNadesDistance / 2.0 && !d.locked)
          {
            if (string.IsNullOrEmpty(d.permissionLevel))
              d.DestroyDoor(true);
            else if (Object.op_Inequality((Object) thrower, (Object) null))
            {
              Inventory component2 = (Inventory) thrower.GetComponent<Inventory>();
              using (IEnumerator<Inventory.SyncItemInfo> enumerator = ((SyncList<Inventory.SyncItemInfo>) component2.items).GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  Inventory.SyncItemInfo current = enumerator.Current;
                  if (((IEnumerable<string>) component2.availableItems[current.id].permissions).Any<string>((Func<string, bool>) (perm => perm == d.permissionLevel)))
                  {
                    if (Object.op_Inequality((Object) d.destroyedPrefab, (Object) null))
                    {
                      foreach (Collider componentsInChild in (Collider[]) ((Component) d).GetComponentsInChildren<Collider>())
                        componentsInChild.set_enabled(false);
                    }
                    d.DestroyDoor(true);
                  }
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
      PlayerStats component = (PlayerStats) player.GetComponent<PlayerStats>();
      if (!Object.op_Equality((Object) component, (Object) null) && component.ccm.curClass != 2)
      {
        float num = this.damageOverDistance.Evaluate(Vector3.Distance(((Component) this).get_transform().get_position(), ((Component) component).get_transform().get_position()));
        float amnt = !component.ccm.IsHuman() ? num * ConfigFile.ServerConfig.GetFloat("scp_grenade_multiplier", 1f) : num * ConfigFile.ServerConfig.GetFloat("human_grenade_multiplier", 0.7f);
        if ((double) amnt > 5.0 && (flag || !Object.op_Inequality((Object) player, (Object) thrower) || !Object.op_Equality((Object) thrower, (Object) null) && ((WeaponManager) thrower.GetComponent<WeaponManager>()).GetShootPermission(component.ccm, false)))
        {
          foreach (Transform grenadePoint in component.grenadePoints)
          {
            Vector3 position = ((Component) this).get_transform().get_position();
            Vector3 vector3 = Vector3.op_Subtraction(grenadePoint.get_position(), ((Component) this).get_transform().get_position());
            Vector3 normalized = ((Vector3) ref vector3).get_normalized();
            RaycastHit raycastHit;
            if (Physics.Raycast(new Ray(position, normalized), ref raycastHit, 100f, LayerMask.op_Implicit(this.layerMask)) && Object.op_Equality((Object) ((Component) ((RaycastHit) ref raycastHit).get_collider()).GetComponentInParent<PlayerStats>(), (Object) component))
            {
              component.HurtPlayer(new PlayerStats.HitInfo(amnt, !Object.op_Inequality((Object) thrower, (Object) null) ? "(UNKNOWN)" : ((CharacterClassManager) thrower.GetComponent<CharacterClassManager>()).SteamId + " (" + ((NicknameSync) thrower.GetComponent<NicknameSync>()).myNick + ")", "GRENADE", attackerID), player);
              break;
            }
          }
        }
      }
    }
  }
}
