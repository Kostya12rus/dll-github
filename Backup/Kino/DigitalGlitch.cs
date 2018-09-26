// Decompiled with JetBrains decompiler
// Type: Kino.DigitalGlitch
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace Kino
{
  [AddComponentMenu("Kino Image Effects/Digital Glitch")]
  [RequireComponent(typeof (Camera))]
  [ExecuteInEditMode]
  public class DigitalGlitch : MonoBehaviour
  {
    [SerializeField]
    [Range(0.0f, 1f)]
    private float _intensity;
    [SerializeField]
    private Shader _shader;
    private Material _material;
    private Texture2D _noiseTexture;
    private RenderTexture _trashFrame1;
    private RenderTexture _trashFrame2;

    public DigitalGlitch()
    {
      base.\u002Ector();
    }

    public float intensity
    {
      get
      {
        return this._intensity;
      }
      set
      {
        this._intensity = value;
      }
    }

    private static Color RandomColor()
    {
      return new Color(Random.get_value(), Random.get_value(), Random.get_value(), Random.get_value());
    }

    private void SetUpResources()
    {
      if (Object.op_Inequality((Object) this._material, (Object) null))
        return;
      this._material = new Material(this._shader);
      ((Object) this._material).set_hideFlags((HideFlags) 52);
      this._noiseTexture = new Texture2D(64, 32, (TextureFormat) 5, false);
      ((Object) this._noiseTexture).set_hideFlags((HideFlags) 52);
      ((Texture) this._noiseTexture).set_wrapMode((TextureWrapMode) 1);
      ((Texture) this._noiseTexture).set_filterMode((FilterMode) 0);
      this._trashFrame1 = new RenderTexture(Screen.get_width(), Screen.get_height(), 0);
      this._trashFrame2 = new RenderTexture(Screen.get_width(), Screen.get_height(), 0);
      ((Object) this._trashFrame1).set_hideFlags((HideFlags) 52);
      ((Object) this._trashFrame2).set_hideFlags((HideFlags) 52);
      this.UpdateNoiseTexture();
    }

    private void UpdateNoiseTexture()
    {
      Color color = DigitalGlitch.RandomColor();
      for (int index1 = 0; index1 < ((Texture) this._noiseTexture).get_height(); ++index1)
      {
        for (int index2 = 0; index2 < ((Texture) this._noiseTexture).get_width(); ++index2)
        {
          if ((double) Random.get_value() > 0.889999985694885)
            color = DigitalGlitch.RandomColor();
          this._noiseTexture.SetPixel(index2, index1, color);
        }
      }
      this._noiseTexture.Apply();
    }

    private void Update()
    {
      if ((double) Random.get_value() <= (double) Mathf.Lerp(0.9f, 0.5f, this._intensity))
        return;
      this.SetUpResources();
      this.UpdateNoiseTexture();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      this.SetUpResources();
      int frameCount = Time.get_frameCount();
      if (frameCount % 13 == 0)
        Graphics.Blit((Texture) source, this._trashFrame1);
      if (frameCount % 73 == 0)
        Graphics.Blit((Texture) source, this._trashFrame2);
      this._material.SetFloat("_Intensity", this._intensity);
      this._material.SetTexture("_NoiseTex", (Texture) this._noiseTexture);
      this._material.SetTexture("_TrashTex", (double) Random.get_value() <= 0.5 ? (Texture) this._trashFrame2 : (Texture) this._trashFrame1);
      Graphics.Blit((Texture) source, destination, this._material);
    }
  }
}
