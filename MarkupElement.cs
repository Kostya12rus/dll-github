// Decompiled with JetBrains decompiler
// Type: MarkupElement
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class MarkupElement : MonoBehaviour
{
  public MarkupStyle markupStyle;
  private string curTag;
  [Space]
  public Image targetPlane;
  public Text targetText;
  private GameObject targetWWW;
  public GameObject templateWWW;
  public Font[] fonts;

  public MarkupElement()
  {
    base.\u002Ector();
  }

  private void Update()
  {
  }

  public void RefreshStyle(string tagName)
  {
    this.curTag = tagName;
    MarkupStyle markupStyle = this.markupStyle;
    foreach (MarkupReader.TagStyleRelation relation in MarkupReader.singleton.relations)
    {
      if (relation.tag == tagName.ToLower())
        markupStyle = relation.style;
    }
    markupStyle.position = this.markupStyle.position;
    markupStyle.rotation = this.markupStyle.rotation;
    markupStyle.size = this.markupStyle.size;
    this.markupStyle = markupStyle;
    RectTransform component1 = (RectTransform) ((Component) this).GetComponent<RectTransform>();
    ((Transform) component1).set_localScale(Vector3.get_one());
    component1.set_sizeDelta(this.markupStyle.size);
    ((Transform) component1).set_localPosition(Vector2.op_Implicit(this.markupStyle.position));
    ((Transform) component1).set_localRotation(Quaternion.Euler(Vector3.op_Multiply(Vector3.get_forward(), this.markupStyle.rotation)));
    if (Color.op_Inequality(this.markupStyle.mainColor, Color.get_clear()))
    {
      ((Behaviour) this.targetPlane).set_enabled(true);
      ((Graphic) this.targetPlane).set_color(this.markupStyle.mainColor);
      ((Graphic) this.targetPlane).set_raycastTarget(this.markupStyle.raycast);
      Outline component2 = (Outline) ((Component) this.targetPlane).GetComponent<Outline>();
      if ((double) this.markupStyle.outlineSize > 0.0)
      {
        ((Behaviour) component2).set_enabled(true);
        ((Shadow) component2).set_effectColor(this.markupStyle.outlineColor);
        ((Shadow) component2).set_effectDistance(Vector2.op_Multiply(Vector2.get_one(), this.markupStyle.outlineSize));
      }
      else
        ((Behaviour) component2).set_enabled(false);
    }
    else
      ((Behaviour) this.targetPlane).set_enabled(false);
    if (!string.IsNullOrEmpty(this.markupStyle.textContent))
    {
      ((Behaviour) this.targetText).set_enabled(true);
      ((Graphic) this.targetText).set_color(this.markupStyle.textColor);
      ((Graphic) this.targetText).set_raycastTarget(this.markupStyle.raycast);
      this.targetText.set_text(this.markupStyle.textContent);
      this.targetText.set_alignment(this.markupStyle.textAlignment);
      this.targetText.set_fontSize(this.markupStyle.fontSize);
      this.targetText.set_font(this.fonts[this.markupStyle.fontID]);
      Outline component2 = (Outline) ((Component) this.targetText).GetComponent<Outline>();
      if ((double) this.markupStyle.textOutlineSize > 0.0)
      {
        ((Behaviour) component2).set_enabled(true);
        ((Shadow) component2).set_effectColor(this.markupStyle.textOutlineColor);
        ((Shadow) component2).set_effectDistance(Vector2.op_Multiply(Vector2.get_one(), this.markupStyle.textOutlineSize));
      }
      else
        ((Behaviour) component2).set_enabled(false);
    }
    else
      ((Behaviour) this.targetText).set_enabled(false);
    if (!string.IsNullOrEmpty(this.markupStyle.imageUrl))
    {
      if (Object.op_Inequality((Object) this.targetWWW, (Object) null))
        Object.DestroyImmediate((Object) this.targetWWW);
      this.targetWWW = (GameObject) Object.Instantiate<GameObject>((M0) this.templateWWW, ((Component) this).get_transform());
      RectTransform component2 = (RectTransform) this.targetWWW.GetComponent<RectTransform>();
      ((Transform) component2).set_localScale(Vector3.get_one());
      component2.set_sizeDelta(Vector2.get_zero());
      ((Transform) component2).set_localPosition(Vector2.op_Implicit(Vector2.get_zero()));
      ((Transform) component2).set_localRotation(Quaternion.Euler(Vector3.get_zero()));
      ((Transform) component2).SetSiblingIndex(1);
      ((MarkupImageRequest) this.targetWWW.GetComponent<MarkupImageRequest>()).DownloadImage(this.markupStyle.imageUrl, this.markupStyle.imageColor);
    }
    else
    {
      if (!Object.op_Inequality((Object) this.targetWWW, (Object) null))
        return;
      Object.Destroy((Object) this.targetWWW);
    }
  }
}
