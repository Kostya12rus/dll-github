// Decompiled with JetBrains decompiler
// Type: Outside
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class Outside : MonoBehaviour
{
  private bool isOutside = true;
  private Transform listenerPos;

  private void Update()
  {
    if ((Object) this.listenerPos == (Object) null)
    {
      SpectatorCamera objectOfType = Object.FindObjectOfType<SpectatorCamera>();
      if ((Object) objectOfType != (Object) null)
        this.listenerPos = objectOfType.cam.transform;
    }
    if ((double) this.listenerPos.position.y > 900.0 && !this.isOutside)
    {
      this.isOutside = true;
      this.SetOutside(true);
    }
    if ((double) this.listenerPos.position.y >= 900.0 || !this.isOutside)
      return;
    this.isOutside = false;
    this.SetOutside(false);
  }

  private void SetOutside(bool b)
  {
    GameObject gameObject = GameObject.Find("Directional light");
    if ((Object) gameObject != (Object) null)
      gameObject.GetComponent<Light>().enabled = b;
    foreach (Camera componentsInChild in this.GetComponentsInChildren<Camera>(true))
    {
      if ((double) componentsInChild.farClipPlane == 600.0 || (double) componentsInChild.farClipPlane == 47.0)
      {
        componentsInChild.farClipPlane = !b ? 47f : 600f;
        if (componentsInChild.clearFlags <= CameraClearFlags.Color)
          componentsInChild.clearFlags = !b ? CameraClearFlags.Color : CameraClearFlags.Skybox;
      }
    }
    foreach (GlobalFog componentsInChild in this.GetComponentsInChildren<GlobalFog>(true))
      componentsInChild.startDistance = !b ? 5f : 50f;
  }
}
