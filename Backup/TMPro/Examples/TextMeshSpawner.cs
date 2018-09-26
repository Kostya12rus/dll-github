// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.TextMeshSpawner
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace TMPro.Examples
{
  public class TextMeshSpawner : MonoBehaviour
  {
    public int SpawnType;
    public int NumberOfNPC;
    public Font TheFont;
    private TextMeshProFloatingText floatingText_Script;

    public TextMeshSpawner()
    {
      base.\u002Ector();
    }

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
          gameObject.get_transform().set_position(new Vector3(Random.Range(-95f, 95f), 0.5f, Random.Range(-95f, 95f)));
          TextMeshPro textMeshPro = (TextMeshPro) gameObject.AddComponent<TextMeshPro>();
          ((TMP_Text) textMeshPro).set_fontSize(96f);
          ((TMP_Text) textMeshPro).set_text("!");
          ((Graphic) textMeshPro).set_color(Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue)));
          this.floatingText_Script = (TextMeshProFloatingText) gameObject.AddComponent<TextMeshProFloatingText>();
          this.floatingText_Script.SpawnType = 0;
        }
        else
        {
          GameObject gameObject = new GameObject();
          gameObject.get_transform().set_position(new Vector3(Random.Range(-95f, 95f), 0.5f, Random.Range(-95f, 95f)));
          TextMesh textMesh = (TextMesh) gameObject.AddComponent<TextMesh>();
          ((Renderer) ((Component) textMesh).GetComponent<Renderer>()).set_sharedMaterial(this.TheFont.get_material());
          textMesh.set_font(this.TheFont);
          textMesh.set_anchor((TextAnchor) 7);
          textMesh.set_fontSize(96);
          textMesh.set_color(Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue)));
          textMesh.set_text("!");
          this.floatingText_Script = (TextMeshProFloatingText) gameObject.AddComponent<TextMeshProFloatingText>();
          this.floatingText_Script.SpawnType = 1;
        }
      }
    }
  }
}
