// Decompiled with JetBrains decompiler
// Type: AmbientSoundPlayer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AmbientSoundPlayer : NetworkBehaviour
{
  private static int kRpcRpcPlaySound = -1539903091;
  public GameObject audioPrefab;
  public int minTime;
  public int maxTime;
  public AmbientSoundPlayer.AmbientClip[] clips;

  public AmbientSoundPlayer()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (!this.get_isLocalPlayer() || !this.get_isServer())
      return;
    for (int index = 0; index < this.clips.Length; ++index)
      this.clips[index].index = index;
    ((MonoBehaviour) this).Invoke("GenerateRandom", 10f);
  }

  private void PlaySound(int clipID)
  {
    GameObject gameObject = (GameObject) Object.Instantiate<GameObject>((M0) this.audioPrefab);
    Vector2 vector2;
    ((Vector2) ref vector2).\u002Ector((float) Random.Range(-1, 1), (float) Random.Range(-1, 1));
    Vector3 vector3_1;
    ((Vector3) ref vector3_1).\u002Ector((float) vector2.x, 0.0f, (float) vector2.y);
    Vector3 vector3_2 = Vector3.op_Multiply(((Vector3) ref vector3_1).get_normalized(), 200f);
    gameObject.get_transform().set_position(Vector3.op_Addition(vector3_2, ((Component) this).get_transform().get_position()));
    ((AudioSource) gameObject.GetComponent<AudioSource>()).set_clip(this.clips[clipID].clip);
    ((AudioSource) gameObject.GetComponent<AudioSource>()).set_spatialBlend(!this.clips[clipID].is3D ? 0.0f : 1f);
    ((AudioSource) gameObject.GetComponent<AudioSource>()).Play();
    Object.Destroy((Object) gameObject, 10f);
  }

  private void GenerateRandom()
  {
    List<AmbientSoundPlayer.AmbientClip> ambientClipList = new List<AmbientSoundPlayer.AmbientClip>();
    foreach (AmbientSoundPlayer.AmbientClip clip in this.clips)
    {
      if (!clip.played)
        ambientClipList.Add(clip);
    }
    int index1 = Random.Range(0, ambientClipList.Count);
    int index2 = ambientClipList[index1].index;
    if (!this.clips[index2].repeatable)
      this.clips[index2].played = true;
    this.CallRpcPlaySound(index2);
    ((MonoBehaviour) this).Invoke(nameof (GenerateRandom), (float) Random.Range(this.minTime, this.maxTime));
  }

  [ClientRpc(channel = 1)]
  private void RpcPlaySound(int id)
  {
    this.PlaySound(id);
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeRpcRpcPlaySound(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcPlaySound called on server.");
    else
      ((AmbientSoundPlayer) obj).RpcPlaySound((int) reader.ReadPackedUInt32());
  }

  public void CallRpcPlaySound(int id)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcPlaySound called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) AmbientSoundPlayer.kRpcRpcPlaySound);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.WritePackedUInt32((uint) id);
      this.SendRPCInternal(networkWriter, 1, "RpcPlaySound");
    }
  }

  static AmbientSoundPlayer()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (AmbientSoundPlayer), AmbientSoundPlayer.kRpcRpcPlaySound, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcPlaySound)));
    NetworkCRC.RegisterBehaviour(nameof (AmbientSoundPlayer), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }

  [Serializable]
  public class AmbientClip
  {
    public bool repeatable = true;
    public bool is3D = true;
    public AudioClip clip;
    public bool played;
    public int index;
  }
}
