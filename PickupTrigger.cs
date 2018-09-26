// Decompiled with JetBrains decompiler
// Type: PickupTrigger
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class PickupTrigger : MonoBehaviour
{
  public int filter;
  public int triggerID;
  public string alias;
  public bool disableOnEnd;
  public int prioirty;

  public PickupTrigger()
  {
    base.\u002Ector();
  }

  public bool Trigger(int item)
  {
    if (this.triggerID == -1)
    {
      Object.Destroy((Object) ((Component) this).get_gameObject());
      return true;
    }
    if (this.filter != -1 && item != this.filter)
      return false;
    if (this.alias != string.Empty)
      ((TutorialManager) Object.FindObjectOfType<TutorialManager>()).Trigger(this.alias);
    else
      ((TutorialManager) Object.FindObjectOfType<TutorialManager>()).Trigger(this.triggerID);
    if (this.disableOnEnd)
      Object.Destroy((Object) ((Component) this).get_gameObject());
    return true;
  }
}
