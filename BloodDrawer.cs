// Decompiled with JetBrains decompiler
// Type: BloodDrawer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BloodDrawer : NetworkBehaviour
{
  private static List<Transform> instances = new List<Transform>();
  public int maxBlood = 500;
  public LayerMask mask;
  public BloodDrawer.BloodType[] bloodTypes;

  private void Start()
  {
    if (this.isLocalPlayer)
    {
      BloodDrawer.instances = new List<Transform>();
      BloodDrawer.instances.Clear();
    }
    this.maxBlood = PlayerPrefs.GetInt("gfxsets_maxblood", 250);
  }

  public void DrawBlood(Vector3 pos, Quaternion rot, int bloodType)
  {
    if (ServerStatic.IsDedicated || bloodType < 0 || this.maxBlood <= 0)
      return;
    Transform transform;
    if (BloodDrawer.instances.Count < this.maxBlood)
    {
      GameObject[] prefabs = this.bloodTypes[bloodType].prefabs;
      transform = Object.Instantiate<GameObject>(prefabs[Random.Range(0, prefabs.Length)], pos, rot).transform;
      BloodDrawer.instances.Add(transform);
    }
    else
    {
      transform = BloodDrawer.instances[0];
      BloodDrawer.instances.Add(transform);
      BloodDrawer.instances.RemoveAt(0);
      transform.transform.position = pos;
      transform.transform.rotation = rot;
    }
    transform.Rotate(0.0f, (float) Random.Range(0, 360), 0.0f, Space.Self);
    float num = Random.Range(1.1f, 2f);
    transform.localScale = new Vector3(num, num, num);
    RaycastHit hitInfo;
    if (!Physics.Raycast(transform.position - transform.forward / 4f, transform.forward, out hitInfo, 0.6f, (int) this.mask))
      return;
    if (hitInfo.collider.transform.tag == "Door")
      transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 0.05f);
    transform.SetParent(hitInfo.collider.transform);
  }

  public void PlaceUnderneath(Transform obj, int type, float amountMultiplier = 1f)
  {
    this.PlaceUnderneath(obj.position, type, amountMultiplier);
  }

  public void PlaceUnderneath(Vector3 pos, int type, float amountMultiplier = 1f)
  {
    RaycastHit hitInfo;
    if (!Physics.Raycast(pos, Vector3.down, out hitInfo, 3f, (int) this.mask))
      return;
    GameObject[] prefabs = this.bloodTypes[type].prefabs;
    Transform transform = Object.Instantiate<GameObject>(prefabs[Random.Range(0, prefabs.Length)], hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal)).transform;
    transform.Rotate(0.0f, (float) Random.Range(0, 360), 0.0f, Space.Self);
    float num = Random.Range(0.8f, 1.6f) * amountMultiplier;
    transform.localScale = new Vector3(num, num, num);
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

  [Serializable]
  public class BloodType
  {
    public GameObject[] prefabs;
  }
}
