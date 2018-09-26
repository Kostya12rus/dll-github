// Decompiled with JetBrains decompiler
// Type: PickupTrigger
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class PickupTrigger : MonoBehaviour
{
  public int filter = -1;
  public bool disableOnEnd = true;
  public int triggerID;
  public string alias;
  public int prioirty;

  public bool Trigger(int item)
  {
    if (this.triggerID == -1)
    {
      Object.Destroy((Object) this.gameObject);
      return true;
    }
    if (this.filter != -1 && item != this.filter)
      return false;
    if (this.alias != string.Empty)
      Object.FindObjectOfType<TutorialManager>().Trigger(this.alias);
    else
      Object.FindObjectOfType<TutorialManager>().Trigger(this.triggerID);
    if (this.disableOnEnd)
      Object.Destroy((Object) this.gameObject);
    return true;
  }
}
