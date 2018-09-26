// Decompiled with JetBrains decompiler
// Type: FlashGrenade
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using RemoteAdmin;
using UnityEngine;

public class FlashGrenade : Grenade
{
  public GameObject explosionEffects;
  public AnimationCurve shakeOverDistance;
  public AnimationCurve powerOverDistance;
  public AnimationCurve powerOverDot;
  public LayerMask viewLayerMask;
  public float distanceMultiplierSurface;
  public float distanceMultiplierFacility;

  public override void ServersideExplosion(GameObject thrower)
  {
    ServerLogs.AddLog(ServerLogs.Modules.Logger, "Player " + (!((Object) thrower != (Object) null) ? "(UNKNOWN)" : thrower.GetComponent<CharacterClassManager>().SteamId + " (" + thrower.GetComponent<NicknameSync>().myNick + ")") + " thew flash grenade.", ServerLogs.ServerLogType.GameEvent);
  }

  public override void ClientsideExplosion(int grenadeOwnerPlayerID)
  {
    Object.Destroy((Object) Object.Instantiate<GameObject>(this.explosionEffects, this.transform.position, this.explosionEffects.transform.rotation), 10f);
    GrenadeManager.grenadesOnScene.Remove((Grenade) this);
    ExplosionCameraShake.singleton.Shake(this.shakeOverDistance.Evaluate(Vector3.Distance(this.transform.position, PlayerManager.localPlayer.transform.position)));
    Transform transform = PlayerManager.localPlayer.GetComponent<Scp049PlayerScript>().plyCam.transform;
    if (!GrenadeManager.flashfire)
    {
      GameObject gameObject = (GameObject) null;
      foreach (GameObject player in PlayerManager.singleton.players)
      {
        if (player.GetComponent<QueryProcessor>().PlayerId == grenadeOwnerPlayerID)
          gameObject = player;
      }
      if ((Object) gameObject != (Object) PlayerManager.localPlayer && ((Object) gameObject == (Object) null || !gameObject.GetComponent<WeaponManager>().GetShootPermission(PlayerManager.localPlayer.GetComponent<CharacterClassManager>(), false)))
      {
        Object.Destroy((Object) this.gameObject);
        return;
      }
    }
    RaycastHit hitInfo;
    if (Physics.Raycast(transform.position, -(transform.position - this.transform.position).normalized, out hitInfo, 1000f, (int) this.viewLayerMask) && hitInfo.collider.gameObject.layer == 20)
      PlayerManager.localPlayer.GetComponent<FlashEffect>().Play(this.powerOverDistance.Evaluate(Vector3.Distance(PlayerManager.localPlayer.transform.position, this.transform.position) / ((double) this.transform.position.y <= 900.0 ? this.distanceMultiplierFacility : this.distanceMultiplierSurface)) * this.powerOverDot.Evaluate(Vector3.Dot(transform.forward, (transform.position - this.transform.position).normalized)));
    Object.Destroy((Object) this.gameObject);
  }
}
