// Decompiled with JetBrains decompiler
// Type: DecontaminationSpeaker
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class DecontaminationSpeaker : MonoBehaviour
{
  private static AudioSource source;
  private static bool isGlobal;
  private static DecontaminationSpeaker singleton;
  public Door[] doorsToOpen;
  private Transform lplayer;

  public DecontaminationSpeaker()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    DecontaminationSpeaker.singleton = this;
    DecontaminationSpeaker.isGlobal = false;
    DecontaminationSpeaker.source = (AudioSource) ((Component) this).GetComponent<AudioSource>();
  }

  public static void OpenDoors()
  {
    foreach (Door door in DecontaminationSpeaker.singleton.doorsToOpen)
    {
      if ((double) door.curCooldown <= 0.0 && !door.isOpen)
        door.OpenDecontamination();
    }
  }

  private void Start()
  {
    this.lplayer = ((Component) ((SpectatorCamera) Object.FindObjectOfType<SpectatorCamera>()).cam).get_transform();
  }

  private void Update()
  {
    if (!DecontaminationSpeaker.source.get_isPlaying())
      DecontaminationSpeaker.isGlobal = false;
    float y = (float) this.lplayer.get_position().y;
    int num = DecontaminationSpeaker.isGlobal || (double) y > -100.0 && (double) y < 100.0 ? 1 : 0;
    if (num == 0 && (double) DecontaminationSpeaker.source.get_volume() > 0.850000023841858 && DecontaminationSpeaker.source.get_isPlaying())
      return;
    DecontaminationSpeaker.source.set_volume(Mathf.Lerp(DecontaminationSpeaker.source.get_volume(), (float) num, Time.get_deltaTime() * 2f));
  }

  public static void PlaySound(AudioClip clip, bool global)
  {
    DecontaminationSpeaker.isGlobal = global;
    DecontaminationSpeaker.source.PlayOneShot(clip);
    if (!DecontaminationSpeaker.isGlobal)
      return;
    DecontaminationSpeaker.source.set_volume(1f);
  }
}
