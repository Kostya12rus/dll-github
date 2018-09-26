﻿// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.AmplifyLensFlare
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyLensFlare : IAmplifyItem
  {
    [SerializeField]
    private float m_overallIntensity = 1f;
    [SerializeField]
    private float m_normalizedGhostIntensity = 0.8f;
    [SerializeField]
    private float m_normalizedHaloIntensity = 0.1f;
    [SerializeField]
    private bool m_applyLensFlare = true;
    [SerializeField]
    private int m_lensFlareGhostAmount = 3;
    [SerializeField]
    private Vector4 m_lensFlareGhostsParams = new Vector4(0.8f, 0.228f, 1f, 4f);
    [SerializeField]
    private float m_lensFlareGhostChrDistortion = 2f;
    private Color[] m_lensFlareGradColor = new Color[256];
    [SerializeField]
    private Vector4 m_lensFlareHaloParams = new Vector4(0.1f, 0.573f, 1f, 128f);
    [SerializeField]
    private float m_lensFlareHaloChrDistortion = 1.51f;
    [SerializeField]
    private int m_lensFlareGaussianBlurAmount = 1;
    private const int LUTTextureWidth = 256;
    [SerializeField]
    private Gradient m_lensGradient;
    [SerializeField]
    private Texture2D m_lensFlareGradTexture;

    public AmplifyLensFlare()
    {
      this.m_lensGradient = new Gradient();
      this.m_lensGradient.SetKeys(new GradientColorKey[5]
      {
        new GradientColorKey(Color.get_white(), 0.0f),
        new GradientColorKey(Color.get_blue(), 0.25f),
        new GradientColorKey(Color.get_green(), 0.5f),
        new GradientColorKey(Color.get_yellow(), 0.75f),
        new GradientColorKey(Color.get_red(), 1f)
      }, new GradientAlphaKey[5]
      {
        new GradientAlphaKey(1f, 0.0f),
        new GradientAlphaKey(1f, 0.25f),
        new GradientAlphaKey(1f, 0.5f),
        new GradientAlphaKey(1f, 0.75f),
        new GradientAlphaKey(1f, 1f)
      });
    }

    public void Destroy()
    {
      if (!Object.op_Inequality((Object) this.m_lensFlareGradTexture, (Object) null))
        return;
      Object.DestroyImmediate((Object) this.m_lensFlareGradTexture);
      this.m_lensFlareGradTexture = (Texture2D) null;
    }

    public void CreateLUTexture()
    {
      this.m_lensFlareGradTexture = new Texture2D(256, 1, (TextureFormat) 5, false);
      ((Texture) this.m_lensFlareGradTexture).set_filterMode((FilterMode) 1);
      this.TextureFromGradient();
    }

    public RenderTexture ApplyFlare(Material material, RenderTexture source)
    {
      RenderTexture tempRenderTarget = AmplifyUtils.GetTempRenderTarget(((Texture) source).get_width(), ((Texture) source).get_height());
      material.SetVector(AmplifyUtils.LensFlareGhostsParamsId, this.m_lensFlareGhostsParams);
      material.SetTexture(AmplifyUtils.LensFlareLUTId, (Texture) this.m_lensFlareGradTexture);
      material.SetVector(AmplifyUtils.LensFlareHaloParamsId, this.m_lensFlareHaloParams);
      material.SetFloat(AmplifyUtils.LensFlareGhostChrDistortionId, this.m_lensFlareGhostChrDistortion);
      material.SetFloat(AmplifyUtils.LensFlareHaloChrDistortionId, this.m_lensFlareHaloChrDistortion);
      Graphics.Blit((Texture) source, tempRenderTarget, material, 3 + this.m_lensFlareGhostAmount);
      return tempRenderTarget;
    }

    public void TextureFromGradient()
    {
      for (int index = 0; index < 256; ++index)
        this.m_lensFlareGradColor[index] = this.m_lensGradient.Evaluate((float) index / (float) byte.MaxValue);
      this.m_lensFlareGradTexture.SetPixels(this.m_lensFlareGradColor);
      this.m_lensFlareGradTexture.Apply();
    }

    public bool ApplyLensFlare
    {
      get
      {
        return this.m_applyLensFlare;
      }
      set
      {
        this.m_applyLensFlare = value;
      }
    }

    public float OverallIntensity
    {
      get
      {
        return this.m_overallIntensity;
      }
      set
      {
        this.m_overallIntensity = (double) value >= 0.0 ? value : 0.0f;
        this.m_lensFlareGhostsParams.x = (__Null) ((double) value * (double) this.m_normalizedGhostIntensity);
        this.m_lensFlareHaloParams.x = (__Null) ((double) value * (double) this.m_normalizedHaloIntensity);
      }
    }

    public int LensFlareGhostAmount
    {
      get
      {
        return this.m_lensFlareGhostAmount;
      }
      set
      {
        this.m_lensFlareGhostAmount = value;
      }
    }

    public Vector4 LensFlareGhostsParams
    {
      get
      {
        return this.m_lensFlareGhostsParams;
      }
      set
      {
        this.m_lensFlareGhostsParams = value;
      }
    }

    public float LensFlareNormalizedGhostsIntensity
    {
      get
      {
        return this.m_normalizedGhostIntensity;
      }
      set
      {
        this.m_normalizedGhostIntensity = (double) value >= 0.0 ? value : 0.0f;
        this.m_lensFlareGhostsParams.x = (__Null) ((double) this.m_overallIntensity * (double) this.m_normalizedGhostIntensity);
      }
    }

    public float LensFlareGhostsIntensity
    {
      get
      {
        return (float) this.m_lensFlareGhostsParams.x;
      }
      set
      {
        this.m_lensFlareGhostsParams.x = (double) value >= 0.0 ? (__Null) (double) value : (__Null) 0.0;
      }
    }

    public float LensFlareGhostsDispersal
    {
      get
      {
        return (float) this.m_lensFlareGhostsParams.y;
      }
      set
      {
        this.m_lensFlareGhostsParams.y = (__Null) (double) value;
      }
    }

    public float LensFlareGhostsPowerFactor
    {
      get
      {
        return (float) this.m_lensFlareGhostsParams.z;
      }
      set
      {
        this.m_lensFlareGhostsParams.z = (__Null) (double) value;
      }
    }

    public float LensFlareGhostsPowerFalloff
    {
      get
      {
        return (float) this.m_lensFlareGhostsParams.w;
      }
      set
      {
        this.m_lensFlareGhostsParams.w = (__Null) (double) value;
      }
    }

    public Gradient LensFlareGradient
    {
      get
      {
        return this.m_lensGradient;
      }
      set
      {
        this.m_lensGradient = value;
      }
    }

    public Vector4 LensFlareHaloParams
    {
      get
      {
        return this.m_lensFlareHaloParams;
      }
      set
      {
        this.m_lensFlareHaloParams = value;
      }
    }

    public float LensFlareNormalizedHaloIntensity
    {
      get
      {
        return this.m_normalizedHaloIntensity;
      }
      set
      {
        this.m_normalizedHaloIntensity = (double) value >= 0.0 ? value : 0.0f;
        this.m_lensFlareHaloParams.x = (__Null) ((double) this.m_overallIntensity * (double) this.m_normalizedHaloIntensity);
      }
    }

    public float LensFlareHaloIntensity
    {
      get
      {
        return (float) this.m_lensFlareHaloParams.x;
      }
      set
      {
        this.m_lensFlareHaloParams.x = (double) value >= 0.0 ? (__Null) (double) value : (__Null) 0.0;
      }
    }

    public float LensFlareHaloWidth
    {
      get
      {
        return (float) this.m_lensFlareHaloParams.y;
      }
      set
      {
        this.m_lensFlareHaloParams.y = (__Null) (double) value;
      }
    }

    public float LensFlareHaloPowerFactor
    {
      get
      {
        return (float) this.m_lensFlareHaloParams.z;
      }
      set
      {
        this.m_lensFlareHaloParams.z = (__Null) (double) value;
      }
    }

    public float LensFlareHaloPowerFalloff
    {
      get
      {
        return (float) this.m_lensFlareHaloParams.w;
      }
      set
      {
        this.m_lensFlareHaloParams.w = (__Null) (double) value;
      }
    }

    public float LensFlareGhostChrDistortion
    {
      get
      {
        return this.m_lensFlareGhostChrDistortion;
      }
      set
      {
        this.m_lensFlareGhostChrDistortion = value;
      }
    }

    public float LensFlareHaloChrDistortion
    {
      get
      {
        return this.m_lensFlareHaloChrDistortion;
      }
      set
      {
        this.m_lensFlareHaloChrDistortion = value;
      }
    }

    public int LensFlareGaussianBlurAmount
    {
      get
      {
        return this.m_lensFlareGaussianBlurAmount;
      }
      set
      {
        this.m_lensFlareGaussianBlurAmount = value;
      }
    }
  }
}
