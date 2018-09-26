// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TMPro_InstructionOverlay
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace TMPro.Examples
{
  public class TMPro_InstructionOverlay : MonoBehaviour
  {
    public TMPro_InstructionOverlay.FpsCounterAnchorPositions AnchorPosition = TMPro_InstructionOverlay.FpsCounterAnchorPositions.BottomLeft;
    private const string instructions = "Camera Control - <#ffff00>Shift + RMB\n</color>Zoom - <#ffff00>Mouse wheel.";
    private TextMeshPro m_TextMeshPro;
    private TextContainer m_textContainer;
    private Transform m_frameCounter_transform;
    private Camera m_camera;

    private void Awake()
    {
      if (!this.enabled)
        return;
      this.m_camera = Camera.main;
      GameObject gameObject = new GameObject("Frame Counter");
      this.m_frameCounter_transform = gameObject.transform;
      this.m_frameCounter_transform.parent = this.m_camera.transform;
      this.m_frameCounter_transform.localRotation = Quaternion.identity;
      this.m_TextMeshPro = gameObject.AddComponent<TextMeshPro>();
      this.m_TextMeshPro.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
      this.m_TextMeshPro.fontSharedMaterial = Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Overlay");
      this.m_TextMeshPro.fontSize = 30f;
      this.m_TextMeshPro.isOverlay = true;
      this.m_textContainer = gameObject.GetComponent<TextContainer>();
      this.Set_FrameCounter_Position(this.AnchorPosition);
      this.m_TextMeshPro.text = "Camera Control - <#ffff00>Shift + RMB\n</color>Zoom - <#ffff00>Mouse wheel.";
    }

    private void Set_FrameCounter_Position(TMPro_InstructionOverlay.FpsCounterAnchorPositions anchor_position)
    {
      switch (anchor_position)
      {
        case TMPro_InstructionOverlay.FpsCounterAnchorPositions.TopLeft:
          this.m_textContainer.anchorPosition = TextContainerAnchors.TopLeft;
          this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(0.0f, 1f, 100f));
          break;
        case TMPro_InstructionOverlay.FpsCounterAnchorPositions.BottomLeft:
          this.m_textContainer.anchorPosition = TextContainerAnchors.BottomLeft;
          this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 100f));
          break;
        case TMPro_InstructionOverlay.FpsCounterAnchorPositions.TopRight:
          this.m_textContainer.anchorPosition = TextContainerAnchors.TopRight;
          this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(1f, 1f, 100f));
          break;
        case TMPro_InstructionOverlay.FpsCounterAnchorPositions.BottomRight:
          this.m_textContainer.anchorPosition = TextContainerAnchors.BottomRight;
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
