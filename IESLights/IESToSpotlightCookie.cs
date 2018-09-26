// Decompiled with JetBrains decompiler
// Type: IESLights.IESToSpotlightCookie
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Linq;
using UnityEngine;

namespace IESLights
{
  [ExecuteInEditMode]
  public class IESToSpotlightCookie : MonoBehaviour
  {
    private Material _spotlightMaterial;
    private Material _fadeSpotlightEdgesMaterial;
    private Material _verticalFlipMaterial;

    private void OnDestroy()
    {
      if ((Object) this._spotlightMaterial != (Object) null)
        Object.Destroy((Object) this._spotlightMaterial);
      if ((Object) this._fadeSpotlightEdgesMaterial != (Object) null)
        Object.Destroy((Object) this._fadeSpotlightEdgesMaterial);
      if (!((Object) this._verticalFlipMaterial != (Object) null))
        return;
      Object.Destroy((Object) this._verticalFlipMaterial);
    }

    public void CreateSpotlightCookie(Texture2D iesTexture, IESData iesData, int resolution, bool applyVignette, bool flipVertically, out Texture2D cookie)
    {
      if (iesData.PhotometricType != PhotometricType.TypeA)
      {
        if ((Object) this._spotlightMaterial == (Object) null)
          this._spotlightMaterial = new Material(Shader.Find("Hidden/IES/IESToSpotlightCookie"));
        this.CalculateAndSetSpotHeight(iesData);
        this.SetShaderKeywords(iesData, applyVignette);
        cookie = this.CreateTexture(iesTexture, resolution, flipVertically);
      }
      else
      {
        if ((Object) this._fadeSpotlightEdgesMaterial == (Object) null)
          this._fadeSpotlightEdgesMaterial = new Material(Shader.Find("Hidden/IES/FadeSpotlightCookieEdges"));
        float verticalCenter = !applyVignette ? 0.0f : this.CalculateCookieVerticalCenter(iesData);
        Vector2 vector2 = !applyVignette ? Vector2.zero : this.CalculateCookieFadeEllipse(iesData);
        cookie = this.BlitToTargetSize(iesTexture, resolution, vector2.x, vector2.y, verticalCenter, applyVignette, flipVertically);
      }
    }

    private float CalculateCookieVerticalCenter(IESData iesData)
    {
      return (float) (1.0 - (double) iesData.PadBeforeAmount / (double) iesData.NormalizedValues[0].Count) - (float) ((double) (iesData.NormalizedValues[0].Count - iesData.PadBeforeAmount - iesData.PadAfterAmount) / (double) iesData.NormalizedValues.Count / 2.0);
    }

    private Vector2 CalculateCookieFadeEllipse(IESData iesData)
    {
      if (iesData.HorizontalAngles.Count > iesData.VerticalAngles.Count)
        return new Vector2(0.5f, (float) (0.5 * ((double) (iesData.NormalizedValues[0].Count - iesData.PadBeforeAmount - iesData.PadAfterAmount) / (double) iesData.NormalizedValues[0].Count)));
      if (iesData.HorizontalAngles.Count < iesData.VerticalAngles.Count)
        return new Vector2((float) (0.5 * ((double) iesData.HorizontalAngles.Max() - (double) iesData.HorizontalAngles.Min()) / ((double) iesData.VerticalAngles.Max() - (double) iesData.VerticalAngles.Min())), 0.5f);
      return new Vector2(0.5f, 0.5f);
    }

    private Texture2D CreateTexture(Texture2D iesTexture, int resolution, bool flipVertically)
    {
      RenderTexture temporary1 = RenderTexture.GetTemporary(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
      temporary1.filterMode = FilterMode.Trilinear;
      temporary1.DiscardContents();
      RenderTexture.active = temporary1;
      Graphics.Blit((Texture) iesTexture, this._spotlightMaterial);
      if (flipVertically)
      {
        RenderTexture temporary2 = RenderTexture.GetTemporary(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        Graphics.Blit((Texture) temporary1, temporary2);
        this.FlipVertically((Texture) temporary2, temporary1);
        RenderTexture.ReleaseTemporary(temporary2);
      }
      Texture2D texture2D1 = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false, true);
      texture2D1.filterMode = FilterMode.Trilinear;
      Texture2D texture2D2 = texture2D1;
      texture2D2.wrapMode = TextureWrapMode.Clamp;
      texture2D2.ReadPixels(new Rect(0.0f, 0.0f, (float) resolution, (float) resolution), 0, 0);
      texture2D2.Apply();
      RenderTexture.active = (RenderTexture) null;
      RenderTexture.ReleaseTemporary(temporary1);
      return texture2D2;
    }

    private Texture2D BlitToTargetSize(Texture2D iesTexture, int resolution, float horizontalFadeDistance, float verticalFadeDistance, float verticalCenter, bool applyVignette, bool flipVertically)
    {
      if (applyVignette)
      {
        this._fadeSpotlightEdgesMaterial.SetFloat("_HorizontalFadeDistance", horizontalFadeDistance);
        this._fadeSpotlightEdgesMaterial.SetFloat("_VerticalFadeDistance", verticalFadeDistance);
        this._fadeSpotlightEdgesMaterial.SetFloat("_VerticalCenter", verticalCenter);
      }
      RenderTexture temporary = RenderTexture.GetTemporary(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
      temporary.filterMode = FilterMode.Trilinear;
      temporary.DiscardContents();
      if (applyVignette)
      {
        RenderTexture.active = temporary;
        Graphics.Blit((Texture) iesTexture, this._fadeSpotlightEdgesMaterial);
      }
      else if (flipVertically)
        this.FlipVertically((Texture) iesTexture, temporary);
      else
        Graphics.Blit((Texture) iesTexture, temporary);
      Texture2D texture2D1 = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false, true);
      texture2D1.filterMode = FilterMode.Trilinear;
      Texture2D texture2D2 = texture2D1;
      texture2D2.wrapMode = TextureWrapMode.Clamp;
      texture2D2.ReadPixels(new Rect(0.0f, 0.0f, (float) resolution, (float) resolution), 0, 0);
      texture2D2.Apply();
      RenderTexture.active = (RenderTexture) null;
      RenderTexture.ReleaseTemporary(temporary);
      return texture2D2;
    }

    private void FlipVertically(Texture iesTexture, RenderTexture renderTarget)
    {
      if ((Object) this._verticalFlipMaterial == (Object) null)
        this._verticalFlipMaterial = new Material(Shader.Find("Hidden/IES/VerticalFlip"));
      Graphics.Blit(iesTexture, renderTarget, this._verticalFlipMaterial);
    }

    private void CalculateAndSetSpotHeight(IESData iesData)
    {
      this._spotlightMaterial.SetFloat("_SpotHeight", 0.5f / Mathf.Tan(iesData.HalfSpotlightFov * ((float) Math.PI / 180f)));
    }

    private void SetShaderKeywords(IESData iesData, bool applyVignette)
    {
      if (applyVignette)
        this._spotlightMaterial.EnableKeyword("VIGNETTE");
      else
        this._spotlightMaterial.DisableKeyword("VIGNETTE");
      if (iesData.VerticalType == VerticalType.Top)
        this._spotlightMaterial.EnableKeyword("TOP_VERTICAL");
      else
        this._spotlightMaterial.DisableKeyword("TOP_VERTICAL");
      if (iesData.HorizontalType == HorizontalType.None)
      {
        this._spotlightMaterial.DisableKeyword("QUAD_HORIZONTAL");
        this._spotlightMaterial.DisableKeyword("HALF_HORIZONTAL");
        this._spotlightMaterial.DisableKeyword("FULL_HORIZONTAL");
      }
      else if (iesData.HorizontalType == HorizontalType.Quadrant)
      {
        this._spotlightMaterial.EnableKeyword("QUAD_HORIZONTAL");
        this._spotlightMaterial.DisableKeyword("HALF_HORIZONTAL");
        this._spotlightMaterial.DisableKeyword("FULL_HORIZONTAL");
      }
      else if (iesData.HorizontalType == HorizontalType.Half)
      {
        this._spotlightMaterial.DisableKeyword("QUAD_HORIZONTAL");
        this._spotlightMaterial.EnableKeyword("HALF_HORIZONTAL");
        this._spotlightMaterial.DisableKeyword("FULL_HORIZONTAL");
      }
      else
      {
        if (iesData.HorizontalType != HorizontalType.Full)
          return;
        this._spotlightMaterial.DisableKeyword("QUAD_HORIZONTAL");
        this._spotlightMaterial.DisableKeyword("HALF_HORIZONTAL");
        this._spotlightMaterial.EnableKeyword("FULL_HORIZONTAL");
      }
    }
  }
}
