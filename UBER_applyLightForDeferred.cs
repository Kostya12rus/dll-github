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

  public UBER_applyLightForDeferred()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    this.Reset();
  }

  private void Reset()
  {
    if (Object.op_Implicit((Object) ((Component) this).GetComponent<Light>()) && Object.op_Equality((Object) this.lightForSelfShadowing, (Object) null))
      this.lightForSelfShadowing = (Light) ((Component) this).GetComponent<Light>();
    if (!Object.op_Implicit((Object) ((Component) this).GetComponent<Renderer>()) || !Object.op_Equality((Object) this._renderer, (Object) null))
      return;
    this._renderer = (Renderer) ((Component) this).GetComponent<Renderer>();
  }

  private void Update()
  {
    if (!Object.op_Implicit((Object) this.lightForSelfShadowing))
      return;
    if (Object.op_Implicit((Object) this._renderer))
    {
      if (this.lightForSelfShadowing.get_type() == 1)
      {
        for (int index = 0; index < this._renderer.get_sharedMaterials().Length; ++index)
          this._renderer.get_sharedMaterials()[index].SetVector("_WorldSpaceLightPosCustom", Vector4.op_Implicit(Vector3.op_UnaryNegation(((Component) this.lightForSelfShadowing).get_transform().get_forward())));
      }
      else
      {
        for (int index = 0; index < this._renderer.get_materials().Length; ++index)
          this._renderer.get_sharedMaterials()[index].SetVector("_WorldSpaceLightPosCustom", new Vector4((float) ((Component) this.lightForSelfShadowing).get_transform().get_position().x, (float) ((Component) this.lightForSelfShadowing).get_transform().get_position().y, (float) ((Component) this.lightForSelfShadowing).get_transform().get_position().z, 1f));
      }
    }
    else if (this.lightForSelfShadowing.get_type() == 1)
      Shader.SetGlobalVector("_WorldSpaceLightPosCustom", Vector4.op_Implicit(Vector3.op_UnaryNegation(((Component) this.lightForSelfShadowing).get_transform().get_forward())));
    else
      Shader.SetGlobalVector("_WorldSpaceLightPosCustom", new Vector4((float) ((Component) this.lightForSelfShadowing).get_transform().get_position().x, (float) ((Component) this.lightForSelfShadowing).get_transform().get_position().y, (float) ((Component) this.lightForSelfShadowing).get_transform().get_position().z, 1f));
  }
}
