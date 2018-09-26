// Decompiled with JetBrains decompiler
// Type: Outside
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class Outside : MonoBehaviour
{
  private bool isOutside;
  private Transform listenerPos;

  public Outside()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    if (Object.op_Equality((Object) this.listenerPos, (Object) null))
    {
      SpectatorCamera objectOfType = (SpectatorCamera) Object.FindObjectOfType<SpectatorCamera>();
      if (Object.op_Inequality((Object) objectOfType, (Object) null))
        this.listenerPos = ((Component) objectOfType.cam).get_transform();
    }
    if (this.listenerPos.get_position().y > 900.0 && !this.isOutside)
    {
      this.isOutside = true;
      this.SetOutside(true);
    }
    if (this.listenerPos.get_position().y >= 900.0 || !this.isOutside)
      return;
    this.isOutside = false;
    this.SetOutside(false);
  }

  private void SetOutside(bool b)
  {
    GameObject gameObject = GameObject.Find("Directional light");
    if (Object.op_Inequality((Object) gameObject, (Object) null))
      ((Behaviour) gameObject.GetComponent<Light>()).set_enabled(b);
    foreach (Camera componentsInChild in (Camera[]) ((Component) this).GetComponentsInChildren<Camera>(true))
    {
      if ((double) componentsInChild.get_farClipPlane() == 600.0 || (double) componentsInChild.get_farClipPlane() == 47.0)
      {
        componentsInChild.set_farClipPlane(!b ? 47f : 600f);
        if (componentsInChild.get_clearFlags() <= 2)
          componentsInChild.set_clearFlags(!b ? (CameraClearFlags) 2 : (CameraClearFlags) 1);
      }
    }
    foreach (GlobalFog componentsInChild in (GlobalFog[]) ((Component) this).GetComponentsInChildren<GlobalFog>(true))
      componentsInChild.startDistance = !b ? (__Null) 5.0 : (__Null) 50.0;
  }
}
