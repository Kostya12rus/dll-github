// Decompiled with JetBrains decompiler
// Type: PositionTrigger
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class PositionTrigger : MonoBehaviour
{
  public bool disableOnEnd;
  public int id;
  public float range;
  private GameObject ply;

  public PositionTrigger()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    if (Object.op_Equality((Object) this.ply, (Object) null))
    {
      this.ply = GameObject.FindGameObjectWithTag("Player");
    }
    else
    {
      if ((double) Vector3.Distance(this.ply.get_transform().get_position(), ((Component) this).get_transform().get_position()) > (double) this.range)
        return;
      ((TutorialManager) Object.FindObjectOfType<TutorialManager>()).Trigger(this.id);
      if (!this.disableOnEnd)
        return;
      Object.Destroy((Object) ((Component) this).get_gameObject());
    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_color(new Color(0.0f, 0.1f, 0.2f, 0.2f));
    Gizmos.DrawSphere(((Component) this).get_transform().get_position(), this.range);
  }
}
