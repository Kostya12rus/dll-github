// Decompiled with JetBrains decompiler
// Type: WeaponManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using MEC;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PostProcessing;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class WeaponManager : NetworkBehaviour
{
  private static int kCmdCmdSyncMods = -407134179;
  [SyncVar]
  private bool friendlyFire;
  [SyncVar(hook = "HookCurWeapon")]
  public int curWeapon;
  [SyncVar]
  private int sync_sight;
  [SyncVar]
  private int sync_barrel;
  [SyncVar]
  private int sync_other;
  [SyncVar]
  private bool sync_flashlight;
  private CharacterClassManager ccm;
  private BloodDrawer drawer;
  private Inventory inv;
  private AmmoBox abox;
  private WeaponShootAnimation weaponShootAnimation;
  private FirstPersonController fpc;
  private AnimationController animationController;
  private KeyCode kc_fire;
  private KeyCode kc_reload;
  private KeyCode kc_zoom;
  private float fireCooldown;
  private float reloadCooldown;
  private float zoomCooldown;
  public float normalFov;
  public Transform camera;
  public Transform weaponInventoryGroup;
  public Camera weaponModelCamera;
  public float fovAdjustingSpeed;
  public bool zoomed;
  public AnimationCurve viewBob;
  public float overallDamagerFactor;
  public LayerMask raycastMask;
  public LayerMask bloodMask;
  public HitboxIdentity[] hitboxes;
  public WeaponManager.Weapon[] weapons;
  public WeaponLaser laserLight;
  public bool flashlightEnabled;
  public bool forceSyncModsNextFrame;
  private int prevSyncWeapon;
  private static int kCmdCmdShoot;
  private static int kCmdCmdReload;
  private static int kRpcRpcPlaceDecal;
  private static int kRpcRpcConfirmShot;
  private static int kRpcRpcReload;

  public WeaponManager()
  {
    base.\u002Ector();
  }

  private void HookCurWeapon(int i)
  {
    this.NetworkcurWeapon = i;
  }

  private void Start()
  {
    this.abox = (AmmoBox) ((Component) this).GetComponent<AmmoBox>();
    this.fpc = (FirstPersonController) ((Component) this).GetComponent<FirstPersonController>();
    this.inv = (Inventory) ((Component) this).GetComponent<Inventory>();
    this.animationController = (AnimationController) ((Component) this).GetComponent<AnimationController>();
    this.weaponShootAnimation = (WeaponShootAnimation) ((Component) this).GetComponentInChildren<WeaponShootAnimation>();
    this.drawer = (BloodDrawer) ((Component) this).GetComponent<BloodDrawer>();
    this.NetworkfriendlyFire = ConfigFile.ServerConfig.GetBool("friendly_fire", false);
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this.kc_fire = NewInput.GetKey("Shoot");
    this.kc_reload = NewInput.GetKey("Reload");
    this.kc_zoom = NewInput.GetKey("Zoom");
    if (this.get_isLocalPlayer())
    {
      for (int index = 0; index < this.weapons.Length; ++index)
        this.weapons[index].SetMods(PlayerPrefs.GetInt("Weapon_" + (object) this.weapons[index].inventoryID + "_sight", 0), PlayerPrefs.GetInt("Weapon_" + (object) this.weapons[index].inventoryID + "_barrel", 0), PlayerPrefs.GetInt("Weapon_" + (object) this.weapons[index].inventoryID + "_other", 0), true, this.flashlightEnabled);
    }
    else
      Object.Destroy((Object) ((Component) this.weaponModelCamera).get_gameObject());
  }

  private void Update()
  {
    this.DeductCooldown();
    if (!this.get_isLocalPlayer())
      return;
    this.CheckForInput();
    this.DoLaserStuff();
    this.UpdateFov();
    this.SetupCameras();
    this.RefreshPositions();
  }

  private void LateUpdate()
  {
    this.SyncMods();
    if (!this.get_isLocalPlayer() || Cursor.get_visible() || (!Input.GetKeyDown(NewInput.GetKey("Toggle flashlight")) || this.curWeapon < 0) || !this.weapons[this.curWeapon].mod_others[this.weapons[this.curWeapon].GetMod(ModPrefab.ModType.Other)].name.ToLower().Contains("flashlight"))
      return;
    this.flashlightEnabled = !this.flashlightEnabled;
  }

  private void SyncMods()
  {
    if (this.curWeapon < 0)
      return;
    int mod1 = this.weapons[this.curWeapon].GetMod(ModPrefab.ModType.Sight);
    int mod2 = this.weapons[this.curWeapon].GetMod(ModPrefab.ModType.Barrel);
    int mod3 = this.weapons[this.curWeapon].GetMod(ModPrefab.ModType.Other);
    if (!this.forceSyncModsNextFrame && this.prevSyncWeapon == this.curWeapon && (this.sync_sight == mod1 && this.sync_barrel == mod2) && (this.sync_other == mod3 && this.flashlightEnabled == this.sync_flashlight))
      return;
    if (this.get_isLocalPlayer())
    {
      this.CallCmdSyncMods(mod1, mod2, mod3, this.flashlightEnabled);
      this.weapons[this.curWeapon].RefreshMods(false, this.flashlightEnabled);
    }
    else
    {
      this.flashlightEnabled = this.sync_flashlight;
      this.weapons[this.curWeapon].SetMods(this.sync_sight, this.sync_barrel, this.sync_other, false, this.flashlightEnabled);
    }
    this.prevSyncWeapon = this.curWeapon;
  }

  private void DeductCooldown()
  {
    if ((double) this.fireCooldown >= 0.0)
      this.fireCooldown -= Time.get_deltaTime();
    if ((double) this.reloadCooldown >= 0.0)
      this.reloadCooldown -= Time.get_deltaTime();
    if ((double) this.zoomCooldown < 0.0)
      return;
    this.zoomCooldown -= Time.get_deltaTime();
  }

  [ClientCallback]
  private void DoLaserStuff()
  {
    if (!NetworkClient.get_active())
      return;
    if (this.curWeapon < 0)
    {
      this.laserLight.forwardDirection = (GameObject) null;
    }
    else
    {
      this.laserLight.maxAngle = this.weapons[this.curWeapon].maxAngleLaser;
      this.laserLight.forwardDirection = this.weapons[this.curWeapon].allEffects.laserDirection;
    }
  }

  [ClientCallback]
  private void UpdateFov()
  {
    if (!NetworkClient.get_active())
      return;
    float num1 = this.normalFov;
    float num2 = this.normalFov - 10f;
    bool flag = this.curWeapon >= 0 && Object.op_Inequality((Object) this.weapons[this.curWeapon].allEffects.customProfile, (Object) null);
    if (this.curWeapon >= 0 && this.zoomed && !flag)
    {
      num1 = this.weapons[this.curWeapon].allEffects.zoomFov;
      num2 = this.weapons[this.curWeapon].allEffects.weaponCameraFov;
    }
    Camera component = (Camera) ((Component) this.camera).GetComponent<Camera>();
    component.set_fieldOfView(!flag ? Mathf.Lerp(component.get_fieldOfView(), num1, Time.get_deltaTime() * this.fovAdjustingSpeed) : num1);
    this.weaponModelCamera.set_fieldOfView(!flag ? Mathf.Lerp(this.weaponModelCamera.get_fieldOfView(), num2, Time.get_deltaTime() * this.fovAdjustingSpeed) : num2);
  }

  [ClientCallback]
  private void RefreshPositions()
  {
    if (!NetworkClient.get_active() || this.curWeapon < 0)
      return;
    Vector3 positionOffset = this.weapons[this.curWeapon].positionOffset;
    Vector3 vector3_1;
    if (this.zoomed)
    {
      vector3_1 = Vector3.op_Addition(positionOffset, this.weapons[this.curWeapon].allEffects.zoomPositionOffset);
    }
    else
    {
      Vector3 vector3_2 = positionOffset;
      Vector3 localPosition = ((Component) this.camera).get_transform().get_localPosition();
      AnimationCurve viewBob = this.viewBob;
      Vector3 vector3_3;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ((Vector3) ref vector3_3).\u002Ector((float) (^(Vector3&) ref this.fpc.m_MoveDir).x, 0.0f, (float) (^(Vector3&) ref this.fpc.m_MoveDir).z);
      double magnitude = (double) ((Vector3) ref vector3_3).get_magnitude();
      double num = (double) viewBob.Evaluate((float) magnitude) * (double) this.weapons[this.curWeapon].bobAnimationScale;
      Vector3 vector3_4 = Vector3.op_Multiply(localPosition, (float) num);
      vector3_1 = Vector3.op_Addition(vector3_2, vector3_4);
    }
    this.weaponInventoryGroup.set_localPosition(Vector3.Lerp(this.weaponInventoryGroup.get_localPosition(), vector3_1, Time.get_deltaTime() * 8f));
  }

  [ClientCallback]
  private void SetZoom(bool b)
  {
    if (!NetworkClient.get_active())
      return;
    bool flag = false;
    if (this.curWeapon >= 0 && this.weapons[this.curWeapon].allowZoom && (double) Inventory.inventoryCooldown <= (!this.inv.WeaponReadyToInstantPickup() ? -(double) this.weapons[this.curWeapon].timeToPickup : 0.0))
    {
      if (b != this.zoomed && (double) this.fireCooldown <= 0.0)
      {
        this.fireCooldown += this.weapons[this.curWeapon].zoomingTime;
        this.zoomCooldown = this.weapons[this.curWeapon].zoomingTime;
        this.zoomed = b;
        flag = true;
      }
    }
    else if (this.zoomed)
    {
      flag = true;
      this.zoomed = false;
    }
    if (this.curWeapon >= 0)
    {
      if (!flag)
        return;
      ((Animator) this.inv.availableItems[this.inv.curItem].firstpersonModel.GetComponent<Animator>()).SetBool("Zoomed", this.zoomed);
      this.fpc.zoomSlowdown = !this.zoomed ? (__Null) 1.0 : (__Null) (double) this.weapons[this.curWeapon].allEffects.zoomSlowdown;
    }
    else
      this.fpc.zoomSlowdown = (__Null) 1.0;
  }

  public int AmmoLeft()
  {
    if (this.curWeapon >= 0)
      return (int) ((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(this.inv.GetItemIndex()).durability;
    return -1;
  }

  [ClientCallback]
  private void SetupCameras()
  {
    if (!NetworkClient.get_active())
      return;
    ((MouseLook) this.fpc.m_MouseLook).sensitivityMultiplier = (__Null) 1.0;
    if (this.ccm.curClass < 0)
      return;
    this.weaponModelCamera.set_nearClipPlane(0.01f);
    PostProcessingBehaviour component = (PostProcessingBehaviour) ((Component) this.weaponModelCamera).GetComponent<PostProcessingBehaviour>();
    component.profile = this.ccm.klasy[this.ccm.curClass].postprocessingProfile;
    if (this.curWeapon >= 0)
    {
      if (Object.op_Inequality((Object) this.weapons[this.curWeapon].allEffects.counterText, (Object) null))
        this.weapons[this.curWeapon].allEffects.counterText.set_text(string.Format(this.weapons[this.curWeapon].allEffects.counterTemplate, (object) this.AmmoLeft(), (object) this.weapons[this.curWeapon].maxAmmo, (object) this.abox.GetAmmo(this.weapons[this.curWeapon].ammoType)).Replace("\\n", Environment.NewLine));
      if (this.zoomed && (double) this.zoomCooldown <= 0.0)
      {
        ((MouseLook) this.fpc.m_MouseLook).sensitivityMultiplier = (__Null) (double) this.weapons[this.curWeapon].allEffects.zoomSensitivity;
        if (Object.op_Inequality((Object) this.weapons[this.curWeapon].allEffects.customProfile, (Object) null))
        {
          component.profile = this.weapons[this.curWeapon].allEffects.customProfile;
          ((Camera) ((Component) this.camera).GetComponent<Camera>()).set_fieldOfView(this.weapons[this.curWeapon].allEffects.zoomFov);
          this.weaponModelCamera.set_nearClipPlane(3.5f);
        }
      }
    }
    Inventory.targetCrosshairAlpha = this.zoomed || this.curWeapon >= 0 && Object.op_Inequality((Object) this.weapons[this.curWeapon].allEffects.laserDirection, (Object) null) ? 0.0f : 1f;
  }

  [ClientCallback]
  private void CheckForInput()
  {
    if (!NetworkClient.get_active())
      return;
    if (!Cursor.get_visible() && (double) Inventory.inventoryCooldown <= 0.0 && (double) this.fireCooldown <= 0.0 && ((double) this.reloadCooldown <= 0.0 || this.zoomed))
      this.SetZoom(Input.GetKey(NewInput.GetKey("Zoom")));
    if (this.curWeapon < 0 || (double) this.reloadCooldown > 0.0 || Cursor.get_visible() || ((double) Inventory.inventoryCooldown > (!this.inv.WeaponReadyToInstantPickup() ? -(double) this.weapons[this.curWeapon].timeToPickup : 0.0) || (double) this.fireCooldown > 0.0))
      return;
    if ((!this.weapons[this.curWeapon].allowFullauto ? (Input.GetKeyDown(this.kc_fire) ? 1 : 0) : (Input.GetKey(this.kc_fire) ? 1 : 0)) != 0)
    {
      this.Shoot();
    }
    else
    {
      if (!Input.GetKey(this.kc_reload))
        return;
      Timing.RunCoroutine(this._Reload(), (Segment) 1);
    }
  }

  [ClientCallback]
  private void Shoot()
  {
    if (!NetworkClient.get_active() || (double) ((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(this.inv.GetItemIndex()).durability == 0.0)
      return;
    this.fireCooldown = (float) (1.0 / ((double) this.weapons[this.curWeapon].shotsPerSecond * (double) this.weapons[this.curWeapon].allEffects.firerateMultiplier));
    Animator component1 = (Animator) this.inv.availableItems[this.inv.curItem].firstpersonModel.GetComponent<Animator>();
    if (this.zoomed)
    {
      foreach (string customZoomshotAnim in this.weapons[this.curWeapon].customZoomshotAnims)
        component1.Play(customZoomshotAnim, 0, 0.0f);
    }
    else
    {
      foreach (string customShootAnim in this.weapons[this.curWeapon].customShootAnims)
        component1.Play(customShootAnim, 0, 0.0f);
    }
    this.animationController.gunSource.PlayOneShot(this.weapons[this.curWeapon].allEffects.shootSound);
    M0 component2 = ((Component) this.camera).GetComponent<Camera>();
    ((Camera) component2).set_fieldOfView(((Camera) component2).get_fieldOfView() - this.weapons[this.curWeapon].recoilAnimation * this.weapons[this.curWeapon].recoil.fovKick);
    this.weapons[this.curWeapon].PlayMuzzleFlashes(true);
    this.weapons[this.curWeapon].husks.Play();
    Vector3 vector3 = ((Component) this.camera).get_transform().get_forward();
    if (!this.zoomed)
      vector3 = Quaternion.op_Multiply(Quaternion.Euler(Vector3.op_Multiply(new Vector3((float) Random.Range(-1, 1), (float) Random.Range(-1, 1), (float) Random.Range(-1, 1)), (float) ((double) this.weapons[this.curWeapon].unfocusedSpread * (double) this.weapons[this.curWeapon].allEffects.unfocusedSpread / 30.0))), vector3);
    Ray ray;
    ((Ray) ref ray).\u002Ector(((Component) this.camera).get_transform().get_position(), vector3);
    RaycastHit raycastHit;
    if (!Physics.Raycast(ray, ref raycastHit, 500f, LayerMask.op_Implicit(this.raycastMask)))
    {
      this.DoRecoil();
      this.CallCmdShoot((GameObject) null, string.Empty, Vector3.get_zero());
    }
    else
    {
      BreakableWindow component3 = (BreakableWindow) ((Component) ((RaycastHit) ref raycastHit).get_collider()).GetComponent<BreakableWindow>();
      HitboxIdentity component4 = (HitboxIdentity) ((Component) ((RaycastHit) ref raycastHit).get_collider()).GetComponent<HitboxIdentity>();
      if (Object.op_Inequality((Object) component4, (Object) null))
      {
        this.DoRecoil();
        this.CallCmdShoot(((Component) ((Component) component4).GetComponentInParent<NetworkIdentity>()).get_gameObject(), component4.id, ((Ray) ref ray).get_direction());
      }
      else if (Object.op_Inequality((Object) component3, (Object) null))
      {
        this.DoRecoil();
        this.CallCmdShoot(((Component) ((Component) component3).GetComponent<NetworkIdentity>()).get_gameObject(), "window", ((Ray) ref ray).get_direction());
      }
      else
      {
        this.DoRecoil();
        this.CallCmdShoot((GameObject) null, "hole", ((Ray) ref ray).get_direction());
      }
    }
  }

  private void DoRecoil()
  {
    this.weaponShootAnimation.Recoil(this.weapons[this.curWeapon].recoilAnimation * (!this.zoomed ? 1f : this.weapons[this.curWeapon].allEffects.zoomRecoilAnimScale));
    Recoil.StaticDoRecoil(this.weapons[this.curWeapon].recoil, this.weapons[this.curWeapon].allEffects.overallRecoilReduction * (!this.zoomed ? 1f : this.weapons[this.curWeapon].allEffects.zoomRecoilReduction));
  }

  [Command]
  private void CmdSyncMods(int _s, int _b, int _o, bool _flashlight)
  {
    this.Networksync_sight = _s;
    this.Networksync_barrel = _b;
    this.Networksync_other = _o;
    this.Networksync_flashlight = _flashlight;
  }

  [Command]
  private void CmdShoot(GameObject target, string hitboxType, Vector3 dir)
  {
    if (this.curWeapon < 0 || ((double) this.reloadCooldown > 0.0 || (double) this.fireCooldown > 0.0) && !this.get_isLocalPlayer() || this.inv.curItem != this.weapons[this.curWeapon].inventoryID || (double) ((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(this.inv.GetItemIndex()).durability <= 0.0)
      return;
    this.inv.items.ModifyDuration(this.inv.GetItemIndex(), ((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(this.inv.GetItemIndex()).durability - 1f);
    this.fireCooldown = (float) (1.0 / ((double) this.weapons[this.curWeapon].shotsPerSecond * (double) this.weapons[this.curWeapon].allEffects.firerateMultiplier) * 0.800000011920929);
    CharacterClassManager c = (CharacterClassManager) null;
    if (Object.op_Inequality((Object) target, (Object) null))
      c = (CharacterClassManager) target.GetComponent<CharacterClassManager>();
    ((Scp939_VisionController) ((Component) this).GetComponent<Scp939_VisionController>()).MakeNoise(Mathf.Clamp((float) ((double) this.weapons[this.curWeapon].allEffects.audioSourceRangeScale / 2.0 * 70.0), 5f, 100f));
    if (hitboxType == "window" && Object.op_Inequality((Object) target.GetComponent<BreakableWindow>(), (Object) null))
    {
      float damage = this.weapons[this.curWeapon].damageOverDistance.Evaluate(Vector3.Distance(((Component) this.camera).get_transform().get_position(), target.get_transform().get_position()));
      ((BreakableWindow) target.GetComponent<BreakableWindow>()).ServerDamageWindow(damage);
      this.CallRpcConfirmShot(true, this.curWeapon);
    }
    else if (Object.op_Inequality((Object) c, (Object) null) && this.GetShootPermission(c, false))
    {
      float distanceAddition = Vector3.Distance(((Component) this.camera).get_transform().get_position(), target.get_transform().get_position());
      float num = this.weapons[this.curWeapon].damageOverDistance.Evaluate(distanceAddition);
      switch (hitboxType.ToUpper())
      {
        case "HEAD":
          num *= 4f;
          break;
        case "LEG":
          num /= 2f;
          break;
        case "SCP106":
          num /= 10f;
          break;
      }
      ((PlayerStats) ((Component) this).GetComponent<PlayerStats>()).HurtPlayer(new PlayerStats.HitInfo(num * this.weapons[this.curWeapon].allEffects.damageMultiplier * this.overallDamagerFactor, this.ccm.SteamId + " (" + ((NicknameSync) ((Component) this.ccm).GetComponent<NicknameSync>()).myNick + ")", "Weapon:" + (object) this.curWeapon, ((QueryProcessor) ((Component) this).GetComponent<QueryProcessor>()).PlayerId), ((Component) c).get_gameObject());
      this.CallRpcConfirmShot(true, this.curWeapon);
      this.PlaceDecal(true, new Ray(this.camera.get_position(), dir), c.curClass, distanceAddition);
    }
    else
    {
      this.PlaceDecal(false, new Ray(this.camera.get_position(), dir), this.curWeapon, 0.0f);
      this.CallRpcConfirmShot(false, this.curWeapon);
    }
  }

  [Command]
  private void CmdReload(bool animationOnly)
  {
    if (this.curWeapon < 0 || this.inv.curItem != this.weapons[this.curWeapon].inventoryID || (double) ((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(this.inv.GetItemIndex()).durability >= (double) this.weapons[this.curWeapon].maxAmmo)
      return;
    if (animationOnly)
    {
      this.CallRpcReload(this.curWeapon);
    }
    else
    {
      int ammoType = this.weapons[this.curWeapon].ammoType;
      int ammo = this.abox.GetAmmo(ammoType);
      int durability = (int) ((SyncList<Inventory.SyncItemInfo>) this.inv.items).get_Item(this.inv.GetItemIndex()).durability;
      for (int maxAmmo = this.weapons[this.curWeapon].maxAmmo; ammo > 0 && durability < maxAmmo; ++durability)
        --ammo;
      this.inv.items.ModifyDuration(this.inv.GetItemIndex(), (float) durability);
      this.abox.SetOneAmount(ammoType, ammo.ToString());
    }
  }

  [ServerCallback]
  private void PlaceDecal(bool isBlood, Ray ray, int classId, float distanceAddition)
  {
    RaycastHit raycastHit;
    if (!NetworkServer.get_active() || (!Physics.Raycast(ray, ref raycastHit, !isBlood ? 100f : 10f + distanceAddition, LayerMask.op_Implicit(this.bloodMask)) || classId < 0))
      return;
    this.CallRpcPlaceDecal(isBlood, !isBlood ? classId : this.ccm.klasy[classId].bloodType, Vector3.op_Addition(((RaycastHit) ref raycastHit).get_point(), Vector3.op_Multiply(((RaycastHit) ref raycastHit).get_normal(), 0.01f)), Quaternion.FromToRotation(Vector3.get_up(), ((RaycastHit) ref raycastHit).get_normal()));
  }

  [ClientRpc]
  private void RpcPlaceDecal(bool isBlood, int type, Vector3 pos, Quaternion rot)
  {
    if (isBlood)
    {
      this.drawer.DrawBlood(pos, rot, type);
    }
    else
    {
      GameObject gameObject;
      Object.Destroy((Object) (gameObject = (GameObject) Object.Instantiate<GameObject>((M0) this.weapons[type].holeEffect)), 4f);
      gameObject.get_transform().set_position(pos);
      gameObject.get_transform().set_rotation(rot);
      gameObject.get_transform().set_localScale(Vector3.get_one());
    }
  }

  [ClientRpc]
  private void RpcConfirmShot(bool hitmarker, int weapon)
  {
    if (this.get_isLocalPlayer())
    {
      if (!hitmarker)
        return;
      Hitmarker.Hit(1f);
    }
    else
    {
      if (!Object.op_Inequality((Object) this.animationController, (Object) null))
        return;
      this.animationController.DoAnimation("Shoot");
      this.weapons[this.curWeapon].PlayMuzzleFlashes(false);
      this.animationController.gunSource.set_maxDistance((float) (80.0 * (this.curWeapon < 0 ? 1.0 : (double) this.weapons[this.curWeapon].allEffects.audioSourceRangeScale) * (((Component) this).get_transform().get_position().y <= 900.0 ? 1.0 : 3.0)));
      this.animationController.gunSource.PlayOneShot(this.weapons[weapon].allEffects.shootSound);
    }
  }

  [ClientRpc]
  private void RpcReload(int weapon)
  {
    if (this.get_isLocalPlayer() || (double) this.reloadCooldown > 0.0)
      return;
    this.animationController.DoAnimation("Reload");
    Timing.RunCoroutine(this._ReloadRpc(weapon), (Segment) 1);
  }

  [DebuggerHidden]
  private IEnumerator<float> _Reload()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new WeaponManager.\u003C_Reload\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator<float> _ReloadRpc(int weapon)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<float>) new WeaponManager.\u003C_ReloadRpc\u003Ec__Iterator1()
    {
      weapon = weapon,
      \u0024this = this
    };
  }

  public bool GetShootPermission(Team target, bool forceFriendlyFire = false)
  {
    if (this.ccm.curClass == 2 || this.ccm.klasy[this.ccm.curClass].team == Team.SCP)
      return false;
    if (this.friendlyFire && !forceFriendlyFire)
      return true;
    Team team = this.ccm.klasy[this.ccm.curClass].team;
    return (team != Team.MTF && team != Team.RSC || target != Team.MTF && target != Team.RSC) && (team != Team.CDP && team != Team.CHI || target != Team.CDP && target != Team.CHI);
  }

  public bool GetShootPermission(CharacterClassManager c, bool forceFriendlyFire = false)
  {
    return this.GetShootPermission(c.klasy[c.curClass].team, forceFriendlyFire);
  }

  private void UNetVersion()
  {
  }

  public bool NetworkfriendlyFire
  {
    get
    {
      return this.friendlyFire;
    }
    [param: In] set
    {
      this.SetSyncVar<bool>((M0) (value ? 1 : 0), (M0&) ref this.friendlyFire, 1U);
    }
  }

  public int NetworkcurWeapon
  {
    get
    {
      return this.curWeapon;
    }
    [param: In] set
    {
      int num1 = value;
      ref int local = ref this.curWeapon;
      int num2 = 2;
      if (NetworkServer.get_localClientActive() && !this.get_syncVarHookGuard())
      {
        this.set_syncVarHookGuard(true);
        this.HookCurWeapon(value);
        this.set_syncVarHookGuard(false);
      }
      this.SetSyncVar<int>((M0) num1, (M0&) ref local, (uint) num2);
    }
  }

  public int Networksync_sight
  {
    get
    {
      return this.sync_sight;
    }
    [param: In] set
    {
      this.SetSyncVar<int>((M0) value, (M0&) ref this.sync_sight, 4U);
    }
  }

  public int Networksync_barrel
  {
    get
    {
      return this.sync_barrel;
    }
    [param: In] set
    {
      this.SetSyncVar<int>((M0) value, (M0&) ref this.sync_barrel, 8U);
    }
  }

  public int Networksync_other
  {
    get
    {
      return this.sync_other;
    }
    [param: In] set
    {
      this.SetSyncVar<int>((M0) value, (M0&) ref this.sync_other, 16U);
    }
  }

  public bool Networksync_flashlight
  {
    get
    {
      return this.sync_flashlight;
    }
    [param: In] set
    {
      this.SetSyncVar<bool>((M0) (value ? 1 : 0), (M0&) ref this.sync_flashlight, 32U);
    }
  }

  protected static void InvokeCmdCmdSyncMods(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdSyncMods called on client.");
    else
      ((WeaponManager) obj).CmdSyncMods((int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32(), (int) reader.ReadPackedUInt32(), reader.ReadBoolean());
  }

  protected static void InvokeCmdCmdShoot(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdShoot called on client.");
    else
      ((WeaponManager) obj).CmdShoot((GameObject) reader.ReadGameObject(), reader.ReadString(), (Vector3) reader.ReadVector3());
  }

  protected static void InvokeCmdCmdReload(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkServer.get_active())
      Debug.LogError((object) "Command CmdReload called on client.");
    else
      ((WeaponManager) obj).CmdReload(reader.ReadBoolean());
  }

  public void CallCmdSyncMods(int _s, int _b, int _o, bool _flashlight)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdSyncMods called on server.");
    else if (this.get_isServer())
    {
      this.CmdSyncMods(_s, _b, _o, _flashlight);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) WeaponManager.kCmdCmdSyncMods);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.WritePackedUInt32((uint) _s);
      networkWriter.WritePackedUInt32((uint) _b);
      networkWriter.WritePackedUInt32((uint) _o);
      networkWriter.Write(_flashlight);
      this.SendCommandInternal(networkWriter, 0, "CmdSyncMods");
    }
  }

  public void CallCmdShoot(GameObject target, string hitboxType, Vector3 dir)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdShoot called on server.");
    else if (this.get_isServer())
    {
      this.CmdShoot(target, hitboxType, dir);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) WeaponManager.kCmdCmdShoot);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write((GameObject) target);
      networkWriter.Write(hitboxType);
      networkWriter.Write((Vector3) dir);
      this.SendCommandInternal(networkWriter, 0, "CmdShoot");
    }
  }

  public void CallCmdReload(bool animationOnly)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "Command function CmdReload called on server.");
    else if (this.get_isServer())
    {
      this.CmdReload(animationOnly);
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 5);
      networkWriter.WritePackedUInt32((uint) WeaponManager.kCmdCmdReload);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(animationOnly);
      this.SendCommandInternal(networkWriter, 0, "CmdReload");
    }
  }

  protected static void InvokeRpcRpcPlaceDecal(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcPlaceDecal called on server.");
    else
      ((WeaponManager) obj).RpcPlaceDecal(reader.ReadBoolean(), (int) reader.ReadPackedUInt32(), (Vector3) reader.ReadVector3(), (Quaternion) reader.ReadQuaternion());
  }

  protected static void InvokeRpcRpcConfirmShot(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcConfirmShot called on server.");
    else
      ((WeaponManager) obj).RpcConfirmShot(reader.ReadBoolean(), (int) reader.ReadPackedUInt32());
  }

  protected static void InvokeRpcRpcReload(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "RPC RpcReload called on server.");
    else
      ((WeaponManager) obj).RpcReload((int) reader.ReadPackedUInt32());
  }

  public void CallRpcPlaceDecal(bool isBlood, int type, Vector3 pos, Quaternion rot)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcPlaceDecal called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) WeaponManager.kRpcRpcPlaceDecal);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(isBlood);
      networkWriter.WritePackedUInt32((uint) type);
      networkWriter.Write((Vector3) pos);
      networkWriter.Write((Quaternion) rot);
      this.SendRPCInternal(networkWriter, 0, "RpcPlaceDecal");
    }
  }

  public void CallRpcConfirmShot(bool hitmarker, int weapon)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcConfirmShot called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) WeaponManager.kRpcRpcConfirmShot);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.Write(hitmarker);
      networkWriter.WritePackedUInt32((uint) weapon);
      this.SendRPCInternal(networkWriter, 0, "RpcConfirmShot");
    }
  }

  public void CallRpcReload(int weapon)
  {
    if (!NetworkServer.get_active())
    {
      Debug.LogError((object) "RPC Function RpcReload called on client.");
    }
    else
    {
      NetworkWriter networkWriter = new NetworkWriter();
      networkWriter.Write((short) 0);
      networkWriter.Write((short) 2);
      networkWriter.WritePackedUInt32((uint) WeaponManager.kRpcRpcReload);
      networkWriter.Write(((NetworkIdentity) ((Component) this).GetComponent<NetworkIdentity>()).get_netId());
      networkWriter.WritePackedUInt32((uint) weapon);
      this.SendRPCInternal(networkWriter, 0, "RpcReload");
    }
  }

  static WeaponManager()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (WeaponManager), WeaponManager.kCmdCmdSyncMods, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdSyncMods)));
    WeaponManager.kCmdCmdShoot = -1101833074;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (WeaponManager), WeaponManager.kCmdCmdShoot, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdShoot)));
    WeaponManager.kCmdCmdReload = 171423498;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterCommandDelegate(typeof (WeaponManager), WeaponManager.kCmdCmdReload, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeCmdCmdReload)));
    WeaponManager.kRpcRpcPlaceDecal = -472825811;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (WeaponManager), WeaponManager.kRpcRpcPlaceDecal, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcPlaceDecal)));
    WeaponManager.kRpcRpcConfirmShot = -986408589;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (WeaponManager), WeaponManager.kRpcRpcConfirmShot, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcConfirmShot)));
    WeaponManager.kRpcRpcReload = 953492064;
    // ISSUE: method pointer
    NetworkBehaviour.RegisterRpcDelegate(typeof (WeaponManager), WeaponManager.kRpcRpcReload, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeRpcRpcReload)));
    NetworkCRC.RegisterBehaviour(nameof (WeaponManager), 0);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      writer.Write(this.friendlyFire);
      writer.WritePackedUInt32((uint) this.curWeapon);
      writer.WritePackedUInt32((uint) this.sync_sight);
      writer.WritePackedUInt32((uint) this.sync_barrel);
      writer.WritePackedUInt32((uint) this.sync_other);
      writer.Write(this.sync_flashlight);
      return true;
    }
    bool flag = false;
    if (((int) this.get_syncVarDirtyBits() & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.friendlyFire);
    }
    if (((int) this.get_syncVarDirtyBits() & 2) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.curWeapon);
    }
    if (((int) this.get_syncVarDirtyBits() & 4) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.sync_sight);
    }
    if (((int) this.get_syncVarDirtyBits() & 8) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.sync_barrel);
    }
    if (((int) this.get_syncVarDirtyBits() & 16) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.WritePackedUInt32((uint) this.sync_other);
    }
    if (((int) this.get_syncVarDirtyBits() & 32) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      writer.Write(this.sync_flashlight);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      this.friendlyFire = reader.ReadBoolean();
      this.curWeapon = (int) reader.ReadPackedUInt32();
      this.sync_sight = (int) reader.ReadPackedUInt32();
      this.sync_barrel = (int) reader.ReadPackedUInt32();
      this.sync_other = (int) reader.ReadPackedUInt32();
      this.sync_flashlight = reader.ReadBoolean();
    }
    else
    {
      int num = (int) reader.ReadPackedUInt32();
      if ((num & 1) != 0)
        this.friendlyFire = reader.ReadBoolean();
      if ((num & 2) != 0)
        this.HookCurWeapon((int) reader.ReadPackedUInt32());
      if ((num & 4) != 0)
        this.sync_sight = (int) reader.ReadPackedUInt32();
      if ((num & 8) != 0)
        this.sync_barrel = (int) reader.ReadPackedUInt32();
      if ((num & 16) != 0)
        this.sync_other = (int) reader.ReadPackedUInt32();
      if ((num & 32) == 0)
        return;
      this.sync_flashlight = reader.ReadBoolean();
    }
  }

  [Serializable]
  public class Weapon
  {
    public float recoilAnimation = 0.5f;
    public float bobAnimationScale = 1f;
    public float timeToPickup = 0.5f;
    public bool useProceduralPickupAnimation = true;
    public float unfocusedSpread = 5f;
    [Header("Misc properties")]
    public int inventoryID;
    public RecoilProperties recoil;
    public AnimationCurve damageOverDistance;
    public float shotsPerSecond;
    public bool allowFullauto;
    public Vector3 positionOffset;
    public GameObject holeEffect;
    public ParticleSystem husks;
    public string[] customShootAnims;
    public string[] customZoomshotAnims;
    public string[] reloadingAnims;
    public string[] reloadingNoammoAnims;
    public float maxAngleLaser;
    [Header("Ammo & reloading")]
    public AudioClip reloadClip;
    public int maxAmmo;
    public int ammoType;
    public float reloadingTime;
    [Header("Zooming")]
    public bool allowZoom;
    public float zoomingTime;
    [Header("Mods")]
    public WeaponManager.Weapon.WeaponMod[] mod_sights;
    public WeaponManager.Weapon.WeaponMod[] mod_barrels;
    public WeaponManager.Weapon.WeaponMod[] mod_others;
    public WeaponManager.Weapon.WeaponMod.WeaponModEffects allEffects;
    private int cur_sight;
    private int cur_barrel;
    private int cur_other;

    public void PlayMuzzleFlashes(bool firstperson)
    {
      GameObject gameObject = !firstperson ? this.mod_barrels[this.cur_barrel].prefab_thirdperson : this.mod_barrels[this.cur_barrel].prefab_firstperson;
      if (!Object.op_Inequality((Object) gameObject, (Object) null))
        return;
      foreach (ParticleSystem componentsInChild in (ParticleSystem[]) gameObject.GetComponentsInChildren<ParticleSystem>())
        componentsInChild.Play();
    }

    public void SetMods(int _s, int _b, int _o, bool savePlayerPrefs, bool _flashlight)
    {
      this.cur_sight = Mathf.Clamp(_s, 0, this.mod_sights.Length - 1);
      this.cur_barrel = Mathf.Clamp(_b, 0, this.mod_barrels.Length - 1);
      this.cur_other = Mathf.Clamp(_o, 0, this.mod_others.Length - 1);
      this.RefreshMods(savePlayerPrefs, _flashlight);
    }

    public void RefreshMods(bool saveToPlayerPrefs, bool _flashlight)
    {
      if (saveToPlayerPrefs)
      {
        PlayerPrefs.SetInt("Weapon_" + (object) this.inventoryID + "_sight", this.cur_sight);
        PlayerPrefs.SetInt("Weapon_" + (object) this.inventoryID + "_barrel", this.cur_barrel);
        PlayerPrefs.SetInt("Weapon_" + (object) this.inventoryID + "_other", this.cur_other);
      }
      for (int index = 0; index < this.mod_sights.Length; ++index)
        this.mod_sights[index].SetVisibility(index == this.cur_sight);
      for (int index = 0; index < this.mod_barrels.Length; ++index)
        this.mod_barrels[index].SetVisibility(index == this.cur_barrel);
      for (int index = 0; index < this.mod_others.Length; ++index)
      {
        this.mod_others[index].SetVisibility(index == this.cur_other);
        if (index == this.cur_other && this.mod_others[index].name.ToLower().Contains("flashlight"))
        {
          if (Object.op_Inequality((Object) this.mod_others[index].prefab_firstperson, (Object) null))
          {
            foreach (Behaviour componentsInChild in (Light[]) this.mod_others[index].prefab_firstperson.GetComponentsInChildren<Light>())
              componentsInChild.set_enabled(_flashlight);
          }
          if (Object.op_Inequality((Object) this.mod_others[index].prefab_thirdperson, (Object) null))
          {
            foreach (Behaviour componentsInChild in (Light[]) this.mod_others[index].prefab_thirdperson.GetComponentsInChildren<Light>())
              componentsInChild.set_enabled(_flashlight);
          }
        }
      }
      this.allEffects = new WeaponManager.Weapon.WeaponMod.WeaponModEffects()
      {
        customProfile = this.mod_sights[this.cur_sight].effects.customProfile,
        zoomRecoilReduction = this.mod_sights[this.cur_sight].effects.zoomRecoilReduction,
        zoomFov = this.mod_sights[this.cur_sight].effects.zoomFov,
        weaponCameraFov = this.mod_sights[this.cur_sight].effects.weaponCameraFov,
        zoomSlowdown = this.mod_sights[this.cur_sight].effects.zoomSlowdown,
        zoomSensitivity = this.mod_sights[this.cur_sight].effects.zoomSensitivity,
        zoomPositionOffset = this.mod_sights[this.cur_sight].effects.zoomPositionOffset,
        shootSound = this.mod_barrels[this.cur_barrel].effects.shootSound,
        firerateMultiplier = this.mod_barrels[this.cur_barrel].effects.firerateMultiplier,
        zoomRecoilAnimScale = this.mod_sights[this.cur_sight].effects.zoomRecoilAnimScale,
        damageMultiplier = this.mod_barrels[this.cur_barrel].effects.damageMultiplier,
        overallRecoilReduction = this.mod_sights[this.cur_sight].effects.overallRecoilReduction * this.mod_barrels[this.cur_barrel].effects.overallRecoilReduction * this.mod_others[this.cur_other].effects.overallRecoilReduction,
        unfocusedSpread = this.mod_sights[this.cur_sight].effects.unfocusedSpread * this.mod_barrels[this.cur_barrel].effects.unfocusedSpread * this.mod_others[this.cur_other].effects.unfocusedSpread,
        laserDirection = this.mod_others[this.cur_other].effects.laserDirection,
        audioSourceRangeScale = this.mod_barrels[this.cur_barrel].effects.audioSourceRangeScale,
        counterText = this.mod_others[this.cur_other].effects.counterText,
        counterTemplate = this.mod_others[this.cur_other].effects.counterTemplate
      };
    }

    public void ChangeMod(ModPrefab.ModType type, int value, bool saveToStats, bool _flashlight)
    {
      if (type == ModPrefab.ModType.Sight)
        this.cur_sight = Mathf.Clamp(value, 0, this.mod_sights.Length - 1);
      if (type == ModPrefab.ModType.Barrel)
        this.cur_barrel = Mathf.Clamp(value, 0, this.mod_barrels.Length - 1);
      if (type == ModPrefab.ModType.Other)
        this.cur_other = Mathf.Clamp(value, 0, this.mod_others.Length - 1);
      this.RefreshMods(saveToStats, _flashlight);
    }

    private string ConvertToStat(int value, bool lessTheBetter)
    {
      string empty = string.Empty;
      bool flag;
      string str;
      if (value < 0)
      {
        flag = lessTheBetter;
        str = "-" + (object) Mathf.Abs(value) + "%";
      }
      else
      {
        flag = !lessTheBetter;
        str = "+" + (object) Mathf.Abs(value) + "%";
      }
      return "<color=" + (!flag ? "red" : "green") + ">" + str + "</color>";
    }

    public string GetStats(ModPrefab.ModType type, int id)
    {
      string str = string.Empty;
      switch (type)
      {
        case ModPrefab.ModType.Sight:
          int num1 = Mathf.RoundToInt((float) (((double) this.mod_sights[id].effects.zoomRecoilReduction - 1.0) * 100.0));
          bool flag = Object.op_Inequality((Object) this.mod_sights[id].effects.customProfile, (Object) null);
          float num2 = !flag ? Mathf.Round((float) (70.0 / (double) this.mod_sights[id].effects.zoomFov * 100.0)) / 100f : 1f;
          int num3 = Mathf.RoundToInt((float) (((double) this.mod_sights[id].effects.overallRecoilReduction - 1.0) * 100.0));
          if (num1 != 0)
            str = str + "Recoil (while aiming) " + this.ConvertToStat(num1, true) + "\n";
          if (flag)
            str += "<color=green>Telescopic-type sight</color>\n";
          if ((double) num2 != 1.0)
            str = str + "Zoom scale <color=green>" + (object) num2 + "</color>";
          if (num3 != 0)
          {
            str = str + "Recoil " + this.ConvertToStat(num3, true) + "\n";
            break;
          }
          break;
        case ModPrefab.ModType.Barrel:
          int num4 = Mathf.RoundToInt((float) (((double) this.mod_barrels[id].effects.damageMultiplier - 1.0) * 100.0));
          int num5 = Mathf.RoundToInt((float) (((double) this.mod_barrels[id].effects.audioSourceRangeScale - 1.0) * 100.0));
          int num6 = Mathf.RoundToInt((float) (((double) this.mod_barrels[id].effects.overallRecoilReduction - 1.0) * 100.0));
          int num7 = Mathf.RoundToInt((float) (((double) this.mod_barrels[id].effects.firerateMultiplier - 1.0) * 100.0));
          if (num4 != 0)
            str = str + "Damage " + this.ConvertToStat(num4, false) + "\n";
          if (num5 != 0)
            str = str + "Shot loudness " + this.ConvertToStat(num5, true) + "\n";
          if (num6 != 0)
            str = str + "Recoil " + this.ConvertToStat(num6, true) + "\n";
          if (num7 != 0)
          {
            str = str + "Fire rate " + this.ConvertToStat(num7, false) + "\n";
            break;
          }
          break;
        default:
          int num8 = Mathf.RoundToInt((float) (((double) this.mod_others[id].effects.overallRecoilReduction - 1.0) * 100.0));
          int num9 = Mathf.RoundToInt((float) (((double) this.mod_others[id].effects.unfocusedSpread - 1.0) * 100.0));
          if (num8 != 0)
            str = str + "Recoil " + this.ConvertToStat(num8, true) + "\n";
          if (num9 != 0)
          {
            str = str + "Bullet spread (without aiming)  " + this.ConvertToStat(num9, true) + "\n";
            break;
          }
          break;
      }
      if (string.IsNullOrEmpty(str))
        str = "No effects";
      return str;
    }

    public int GetMod(ModPrefab.ModType type)
    {
      switch (type)
      {
        case ModPrefab.ModType.Sight:
          return this.cur_sight;
        case ModPrefab.ModType.Barrel:
          return this.cur_barrel;
        case ModPrefab.ModType.Other:
          return this.cur_other;
        default:
          return 0;
      }
    }

    [Serializable]
    public class WeaponMod
    {
      public string name;
      public GameObject prefab_firstperson;
      public GameObject prefab_thirdperson;
      public WeaponManager.Weapon.WeaponMod.WeaponModEffects effects;
      public Texture icon;
      public bool isActive;

      public void SetVisibility(bool b)
      {
        this.isActive = b;
        if (Object.op_Inequality((Object) this.prefab_firstperson, (Object) null))
          this.prefab_firstperson.SetActive(b);
        if (!Object.op_Inequality((Object) this.prefab_thirdperson, (Object) null))
          return;
        this.prefab_thirdperson.SetActive(b);
      }

      [Serializable]
      public class WeaponModEffects
      {
        [Tooltip("FOV")]
        public float zoomFov = 70f;
        public float weaponCameraFov = 60f;
        [Tooltip("RECOIL SCALE")]
        public float zoomRecoilReduction = 1f;
        [Tooltip("WALK SLOW")]
        public float zoomSlowdown = 1f;
        [Tooltip("SENSITIVITY")]
        public float zoomSensitivity = 1f;
        [Tooltip("RECOIL ANIMATION SCALE")]
        public float zoomRecoilAnimScale = 1f;
        public Vector3 zoomPositionOffset = Vector3.get_zero();
        public float damageMultiplier = 1f;
        public float audioSourceRangeScale = 1f;
        public float firerateMultiplier = 1f;
        [Header("Mixed effects")]
        public float overallRecoilReduction = 1f;
        public float unfocusedSpread = 1f;
        [Header("Sights only effects")]
        public PostProcessingProfile customProfile;
        [Header("Barrels only effects")]
        public AudioClip shootSound;
        [Header("Ammo Counter Effects")]
        public Text counterText;
        public string counterTemplate;
        public GameObject laserDirection;
      }
    }
  }
}
