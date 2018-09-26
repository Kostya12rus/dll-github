// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Lut_2_Lut_Extra
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Lut/Lut 2 Lut Extra")]
public class CameraFilterPack_Lut_2_Lut_Extra : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Vector4 ScreenResolution;
  private Material SCMaterial;
  public Texture2D LutTexture;
  public Texture2D LutTexture2;
  private Texture3D converted3DLut;
  private Texture3D converted3DLut2;
  [Range(0.0f, 1f)]
  public float FadeLut1;
  [Range(0.0f, 1f)]
  public float FadeLut2;
  [Range(0.0f, 1f)]
  public float Pos;
  [Range(0.0f, 1f)]
  public float Smooth;
  private string MemoPath;
  private string MemoPath2;

  public CameraFilterPack_Lut_2_Lut_Extra()
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
    this.SCShader = Shader.Find("CameraFilterPack/Lut_2_lut_Extra");
    if (SystemInfo.get_supportsImageEffects())
      return;
    ((Behaviour) this).set_enabled(false);
  }

  public void SetIdentityLut()
  {
    int num1 = 16;
    Color[] colorArray = new Color[num1 * num1 * num1];
    float num2 = (float) (1.0 / (1.0 * (double) num1 - 1.0));
    for (int index1 = 0; index1 < num1; ++index1)
    {
      for (int index2 = 0; index2 < num1; ++index2)
      {
        for (int index3 = 0; index3 < num1; ++index3)
          colorArray[index1 + index2 * num1 + index3 * num1 * num1] = new Color((float) index1 * 1f * num2, (float) index2 * 1f * num2, (float) index3 * 1f * num2, 1f);
      }
    }
    if (Object.op_Implicit((Object) this.converted3DLut))
      Object.DestroyImmediate((Object) this.converted3DLut);
    this.converted3DLut = new Texture3D(num1, num1, num1, (TextureFormat) 5, false);
    this.converted3DLut.SetPixels(colorArray);
    this.converted3DLut.Apply();
    if (Object.op_Implicit((Object) this.converted3DLut2))
      Object.DestroyImmediate((Object) this.converted3DLut2);
    this.converted3DLut2 = new Texture3D(num1, num1, num1, (TextureFormat) 5, false);
    this.converted3DLut2.SetPixels(colorArray);
    this.converted3DLut2.Apply();
  }

  public bool ValidDimensions(Texture2D tex2d)
  {
    return Object.op_Implicit((Object) tex2d) && ((Texture) tex2d).get_height() == Mathf.FloorToInt(Mathf.Sqrt((float) ((Texture) tex2d).get_width()));
  }

  public Texture3D Convert(Texture2D temp2DTex, Texture3D cv3D)
  {
    int num1 = 4096;
    if (Object.op_Implicit((Object) temp2DTex))
    {
      int num2 = ((Texture) temp2DTex).get_width() * ((Texture) temp2DTex).get_height();
      num1 = ((Texture) temp2DTex).get_height();
      if (!this.ValidDimensions(temp2DTex))
      {
        Debug.LogWarning((object) ("The given 2D texture " + ((Object) temp2DTex).get_name() + " cannot be used as a 3D LUT."));
        return cv3D;
      }
    }
    Color[] pixels = temp2DTex.GetPixels();
    Color[] colorArray = new Color[pixels.Length];
    for (int index1 = 0; index1 < num1; ++index1)
    {
      for (int index2 = 0; index2 < num1; ++index2)
      {
        for (int index3 = 0; index3 < num1; ++index3)
        {
          int num2 = num1 - index2 - 1;
          colorArray[index1 + index2 * num1 + index3 * num1 * num1] = pixels[index3 * num1 + index1 + num2 * num1 * num1];
        }
      }
    }
    if (Object.op_Implicit((Object) cv3D))
      Object.DestroyImmediate((Object) cv3D);
    cv3D = new Texture3D(num1, num1, num1, (TextureFormat) 5, false);
    cv3D.SetPixels(colorArray);
    cv3D.Apply();
    return cv3D;
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if (Object.op_Inequality((Object) this.SCShader, (Object) null) || !SystemInfo.get_supports3DTextures())
    {
      this.TimeX += Time.get_deltaTime();
      if ((double) this.TimeX > 100.0)
        this.TimeX = 0.0f;
      if (Object.op_Equality((Object) this.converted3DLut, (Object) null))
      {
        if (!Object.op_Implicit((Object) this.LutTexture))
          this.SetIdentityLut();
        if (Object.op_Implicit((Object) this.LutTexture))
          this.converted3DLut = this.Convert(this.LutTexture, this.converted3DLut);
      }
      if (Object.op_Equality((Object) this.converted3DLut2, (Object) null))
      {
        if (!Object.op_Implicit((Object) this.LutTexture2))
          this.SetIdentityLut();
        if (Object.op_Implicit((Object) this.LutTexture2))
          this.converted3DLut2 = this.Convert(this.LutTexture2, this.converted3DLut2);
      }
      if (Object.op_Implicit((Object) this.LutTexture))
        ((Texture) this.converted3DLut).set_wrapMode((TextureWrapMode) 1);
      if (Object.op_Implicit((Object) this.LutTexture2))
        ((Texture) this.converted3DLut2).set_wrapMode((TextureWrapMode) 1);
      this.material.SetFloat("_Fade", this.FadeLut1);
      this.material.SetFloat("_Fade2", this.FadeLut2);
      this.material.SetFloat("_Pos", this.Pos);
      this.material.SetFloat("_Smooth", this.Smooth);
      this.material.SetTexture("_LutTex", (Texture) this.converted3DLut);
      this.material.SetTexture("_LutTex2", (Texture) this.converted3DLut2);
      Graphics.Blit((Texture) sourceTexture, destTexture, this.material, QualitySettings.get_activeColorSpace() != 1 ? 0 : 1);
    }
    else
      Graphics.Blit((Texture) sourceTexture, destTexture);
  }

  private void OnValidate()
  {
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
