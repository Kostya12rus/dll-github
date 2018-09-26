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
  public LayerMask mask;
  public int maxBlood;
  public BloodDrawer.BloodType[] bloodTypes;

  public BloodDrawer()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    if (this.get_isLocalPlayer())
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
      transform = ((GameObject) Object.Instantiate<GameObject>((M0) prefabs[Random.Range(0, prefabs.Length)], pos, rot)).get_transform();
      BloodDrawer.instances.Add(transform);
    }
    else
    {
      transform = BloodDrawer.instances[0];
      BloodDrawer.instances.Add(transform);
      BloodDrawer.instances.RemoveAt(0);
      ((Component) transform).get_transform().set_position(pos);
      ((Component) transform).get_transform().set_rotation(rot);
    }
    transform.Rotate(0.0f, (float) Random.Range(0, 360), 0.0f, (Space) 1);
    float num = Random.Range(1.1f, 2f);
    transform.set_localScale(new Vector3(num, num, num));
    RaycastHit raycastHit;
    if (!Physics.Raycast(Vector3.op_Subtraction(transform.get_position(), Vector3.op_Division(transform.get_forward(), 4f)), transform.get_forward(), ref raycastHit, 0.6f, LayerMask.op_Implicit(this.mask)))
      return;
    if (((Component) ((Component) ((RaycastHit) ref raycastHit).get_collider()).get_transform()).get_tag() == "Door")
      transform.set_localScale(new Vector3((float) transform.get_localScale().x, (float) transform.get_localScale().y, 0.05f));
    transform.SetParent(((Component) ((RaycastHit) ref raycastHit).get_collider()).get_transform());
  }

  public void PlaceUnderneath(Transform obj, int type, float amountMultiplier = 1f)
  {
    this.PlaceUnderneath(obj.get_position(), type, amountMultiplier);
  }

  public void PlaceUnderneath(Vector3 pos, int type, float amountMultiplier = 1f)
  {
    RaycastHit raycastHit;
    if (!Physics.Raycast(pos, Vector3.get_down(), ref raycastHit, 3f, LayerMask.op_Implicit(this.mask)))
      return;
    GameObject[] prefabs = this.bloodTypes[type].prefabs;
    Transform transform = ((GameObject) Object.Instantiate<GameObject>((M0) prefabs[Random.Range(0, prefabs.Length)], ((RaycastHit) ref raycastHit).get_point(), Quaternion.FromToRotation(Vector3.get_up(), ((RaycastHit) ref raycastHit).get_normal()))).get_transform();
    transform.Rotate(0.0f, (float) Random.Range(0, 360), 0.0f, (Space) 1);
    float num = Random.Range(0.8f, 1.6f) * amountMultiplier;
    transform.set_localScale(new Vector3(num, num, num));
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

  [Serializable]
  public class BloodType
  {
    public GameObject[] prefabs;
  }
}
