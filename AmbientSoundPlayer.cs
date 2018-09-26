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
  public int minTime = 30;
  public int maxTime = 60;
  public GameObject audioPrefab;
  public AmbientSoundPlayer.AmbientClip[] clips;

  private void Start()
  {
    if (!this.isLocalPlayer || !this.isServer)
      return;
    for (int index = 0; index < this.clips.Length; ++index)
      this.clips[index].index = index;
    this.Invoke("GenerateRandom", 10f);
  }

  private void PlaySound(int clipID)
  {
    GameObject gameObject = Object.Instantiate<GameObject>(this.audioPrefab);
    Vector2 vector2 = new Vector2((float) Random.Range(-1, 1), (float) Random.Range(-1, 1));
    Vector3 vector3 = new Vector3(vector2.x, 0.0f, vector2.y).normalized * 200f;
    gameObject.transform.position = vector3 + this.transform.position;
    gameObject.GetComponent<AudioSource>().clip = this.clips[clipID].clip;
    gameObject.GetComponent<AudioSource>().spatialBlend = !this.clips[clipID].is3D ? 0.0f : 1f;
    gameObject.GetComponent<AudioSource>().Play();
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
    this.Invoke(nameof (GenerateRandom), (float) Random.Range(this.minTime, this.maxTime));
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
    if (!NetworkClient.active)
      Debug.LogError((object) "RPC RpcPlaySound called on server.");
    else
      ((AmbientSoundPlayer) obj).RpcPlaySound((int) reader.ReadPackedUInt32());
  }

  public void CallRpcPlaySound(int id)
  {
    if (!NetworkServer.active)
    {
      Debug.LogError((object) "RPC Function RpcPlaySound called on client.");
    }
    else
    {
      NetworkWriter writer = new NetworkWriter();
      writer.Write((short) 0);
      writer.Write((short) 2);
      writer.WritePackedUInt32((uint) AmbientSoundPlayer.kRpcRpcPlaySound);
      writer.Write(this.GetComponent<NetworkIdentity>().netId);
      writer.WritePackedUInt32((uint) id);
      this.SendRPCInternal(writer, 1, "RpcPlaySound");
    }
  }

  static AmbientSoundPlayer()
  {
    NetworkBehaviour.RegisterRpcDelegate(typeof (AmbientSoundPlayer), AmbientSoundPlayer.kRpcRpcPlaySound, new NetworkBehaviour.CmdDelegate(AmbientSoundPlayer.InvokeRpcRpcPlaySound));
    NetworkCRC.RegisterBehaviour(nameof (AmbientSoundPlayer), 0);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
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
