// Decompiled with JetBrains decompiler
// Type: MotionBlurController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.PostProcessing;

public class MotionBlurController : MonoBehaviour
{
  private int f;
  private float t;
  private bool b;
  private PostProcessingProfile[] profiles;

  private void Start()
  {
    this.profiles = Resources.FindObjectsOfTypeAll<PostProcessingProfile>();
  }

  private void Update()
  {
    this.t += Time.deltaTime;
    ++this.f;
    if ((double) this.t <= 1.0)
      return;
    --this.t;
    if (this.b && this.f < 30 || !this.b && this.f > 50)
      this.Change();
    this.f = 0;
  }

  private void Change()
  {
    this.b = !this.b;
    if (PlayerPrefs.GetInt("gfxsets_mb", 1) != 1)
      return;
    foreach (PostProcessingProfile profile in this.profiles)
      profile.motionBlur.enabled = false;
  }
}
