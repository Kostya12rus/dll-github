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

  public SECTR_FogDisable()
  {
    base.\u002Ector();
  }

  private void OnPreRender()
  {
    this.previousFogState = RenderSettings.get_fog();
    RenderSettings.set_fog(false);
  }

  private void OnPostRender()
  {
    RenderSettings.set_fog(this.previousFogState);
  }
}
