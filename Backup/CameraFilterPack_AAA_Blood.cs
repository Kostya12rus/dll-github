// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_AAA_Blood
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/AAA/Blood")]
public class CameraFilterPack_AAA_Blood : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  [Range(0.0f, 128f)]
  public float Blood1;
  [Range(0.0f, 128f)]
  public float Blood2;
  [Range(0.0f, 128f)]
  public float Blood3;
  [Range(0.0f, 128f)]
  public float Blood4;
  [Range(0.0f, 0.004f)]
  public float LightReflect;
  private Material SCMaterial;
  private Texture2D Texture2;

  public CameraFilterPack_AAA_Blood()
  {
    base.\u002Ector();
  }

  private Material material
  {
    get
    {
      if (Object.op_Equality((Object) this.SCMaterial, (Object) null))
      {
        this.SCMaterial = new Material(this.SCShader);
        ((Object) this.SCMaterial).set_hideFlags((HideFlags) 61);
      }
      return this.SCMaterial;
    }
  }

  private void Start()
  {
    this.Texture2 = Resources.Load("CameraFilterPack_AAA_Blood1") as Texture2D;
    this.SCShader = Shader.Find("CameraFilterPack/AAA_Blood");
    if (SystemInfo.get_supportsImageEffects())
      return;
    ((Behaviour) this).set_enabled(false);
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if (Object.op_Inequality((Object) this.SCShader, (Object) null))
    {
      this.TimeX += Time.get_deltaTime();
      if ((double) this.TimeX > 100.0)
        this.TimeX = 0.0f;
      this.material.SetFloat("_TimeX", this.TimeX);
      this.material.SetFloat("_Value", this.LightReflect);
      this.material.SetFloat("_Value2", this.Blood1);
      this.material.SetFloat("_Value3", this.Blood2);
      this.material.SetFloat("_Value4", this.Blood3);
      this.material.SetFloat("_Value5", this.Blood4);
      this.material.SetTexture("_MainTex2", (Texture) this.Texture2);
      Graphics.Blit((Texture) sourceTexture, destTexture, this.material);
    }
    else
      Graphics.Blit((Texture) sourceTexture, destTexture);
  }

  private void Update()
  {
  }

  private void OnDisable()
  {
    if (!Object.op_Implicit((Object) this.SCMaterial))
      return;
    Object.DestroyImmediate((Object) this.SCMaterial);
  }
}
