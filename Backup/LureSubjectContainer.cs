// Decompiled with JetBrains decompiler
// Type: LureSubjectContainer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class LureSubjectContainer : NetworkBehaviour
{
  private Vector3 position;
  private Vector3 rotation;
  public float range;
  [SyncVar(hook = "SetState")]
  public bool allowContain;
  private CharacterClassManager ccm;
  [Space(10f)]
  public Transform hatch;
  public Vector3 closedPos;
  public Vector3 openPosition;
  private GameObject localplayer;

  public LureSubjectContainer()
  {
    base.\u002Ector();
  }

  public void SetState(bool b)
  {
    this.NetworkallowContain = b;
    if (!b)
      return;
    ((AudioSource) ((Component) this.hatch).GetComponent<AudioSource>()).Play();
  }

  private void Start()
  {
    ((Component) this).get_transform().set_localPosition(this.position);
    ((Component) this).get_transform().set_localRotation(Quaternion.Euler(this.rotation));
  }

  private void Update()
  {
    this.CheckForLure();
    this.hatch.set_localPosition(Vector3.Slerp(this.hatch.get_localPosition(), !this.allowContain ? this.openPosition : this.closedPos, Time.get_deltaTime() * 3f));
  }

  private void CheckForLure()
  {
    if (Object.op_Equality((Object) this.ccm, (Object) null))
    {
      this.localplayer = PlayerManager.localPlayer;
      if (!Object.op_Inequality((Object) this.localplayer, (Object) null))
        return;
      this.ccm = (CharacterClassManager) this.localplayer.GetComponent<CharacterClassManager>();
    }
    else
    {
      if (this.ccm.curClass < 0)
        return;
      ((Collider) ((Component) this).GetComponent<BoxCollider>()).set_enabled(this.ccm.klasy[this.ccm.curClass].team == Team.SCP);
    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_color(Color.get_green());
    Gizmos.DrawWireSphere(((Component) this).get_transform().get_position(), this.range);
  }

  private void UNetVersion()
  {
  }

  public bool NetworkallowContain
  {
    get
    {
      return this.allowContain;
    }
    [param: In] set
    {
      int num1 = value ? 1 : 0;
      ref bool local = ref this.allowContain;
      int num2 = 1;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.SetState(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<bool>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.allowContain);
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
      writer.Write(this.allowContain);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.allowContain = reader.ReadBoolean();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetState(reader.ReadBoolean());
    }
  }
}
