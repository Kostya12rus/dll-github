// Decompiled with JetBrains decompiler
// Type: CameraFilterPack_Rain_RainFX
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Weather/New Rain FX")]
public class CameraFilterPack_Rain_RainFX : MonoBehaviour
{
  public Shader SCShader;
  private float TimeX;
  private Material SCMaterial;
  [Range(-8f, 8f)]
  public float Speed;
  [Range(0.0f, 1f)]
  public float Fade;
  [HideInInspector]
  public int Count;
  private Vector4[] Coord;
  public static Color ChangeColorRGB;
  private Texture2D Texture2;
  private Texture2D Texture3;

  public CameraFilterPack_Rain_RainFX()
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
    this.Texture2 = Resources.Load("CameraFilterPack_RainFX_Anm2") as Texture2D;
    this.Texture3 = Resources.Load("CameraFilterPack_RainFX_Anm") as Texture2D;
    this.SCShader = Shader.Find("CameraFilterPack/RainFX");
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
      this.material.SetFloat("_Value", this.Fade);
      this.material.SetFloat("_Speed", this.Speed);
      this.material.SetVector("_ScreenResolution", new Vector4((float) ((Texture) sourceTexture).get_width(), (float) ((Texture) sourceTexture).get_height(), 0.0f, 0.0f));
      ((Camera) ((Component) this).GetComponent<Camera>()).set_depthTextureMode((DepthTextureMode) 1);
      AnimationCurve animationCurve1 = new AnimationCurve();
      AnimationCurve animationCurve2 = new AnimationCurve();
      animationCurve2.AddKey(0.0f, 0.01f);
      animationCurve2.AddKey(64f, 5f);
      animationCurve2.AddKey(128f, 80f);
      animationCurve2.AddKey((float) byte.MaxValue, (float) byte.MaxValue);
      animationCurve2.AddKey(300f, (float) byte.MaxValue);
      for (int index = 0; index < 4; ++index)
      {
        ref Vector4 local = ref this.Coord[index];
        local.z = (__Null) (local.z + 0.5);
        if (this.Coord[index].w == -1.0)
          this.Coord[index].x = (__Null) -5.0;
        if (this.Coord[index].z > 254.0)
          this.Coord[index] = new Vector4(Random.Range(0.0f, 0.9f), Random.Range(0.2f, 1.1f), 0.0f, (float) Random.Range(0, 3));
        this.material.SetVector("Coord" + (index + 1).ToString(), new Vector4((float) this.Coord[index].x, (float) this.Coord[index].y, (float) (int) animationCurve2.Evaluate((float) this.Coord[index].z), (float) this.Coord[index].w));
      }
      this.material.SetTexture("Texture2", (Texture) this.Texture2);
      this.material.SetTexture("Texture3", (Texture) this.Texture3);
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
