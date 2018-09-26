// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.Benchmark03
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace TMPro.Examples
{
  public class Benchmark03 : MonoBehaviour
  {
    public int NumberOfNPC = 12;
    public int SpawnType;
    public Font TheFont;

    private void Awake()
    {
    }

    private void Start()
    {
      for (int index = 0; index < this.NumberOfNPC; ++index)
      {
        if (this.SpawnType == 0)
        {
          TextMeshPro textMeshPro = new GameObject() { transform = { position = new Vector3(0.0f, 0.0f, 0.0f) } }.AddComponent<TextMeshPro>();
          textMeshPro.alignment = TextAlignmentOptions.Center;
          textMeshPro.fontSize = 96f;
          textMeshPro.text = "@";
          textMeshPro.color = (Color) new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue);
        }
        else
        {
          TextMesh textMesh = new GameObject() { transform = { position = new Vector3(0.0f, 0.0f, 0.0f) } }.AddComponent<TextMesh>();
          textMesh.GetComponent<Renderer>().sharedMaterial = this.TheFont.material;
          textMesh.font = this.TheFont;
          textMesh.anchor = TextAnchor.MiddleCenter;
          textMesh.fontSize = 96;
          textMesh.color = (Color) new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue);
          textMesh.text = "@";
        }
      }
    }
  }
}
