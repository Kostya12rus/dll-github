// Decompiled with JetBrains decompiler
// Type: Scp914
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class Scp914 : NetworkBehaviour
{
  private int prevStatus = -1;
  public static Scp914 singleton;
  public Texture burntIcon;
  public AudioSource soundSource;
  public Transform doors;
  public Transform knob;
  public Transform intake_obj;
  public Transform output_obj;
  public float colliderSize;
  public Scp914.Recipe[] recipes;
  [SyncVar(hook = "SetStatus")]
  public int knobStatus;
  private float cooldown;
  public bool working;

  private void Awake()
  {
    Scp914.singleton = this;
  }

  private void SetStatus(int i)
  {
    this.NetworkknobStatus = i;
  }

  public void ChangeKnobStatus()
  {
    if (this.working || (double) this.cooldown >= 0.0)
      return;
    this.cooldown = 0.2f;
    Scp914 scp914 = this;
    scp914.NetworkknobStatus = scp914.knobStatus + 1;
    if (this.knobStatus < 5)
      return;
    this.NetworkknobStatus = 0;
  }

  public void StartRefining()
  {
    if (this.working)
      return;
    this.working = true;
    Timing.RunCoroutine(this._Animation(), Segment.Update);
  }

  private void Update()
  {
    if (this.knobStatus != this.prevStatus)
    {
      this.knob.GetComponent<AudioSource>().Play();
      this.prevStatus = this.knobStatus;
    }
    if ((double) this.cooldown >= 0.0)
      this.cooldown -= Time.deltaTime;
    this.knob.transform.localRotation = Quaternion.Lerp(this.knob.transform.localRotation, Quaternion.Euler(Vector3.forward * Mathf.Lerp(-89f, 89f, (float) this.knobStatus / 4f)), Time.deltaTime * 4f);
  }

  private IEnumerator<float> _Animation()
  {
    this.soundSource.Play();
    yield return Timing.WaitForSeconds(1f);
    float t = 0.0f;
    while ((double) t < 1.0)
    {
      t += Time.deltaTime * 0.85f;
      this.doors.transform.localPosition = Vector3.right * Mathf.Lerp(1.74f, 0.0f, t);
      yield return 0.0f;
    }
    yield return Timing.WaitForSeconds(0.0f);
    this.UpgradeItems();
    yield return Timing.WaitForSeconds(0.0f);
    while ((double) t > 0.0)
    {
      t -= Time.deltaTime * 0.85f;
      this.SetDoorPos(t);
      yield return 0.0f;
    }
    yield return Timing.WaitForSeconds(1f);
    this.working = false;
  }

  [ServerCallback]
  private void UpgradeItems()
  {
    if (!NetworkServer.active)
      return;
    foreach (Collider collider in Physics.OverlapBox(this.intake_obj.position, Vector3.one * this.colliderSize / 2f))
    {
      Pickup component = collider.GetComponent<Pickup>();
      collider.GetComponentInParent<PlayerStats>();
      if ((Object) component != (Object) null)
      {
        GameObject gameObject = (GameObject) null;
        foreach (GameObject player in PlayerManager.singleton.players)
        {
          if (player.GetComponent<QueryProcessor>().PlayerId == component.info.ownerPlayerID)
            gameObject = player;
        }
        component.transform.position = component.transform.position + (this.output_obj.position - this.intake_obj.position) + Vector3.up;
        if (component.info.itemId < this.recipes.Length)
        {
          int[] array = this.recipes[component.info.itemId].outputs[this.knobStatus].outputs.ToArray();
          int num = array[Random.Range(0, array.Length)];
          if (num < 0)
          {
            component.Delete();
            if (TutorialManager.status)
              Object.FindObjectOfType<TutorialManager>().Tutorial3_KeycardBurnt();
          }
          else
          {
            if (num <= 11 && (Object) gameObject != (Object) null && gameObject.GetComponent<CharacterClassManager>().curClass == 6)
            {
              foreach (GameObject player in PlayerManager.singleton.players)
              {
                if (player.GetComponent<CharacterClassManager>().curClass == 1 && (double) Vector3.Distance(player.transform.position, gameObject.transform.position) < 10.0)
                  PlayerManager.localPlayer.GetComponent<PlayerStats>().CallTargetAchieve(gameObject.GetComponent<CharacterClassManager>().connectionToClient, "friendship");
              }
            }
            Pickup.PickupInfo info = component.info;
            info.itemId = num;
            component.Networkinfo = info;
            component.RefreshDurability(false);
          }
        }
      }
    }
  }

  private void SetDoorPos(float t)
  {
    this.doors.transform.localPosition = Vector3.right * Mathf.Lerp(1.74f, 0.0f, t);
  }

  private void UNetVersion()
  {
  }

  public int NetworkknobStatus
  {
    get
    {
      return this.knobStatus;
    }
    [param: In] set
    {
      int num1 = value;
      ref int local = ref this.knobStatus;
      int num2 = 1;
      if (NetworkServer.localClientActive && !this.syncVarHookGuard)
      {
        this.syncVarHookGuard = true;
        this.SetStatus(value);
        this.syncVarHookGuard = false;
      }
      this.SetSyncVar<int>(num1, ref local, (uint) num2);
    }
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.WritePackedUInt32((uint) this.knobStatus);
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
      writer.WritePackedUInt32((uint) this.knobStatus);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.knobStatus = (int) reader.ReadPackedUInt32();
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      this.SetStatus((int) reader.ReadPackedUInt32());
    }
  }

  [Serializable]
  public class Recipe
  {
    public List<Scp914.Recipe.Output> outputs = new List<Scp914.Recipe.Output>();

    [Serializable]
    public class Output
    {
      public List<int> outputs = new List<int>();
    }
  }
}
