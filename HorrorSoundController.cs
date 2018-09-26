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
  private CharacterClassManager cmng;
  public LayerMask mask;
  [SerializeField]
  private Transform plyCamera;
  public AudioSource horrorSoundSource;
  [SerializeField]
  private HorrorSoundController.DistanceSound[] sounds;
  private float cooldown;
  private PlayerManager pmng;
  public AudioClip blindedSoundClip;

  public HorrorSoundController()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.pmng = PlayerManager.singleton;
    this.cmng = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
  }

  public void BlindSFX()
  {
    this.horrorSoundSource.PlayOneShot(this.blindedSoundClip);
  }

  private void Update()
  {
    if (!this.get_isLocalPlayer() || this.cmng.curClass < 0 || (this.cmng.curClass == 2 || this.cmng.klasy[this.cmng.curClass].team == Team.SCP))
      return;
    List<GameObject> gameObjectList1 = new List<GameObject>();
    foreach (GameObject player in this.pmng.players)
    {
      CharacterClassManager component = (CharacterClassManager) player.GetComponent<CharacterClassManager>();
      if (component.curClass >= 0 && component.klasy[component.curClass].team == Team.SCP)
        gameObjectList1.Add(player);
    }
    List<GameObject> gameObjectList2 = new List<GameObject>();
    using (List<GameObject>.Enumerator enumerator = gameObjectList1.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        GameObject current = enumerator.Current;
        Vector3 position = this.plyCamera.get_position();
        Vector3 vector3 = Vector3.op_Subtraction(current.get_transform().get_position(), this.plyCamera.get_position());
        Vector3 normalized = ((Vector3) ref vector3).get_normalized();
        RaycastHit raycastHit;
        ref RaycastHit local = ref raycastHit;
        double num1 = 50.0;
        int num2 = LayerMask.op_Implicit(this.mask);
        if (Physics.Raycast(position, normalized, ref local, (float) num1, num2) && ((double) Mathf.Abs((float) ((double) Vector3.Angle(((Component) this).get_transform().get_forward(), ((RaycastHit) ref raycastHit).get_normal()) + ((Component) this).get_transform().get_rotation().y - 180.0)) < 50.0 && Object.op_Inequality((Object) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponentInParent<CharacterClassManager>(), (Object) null) && ((CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>()).klasy[((CharacterClassManager) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponentInParent<CharacterClassManager>()).curClass].team == Team.SCP))
          gameObjectList2.Add(current);
      }
    }
    if (gameObjectList2.Count == 0)
    {
      this.cooldown -= Time.get_deltaTime();
      if ((double) this.cooldown >= 0.0 || TutorialManager.status)
        return;
      SoundtrackManager.singleton.StopOverlay(0);
    }
    else
    {
      if ((double) this.cooldown < 0.0)
      {
        float num1 = float.PositiveInfinity;
        using (List<GameObject>.Enumerator enumerator = gameObjectList2.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            float num2 = Vector3.Distance(((Component) this).get_transform().get_position(), enumerator.Current.get_transform().get_position());
            if ((double) num2 < (double) num1)
              num1 = num2;
          }
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

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }

  [Serializable]
  public struct DistanceSound
  {
    public float distance;
    public AudioClip clip;
  }
}
