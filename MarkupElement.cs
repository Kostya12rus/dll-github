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
    RectTransform component1 = this.GetComponent<RectTransform>();
    component1.localScale = Vector3.one;
    component1.sizeDelta = this.markupStyle.size;
    component1.localPosition = (Vector3) this.markupStyle.position;
    component1.localRotation = Quaternion.Euler(Vector3.forward * this.markupStyle.rotation);
    if (this.markupStyle.mainColor != Color.clear)
    {
      this.targetPlane.enabled = true;
      this.targetPlane.color = this.markupStyle.mainColor;
      this.targetPlane.raycastTarget = this.markupStyle.raycast;
      Outline component2 = this.targetPlane.GetComponent<Outline>();
      if ((double) this.markupStyle.outlineSize > 0.0)
      {
        component2.enabled = true;
        component2.effectColor = this.markupStyle.outlineColor;
        component2.effectDistance = Vector2.one * this.markupStyle.outlineSize;
      }
      else
        component2.enabled = false;
    }
    else
      this.targetPlane.enabled = false;
    if (!string.IsNullOrEmpty(this.markupStyle.textContent))
    {
      this.targetText.enabled = true;
      this.targetText.color = this.markupStyle.textColor;
      this.targetText.raycastTarget = this.markupStyle.raycast;
      this.targetText.text = this.markupStyle.textContent;
      this.targetText.alignment = this.markupStyle.textAlignment;
      this.targetText.fontSize = this.markupStyle.fontSize;
      this.targetText.font = this.fonts[this.markupStyle.fontID];
      Outline component2 = this.targetText.GetComponent<Outline>();
      if ((double) this.markupStyle.textOutlineSize > 0.0)
      {
        component2.enabled = true;
        component2.effectColor = this.markupStyle.textOutlineColor;
        component2.effectDistance = Vector2.one * this.markupStyle.textOutlineSize;
      }
      else
        component2.enabled = false;
    }
    else
      this.targetText.enabled = false;
    if (!string.IsNullOrEmpty(this.markupStyle.imageUrl))
    {
      if ((Object) this.targetWWW != (Object) null)
        Object.DestroyImmediate((Object) this.targetWWW);
      this.targetWWW = Object.Instantiate<GameObject>(this.templateWWW, this.transform);
      RectTransform component2 = this.targetWWW.GetComponent<RectTransform>();
      component2.localScale = Vector3.one;
      component2.sizeDelta = Vector2.zero;
      component2.localPosition = (Vector3) Vector2.zero;
      component2.localRotation = Quaternion.Euler(Vector3.zero);
      component2.SetSiblingIndex(1);
      this.targetWWW.GetComponent<MarkupImageRequest>().DownloadImage(this.markupStyle.imageUrl, this.markupStyle.imageColor);
    }
    else
    {
      if (!((Object) this.targetWWW != (Object) null))
        return;
      Object.Destroy((Object) this.targetWWW);
    }
  }
}
