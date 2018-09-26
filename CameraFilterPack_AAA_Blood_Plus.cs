// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_AAA_Blood_Plus
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("Camera Filter Pack/AAA/Blood_Plus")]
[ExecuteInEditMode]
public class CameraFilterPack_AAA_Blood_Plus : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  [Range(0.0f, 1f)]
  public float Blood_1;
  [Range(0.0f, 1f)]
  public float Blood_2;
  [Range(0.0f, 1f)]
  public float Blood_3;
  [Range(0.0f, 1f)]
  public float Blood_4;
  [Range(0.0f, 1f)]
  public float Blood_5;
  [Range(0.0f, 1f)]
  public float Blood_6;
  [Range(0.0f, 1f)]
  public float Blood_7;
  [Range(0.0f, 1f)]
  public float Blood_8;
  [Range(0.0f, 1f)]
  public float Blood_9;
  [Range(0.0f, 1f)]
  public float Blood_10;
  [Range(0.0f, 1f)]
  public float Blood_11;
  [Range(0.0f, 1f)]
  public float Blood_12;
  [Range(0.0f, 1f)]
  public float LightReflect;
  private Material SCMaterial;
  private Texture2D Texture2;

  public CameraFilterPack_AAA_Blood_Plus()
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
    this.Texture2 = Resources.Load("CameraFilterPack_AAA_Blood2") as Texture2D;
    this.SCShader = Shader.Find("CameraFilterPack/AAA_Blood_Plus");
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
      this.material.SetFloat("_Value2", Mathf.Clamp(this.Blood_1, 0.0f, 1f));
      this.material.SetFloat("_Value3", Mathf.Clamp(this.Blood_2, 0.0f, 1f));
      this.material.SetFloat("_Value4", Mathf.Clamp(this.Blood_3, 0.0f, 1f));
      this.material.SetFloat("_Value5", Mathf.Clamp(this.Blood_4, 0.0f, 1f));
      this.material.SetFloat("_Value6", Mathf.Clamp(this.Blood_5, 0.0f, 1f));
      this.material.SetFloat("_Value7", Mathf.Clamp(this.Blood_6, 0.0f, 1f));
      this.material.SetFloat("_Value8", Mathf.Clamp(this.Blood_7, 0.0f, 1f));
      this.material.SetFloat("_Value9", Mathf.Clamp(this.Blood_8, 0.0f, 1f));
      this.material.SetFloat("_Value10", Mathf.Clamp(this.Blood_9, 0.0f, 1f));
      this.material.SetFloat("_Value11", Mathf.Clamp(this.Blood_10, 0.0f, 1f));
      this.material.SetFloat("_Value12", Mathf.Clamp(this.Blood_11, 0.0f, 1f));
      this.material.SetFloat("_Value13", Mathf.Clamp(this.Blood_12, 0.0f, 1f));
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
