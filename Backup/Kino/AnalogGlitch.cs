// Decompiled with JetBrains decompiler
// Type: Kino.AnalogGlitch
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace Kino
{
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Kino Image Effects/Analog Glitch")]
  [ExecuteInEditMode]
  public class AnalogGlitch : MonoBehaviour
  {
    [SerializeField]
    [Range(0.0f, 1f)]
    private float _scanLineJitter;
    [SerializeField]
    [Range(0.0f, 1f)]
    private float _verticalJump;
    [Range(0.0f, 1f)]
    [SerializeField]
    private float _horizontalShake;
    [SerializeField]
    [Range(0.0f, 1f)]
    private float _colorDrift;
    [SerializeField]
    private Shader _shader;
    private Material _material;
    private float _verticalJumpTime;

    public AnalogGlitch()
    {
      base.\u002Ector();
    }

    public float scanLineJitter
    {
      get
      {
        return this._scanLineJitter;
      }
      set
      {
        this._scanLineJitter = value;
      }
    }

    public float verticalJump
    {
      get
      {
        return this._verticalJump;
      }
      set
      {
        this._verticalJump = value;
      }
    }

    public float horizontalShake
    {
      get
      {
        return this._horizontalShake;
      }
      set
      {
        this._horizontalShake = value;
      }
    }

    public float colorDrift
    {
      get
      {
        return this._colorDrift;
      }
      set
      {
        this._colorDrift = value;
      }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (Object.op_Equality((Object) this._material, (Object) null))
      {
        this._material = new Material(this._shader);
        ((Object) this._material).set_hideFlags((HideFlags) 52);
      }
      this._verticalJumpTime += (float) ((double) Time.get_deltaTime() * (double) this._verticalJump * 11.3000001907349);
      this._material.SetVector("_ScanLineJitter", Vector4.op_Implicit(new Vector2((float) (1.0 / 500.0 + (double) Mathf.Pow(this._scanLineJitter, 3f) * 0.0500000007450581), Mathf.Clamp01((float) (1.0 - (double) this._scanLineJitter * 1.20000004768372)))));
      Vector2 vector2_1;
      ((Vector2) ref vector2_1).\u002Ector(this._verticalJump, this._verticalJumpTime);
      this._material.SetVector("_VerticalJump", Vector4.op_Implicit(vector2_1));
      this._material.SetFloat("_HorizontalShake", this._horizontalShake * 0.2f);
      Vector2 vector2_2;
      ((Vector2) ref vector2_2).\u002Ector(this._colorDrift * 0.04f, Time.get_time() * 606.11f);
      this._material.SetVector("_ColorDrift", Vector4.op_Implicit(vector2_2));
      Graphics.Blit((Texture) source, destination, this._material);
    }
  }
}
