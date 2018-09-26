// Decompiled with JetBrains decompiler
// Type: GameMenuButton
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class GameMenuButton : MonoBehaviour
{
  public Vector3 normalPos;
  public Vector3 focusedPos;
  public AnimationCurve anim;
  private bool isFocused;
  private float status;
  private RectTransform rectTransform;

  private void Start()
  {
    this.rectTransform = this.GetComponent<RectTransform>();
  }

  public void Focus(bool b)
  {
    this.isFocused = b;
  }

  private void Update()
  {
    this.status += Time.deltaTime * (!this.isFocused ? -1f : 1f);
    this.status = Mathf.Clamp01(this.status);
    this.rectTransform.localPosition = this.normalPos + (this.focusedPos - this.normalPos) * this.anim.Evaluate(this.status);
  }
}
