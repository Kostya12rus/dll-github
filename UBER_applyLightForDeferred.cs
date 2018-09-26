// Decompiled with JetBrains decompiler
// Type: UBER_applyLightForDeferred
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("UBER/Apply Light for Deferred")]
public class UBER_applyLightForDeferred : MonoBehaviour
{
  public Light lightForSelfShadowing;
  private Renderer _renderer;

  private void Start()
  {
    this.Reset();
  }

  private void Reset()
  {
    if ((bool) ((Object) this.GetComponent<Light>()) && (Object) this.lightForSelfShadowing == (Object) null)
      this.lightForSelfShadowing = this.GetComponent<Light>();
    if (!(bool) ((Object) this.GetComponent<Renderer>()) || !((Object) this._renderer == (Object) null))
      return;
    this._renderer = this.GetComponent<Renderer>();
  }

  private void Update()
  {
    if (!(bool) ((Object) this.lightForSelfShadowing))
      return;
    if ((bool) ((Object) this._renderer))
    {
      if (this.lightForSelfShadowing.type == LightType.Directional)
      {
        for (int index = 0; index < this._renderer.sharedMaterials.Length; ++index)
          this._renderer.sharedMaterials[index].SetVector("_WorldSpaceLightPosCustom", (Vector4) (-this.lightForSelfShadowing.transform.forward));
      }
      else
      {
        for (int index = 0; index < this._renderer.materials.Length; ++index)
          this._renderer.sharedMaterials[index].SetVector("_WorldSpaceLightPosCustom", new Vector4(this.lightForSelfShadowing.transform.position.x, this.lightForSelfShadowing.transform.position.y, this.lightForSelfShadowing.transform.position.z, 1f));
      }
    }
    else if (this.lightForSelfShadowing.type == LightType.Directional)
      Shader.SetGlobalVector("_WorldSpaceLightPosCustom", (Vector4) (-this.lightForSelfShadowing.transform.forward));
    else
      Shader.SetGlobalVector("_WorldSpaceLightPosCustom", new Vector4(this.lightForSelfShadowing.transform.position.x, this.lightForSelfShadowing.transform.position.y, this.lightForSelfShadowing.transform.position.z, 1f));
  }
}
