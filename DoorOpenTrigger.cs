// Decompiled with JetBrains decompiler
// Type: DoorOpenTrigger
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class DoorOpenTrigger : MonoBehaviour
{
  public bool stageToTrigger = true;
  public Door door;
  public int id;
  public string alias;

  private void Update()
  {
    if (this.door.isOpen != this.stageToTrigger)
      return;
    if (this.alias != string.Empty)
      Object.FindObjectOfType<TutorialManager>().Trigger(this.alias);
    else
      Object.FindObjectOfType<TutorialManager>().Trigger(this.id);
    Object.Destroy((Object) this.gameObject);
  }
}
