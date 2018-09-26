// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.Benchmark04
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace TMPro.Examples
{
  public class Benchmark04 : MonoBehaviour
  {
    public int MinPointSize = 12;
    public int MaxPointSize = 64;
    public int Steps = 4;
    public int SpawnType;
    private Transform m_Transform;

    private void Start()
    {
      this.m_Transform = this.transform;
      float num1 = 0.0f;
      float num2 = (float) (Screen.height / 2);
      Camera.main.orthographicSize = num2;
      float num3 = num2;
      float num4 = (float) Screen.width / (float) Screen.height;
      int minPointSize = this.MinPointSize;
      while (minPointSize <= this.MaxPointSize)
      {
        if (this.SpawnType == 0)
        {
          GameObject gameObject = new GameObject("Text - " + (object) minPointSize + " Pts");
          if ((double) num1 > (double) num3 * 2.0)
            break;
          gameObject.transform.position = this.m_Transform.position + new Vector3((float) ((double) num4 * -(double) num3 * 0.975000023841858), num3 * 0.975f - num1, 0.0f);
          TextMeshPro textMeshPro = gameObject.AddComponent<TextMeshPro>();
          textMeshPro.rectTransform.pivot = new Vector2(0.0f, 0.5f);
          textMeshPro.enableWordWrapping = false;
          textMeshPro.extraPadding = true;
          textMeshPro.isOrthographic = true;
          textMeshPro.fontSize = (float) minPointSize;
          textMeshPro.text = minPointSize.ToString() + " pts - Lorem ipsum dolor sit...";
          textMeshPro.color = (Color) new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
          num1 += (float) minPointSize;
        }
        minPointSize += this.Steps;
      }
    }
  }
}
