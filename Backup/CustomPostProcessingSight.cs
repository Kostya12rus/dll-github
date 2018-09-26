// Decompiled with JetBrains decompiler
// Type: CustomPostProcessingSight
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.PostProcessing;

public class CustomPostProcessingSight : MonoBehaviour
{
  [HideInInspector]
  public WeaponManager wm;
  [HideInInspector]
  public PostProcessingBehaviour ppb;
  public GameObject canvas;
  public PostProcessingProfile targetProfile;
  public static CustomPostProcessingSight singleton;
  public static bool raycast_bool;
  public static RaycastHit raycast_hit;

  public CustomPostProcessingSight()
  {
    base.\u002Ector();
  }

  public int GetAmmoLeft()
  {
    return this.wm.AmmoLeft();
  }

  public bool IsHumanHit()
  {
    return Object.op_Equality((Object) ((Component) ((RaycastHit) ref CustomPostProcessingSight.raycast_hit).get_collider()).GetComponentInParent<CharacterClassManager>(), (Object) null);
  }
}
