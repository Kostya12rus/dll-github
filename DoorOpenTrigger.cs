// Decompiled with JetBrains decompiler
// Type: DoorOpenTrigger
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class DoorOpenTrigger : MonoBehaviour
{
  public Door door;
  public bool stageToTrigger;
  public int id;
  public string alias;

  public DoorOpenTrigger()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    if (this.door.isOpen != this.stageToTrigger)
      return;
    if (this.alias != string.Empty)
      ((TutorialManager) Object.FindObjectOfType<TutorialManager>()).Trigger(this.alias);
    else
      ((TutorialManager) Object.FindObjectOfType<TutorialManager>()).Trigger(this.id);
    Object.Destroy((Object) ((Component) this).get_gameObject());
  }
}
