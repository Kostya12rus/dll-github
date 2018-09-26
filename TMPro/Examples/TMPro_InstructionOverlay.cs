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
    public TMPro_InstructionOverlay.FpsCounterAnchorPositions AnchorPosition;
    private const string instructions = "Camera Control - <#ffff00>Shift + RMB\n</color>Zoom - <#ffff00>Mouse wheel.";
    private TextMeshPro m_TextMeshPro;
    private TextContainer m_textContainer;
    private Transform m_frameCounter_transform;
    private Camera m_camera;

    public TMPro_InstructionOverlay()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      if (!((Behaviour) this).get_enabled())
        return;
      this.m_camera = Camera.get_main();
      GameObject gameObject = new GameObject("Frame Counter");
      this.m_frameCounter_transform = gameObject.get_transform();
      this.m_frameCounter_transform.set_parent(((Component) this.m_camera).get_transform());
      this.m_frameCounter_transform.set_localRotation(Quaternion.get_identity());
      this.m_TextMeshPro = (TextMeshPro) gameObject.AddComponent<TextMeshPro>();
      ((TMP_Text) this.m_TextMeshPro).set_font((TMP_FontAsset) Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF"));
      ((TMP_Text) this.m_TextMeshPro).set_fontSharedMaterial((Material) Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Overlay"));
      ((TMP_Text) this.m_TextMeshPro).set_fontSize(30f);
      ((TMP_Text) this.m_TextMeshPro).set_isOverlay(true);
      this.m_textContainer = (TextContainer) gameObject.GetComponent<TextContainer>();
      this.Set_FrameCounter_Position(this.AnchorPosition);
      ((TMP_Text) this.m_TextMeshPro).set_text("Camera Control - <#ffff00>Shift + RMB\n</color>Zoom - <#ffff00>Mouse wheel.");
    }

    private void Set_FrameCounter_Position(TMPro_InstructionOverlay.FpsCounterAnchorPositions anchor_position)
    {
      switch (anchor_position)
      {
        case TMPro_InstructionOverlay.FpsCounterAnchorPositions.TopLeft:
          this.m_textContainer.set_anchorPosition((TextContainerAnchors) 0);
          this.m_frameCounter_transform.set_position(this.m_camera.ViewportToWorldPoint(new Vector3(0.0f, 1f, 100f)));
          break;
        case TMPro_InstructionOverlay.FpsCounterAnchorPositions.BottomLeft:
          this.m_textContainer.set_anchorPosition((TextContainerAnchors) 6);
          this.m_frameCounter_transform.set_position(this.m_camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 100f)));
          break;
        case TMPro_InstructionOverlay.FpsCounterAnchorPositions.TopRight:
          this.m_textContainer.set_anchorPosition((TextContainerAnchors) 2);
          this.m_frameCounter_transform.set_position(this.m_camera.ViewportToWorldPoint(new Vector3(1f, 1f, 100f)));
          break;
        case TMPro_InstructionOverlay.FpsCounterAnchorPositions.BottomRight:
          this.m_textContainer.set_anchorPosition((TextContainerAnchors) 8);
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
