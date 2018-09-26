// Decompiled with JetBrains decompiler
// Type: VideoRepairer
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Video;

public class VideoRepairer : MonoBehaviour
{
  private void Start()
  {
    this.Invoke("Repair", 5f);
  }

  private void Repair()
  {
    this.GetComponent<VideoPlayer>().targetMaterialProperty = "_EmissionMap";
  }
}
