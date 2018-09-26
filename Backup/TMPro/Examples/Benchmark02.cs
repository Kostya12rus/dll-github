// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.Benchmark02
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace TMPro.Examples
{
  public class Benchmark02 : MonoBehaviour
  {
    public int SpawnType;
    public int NumberOfNPC;
    private TextMeshProFloatingText floatingText_Script;

    public Benchmark02()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      for (int index = 0; index < this.NumberOfNPC; ++index)
      {
        if (this.SpawnType == 0)
        {
          GameObject gameObject = new GameObject();
          gameObject.get_transform().set_position(new Vector3(Random.Range(-95f, 95f), 0.25f, Random.Range(-95f, 95f)));
          TextMeshPro textMeshPro = (TextMeshPro) gameObject.AddComponent<TextMeshPro>();
          ((TMP_Text) textMeshPro).set_autoSizeTextContainer(true);
          ((TMP_Text) textMeshPro).get_rectTransform().set_pivot(new Vector2(0.5f, 0.0f));
          ((TMP_Text) textMeshPro).set_alignment((TextAlignmentOptions) 1026);
          ((TMP_Text) textMeshPro).set_fontSize(96f);
          ((TMP_Text) textMeshPro).set_enableKerning(false);
          ((Graphic) textMeshPro).set_color(Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue)));
          ((TMP_Text) textMeshPro).set_text("!");
          this.floatingText_Script = (TextMeshProFloatingText) gameObject.AddComponent<TextMeshProFloatingText>();
          this.floatingText_Script.SpawnType = 0;
        }
        else if (this.SpawnType == 1)
        {
          GameObject gameObject = new GameObject();
          gameObject.get_transform().set_position(new Vector3(Random.Range(-95f, 95f), 0.25f, Random.Range(-95f, 95f)));
          TextMesh textMesh = (TextMesh) gameObject.AddComponent<TextMesh>();
          textMesh.set_font((Font) Resources.Load<Font>("Fonts/ARIAL"));
          ((Renderer) ((Component) textMesh).GetComponent<Renderer>()).set_sharedMaterial(textMesh.get_font().get_material());
          textMesh.set_anchor((TextAnchor) 7);
          textMesh.set_fontSize(96);
          textMesh.set_color(Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue)));
          textMesh.set_text("!");
          this.floatingText_Script = (TextMeshProFloatingText) gameObject.AddComponent<TextMeshProFloatingText>();
          this.floatingText_Script.SpawnType = 1;
        }
        else if (this.SpawnType == 2)
        {
          GameObject gameObject = new GameObject();
          ((Canvas) gameObject.AddComponent<Canvas>()).set_worldCamera(Camera.get_main());
          gameObject.get_transform().set_localScale(new Vector3(0.1f, 0.1f, 0.1f));
          gameObject.get_transform().set_position(new Vector3(Random.Range(-95f, 95f), 5f, Random.Range(-95f, 95f)));
          TextMeshProUGUI textMeshProUgui = (TextMeshProUGUI) new GameObject().AddComponent<TextMeshProUGUI>();
          ((Transform) ((TMP_Text) textMeshProUgui).get_rectTransform()).SetParent(gameObject.get_transform(), false);
          ((Graphic) textMeshProUgui).set_color(Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, (byte) 0, byte.MaxValue)));
          ((TMP_Text) textMeshProUgui).set_alignment((TextAlignmentOptions) 1026);
          ((TMP_Text) textMeshProUgui).set_fontSize(96f);
          ((TMP_Text) textMeshProUgui).set_text("!");
          this.floatingText_Script = (TextMeshProFloatingText) gameObject.AddComponent<TextMeshProFloatingText>();
          this.floatingText_Script.SpawnType = 0;
        }
      }
    }
  }
}
