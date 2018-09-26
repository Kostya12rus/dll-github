// Decompiled with JetBrains decompiler
// Type: AnimationController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class AnimationController : NetworkBehaviour
{
  public static List<AnimationController> controllers = new List<AnimationController>();
  private static int kCmdCmdSyncData = 1138927717;
  public AnimationController.AnimAudioClip[] clips;
  public AudioSource walkSource;
  public AudioSource runSource;
  public AudioSource gunSource;
  public Animator animator;
  public Animator handAnimator;
  public Animator headAnimator;
  [SyncVar]
  public int curAnim;
  [SyncVar]
  public Vector2 speed;
  public bool cuffed;
  private FirstPersonController fpc;
  private Inventory inv;
  private PlyMovementSync pms;
  private Scp096PlayerScript scp096;
  private DistanceTo dt;
  private CharacterClassManager ccm;
  private float prevRotX;
  private int prevItem;
  private bool antiposed;

  private void Start()
  {
    this.ccm = this.GetComponent<CharacterClassManager>();
    this.dt = this.GetComponent<DistanceTo>();
    this.scp096 = this.GetComponent<Scp096PlayerScript>();
    this.pms = this.GetComponent<PlyMovementSync>();
    this.fpc = this.GetComponent<FirstPersonController>();
    this.inv = this.GetComponent<Inventory>();
    if (this.isLocalPlayer)
      return;
    AnimationController.controllers.Add(this);
    this.Invoke("RefreshItems", 6f);
  }

  private void OnDestroy()
  {
    if (this.isLocalPlayer)
      return;
    AnimationController.controllers.Remove(this);
  }

  private Quaternion GetCameraRotation()
  {
    if ((Object) this.pms == (Object) null)
      return Quaternion.Euler(Vector3.zero);
    float rotX = this.pms.rotX;
    float num = Mathf.Lerp(this.prevRotX, ((double) rotX <= 270.0 ? rotX : rotX - 360f) / 3f, Time.deltaTime * 15f);
    this.prevRotX = num;
    return Quaternion.Euler(Vector3.right * num);
  }

  private void LateUpdate()
  {
    if (!this.isLocalPlayer && (Object) this.headAnimator != (Object) null)
      this.headAnimator.transform.localRotation = this.GetCameraRotation();
    if (!this.isLocalPlayer && (Object) this.handAnimator != (Object) null)
      this.handAnimator.SetBool("Cuffed", this.cuffed);
    this.cuffed = false;
  }

  public void PlaySound(int id, bool isGun)
  {
    if (this.isLocalPlayer)
      return;
    if (isGun)
      this.gunSource.PlayOneShot(this.clips[id].audio);
    else
      this.runSource.PlayOneShot(this.clips[id].audio);
  }

  public void PlaySound(string label, bool isGun)
  {
    if (this.isLocalPlayer)
      return;
    int index1 = 0;
    for (int index2 = 0; index2 < this.clips.Length; ++index2)
    {
      if (this.clips[index2].clipName == label)
        index1 = index2;
    }
    if (isGun)
      this.gunSource.PlayOneShot(this.clips[index1].audio);
    else
      this.runSource.PlayOneShot(this.clips[index1].audio);
  }

  public void DoAnimation(string trigger)
  {
    if (this.isLocalPlayer || !((Object) this.handAnimator != (Object) null))
      return;
    this.handAnimator.SetTrigger(trigger);
  }

  private void FixedUpdate()
  {
    if (!this.isLocalPlayer)
    {
      if (this.prevItem != this.inv.curItem)
      {
        this.prevItem = this.inv.curItem;
        this.RefreshItems();
      }
      this.RecieveData();
    }
    else
      this.TransmitData(this.fpc.animationID, this.fpc.plySpeed);
  }

  private void RefreshItems()
  {
    foreach (MonoBehaviour componentsInChild in this.GetComponentsInChildren<HandPart>(true))
      componentsInChild.Invoke("UpdateItem", 0.3f);
  }

  public void SetState(int i)
  {
    this.NetworkcurAnim = i;
  }

  [DebuggerHidden]
  private IEnumerator<float> _StartAntiposing()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new AnimationController.\u003C_StartAntiposing\u003Ec__Iterator0() { \u0024this = this };
  }

  public void OnChangeClass()
  {
    this.antiposed = false;
  }

  public void RecieveData()
  {
    bool flag;
    if (TutorialManager.status)
      flag = true;
    else if ((flag = this.dt.IsInRange()) && !this.antiposed && (Object) this.handAnimator != (Object) null)
    {
      Timing.RunCoroutine(this._StartAntiposing(), Segment.FixedUpdate);
      this.antiposed = true;
    }
    if ((Object) this.handAnimator != (Object) null)
      this.handAnimator.enabled = flag;
    if (!((Object) this.animator != (Object) null))
      return;
    this.animator.enabled = flag || this.ccm.curClass == 3;
    if (!flag)
      return;
    this.CalculateAnimation();
    if ((Object) this.handAnimator == (Object) null)
    {
      foreach (Animator componentsInChild in this.animator.GetComponentsInChildren<Animator>())
      {
        if ((Object) componentsInChild != (Object) this.animator)
        {
          if (componentsInChild.transform.name.ToUpper().Contains("NECK"))
            this.headAnimator = componentsInChild;
          else
            this.handAnimator = componentsInChild;
        }
      }
    }
    else
    {
      this.handAnimator.SetInteger("CurItem", this.inv.curItem);
      this.handAnimator.SetInteger("Running", (double) this.speed.x == 0.0 ? 0 : (this.curAnim != 1 ? 1 : 2));
    }
  }

  private void CalculateAnimation()
  {
    this.animator.SetBool("Stafe", this.curAnim != 2 & ((double) Mathf.Abs(this.speed.y) > 0.0 & ((double) this.speed.x == 0.0 | (double) this.speed.x > 0.0 & this.curAnim == 0)));
    this.animator.SetBool("Jump", this.curAnim == 2);
    float num1 = 0.0f;
    float num2 = 0.0f;
    if (this.curAnim != 2)
    {
      if ((double) this.speed.x != 0.0)
      {
        num1 = this.curAnim != 1 ? 1f : 2f;
        if ((double) this.speed.x < 0.0)
          num1 = -1f;
      }
      if ((double) this.speed.y != 0.0)
        num2 = (double) this.speed.y <= 0.0 ? -1f : 1f;
    }
    this.animator.SetFloat("Speed", num1, 0.1f, Time.deltaTime);
    this.animator.SetFloat("Direction", num2, 0.1f, Time.deltaTime);
  }

  [ClientCallback]
  private void TransmitData(int state, Vector2 v2)
  {
    if (!NetworkClient.active)
      return;
    this.CallCmdSyncData(state, v2);
  }

  [Command(channel = 3)]
  private void CmdSyncData(int state, Vector2 v2)
  {
    this.NetworkcurAnim = state;
    this.Networkspeed = v2;
    Color red = Color.red;
  }

  static AnimationController()
  {
    NetworkBehaviour.RegisterCommandDelegate(typeof (AnimationController), AnimationController.kCmdCmdSyncData, new NetworkBehaviour.CmdDelegate(AnimationController.InvokeCmdCmdSyncData));
    NetworkCRC.RegisterBehaviour(nameof (AnimationController), 0);
  }

  private void UNetVersion()
  {
  }

  public int NetworkcurAnim
  {
    get
    {
      return this.curAnim;
    }
    [param: In] set
    {
      this.SetSyncVar<int>(value, ref this.curAnim, 1U);
    }
  }

  public Vector2 Networkspeed
  {
    get
    {
      return this.speed;
    }
    [param: In] set
    {
      this.SetSyncVar<Vector2>(value, ref this.speed, 2U);
    }
  }

  protected static void InvokeCmdCmdSyncData(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.active)
      Debug.LogError((object) "Command CmdSyncData called on client.");
    else
      ((AnimationController) obj).CmdSyncData((int) reader.ReadPackedUInt32(), reader.ReadVector2());
  }

  public void CallCmdSyncData(int state, Vector2 v2)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "Command function CmdSyncData called on server.");
    else if (this.isServer)
    {
      this.CmdSyncData(state, v2);
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 5);
      writer.WritePackedUInt32((uint) AnimationController.kCmdCmdSyncData);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.WritePackedUInt32((uint) state);
      writer.Write(v2);
      this.SendCommandInternal(writer, 3, "CmdSyncData");
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.WritePackedUInt32((uint) this.curAnim);
      writer.Write(this.speed);
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
      writer.WritePackedUInt32((uint) this.curAnim);
    }
    if (((int) this.syncVarDirtyBits & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      writer.Write(this.speed);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.curAnim = (int) reader.ReadPackedUInt32();
      this.speed = reader.ReadVector2();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.curAnim = (int) reader.ReadPackedUInt32();
      if ((num & 2) == 0)
        return;
      this.speed = reader.ReadVector2();
    }
  }

  [Serializable]
  public class AnimAudioClip
  {
    public string clipName;
    public AudioClip audio;
  }
}
