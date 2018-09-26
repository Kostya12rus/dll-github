// Decompiled with JetBrains decompiler
// Type: WorkStation
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WorkStation : NetworkBehaviour
{
  private bool prevConn;
  [SyncVar]
  public bool isTabletConnected;
  [SyncVar]
  private GameObject playerConnected;
  [SyncVar(hook = "SetPosition")]
  private Offset position;
  private static WorkStation updateRoot;
  private float animationCooldown;
  private MeshRenderer[] meshRenderers;
  public GameObject ui_place;
  public GameObject ui_take;
  public GameObject ui_using;
  public GameObject ui_notablet;
  public Animator animator;
  public WorkStation.WorkStationScreenGroup screenGroup;
  private string currentGroup;
  private Button[] buttons;
  public AudioClip beepClip;
  public AudioClip powerOnClip;
  public AudioClip powerOffClip;
  private NetworkInstanceId ___playerConnectedNetId;

  public WorkStation()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    WorkStation.updateRoot = this;
    Timing.RunCoroutine(this._Update(), (Segment) 0);
    ((MonoBehaviour) this).Invoke("UnmuteSource", 10f);
    this.meshRenderers = (MeshRenderer[]) ((Component) this).GetComponentsInChildren<MeshRenderer>(true);
  }

  private void UnmuteSource()
  {
    ((AudioSource) ((Component) this).GetComponent<AudioSource>()).set_mute(false);
  }

  public void SetPosition(Offset pos)
  {
    this.Networkposition = pos;
  }

  private void Update()
  {
    if (Vector3.op_Inequality(((Component) this).get_transform().get_localPosition(), this.position.position))
    {
      ((Component) this).get_transform().set_localPosition(this.position.position);
      ((Component) this).get_transform().set_localRotation(Quaternion.Euler(this.position.rotation));
    }
    this.CheckConnectionChange();
    this.screenGroup.SetScreenByName(this.currentGroup);
    if ((double) this.animationCooldown < 0.0)
      return;
    this.animationCooldown -= Time.get_deltaTime();
  }

  public void ChangeScreen(string scene)
  {
    this.currentGroup = scene;
  }

  public void UseButton(Button button)
  {
    if (!((CharacterClassManager) PlayerManager.localPlayer.GetComponent<CharacterClassManager>()).IsHuman())
      return;
    foreach (Button button1 in this.buttons)
    {
      if (Object.op_Equality((Object) button1, (Object) button))
      {
        ((AudioSource) ((Component) this).GetComponent<AudioSource>()).PlayOneShot(this.beepClip);
        ((UnityEvent) button1.get_onClick()).Invoke();
      }
    }
  }

  [DebuggerHidden]
  private IEnumerator<float> _Update()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new WorkStation.\u003C_Update\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void CheckConnectionChange()
  {
    foreach (Renderer meshRenderer in this.meshRenderers)
      meshRenderer.set_enabled(true);
    if (this.prevConn == this.isTabletConnected)
      return;
    this.prevConn = this.isTabletConnected;
    if (this.prevConn)
      Timing.RunCoroutine(this._OnTabletConnected(), (Segment) 1);
    else
      Timing.RunCoroutine(this._OnTabletDisconnected(), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _OnTabletConnected()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new WorkStation.\u003C_OnTabletConnected\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator<float> _OnTabletDisconnected()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new WorkStation.\u003C_OnTabletDisconnected\u003Ec__Iterator2()
    {
      \u0024this = this
    };
  }

  public bool CanPlace(GameObject tabletOwner)
  {
    if (Object.op_Inequality((Object) this.playerConnected, (Object) null) || this.isTabletConnected)
      return false;
    return this.HasInInventory(tabletOwner);
  }

  private bool HasInInventory(GameObject tabletOwner)
  {
    using (IEnumerator<Inventory.SyncItemInfo> enumerator = ((SyncList<Inventory.SyncItemInfo>) ((Inventory) tabletOwner.GetComponent<Inventory>()).items).GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        if (enumerator.Current.id == 19)
          return true;
      }
    }
    return false;
  }

  public bool CanTake(GameObject taker)
  {
    if (Object.op_Inequality((Object) taker, (Object) this.playerConnected) && Object.op_Inequality((Object) this.playerConnected, (Object) null) && (double) Vector3.Distance(this.playerConnected.get_transform().get_position(), ((Component) this).get_transform().get_position()) < 10.0)
      return false;
    return ((Inventory) taker.GetComponent<Inventory>()).items.get_Count() < (ushort) 8;
  }

  public void UnconnectTablet(GameObject taker)
  {
    if (!this.CanTake(taker) || (double) this.animationCooldown > 0.0)
      return;
    ((Inventory) taker.GetComponent<Inventory>()).AddNewItem(19, -4.656647E+11f);
    this.NetworkplayerConnected = (GameObject) null;
    this.NetworkisTabletConnected = false;
    this.animationCooldown = 3.5f;
  }

  public void ConnectTablet(GameObject tabletOwner)
  {
    if (!this.CanPlace(tabletOwner) || (double) this.animationCooldown > 0.0)
      return;
    Inventory component = (Inventory) tabletOwner.GetComponent<Inventory>();
    using (IEnumerator<Inventory.SyncItemInfo> enumerator = ((SyncList<Inventory.SyncItemInfo>) component.items).GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        Inventory.SyncItemInfo current = enumerator.Current;
        if (current.id == 19)
        {
          ((SyncList<Inventory.SyncItemInfo>) component.items).Remove(current);
          this.NetworkisTabletConnected = true;
          this.animationCooldown = 6.5f;
          this.NetworkplayerConnected = tabletOwner;
          break;
        }
      }
    }
  }

  private void UNetVersion()
  {
  }

  public bool NetworkisTabletConnected
  {
    get
    {
      return this.isTabletConnected;
    }
    [param: In] set
    {
      this.SetSyncVar<bool>((M0) (value ? 1 : 0), (M0&) ref this.isTabletConnected, 1U);
    }
  }

  public GameObject NetworkplayerConnected
  {
    get
    {
      return this.playerConnected;
    }
    [param: In] set
    {
      this.SetSyncVarGameObject((GameObject) value, (GameObject&) ref this.playerConnected, 2U, ref this.___playerConnectedNetId);
    }
  }

  public Offset Networkposition
  {
    get
    {
      return this.position;
    }
    [param: In] set
    {
      Offset offset = value;
      ref Offset local = ref this.position;
      int num = 4;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetPosition(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<Offset>((M0) offset, (M0&) ref local, (uint) num);
    }
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.isTabletConnected);
      writer.Write((GameObject) this.playerConnected);
      GeneratedNetworkCode._WriteOffset_None(writer, this.position);
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
      writer.Write(this.isTabletConnected);
    }
    if (((int) this.get_syncVarDirtyBits() & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write((GameObject) this.playerConnected);
    }
    if (((int) this.get_syncVarDirtyBits() & 4) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      GeneratedNetworkCode._WriteOffset_None(writer, this.position);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.isTabletConnected = reader.ReadBoolean();
      this.___playerConnectedNetId = reader.ReadNetworkId();
      this.position = GeneratedNetworkCode._ReadOffset_None(reader);
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.isTabletConnected = reader.ReadBoolean();
      if ((num & 2) != 0)
        this.playerConnected = (GameObject) reader.ReadGameObject();
      if ((num & 4) == 0)
        return;
      this.SetPosition(GeneratedNetworkCode._ReadOffset_None(reader));
    }
  }

  public virtual void PreStartClient()
  {
    if (((NetworkInstanceId) ref this.___playerConnectedNetId).IsEmpty())
      return;
    this.NetworkplayerConnected = (GameObject) ClientScene.FindLocalObject(this.___playerConnectedNetId);
  }

  [Serializable]
  public class WorkStationScreenGroup
  {
    public WorkStation.WorkStationScreenGroup.WorkStationScreen[] screens;
    private string curScreen;
    private WorkStation station;

    public void SetWorkstation(WorkStation s)
    {
      this.station = s;
    }

    public void SetScreenByName(string _label)
    {
      if (this.curScreen == _label)
        return;
      this.curScreen = _label;
      if (!((AudioSource) ((Component) this.station).GetComponent<AudioSource>()).get_isPlaying())
        ((AudioSource) ((Component) this.station).GetComponent<AudioSource>()).PlayOneShot(this.station.beepClip);
      foreach (WorkStation.WorkStationScreenGroup.WorkStationScreen screen in this.screens)
        screen.screenObject.SetActive(screen.label == _label);
    }

    [Serializable]
    public class WorkStationScreen
    {
      public string label;
      public GameObject screenObject;
    }
  }
}
