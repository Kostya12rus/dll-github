// Decompiled with JetBrains decompiler
// Type: Handcuffs
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Handcuffs : NetworkBehaviour
{
  private static int kCmdCmdTarget = 624996931;
  public TextMeshProUGUI distanceText;
  private Transform plyCam;
  private CharacterClassManager ccm;
  private Inventory inv;
  public LayerMask mask;
  public float maxDistance;
  private Image uncuffProgress;
  [SyncVar(hook = "SetTarget")]
  public GameObject cuffTarget;
  private float progress;
  private float lostCooldown;
  private NetworkInstanceId ___cuffTargetNetId;
  private static int kCmdCmdResetTarget;

  public Handcuffs()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.uncuffProgress = (Image) GameObject.Find("UncuffProgress").GetComponent<Image>();
    this.inv = (Inventory) ((Component) this).GetComponent<Inventory>();
    this.plyCam = ((Scp049PlayerScript) ((Component) this).GetComponent<Scp049PlayerScript>()).plyCam.get_transform();
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
  }

  private void Update()
  {
    if (this.get_isLocalPlayer())
    {
      this.CheckForInput();
      this.UpdateText();
    }
    if (!Object.op_Inequality((Object) this.cuffTarget, (Object) null))
      return;
    ((AnimationController) this.cuffTarget.GetComponent<AnimationController>()).cuffed = true;
  }

  private void CheckForInput()
  {
    if (Object.op_Inequality((Object) this.cuffTarget, (Object) null))
    {
      bool flag = false;
      using (IEnumerator<Inventory.SyncItemInfo> enumerator = ((SyncList<Inventory.SyncItemInfo>) this.inv.items).GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          if (enumerator.Current.id == 27)
            flag = true;
        }
      }
      if (!flag)
        this.CallCmdTarget((GameObject) null);
    }
    if ((double) Inventory.inventoryCooldown > 0.0)
      return;
    if (this.inv.curItem == 27)
    {
      if (Input.GetKeyDown(NewInput.GetKey("Shoot")) && Object.op_Equality((Object) this.cuffTarget, (Object) null))
        this.CuffPlayer();
      else if (Input.GetKeyDown(NewInput.GetKey("Zoom")) && Object.op_Inequality((Object) this.cuffTarget, (Object) null))
        this.CallCmdTarget((GameObject) null);
    }
    if (this.ccm.curClass >= 0 && this.ccm.klasy[this.ccm.curClass].team != Team.SCP && Input.GetKey(NewInput.GetKey("Interact")))
    {
      RaycastHit raycastHit;
      if (Physics.Raycast(this.plyCam.get_position(), this.plyCam.get_forward(), ref raycastHit, this.maxDistance, LayerMask.op_Implicit(((PlayerInteract) ((Component) this).GetComponent<PlayerInteract>()).mask)))
      {
        Handcuffs componentInParent = (Handcuffs) ((Component) ((RaycastHit) ref raycastHit).get_collider()).GetComponentInParent<Handcuffs>();
        if (Object.op_Inequality((Object) componentInParent, (Object) null) && Object.op_Inequality((Object) ((AnimationController) ((Component) componentInParent).GetComponent<AnimationController>()).handAnimator, (Object) null) && ((AnimationController) ((Component) componentInParent).GetComponent<AnimationController>()).handAnimator.GetBool("Cuffed"))
        {
          this.progress += Time.get_deltaTime();
          if ((double) this.progress >= 1.5)
          {
            this.progress = 0.0f;
            foreach (GameObject player in PlayerManager.singleton.players)
            {
              if (Object.op_Equality((Object) ((Handcuffs) player.GetComponent<Handcuffs>()).cuffTarget, (Object) ((Component) componentInParent).get_gameObject()))
                this.CallCmdResetTarget(player);
            }
          }
        }
        else
          this.progress = 0.0f;
      }
      else
        this.progress = 0.0f;
    }
    else
      this.progress = 0.0f;
    if (this.ccm.curClass == 3)
      return;
    this.uncuffProgress.set_fillAmount(Mathf.Clamp01(this.progress / 1.5f));
  }

  private void CuffPlayer()
  {
    Ray ray;
    ((Ray) ref ray).\u002Ector(this.plyCam.get_position(), this.plyCam.get_forward());
    RaycastHit raycastHit;
    if (!Physics.Raycast(ray, ref raycastHit, this.maxDistance, LayerMask.op_Implicit(this.mask)))
      return;
    CharacterClassManager componentInParent = (CharacterClassManager) ((Component) ((RaycastHit) ref raycastHit).get_collider()).GetComponentInParent<CharacterClassManager>();
    if (!Object.op_Inequality((Object) componentInParent, (Object) null))
      return;
    Class @class = this.ccm.klasy[componentInParent.curClass];
    if (@class.team == Team.SCP || ((@class.team == Team.CDP ? 1 : (@class.team == Team.CHI ? 1 : 0)) == (this.ccm.klasy[this.ccm.curClass].team == Team.CDP ? 1 : (this.ccm.klasy[this.ccm.curClass].team == Team.CHI ? 1 : 0)) || ((AnimationController) ((Component) componentInParent).GetComponent<AnimationController>()).curAnim != 0 || !Vector2.op_Equality(((AnimationController) ((Component) componentInParent).GetComponent<AnimationController>()).speed, Vector2.get_zero())))
      return;
    if (this.ccm.klasy[this.ccm.curClass].team == Team.CDP && @class.team == Team.MTF)
      AchievementManager.Achieve("tableshaveturned");
    this.CallCmdTarget(((Component) componentInParent).get_gameObject());
  }

  [Command(channel = 2)]
  public void CmdTarget(GameObject t)
  {
    if (!Object.op_Equality((Object) t, (Object) null) && ((double) Vector3.Distance(((Component) this).get_transform().get_position(), t.get_transform().get_position()) >= 3.0 || this.inv.curItem != 27))
      return;
    this.SetTarget(t);
    if (!Object.op_Inequality((Object) t, (Object) null))
      return;
    ((Inventory) t.GetComponent<Inventory>()).ServerDropAll();
  }

  [Command(channel = 2)]
  public void CmdResetTarget(GameObject t)
  {
    ((Handcuffs) t.GetComponent<Handcuffs>()).SetTarget((GameObject) null);
  }

  private void SetTarget(GameObject t)
  {
    this.NetworkcuffTarget = t;
  }

  private void UpdateText()
  {
    if (Object.op_Inequality((Object) this.cuffTarget, (Object) null))
    {
      float num = Vector3.Distance(((Component) this).get_transform().get_position(), this.cuffTarget.get_transform().get_position());
      if ((double) num > 200.0)
      {
        num = 200f;
        this.lostCooldown += Time.get_deltaTime();
        if ((double) this.lostCooldown > 1.0)
          this.CallCmdTarget((GameObject) null);
      }
      else
        this.lostCooldown = 0.0f;
      ((TMP_Text) this.distanceText).set_text((num * 1.5f).ToString("0 m"));
    }
    else
      ((TMP_Text) this.distanceText).set_text("NONE");
  }

  private void UNetVersion()
  {
  }

  public GameObject NetworkcuffTarget
  {
    get
    {
      return this.cuffTarget;
    }
    [param: In] set
    {
      GameObject gameObject = value;
      ref GameObject local1 = ref this.cuffTarget;
      int num = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetTarget(value);
        this.set_syncVarHookGuard(false);
      }
      ref NetworkInstanceId local2 = ref this.___cuffTargetNetId;
      this.SetSyncVarGameObject((GameObject) gameObject, (GameObject&) ref local1, (uint) num, ref local2);
    }
  }

  protected static void InvokeCmdCmdTarget(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdTarget called on client.");
    else
      ((Handcuffs) obj).CmdTarget((GameObject) reader.ReadGameObject());
  }

  protected static void InvokeCmdCmdResetTarget(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdResetTarget called on client.");
    else
      ((Handcuffs) obj).CmdResetTarget((GameObject) reader.ReadGameObject());
  }

  public void CallCmdTarget(GameObject t)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdTarget called on server.");
    else if (this.get_isServer())
    {
      this.CmdTarget(t);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Handcuffs.kCmdCmdTarget);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) t);
      this.SendCommandInternal(networkWriter, 2, "CmdTarget");
    }
  }

  public void CallCmdResetTarget(GameObject t)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdResetTarget called on server.");
    else if (this.get_isServer())
    {
      this.CmdResetTarget(t);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) Handcuffs.kCmdCmdResetTarget);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) t);
      this.SendCommandInternal(networkWriter, 2, "CmdResetTarget");
    }
  }

  static Handcuffs()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Handcuffs), Handcuffs.kCmdCmdTarget, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdTarget)));
    Handcuffs.kCmdCmdResetTarget = -1476369842;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (Handcuffs), Handcuffs.kCmdCmdResetTarget, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdResetTarget)));
    NetworkCRC.RegisterBehaviour(nameof (Handcuffs), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write((GameObject) this.cuffTarget);
      return true;
    }
    bool flag = false;
    if (((int) this.get_syncVarDirtyBits() & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write((GameObject) this.cuffTarget);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.___cuffTargetNetId = reader.ReadNetworkId();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetTarget((GameObject) reader.ReadGameObject());
    }
  }

  public virtual void PreStartClient()
  {
    if (((NetworkInstanceId) ref this.___cuffTargetNetId).IsEmpty())
      return;
    this.NetworkcuffTarget = (GameObject) ClientScene.FindLocalObject(this.___cuffTargetNetId);
  }
}
