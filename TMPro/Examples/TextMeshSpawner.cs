// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TextMeshSpawner
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace TMPro.Examples
{
  public class TextMeshSpawner : MonoBehaviour
  {
    public int NumberOfNPC = 12;
    public int SpawnType;
    public Font TheFont;
    private TextMeshProFloatingText floatingText_Script;

    private void Awake()
    {
    }

    private void Start()
    {
      for (int index = 0; index < this.NumberOfNPC; ++index)
      {
        if (this.SpawnType == 0)
        {
          GameObject gameObject = new GameObject();
          gameObject.transform.position = new Vector3(Random.Range(-95f, 95f), 0.5f, Random.Range(-95f, 95f));
          TextMeshPro textMeshPro = gameObject.AddComponent<TextMeshPro>();
          textMeshPro.fontSize = 96f;
          textMeshPro.text = "!";
          textMeshPro.color = (Color) new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue);
          this.floatingText_Script = gameObject.AddComponent<TextMeshProFloatingText>();
          this.floatingText_Script.SpawnType = 0;
        }
        else
        {
          GameObject gameObject = new GameObject();
          gameObject.transform.position = new Vector3(Random.Range(-95f, 95f), 0.5f, Random.Range(-95f, 95f));
          TextMesh textMesh = gameObject.AddComponent<TextMesh>();
          textMesh.GetComponent<Renderer>().sharedMaterial = this.TheFont.material;
          textMesh.font = this.TheFont;
          textMesh.anchor = TextAnchor.LowerCenter;
          textMesh.fontSize = 96;
          textMesh.color = (Color) new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue);
          textMesh.text = "!";
          this.floatingText_Script = gameObject.AddComponent<TextMeshProFloatingText>();
          this.floatingText_Script.SpawnType = 1;
        }
      }
    }
  }
}
