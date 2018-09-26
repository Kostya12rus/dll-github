// Decompiled with JetBrains decompiler
// Type: MenuMusicManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
  public float lerpSpeed = 1f;
  private float curState;
  private bool creditsChanged;
  [Space(15f)]
  public AudioSource mainSource;
  public AudioSource creditsSource;
  [Space(8f)]
  public GameObject creditsHolder;

  private void Update()
  {
    this.curState = Mathf.Lerp(this.curState, !this.creditsHolder.activeSelf ? 0.0f : 1f, this.lerpSpeed * Time.deltaTime);
    this.mainSource.volume = 1f - this.curState;
    this.creditsSource.volume = this.curState;
    if (this.creditsChanged == this.creditsHolder.activeSelf)
      return;
    this.creditsChanged = this.creditsHolder.activeSelf;
    if (!this.creditsChanged)
      return;
    this.creditsSource.Play();
  }
}
