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
  private Vector3 position = new Vector3(-1471f, 160.5f, -3426.9f);
  private Vector3 rotation = new Vector3(0.0f, 180f, 0.0f);
  public float range;
  [SyncVar(hook = "SetState")]
  public bool allowContain;
  private CharacterClassManager ccm;
  [Space(10f)]
  public Transform hatch;
  public Vector3 closedPos;
  public Vector3 openPosition;
  private GameObject localplayer;

  public void SetState(bool b)
  {
    this.NetworkallowContain = b;
    if (!b)
      return;
    this.hatch.GetComponent<AudioSource>().Play();
  }

  private void Start()
  {
    this.transform.localPosition = this.position;
    this.transform.localRotation = Quaternion.Euler(this.rotation);
  }

  private void Update()
  {
    this.CheckForLure();
    this.hatch.localPosition = Vector3.Slerp(this.hatch.localPosition, !this.allowContain ? this.openPosition : this.closedPos, Time.deltaTime * 3f);
  }

  private void CheckForLure()
  {
    if ((Object) this.ccm == (Object) null)
    {
      this.localplayer = PlayerManager.localPlayer;
      if (!((Object) this.localplayer != (Object) null))
        return;
      this.ccm = this.localplayer.GetComponent<CharacterClassManager>();
    }
    else
    {
      if (this.ccm.curClass < 0)
        return;
      this.GetComponent<BoxCollider>().enabled = this.ccm.klasy[this.ccm.curClass].team == Team.SCP;
    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(this.transform.position, this.range);
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
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetState(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<bool>(num1 != 0, ref local, (uint) num2);
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.allowContain);
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
      writer.Write(this.allowContain);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
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
