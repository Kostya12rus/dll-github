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
    private static AntiFakeCommands host;
    private Scp173PlayerScript scp173;
    private Scp096PlayerScript scp096;
    private PlyMovementSync pms;
    private CharacterClassManager ccm;
    private float distanceTraveled;
    private Vector3 prevPos;
    private float maxDistance;
    [Header("Noclip Protection")]
    private bool noclip_protection;
    public LayerMask mask;

    public AntiFakeCommands()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      this.noclip_protection = ConfigFile.ServerConfig.GetBool("noclip_protection", this.noclip_protection);
      this.scp173 = (Scp173PlayerScript) ((Component) this).GetComponent<Scp173PlayerScript>();
      this.scp096 = (Scp096PlayerScript) ((Component) this).GetComponent<Scp096PlayerScript>();
      if (TutorialManager.status)
        return;
      if (this.get_isLocalPlayer() && this.get_isServer())
      {
        AntiFakeCommands.allowedTeleportPositions.Clear();
        this.AddTypeToList("Spawnpoint");
        AntiFakeCommands.host = this;
      }
      this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
      this.pms = (PlyMovementSync) ((Component) this).GetComponent<PlyMovementSync>();
      Timing.RunCoroutine(this._AntiSpeedhack(), (Segment) 0);
    }

    public bool CheckMovement(Vector3 pos)
    {
      if (TutorialManager.status || this.get_isLocalPlayer() && this.get_isServer() || (this.ccm.curClass == -1 || this.ccm.curClass == 2))
      {
        this.prevPos = pos;
        return true;
      }
      this.distanceTraveled += Vector2.Distance(new Vector2((float) this.prevPos.x, (float) this.prevPos.z), new Vector2((float) pos.x, (float) pos.z));
      if (this.ccm.curClass == 0)
        this.maxDistance = !this.scp173.CanMove() ? 3f : this.scp173.boost_teleportDistance.Evaluate(((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).GetHealthPercent()) * 2f;
      else if (this.ccm.curClass > 0)
        this.maxDistance = this.ccm.klasy[this.ccm.curClass].runSpeed;
      if (this.ccm.curClass == 9 && this.scp096.enraged == Scp096PlayerScript.RageState.Enraged)
        this.maxDistance *= 4.9f;
      if ((double) this.distanceTraveled >= (double) this.maxDistance * 1.29999995231628)
        return false;
      RaycastHit raycastHit;
      if (this.noclip_protection && Physics.Linecast(this.prevPos, pos, ref raycastHit, LayerMask.op_Implicit(this.mask)))
      {
        bool flag = true;
        Door componentInParent = (Door) ((Component) ((RaycastHit) ref raycastHit).get_collider()).GetComponentInParent<Door>();
        if (Object.op_Inequality((Object) componentInParent, (Object) null))
        {
          if (this.ccm.curClass == 3)
            flag = false;
          else if ((double) componentInParent.curCooldown > 0.699999988079071)
            flag = false;
          else if (this.ccm.curClass == 9 && Object.op_Inequality((Object) componentInParent.destroyedPrefab, (Object) null) && ((Scp096PlayerScript) ((Component) this).GetComponent<Scp096PlayerScript>()).enraged == Scp096PlayerScript.RageState.Enraged)
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
      if ((double) Vector3.Distance(pos, this.ccm.deathPosition) < 10.0 || pos.y > 2000.0 || pos.y < -1500.0)
        return true;
      using (List<Transform>.Enumerator enumerator = AntiFakeCommands.allowedTeleportPositions.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Transform current = enumerator.Current;
          if ((double) Vector3.Distance(pos, current.get_position()) < 10.0)
            return (!(((Component) current).get_tag() == "SP_CDP") || curClass == 1) && (!(((Component) current).get_tag() == "SP_173") || curClass == 0) && ((!(((Component) current).get_tag() == "SP_106") || curClass == 3) && (!(((Component) current).get_tag() == "SP_049") || curClass == 5)) && ((!(((Component) current).get_tag() == "SP_MTF") || this.ccm.klasy[curClass].team == Team.MTF) && (!(((Component) current).get_tag() == "SP_RSC") || curClass == 6) && (!(((Component) current).get_tag() == "SP_CI") || curClass == 8));
        }
      }
      return curClass == 3 && (double) Vector3.Distance(pos, GameObject.Find("SCP106_PORTAL").get_transform().get_position()) < 10.0;
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
        AntiFakeCommands.allowedTeleportPositions.Add(gameObject.get_transform());
    }

    public void SetPosition(Vector3 pos)
    {
      this.prevPos = pos;
      this.distanceTraveled = 0.0f;
    }

    private void UNetVersion()
    {
    }

    public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
    {
      bool flag;
      return flag;
    }

    public virtual void OnDeserialize(NetworkReader reader, bool initialState)
    {
    }
  }
}
