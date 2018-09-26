// Decompiled with JetBrains decompiler
// Type: IESLights.IESToCubemap
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace IESLights
{
  [ExecuteInEditMode]
  public class IESToCubemap : MonoBehaviour
  {
    private Material _iesMaterial;
    private Material _horizontalMirrorMaterial;

    public IESToCubemap()
    {
      base.\u002Ector();
    }

    private void OnDestroy()
    {
      if (!Object.op_Inequality((Object) this._horizontalMirrorMaterial, (Object) null))
        return;
      Object.DestroyImmediate((Object) this._horizontalMirrorMaterial);
    }

    public void CreateCubemap(Texture2D iesTexture, IESData iesData, int resolution, out Cubemap cubemap)
    {
      this.PrepMaterial(iesTexture, iesData);
      this.CreateCubemap(resolution, out cubemap);
    }

    public Color[] CreateRawCubemap(Texture2D iesTexture, IESData iesData, int resolution)
    {
      this.PrepMaterial(iesTexture, iesData);
      RenderTexture[] renderTextureArray = new RenderTexture[6];
      for (int index = 0; index < 6; ++index)
      {
        renderTextureArray[index] = RenderTexture.GetTemporary(resolution, resolution, 0, (RenderTextureFormat) 11, (RenderTextureReadWrite) 1);
        ((Texture) renderTextureArray[index]).set_filterMode((FilterMode) 2);
      }
      Camera[] componentsInChildren = (Camera[]) ((Component) ((Component) this).get_transform().GetChild(0)).GetComponentsInChildren<Camera>();
      for (int index = 0; index < 6; ++index)
      {
        componentsInChildren[index].set_targetTexture(renderTextureArray[index]);
        componentsInChildren[index].Render();
        componentsInChildren[index].set_targetTexture((RenderTexture) null);
      }
      RenderTexture temporary = RenderTexture.GetTemporary(resolution * 6, resolution, 0, (RenderTextureFormat) 11, (RenderTextureReadWrite) 1);
      ((Texture) temporary).set_filterMode((FilterMode) 2);
      if (Object.op_Equality((Object) this._horizontalMirrorMaterial, (Object) null))
        this._horizontalMirrorMaterial = new Material(Shader.Find("Hidden/IES/HorizontalFlip"));
      RenderTexture.set_active(temporary);
      for (int index = 0; index < 6; ++index)
      {
        GL.PushMatrix();
        GL.LoadPixelMatrix(0.0f, (float) (resolution * 6), 0.0f, (float) resolution);
        Graphics.DrawTexture(new Rect((float) (index * resolution), 0.0f, (float) resolution, (float) resolution), (Texture) renderTextureArray[index], this._horizontalMirrorMaterial);
        GL.PopMatrix();
      }
      Texture2D texture2D1 = new Texture2D(resolution * 6, resolution, (TextureFormat) 20, false, true);
      ((Texture) texture2D1).set_filterMode((FilterMode) 2);
      Texture2D texture2D2 = texture2D1;
      texture2D2.ReadPixels(new Rect(0.0f, 0.0f, (float) ((Texture) texture2D2).get_width(), (float) ((Texture) texture2D2).get_height()), 0, 0);
      Color[] pixels = texture2D2.GetPixels();
      RenderTexture.set_active((RenderTexture) null);
      foreach (RenderTexture renderTexture in renderTextureArray)
        RenderTexture.ReleaseTemporary(renderTexture);
      RenderTexture.ReleaseTemporary(temporary);
      Object.DestroyImmediate((Object) texture2D2);
      return pixels;
    }

    private void PrepMaterial(Texture2D iesTexture, IESData iesData)
    {
      if (Object.op_Equality((Object) this._iesMaterial, (Object) null))
        this._iesMaterial = ((Renderer) ((Component) this).GetComponent<Renderer>()).get_sharedMaterial();
      this._iesMaterial.set_mainTexture((Texture) iesTexture);
      this.SetShaderKeywords(iesData, this._iesMaterial);
    }

    private void SetShaderKeywords(IESData iesData, Material iesMaterial)
    {
      if (iesData.VerticalType == VerticalType.Bottom)
      {
        iesMaterial.EnableKeyword("BOTTOM_VERTICAL");
        iesMaterial.DisableKeyword("TOP_VERTICAL");
        iesMaterial.DisableKeyword("FULL_VERTICAL");
      }
      else if (iesData.VerticalType == VerticalType.Top)
      {
        iesMaterial.EnableKeyword("TOP_VERTICAL");
        iesMaterial.DisableKeyword("BOTTOM_VERTICAL");
        iesMaterial.DisableKeyword("FULL_VERTICAL");
      }
      else
      {
        iesMaterial.DisableKeyword("TOP_VERTICAL");
        iesMaterial.DisableKeyword("BOTTOM_VERTICAL");
        iesMaterial.EnableKeyword("FULL_VERTICAL");
      }
      if (iesData.HorizontalType == HorizontalType.None)
      {
        iesMaterial.DisableKeyword("QUAD_HORIZONTAL");
        iesMaterial.DisableKeyword("HALF_HORIZONTAL");
        iesMaterial.DisableKeyword("FULL_HORIZONTAL");
      }
      else if (iesData.HorizontalType == HorizontalType.Quadrant)
      {
        iesMaterial.EnableKeyword("QUAD_HORIZONTAL");
        iesMaterial.DisableKeyword("HALF_HORIZONTAL");
        iesMaterial.DisableKeyword("FULL_HORIZONTAL");
      }
      else if (iesData.HorizontalType == HorizontalType.Half)
      {
        iesMaterial.DisableKeyword("QUAD_HORIZONTAL");
        iesMaterial.EnableKeyword("HALF_HORIZONTAL");
        iesMaterial.DisableKeyword("FULL_HORIZONTAL");
      }
      else
      {
        if (iesData.HorizontalType != HorizontalType.Full)
          return;
        iesMaterial.DisableKeyword("QUAD_HORIZONTAL");
        iesMaterial.DisableKeyword("HALF_HORIZONTAL");
        iesMaterial.EnableKeyword("FULL_HORIZONTAL");
      }
    }

    private void CreateCubemap(int resolution, out Cubemap cubemap)
    {
      ref Cubemap local = ref cubemap;
      Cubemap cubemap1 = new Cubemap(resolution, (TextureFormat) 5, false);
      ((Texture) cubemap1).set_filterMode((FilterMode) 2);
      Cubemap cubemap2 = cubemap1;
      local = cubemap2;
      ((Camera) ((Component) this).GetComponent<Camera>()).RenderToCubemap(cubemap);
    }
  }
}
