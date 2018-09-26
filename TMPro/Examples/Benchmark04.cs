// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.Benchmark04
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace TMPro.Examples
{
  public class Benchmark04 : MonoBehaviour
  {
    public int SpawnType;
    public int MinPointSize;
    public int MaxPointSize;
    public int Steps;
    private Transform m_Transform;

    public Benchmark04()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      this.m_Transform = ((Component) this).get_transform();
      float num1 = 0.0f;
      float num2 = (float) (Screen.get_height() / 2);
      Camera.get_main().set_orthographicSize(num2);
      float num3 = num2;
      float num4 = (float) Screen.get_width() / (float) Screen.get_height();
      int minPointSize = this.MinPointSize;
      while (minPointSize <= this.MaxPointSize)
      {
        if (this.SpawnType == 0)
        {
          GameObject gameObject = new GameObject("Text - " + (object) minPointSize + " Pts");
          if ((double) num1 > (double) num3 * 2.0)
            break;
          gameObject.get_transform().set_position(Vector3.op_Addition(this.m_Transform.get_position(), new Vector3((float) ((double) num4 * -(double) num3 * 0.975000023841858), num3 * 0.975f - num1, 0.0f)));
          TextMeshPro textMeshPro = (TextMeshPro) gameObject.AddComponent<TextMeshPro>();
          ((TMP_Text) textMeshPro).get_rectTransform().set_pivot(new Vector2(0.0f, 0.5f));
          ((TMP_Text) textMeshPro).set_enableWordWrapping(false);
          ((TMP_Text) textMeshPro).set_extraPadding(true);
          ((TMP_Text) textMeshPro).set_isOrthographic(true);
          ((TMP_Text) textMeshPro).set_fontSize((float) minPointSize);
          ((TMP_Text) textMeshPro).set_text(minPointSize.ToString() + " pts - Lorem ipsum dolor sit...");
          ((Graphic) textMeshPro).set_color(Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)));
          num1 += (float) minPointSize;
        }
        minPointSize += this.Steps;
      }
    }
  }
}
