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
    ServerLogs.AddLog(ServerLogs.Modules.Logger, "Player " + (!Object.op_Inequality((Object) thrower, (Object) null) ? "(UNKNOWN)" : ((CharacterClassManager) thrower.GetComponent<CharacterClassManager>()).SteamId + " (" + ((NicknameSync) thrower.GetComponent<NicknameSync>()).myNick + ")") + " thew flash grenade.", ServerLogs.ServerLogType.GameEvent);
  }

  public override void ClientsideExplosion(int grenadeOwnerPlayerID)
  {
    Object.Destroy((Object) Object.Instantiate<GameObject>((M0) this.explosionEffects, ((Component) this).get_transform().get_position(), this.explosionEffects.get_transform().get_rotation()), 10f);
    GrenadeManager.grenadesOnScene.Remove((Grenade) this);
    ExplosionCameraShake.singleton.Shake(this.shakeOverDistance.Evaluate(Vector3.Distance(((Component) this).get_transform().get_position(), PlayerManager.localPlayer.get_transform().get_position())));
    Transform transform = ((Scp049PlayerScript) PlayerManager.localPlayer.GetComponent<Scp049PlayerScript>()).plyCam.get_transform();
    if (!GrenadeManager.flashfire)
    {
      GameObject gameObject = (GameObject) null;
      foreach (GameObject player in PlayerManager.singleton.players)
      {
        if (((QueryProcessor) player.GetComponent<QueryProcessor>()).PlayerId == grenadeOwnerPlayerID)
          gameObject = player;
      }
      if (Object.op_Inequality((Object) gameObject, (Object) PlayerManager.localPlayer) && (Object.op_Equality((Object) gameObject, (Object) null) || !((WeaponManager) gameObject.GetComponent<WeaponManager>()).GetShootPermission((CharacterClassManager) PlayerManager.localPlayer.GetComponent<CharacterClassManager>(), false)))
      {
        Object.Destroy((Object) ((Component) this).get_gameObject());
        return;
      }
    }
    Vector3 position = transform.get_position();
    Vector3 vector3_1 = Vector3.op_Subtraction(transform.get_position(), ((Component) this).get_transform().get_position());
    Vector3 vector3_2 = Vector3.op_UnaryNegation(((Vector3) ref vector3_1).get_normalized());
    RaycastHit raycastHit;
    ref RaycastHit local = ref raycastHit;
    double num1 = 1000.0;
    int num2 = LayerMask.op_Implicit(this.viewLayerMask);
    if (Physics.Raycast(position, vector3_2, ref local, (float) num1, num2) && ((Component) ((RaycastHit) ref raycastHit).get_collider()).get_gameObject().get_layer() == 20)
    {
      M0 component = PlayerManager.localPlayer.GetComponent<FlashEffect>();
      double num3 = (double) this.powerOverDistance.Evaluate(Vector3.Distance(PlayerManager.localPlayer.get_transform().get_position(), ((Component) this).get_transform().get_position()) / (((Component) this).get_transform().get_position().y <= 900.0 ? this.distanceMultiplierFacility : this.distanceMultiplierSurface));
      AnimationCurve powerOverDot = this.powerOverDot;
      Vector3 forward = transform.get_forward();
      Vector3 vector3_3 = Vector3.op_Subtraction(transform.get_position(), ((Component) this).get_transform().get_position());
      Vector3 normalized = ((Vector3) ref vector3_3).get_normalized();
      double num4 = (double) Vector3.Dot(forward, normalized);
      double num5 = (double) powerOverDot.Evaluate((float) num4);
      double num6 = num3 * num5;
      ((FlashEffect) component).Play((float) num6);
    }
    Object.Destroy((Object) ((Component) this).get_gameObject());
  }
}
