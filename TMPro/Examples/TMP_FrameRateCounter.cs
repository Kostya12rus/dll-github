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
    public float UpdateInterval = 5f;
    public TMP_FrameRateCounter.FpsCounterAnchorPositions AnchorPosition = TMP_FrameRateCounter.FpsCounterAnchorPositions.TopRight;
    private float m_LastInterval;
    private int m_Frames;
    private string htmlColorTag;
    private const string fpsLabel = "{0:2}</color> <#8080ff>FPS \n<#FF8000>{1:2} <#8080ff>MS";
    private TextMeshPro m_TextMeshPro;
    private Transform m_frameCounter_transform;
    private Camera m_camera;
    private TMP_FrameRateCounter.FpsCounterAnchorPositions last_AnchorPosition;

    private void Awake()
    {
      if (!this.enabled)
        return;
      this.m_camera = Camera.main;
      Application.targetFrameRate = -1;
      GameObject gameObject = new GameObject("Frame Counter");
      this.m_TextMeshPro = gameObject.AddComponent<TextMeshPro>();
      this.m_TextMeshPro.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
      this.m_TextMeshPro.fontSharedMaterial = Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Overlay");
      this.m_frameCounter_transform = gameObject.transform;
      this.m_frameCounter_transform.SetParent(this.m_camera.transform);
      this.m_frameCounter_transform.localRotation = Quaternion.identity;
      this.m_TextMeshPro.enableWordWrapping = false;
      this.m_TextMeshPro.fontSize = 24f;
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

    private void Set_FrameCounter_Position(TMP_FrameRateCounter.FpsCounterAnchorPositions anchor_position)
    {
      this.m_TextMeshPro.margin = new Vector4(1f, 1f, 1f, 1f);
      switch (anchor_position)
      {
        case TMP_FrameRateCounter.FpsCounterAnchorPositions.TopLeft:
          this.m_TextMeshPro.alignment = TextAlignmentOptions.TopLeft;
          this.m_TextMeshPro.rectTransform.pivot = new Vector2(0.0f, 1f);
          this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(0.0f, 1f, 100f));
          break;
        case TMP_FrameRateCounter.FpsCounterAnchorPositions.BottomLeft:
          this.m_TextMeshPro.alignment = TextAlignmentOptions.BottomLeft;
          this.m_TextMeshPro.rectTransform.pivot = new Vector2(0.0f, 0.0f);
          this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 100f));
          break;
        case TMP_FrameRateCounter.FpsCounterAnchorPositions.TopRight:
          this.m_TextMeshPro.alignment = TextAlignmentOptions.TopRight;
          this.m_TextMeshPro.rectTransform.pivot = new Vector2(1f, 1f);
          this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(1f, 1f, 100f));
          break;
        case TMP_FrameRateCounter.FpsCounterAnchorPositions.BottomRight:
          this.m_TextMeshPro.alignment = TextAlignmentOptions.BottomRight;
          this.m_TextMeshPro.rectTransform.pivot = new Vector2(1f, 0.0f);
          this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(1f, 0.0f, 100f));
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
