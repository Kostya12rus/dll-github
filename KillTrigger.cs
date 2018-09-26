// Decompiled with JetBrains decompiler
// Type: KillTrigger
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class KillTrigger : MonoBehaviour
{
  public bool disableOnEnd = true;
  public int killsToTrigger;
  public int triggerID;
  public string alias;
  public int prioirty;

  public void Trigger(int amount)
  {
    if (amount != this.killsToTrigger)
      return;
    if (this.triggerID == -1)
      Object.FindObjectOfType<TutorialManager>().Tutorial2_Result();
    else if (this.alias != string.Empty)
      Object.FindObjectOfType<TutorialManager>().Trigger(this.alias);
    else
      Object.FindObjectOfType<TutorialManager>().Trigger(this.triggerID);
    if (!this.disableOnEnd)
      return;
    Object.Destroy((Object) this.gameObject);
  }
}
