// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TMP_UiFrameRateCounter
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace TMPro.Examples
{
  public class TMP_UiFrameRateCounter : MonoBehaviour
  {
    public float UpdateInterval;
    private float m_LastInterval;
    private int m_Frames;
    public TMP_UiFrameRateCounter.FpsCounterAnchorPositions AnchorPosition;
    private string htmlColorTag;
    private const string fpsLabel = "{0:2}</color> <#8080ff>FPS \n<#FF8000>{1:2} <#8080ff>MS";
    private TextMeshProUGUI m_TextMeshPro;
    private RectTransform m_frameCounter_transform;
    private TMP_UiFrameRateCounter.FpsCounterAnchorPositions last_AnchorPosition;

    public TMP_UiFrameRateCounter()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      if (!((Behaviour) this).get_enabled())
        return;
      Application.set_targetFrameRate(-1);
      GameObject gameObject = new GameObject("Frame Counter");
      this.m_frameCounter_transform = (RectTransform) gameObject.AddComponent<RectTransform>();
      ((Transform) this.m_frameCounter_transform).SetParent(((Component) this).get_transform(), false);
      this.m_TextMeshPro = (TextMeshProUGUI) gameObject.AddComponent<TextMeshProUGUI>();
      ((TMP_Text) this.m_TextMeshPro).set_font((TMP_FontAsset) Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF"));
      ((TMP_Text) this.m_TextMeshPro).set_fontSharedMaterial((Material) Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Overlay"));
      ((TMP_Text) this.m_TextMeshPro).set_enableWordWrapping(false);
      ((TMP_Text) this.m_TextMeshPro).set_fontSize(36f);
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

    private void Set_FrameCounter_Position(TMP_UiFrameRateCounter.FpsCounterAnchorPositions anchor_position)
    {
      switch (anchor_position)
      {
        case TMP_UiFrameRateCounter.FpsCounterAnchorPositions.TopLeft:
          ((TMP_Text) this.m_TextMeshPro).set_alignment((TextAlignmentOptions) 257);
          this.m_frameCounter_transform.set_pivot(new Vector2(0.0f, 1f));
          this.m_frameCounter_transform.set_anchorMin(new Vector2(0.01f, 0.99f));
          this.m_frameCounter_transform.set_anchorMax(new Vector2(0.01f, 0.99f));
          this.m_frameCounter_transform.set_anchoredPosition(new Vector2(0.0f, 1f));
          break;
        case TMP_UiFrameRateCounter.FpsCounterAnchorPositions.BottomLeft:
          ((TMP_Text) this.m_TextMeshPro).set_alignment((TextAlignmentOptions) 1025);
          this.m_frameCounter_transform.set_pivot(new Vector2(0.0f, 0.0f));
          this.m_frameCounter_transform.set_anchorMin(new Vector2(0.01f, 0.01f));
          this.m_frameCounter_transform.set_anchorMax(new Vector2(0.01f, 0.01f));
          this.m_frameCounter_transform.set_anchoredPosition(new Vector2(0.0f, 0.0f));
          break;
        case TMP_UiFrameRateCounter.FpsCounterAnchorPositions.TopRight:
          ((TMP_Text) this.m_TextMeshPro).set_alignment((TextAlignmentOptions) 260);
          this.m_frameCounter_transform.set_pivot(new Vector2(1f, 1f));
          this.m_frameCounter_transform.set_anchorMin(new Vector2(0.99f, 0.99f));
          this.m_frameCounter_transform.set_anchorMax(new Vector2(0.99f, 0.99f));
          this.m_frameCounter_transform.set_anchoredPosition(new Vector2(1f, 1f));
          break;
        case TMP_UiFrameRateCounter.FpsCounterAnchorPositions.BottomRight:
          ((TMP_Text) this.m_TextMeshPro).set_alignment((TextAlignmentOptions) 1028);
          this.m_frameCounter_transform.set_pivot(new Vector2(1f, 0.0f));
          this.m_frameCounter_transform.set_anchorMin(new Vector2(0.99f, 0.01f));
          this.m_frameCounter_transform.set_anchorMax(new Vector2(0.99f, 0.01f));
          this.m_frameCounter_transform.set_anchoredPosition(new Vector2(1f, 0.0f));
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
