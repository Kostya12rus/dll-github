// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.DemoFPSCounter
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace AmplifyBloom
{
  public class DemoFPSCounter : MonoBehaviour
  {
    public float UpdateInterval;
    private Text m_fpsText;
    private float m_accum;
    private int m_frames;
    private float m_timeleft;
    private float m_fps;
    private string m_format;

    public DemoFPSCounter()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      this.m_fpsText = (Text) ((Component) this).GetComponent<Text>();
      this.m_timeleft = this.UpdateInterval;
    }

    private void Update()
    {
      this.m_timeleft -= Time.get_deltaTime();
      this.m_accum += Time.get_timeScale() / Time.get_deltaTime();
      ++this.m_frames;
      if ((double) this.m_timeleft > 0.0)
        return;
      this.m_fps = this.m_accum / (float) this.m_frames;
      this.m_format = string.Format("{0:F2} FPS", (object) this.m_fps);
      this.m_fpsText.set_text(this.m_format);
      if ((double) this.m_fps < 50.0)
        ((Graphic) this.m_fpsText).set_color(Color.get_yellow());
      else if ((double) this.m_fps < 30.0)
        ((Graphic) this.m_fpsText).set_color(Color.get_red());
      else
        ((Graphic) this.m_fpsText).set_color(Color.get_green());
      this.m_timeleft = this.UpdateInterval;
      this.m_accum = 0.0f;
      this.m_frames = 0;
    }
  }
}
