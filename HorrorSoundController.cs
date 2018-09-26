// Decompiled with JetBrains decompiler
// Type: HorrorSoundController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HorrorSoundController : NetworkBehaviour
{
  private float cooldown = 20f;
  private CharacterClassManager cmng;
  public LayerMask mask;
  [SerializeField]
  private Transform plyCamera;
  public AudioSource horrorSoundSource;
  [SerializeField]
  private HorrorSoundController.DistanceSound[] sounds;
  private PlayerManager pmng;
  public AudioClip blindedSoundClip;

  private void Start()
  {
    this.pmng = PlayerManager.singleton;
    this.cmng = this.GetComponent<CharacterClassManager>();
  }

  public void BlindSFX()
  {
    this.horrorSoundSource.PlayOneShot(this.blindedSoundClip);
  }

  private void Update()
  {
    if (!this.isLocalPlayer || this.cmng.curClass < 0 || (this.cmng.curClass == 2 || this.cmng.klasy[this.cmng.curClass].team == Team.SCP))
      return;
    List<GameObject> gameObjectList1 = new List<GameObject>();
    foreach (GameObject player in this.pmng.players)
    {
      CharacterClassManager component = player.GetComponent<CharacterClassManager>();
      if (component.curClass >= 0 && component.klasy[component.curClass].team == Team.SCP)
        gameObjectList1.Add(player);
    }
    List<GameObject> gameObjectList2 = new List<GameObject>();
    foreach (GameObject gameObject in gameObjectList1)
    {
      RaycastHit hitInfo;
      if (Physics.Raycast(this.plyCamera.position, (gameObject.transform.position - this.plyCamera.position).normalized, out hitInfo, 50f, (int) this.mask) && ((double) Mathf.Abs((float) ((double) Vector3.Angle(this.transform.forward, hitInfo.normal) + (double) this.transform.rotation.y - 180.0)) < 50.0 && (Object) hitInfo.transform.GetComponentInParent<CharacterClassManager>() != (Object) null && this.GetComponent<CharacterClassManager>().klasy[hitInfo.transform.GetComponentInParent<CharacterClassManager>().curClass].team == Team.SCP))
        gameObjectList2.Add(gameObject);
    }
    if (gameObjectList2.Count == 0)
    {
      this.cooldown -= Time.deltaTime;
      if ((double) this.cooldown >= 0.0 || TutorialManager.status)
        return;
      SoundtrackManager.singleton.StopOverlay(0);
    }
    else
    {
      if ((double) this.cooldown < 0.0)
      {
        float num1 = float.PositiveInfinity;
        foreach (GameObject gameObject in gameObjectList2)
        {
          float num2 = Vector3.Distance(this.transform.position, gameObject.transform.position);
          if ((double) num2 < (double) num1)
            num1 = num2;
        }
        for (int index = 0; index < this.sounds.Length; ++index)
        {
          if ((double) this.sounds[index].distance > (double) num1)
          {
            this.horrorSoundSource.PlayOneShot(this.sounds[index].clip);
            this.cooldown = 20f;
            SoundtrackManager.singleton.PlayOverlay(0);
            return;
          }
        }
      }
      this.cooldown = 20f;
    }
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
  public struct DistanceSound
  {
    public float distance;
    public AudioClip clip;
  }
}
