// Decompiled with JetBrains decompiler
// Type: Handcuffs
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

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

  private void Start()
  {
    this.uncuffProgress = GameObject.Find("UncuffProgress").GetComponent<Image>();
    this.inv = this.GetComponent<Inventory>();
    this.plyCam = this.GetComponent<Scp049PlayerScript>().plyCam.transform;
    this.ccm = this.GetComponent<CharacterClassManager>();
  }

  private void Update()
  {
    if (this.isLocalPlayer)
    {
      this.CheckForInput();
      this.UpdateText();
    }
    if (!((Object) this.cuffTarget != (Object) null))
      return;
    this.cuffTarget.GetComponent<AnimationController>().cuffed = true;
  }

  private void CheckForInput()
  {
    if ((Object) this.cuffTarget != (Object) null)
    {
      bool flag = false;
      foreach (Inventory.SyncItemInfo syncItemInfo in (SyncList<Inventory.SyncItemInfo>) this.inv.items)
      {
        if (syncItemInfo.id == 27)
          flag = true;
      }
      if (!flag)
        this.CallCmdTarget((GameObject) null);
    }
    if ((double) Inventory.inventoryCooldown > 0.0)
      return;
    if (this.inv.curItem == 27)
    {
      if (Input.GetKeyDown(NewInput.GetKey("Shoot")) && (Object) this.cuffTarget == (Object) null)
        this.CuffPlayer();
      else if (Input.GetKeyDown(NewInput.GetKey("Zoom")) && (Object) this.cuffTarget != (Object) null)
        this.CallCmdTarget((GameObject) null);
    }
    if (this.ccm.curClass >= 0 && this.ccm.klasy[this.ccm.curClass].team != Team.SCP && Input.GetKey(NewInput.GetKey("Interact")))
    {
      RaycastHit hitInfo;
      if (Physics.Raycast(this.plyCam.position, this.plyCam.forward, out hitInfo, this.maxDistance, (int) this.GetComponent<PlayerInteract>().mask))
      {
        Handcuffs componentInParent = hitInfo.collider.GetComponentInParent<Handcuffs>();
        if ((Object) componentInParent != (Object) null && (Object) componentInParent.GetComponent<AnimationController>().handAnimator != (Object) null && componentInParent.GetComponent<AnimationController>().handAnimator.GetBool("Cuffed"))
        {
          this.progress += Time.deltaTime;
          if ((double) this.progress >= 1.5)
          {
            this.progress = 0.0f;
            foreach (GameObject player in PlayerManager.singleton.players)
            {
              if ((Object) player.GetComponent<Handcuffs>().cuffTarget == (Object) componentInParent.gameObject)
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
    this.uncuffProgress.fillAmount = Mathf.Clamp01(this.progress / 1.5f);
  }

  private void CuffPlayer()
  {
    RaycastHit hitInfo;
    if (!Physics.Raycast(new Ray(this.plyCam.position, this.plyCam.forward), out hitInfo, this.maxDistance, (int) this.mask))
      return;
    CharacterClassManager componentInParent = hitInfo.collider.GetComponentInParent<CharacterClassManager>();
    if (!((Object) componentInParent != (Object) null))
      return;
    Class @class = this.ccm.klasy[componentInParent.curClass];
    if (@class.team == Team.SCP || ((@class.team == Team.CDP ? 1 : (@class.team == Team.CHI ? 1 : 0)) == (this.ccm.klasy[this.ccm.curClass].team == Team.CDP ? 1 : (this.ccm.klasy[this.ccm.curClass].team == Team.CHI ? 1 : 0)) || componentInParent.GetComponent<AnimationController>().curAnim != 0 || !(componentInParent.GetComponent<AnimationController>().speed == Vector2.zero)))
      return;
    if (this.ccm.klasy[this.ccm.curClass].team == Team.CDP && @class.team == Team.MTF)
      AchievementManager.Achieve("tableshaveturned");
    this.CallCmdTarget(componentInParent.gameObject);
  }

  [Command(channel = 2)]
  public void CmdTarget(GameObject t)
  {
    if (!((Object) t == (Object) null) && ((double) Vector3.Distance(this.transform.position, t.transform.position) >= 3.0 || this.inv.curItem != 27))
      return;
    this.SetTarget(t);
    if (!((Object) t != (Object) null))
      return;
    t.GetComponent<Inventory>().ServerDropAll();
  }

  [Command(channel = 2)]
  public void CmdResetTarget(GameObject t)
  {
    t.GetComponent<Handcuffs>().SetTarget((GameObject) null);
  }

  private void SetTarget(GameObject t)
  {
    this.NetworkcuffTarget = t;
  }

  private void UpdateText()
  {
    if ((Object) this.cuffTarget != (Object) null)
    {
      float num = Vector3.Distance(this.transform.position, this.cuffTarget.transform.position);
      if ((double) num > 200.0)
      {
        num = 200f;
        this.lostCooldown += Time.deltaTime;
        if ((double) this.lostCooldown > 1.0)
          this.CallCmdTarget((GameObject) null);
      }
      else
        this.lostCooldown = 0.0f;
      this.distanceText.text = (num * 1.5f).ToString("0 m");
    }
    else
      this.distanceText.text = "NONE";
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
      GameObject newGameObject = value;
      ref GameObject local1 = ref this.cuffTarget;
      int num = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetTarget(value);
        this.syncVarHookGuard = false;
      }
      ref NetworkInstanceId local2 = ref this.___cuffTargetNetId;
      this.SetSyncVarGameObject(newGameObject, ref local1, (uint) num, ref local2);
    }
  }

  protected static void InvokeCmdCmdTarget(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdTarget called on client.");
    else
      ((Handcuffs) obj).CmdTarget(reader.ReadGameObject());
  }

  protected static void InvokeCmdCmdResetTarget(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdResetTarget called on client.");
    else
      ((Handcuffs) obj).CmdResetTarget(reader.ReadGameObject());
  }

  public void CallCmdTarget(GameObject t)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdTarget called on server.");
    else if (this.isServer)
    {
      this.CmdTarget(t);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Handcuffs.kCmdCmdTarget);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(t);
      this.SendCommandInternal(writer, 2, "CmdTarget");
    }
  }

  public void CallCmdResetTarget(GameObject t)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdResetTarget called on server.");
    else if (this.isServer)
    {
      this.CmdResetTarget(t);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) Handcuffs.kCmdCmdResetTarget);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.Write(t);
      this.SendCommandInternal(writer, 2, "CmdResetTarget");
    }
  }

  static Handcuffs()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (Handcuffs), Handcuffs.kCmdCmdTarget, new NetworkBehaviour.CmdDelegate(Handcuffs.InvokeCmdCmdTarget));
    Handcuffs.kCmdCmdResetTarget = -1476369842;
    NetworkBehaviour.RegisterCommandDelegate(typeof (Handcuffs), Handcuffs.kCmdCmdResetTarget, new NetworkBehaviour.CmdDelegate(Handcuffs.InvokeCmdCmdResetTarget));
    NetworkCRC.RegisterBehaviour(nameof (Handcuffs), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.cuffTarget);
      return true;
    }
    bool flag = false;
    if (((int) this.syncVarDirtyBits & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.cuffTarget);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.___cuffTargetNetId = reader.ReadNetworkId();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetTarget(reader.ReadGameObject());
    }
  }

  public override void PreStartClient()
  {
    if (this.___cuffTargetNetId.IsEmpty())
      return;
    this.NetworkcuffTarget = ClientScene.FindLocalObject(this.___cuffTargetNetId);
  }
}
