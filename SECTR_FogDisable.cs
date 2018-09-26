// Decompiled with JetBrains decompiler
// Type: SECTR_FogDisable
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
public class SECTR_FogDisable : MonoBehaviour
{
  private bool previousFogState;

  private void OnPreRender()
  {
    this.previousFogState = RenderSettings.fog;
    RenderSettings.fog = false;
  }

  private void OnPostRender()
  {
    RenderSettings.fog = this.previousFogState;
  }
}
