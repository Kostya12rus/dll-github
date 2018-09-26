// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Lut_PlayWith
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Lut/PlayWith")]
public class CameraFilterPack_Lut_PlayWith : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  public Texture2D LutTexture;
  private Texture3D converted3DLut;
  [Range(0.0f, 1f)]
  public float Blend;
  [Range(0.0f, 3f)]
  public float OriginalIntensity;
  [Range(-1f, 1f)]
  public float ResultIntensity;
  [Range(-1f, 1f)]
  public float FinalIntensity;
  private string MemoPath;

  public CameraFilterPack_Lut_PlayWith()
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
    this.SCShader = Shader.Find("CameraFilterPack/Lut_PlayWith");
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
  }

  public bool ValidDimensions(Texture2D tex2d)
  {
    return Object.op_Implicit((Object) tex2d) && ((Texture) tex2d).get_height() == Mathf.FloorToInt(Mathf.Sqrt((float) ((Texture) tex2d).get_width()));
  }

  public void Convert(Texture2D temp2DTex)
  {
    if (Object.op_Implicit((Object) temp2DTex))
    {
      int num1 = ((Texture) temp2DTex).get_width() * ((Texture) temp2DTex).get_height();
      int height = ((Texture) temp2DTex).get_height();
      if (!this.ValidDimensions(temp2DTex))
      {
        Debug.LogWarning((object) ("The given 2D texture " + ((Object) temp2DTex).get_name() + " cannot be used as a 3D LUT."));
      }
      else
      {
        Color[] pixels = temp2DTex.GetPixels();
        Color[] colorArray = new Color[pixels.Length];
        for (int index1 = 0; index1 < height; ++index1)
        {
          for (int index2 = 0; index2 < height; ++index2)
          {
            for (int index3 = 0; index3 < height; ++index3)
            {
              int num2 = height - index2 - 1;
              colorArray[index1 + index2 * height + index3 * height * height] = pixels[index3 * height + index1 + num2 * height * height];
            }
          }
        }
        if (Object.op_Implicit((Object) this.converted3DLut))
          Object.DestroyImmediate((Object) this.converted3DLut);
        this.converted3DLut = new Texture3D(height, height, height, (TextureFormat) 5, false);
        this.converted3DLut.SetPixels(colorArray);
        this.converted3DLut.Apply();
      }
    }
    else
      this.SetIdentityLut();
  }

  private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
  {
    if (Object.op_Inequality((Object) this.SCShader, (Object) null) || !SystemInfo.get_supports3DTextures())
    {
      this.TimeX += Time.get_deltaTime();
      if ((double) this.TimeX > 100.0)
        this.TimeX = 0.0f;
      if (Object.op_Equality((Object) this.converted3DLut, (Object) null))
        this.Convert(this.LutTexture);
      ((Texture) this.converted3DLut).set_wrapMode((TextureWrapMode) 1);
      this.material.SetTexture("_LutTex", (Texture) this.converted3DLut);
      this.material.SetFloat("_Blend", this.Blend);
      this.material.SetFloat("_Intensity", this.OriginalIntensity);
      this.material.SetFloat("_Extra", this.ResultIntensity);
      this.material.SetFloat("_Extra2", this.FinalIntensity);
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
