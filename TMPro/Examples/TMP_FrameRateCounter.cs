// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TMP_FrameRateCounter
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace TMPro.Examples
{
  public class TMP_FrameRateCounter : MonoBehaviour
  {
    public float UpdateInterval;
    private float m_LastInterval;
    private int m_Frames;
    public TMP_FrameRateCounter.FpsCounterAnchorPositions AnchorPosition;
    private string htmlColorTag;
    private const string fpsLabel = "{0:2}</color> <#8080ff>FPS \n<#FF8000>{1:2} <#8080ff>MS";
    private TextMeshPro m_TextMeshPro;
    private Transform m_frameCounter_transform;
    private Camera m_camera;
    private TMP_FrameRateCounter.FpsCounterAnchorPositions last_AnchorPosition;

    public TMP_FrameRateCounter()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      if (!((Behaviour) this).get_enabled())
        return;
      this.m_camera = Camera.get_main();
      Application.set_targetFrameRate(-1);
      GameObject gameObject = new GameObject("Frame Counter");
      this.m_TextMeshPro = (TextMeshPro) gameObject.AddComponent<TextMeshPro>();
      ((TMP_Text) this.m_TextMeshPro).set_font((TMP_FontAsset) Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF"));
      ((TMP_Text) this.m_TextMeshPro).set_fontSharedMaterial((Material) Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Overlay"));
      this.m_frameCounter_transform = gameObject.get_transform();
      this.m_frameCounter_transform.SetParent(((Component) this.m_camera).get_transform());
      this.m_frameCounter_transform.set_localRotation(Quaternion.get_identity());
      ((TMP_Text) this.m_TextMeshPro).set_enableWordWrapping(false);
      ((TMP_Text) this.m_TextMeshPro).set_fontSize(24f);
      ((TMP_Text) this.m_TextMeshPro).set_isOverlay(true);
      this.Set_FrameCounter_Position(this.AnchorPosition);
      this.last_AnchorPosition = this.AnchorPosition;
    }

    private void Start()
    {
      this.m_LastInterval = Time.get_realtimeSinceStartup();
      this.m_Frames = 0;
    }

    private void Update()
    {
      if (this.AnchorPosition != this.last_AnchorPosition)
        this.Set_FrameCounter_Position(this.AnchorPosition);
      this.last_AnchorPosition = this.AnchorPosition;
      ++this.m_Frames;
      float realtimeSinceStartup = Time.get_realtimeSinceStartup();
      if ((double) realtimeSinceStartup <= (double) this.m_LastInterval + (double) this.UpdateInterval)
        return;
      float num1 = (float) this.m_Frames / (realtimeSinceStartup - this.m_LastInterval);
      float num2 = 1000f / Mathf.Max(num1, 1E-05f);
      this.htmlColorTag = (double) num1 >= 30.0 ? ((double) num1 >= 10.0 ? "<color=green>" : "<color=red>") : "<color=yellow>";
      ((TMP_Text) this.m_TextMeshPro).SetText(this.htmlColorTag + "{0:2}</color> <#8080ff>FPS \n<#FF8000>{1:2} <#8080ff>MS", num1, num2);
      this.m_Frames = 0;
      this.m_LastInterval = realtimeSinceStartup;
    }

    private void Set_FrameCounter_Position(TMP_FrameRateCounter.FpsCounterAnchorPositions anchor_position)
    {
      ((TMP_Text) this.m_TextMeshPro).set_margin(new Vector4(1f, 1f, 1f, 1f));
      switch (anchor_position)
      {
        case TMP_FrameRateCounter.FpsCounterAnchorPositions.TopLeft:
          ((TMP_Text) this.m_TextMeshPro).set_alignment((TextAlignmentOptions) 257);
          ((TMP_Text) this.m_TextMeshPro).get_rectTransform().set_pivot(new Vector2(0.0f, 1f));
          this.m_frameCounter_transform.set_position(this.m_camera.ViewportToWorldPoint(new Vector3(0.0f, 1f, 100f)));
          break;
        case TMP_FrameRateCounter.FpsCounterAnchorPositions.BottomLeft:
          ((TMP_Text) this.m_TextMeshPro).set_alignment((TextAlignmentOptions) 1025);
          ((TMP_Text) this.m_TextMeshPro).get_rectTransform().set_pivot(new Vector2(0.0f, 0.0f));
          this.m_frameCounter_transform.set_position(this.m_camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 100f)));
          break;
        case TMP_FrameRateCounter.FpsCounterAnchorPositions.TopRight:
          ((TMP_Text) this.m_TextMeshPro).set_alignment((TextAlignmentOptions) 260);
          ((TMP_Text) this.m_TextMeshPro).get_rectTransform().set_pivot(new Vector2(1f, 1f));
          this.m_frameCounter_transform.set_position(this.m_camera.ViewportToWorldPoint(new Vector3(1f, 1f, 100f)));
          break;
        case TMP_FrameRateCounter.FpsCounterAnchorPositions.BottomRight:
          ((TMP_Text) this.m_TextMeshPro).set_alignment((TextAlignmentOptions) 1028);
          ((TMP_Text) this.m_TextMeshPro).get_rectTransform().set_pivot(new Vector2(1f, 0.0f));
          this.m_frameCounter_transform.set_position(this.m_camera.ViewportToWorldPoint(new Vector3(1f, 0.0f, 100f)));
          break;
      }
    }

    public enum FpsCounterAnchorPositions
    {
      TopLeft,
      BottomLeft,
      TopRight,
      BottomRight,
    }
  }
}
