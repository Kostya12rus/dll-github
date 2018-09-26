// Decompiled with JetBrains decompiler
// Type: CullingDisabler
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;

public class CullingDisabler : MonoBehaviour
{
  public Behaviour camera;
  public Behaviour culler;
  private bool state;

  public CullingDisabler()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    NetworkBehaviour componentInParent = (NetworkBehaviour) ((Component) this).GetComponentInParent<NetworkBehaviour>();
    if (!Object.op_Inequality((Object) componentInParent, (Object) null) || componentInParent.get_isLocalPlayer())
      return;
    Object.Destroy((Object) this.culler);
    Object.Destroy((Object) ((Component) this).GetComponent<GlobalFog>());
    Object.Destroy((Object) ((Component) this).GetComponent<VignetteAndChromaticAberration>());
    Object.Destroy((Object) ((Component) this).GetComponent<NoiseAndGrain>());
    Object.Destroy((Object) ((Component) this).GetComponent<FlareLayer>());
    Object.Destroy((Object) ((Component) this.camera).GetComponent<PostProcessingBehaviour>());
    Object.Destroy((Object) this.camera);
    Object.Destroy((Object) this);
  }

  private void Update()
  {
    if (this.state == this.camera.get_enabled())
      return;
    this.state = !this.state;
    this.culler.set_enabled(this.state);
  }
}
