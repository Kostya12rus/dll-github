// Decompiled with JetBrains decompiler
// Type: AntiFaker.AntiFakeCommands
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

namespace AntiFaker
{
  public class AntiFakeCommands : NetworkBehaviour
  {
    private static List<Transform> allowedTeleportPositions = new List<Transform>();
    private Vector3 prevPos = Vector3.zero;
    private static AntiFakeCommands host;
    private Scp173PlayerScript scp173;
    private Scp096PlayerScript scp096;
    private PlyMovementSync pms;
    private CharacterClassManager ccm;
    private float distanceTraveled;
    private float maxDistance;
    [Header("Noclip Protection")]
    private bool noclip_protection;
    public LayerMask mask;

    private void Start()
    {
      this.noclip_protection = ConfigFile.ServerConfig.GetBool("noclip_protection", this.noclip_protection);
      this.scp173 = this.GetComponent<Scp173PlayerScript>();
      this.scp096 = this.GetComponent<Scp096PlayerScript>();
      if (TutorialManager.status)
        return;
      if (this.isLocalPlayer && this.isServer)
      {
        AntiFakeCommands.allowedTeleportPositions.Clear();
        this.AddTypeToList("Spawnpoint");
        AntiFakeCommands.host = this;
      }
      this.ccm = this.GetComponent<CharacterClassManager>();
      this.pms = this.GetComponent<PlyMovementSync>();
      Timing.RunCoroutine(this._AntiSpeedhack(), Segment.Update);
    }

    public bool CheckMovement(Vector3 pos)
    {
      if (TutorialManager.status || this.isLocalPlayer && this.isServer || (this.ccm.curClass == -1 || this.ccm.curClass == 2))
      {
        this.prevPos = pos;
        return true;
      }
      this.distanceTraveled += Vector2.Distance(new Vector2(this.prevPos.x, this.prevPos.z), new Vector2(pos.x, pos.z));
      if (this.ccm.curClass == 0)
        this.maxDistance = !this.scp173.CanMove() ? 3f : this.scp173.boost_teleportDistance.Evaluate(this.GetComponent<PlayerStats>().GetHealthPercent()) * 2f;
      else if (this.ccm.curClass > 0)
        this.maxDistance = this.ccm.klasy[this.ccm.curClass].runSpeed;
      if (this.ccm.curClass == 9 && this.scp096.enraged == Scp096PlayerScript.RageState.Enraged)
        this.maxDistance *= 4.9f;
      if ((double) this.distanceTraveled >= (double) this.maxDistance * 1.29999995231628)
        return false;
      RaycastHit hitInfo;
      if (this.noclip_protection && Physics.Linecast(this.prevPos, pos, out hitInfo, (int) this.mask))
      {
        bool flag = true;
        Door componentInParent = hitInfo.collider.GetComponentInParent<Door>();
        if ((Object) componentInParent != (Object) null)
        {
          if (this.ccm.curClass == 3)
            flag = false;
          else if ((double) componentInParent.curCooldown > 0.699999988079071)
            flag = false;
          else if (this.ccm.curClass == 9 && (Object) componentInParent.destroyedPrefab != (Object) null && this.GetComponent<Scp096PlayerScript>().enraged == Scp096PlayerScript.RageState.Enraged)
            flag = false;
        }
        if (flag)
          return false;
      }
      this.prevPos = pos;
      return true;
    }

    [DebuggerHidden]
    private IEnumerator<float> _AntiSpeedhack()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator<float>) new AntiFakeCommands.\u003C_AntiSpeedhack\u003Ec__Iterator0() { \u0024this = this };
    }

    public bool SpeedhackJustification(Vector3 pos)
    {
      int curClass = this.ccm.curClass;
      if ((double) Vector3.Distance(pos, this.ccm.deathPosition) < 10.0 || (double) pos.y > 2000.0 || (double) pos.y < -1500.0)
        return true;
      foreach (Transform teleportPosition in AntiFakeCommands.allowedTeleportPositions)
      {
        if ((double) Vector3.Distance(pos, teleportPosition.position) < 10.0)
          return (!(teleportPosition.tag == "SP_CDP") || curClass == 1) && (!(teleportPosition.tag == "SP_173") || curClass == 0) && ((!(teleportPosition.tag == "SP_106") || curClass == 3) && (!(teleportPosition.tag == "SP_049") || curClass == 5)) && ((!(teleportPosition.tag == "SP_MTF") || this.ccm.klasy[curClass].team == Team.MTF) && (!(teleportPosition.tag == "SP_RSC") || curClass == 6) && (!(teleportPosition.tag == "SP_CI") || curClass == 8));
      }
      return curClass == 3 && (double) Vector3.Distance(pos, GameObject.Find("SCP106_PORTAL").transform.position) < 10.0;
    }

    public void FindAllowedTeleportPositions()
    {
      if (TutorialManager.status)
        return;
      this.AddTypeToList("SP_CDP");
      this.AddTypeToList("SP_173");
      this.AddTypeToList("SP_106");
      this.AddTypeToList("SP_049");
      this.AddTypeToList("SP_MTF");
      this.AddTypeToList("SP_RSC");
      this.AddTypeToList("SP_079");
      this.AddTypeToList("SCP_096");
      this.AddTypeToList("PD_EXIT");
      this.AddTypeToList("SP_CI");
      this.AddTypeToList("LiftTarget");
    }

    private void AddTypeToList(string type)
    {
      foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag(type))
        AntiFakeCommands.allowedTeleportPositions.Add(gameObject.transform);
    }

    public void SetPosition(Vector3 pos)
    {
      this.prevPos = pos;
      this.distanceTraveled = 0.0f;
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
  }
}
