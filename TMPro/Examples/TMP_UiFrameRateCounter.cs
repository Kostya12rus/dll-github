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
    public float UpdateInterval = 5f;
    public TMP_UiFrameRateCounter.FpsCounterAnchorPositions AnchorPosition = TMP_UiFrameRateCounter.FpsCounterAnchorPositions.TopRight;
    private float m_LastInterval;
    private int m_Frames;
    private string htmlColorTag;
    private const string fpsLabel = "{0:2}</color> <#8080ff>FPS \n<#FF8000>{1:2} <#8080ff>MS";
    private TextMeshProUGUI m_TextMeshPro;
    private RectTransform m_frameCounter_transform;
    private TMP_UiFrameRateCounter.FpsCounterAnchorPositions last_AnchorPosition;

    private void Awake()
    {
      if (!this.enabled)
        return;
      Application.targetFrameRate = -1;
      GameObject gameObject = new GameObject("Frame Counter");
      this.m_frameCounter_transform = gameObject.AddComponent<RectTransform>();
      this.m_frameCounter_transform.SetParent(this.transform, false);
      this.m_TextMeshPro = gameObject.AddComponent<TextMeshProUGUI>();
      this.m_TextMeshPro.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
      this.m_TextMeshPro.fontSharedMaterial = Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Overlay");
      this.m_TextMeshPro.enableWordWrapping = false;
      this.m_TextMeshPro.fontSize = 36f;
      this.m_TextMeshPro.isOverlay = true;
      this.Set_FrameCounter_Position(this.AnchorPosition);
      this.last_AnchorPosition = this.AnchorPosition;
    }

    private void Start()
    {
      this.m_LastInterval = Time.realtimeSinceStartup;
      this.m_Frames = 0;
    }

    private void Update()
    {
      if (this.AnchorPosition != this.last_AnchorPosition)
        this.Set_FrameCounter_Position(this.AnchorPosition);
      this.last_AnchorPosition = this.AnchorPosition;
      ++this.m_Frames;
      float realtimeSinceStartup = Time.realtimeSinceStartup;
      if ((double) realtimeSinceStartup <= (double) this.m_LastInterval + (double) this.UpdateInterval)
        return;
      float a = (float) this.m_Frames / (realtimeSinceStartup - this.m_LastInterval);
      float num = 1000f / Mathf.Max(a, 1E-05f);
      this.htmlColorTag = (double) a >= 30.0 ? ((double) a >= 10.0 ? "<color=green>" : "<color=red>") : "<color=yellow>";
      this.m_TextMeshPro.SetText(this.htmlColorTag + "{0:2}</color> <#8080ff>FPS \n<#FF8000>{1:2} <#8080ff>MS", a, num);
      this.m_Frames = 0;
      this.m_LastInterval = realtimeSinceStartup;
    }

    private void Set_FrameCounter_Position(TMP_UiFrameRateCounter.FpsCounterAnchorPositions anchor_position)
    {
      switch (anchor_position)
      {
        case TMP_UiFrameRateCounter.FpsCounterAnchorPositions.TopLeft:
          this.m_TextMeshPro.alignment = TextAlignmentOptions.TopLeft;
          this.m_frameCounter_transform.pivot = new Vector2(0.0f, 1f);
          this.m_frameCounter_transform.anchorMin = new Vector2(0.01f, 0.99f);
          this.m_frameCounter_transform.anchorMax = new Vector2(0.01f, 0.99f);
          this.m_frameCounter_transform.anchoredPosition = new Vector2(0.0f, 1f);
          break;
        case TMP_UiFrameRateCounter.FpsCounterAnchorPositions.BottomLeft:
          this.m_TextMeshPro.alignment = TextAlignmentOptions.BottomLeft;
          this.m_frameCounter_transform.pivot = new Vector2(0.0f, 0.0f);
          this.m_frameCounter_transform.anchorMin = new Vector2(0.01f, 0.01f);
          this.m_frameCounter_transform.anchorMax = new Vector2(0.01f, 0.01f);
          this.m_frameCounter_transform.anchoredPosition = new Vector2(0.0f, 0.0f);
          break;
        case TMP_UiFrameRateCounter.FpsCounterAnchorPositions.TopRight:
          this.m_TextMeshPro.alignment = TextAlignmentOptions.TopRight;
          this.m_frameCounter_transform.pivot = new Vector2(1f, 1f);
          this.m_frameCounter_transform.anchorMin = new Vector2(0.99f, 0.99f);
          this.m_frameCounter_transform.anchorMax = new Vector2(0.99f, 0.99f);
          this.m_frameCounter_transform.anchoredPosition = new Vector2(1f, 1f);
          break;
        case TMP_UiFrameRateCounter.FpsCounterAnchorPositions.BottomRight:
          this.m_TextMeshPro.alignment = TextAlignmentOptions.BottomRight;
          this.m_frameCounter_transform.pivot = new Vector2(1f, 0.0f);
          this.m_frameCounter_transform.anchorMin = new Vector2(0.99f, 0.01f);
          this.m_frameCounter_transform.anchorMax = new Vector2(0.99f, 0.01f);
          this.m_frameCounter_transform.anchoredPosition = new Vector2(1f, 0.0f);
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
