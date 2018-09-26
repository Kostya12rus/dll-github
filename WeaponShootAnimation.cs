// Decompiled with JetBrains decompiler
// Type: WeaponShootAnimation
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class WeaponShootAnimation : MonoBehaviour
{
  public float curPosition;
  public Vector3 maxRecoilPos;
  public Vector3 maxRecoilRot;
  public float backSpeed;
  public float backY_Speed;
  private float yOverride;
  private float curY;

  private void LateUpdate()
  {
    this.curPosition = Mathf.Lerp(this.curPosition, 0.0f, Time.deltaTime * this.backSpeed * this.curPosition);
    this.yOverride = Mathf.Lerp(0.0f, this.yOverride, this.curPosition);
    this.curY = Mathf.Lerp(this.curY, this.yOverride, Time.deltaTime * this.backY_Speed * this.curPosition);
    this.transform.localPosition = Vector3.Lerp(Vector3.zero, this.maxRecoilPos, this.curPosition);
    this.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(this.maxRecoilRot + Vector3.up * this.curY), this.curPosition);
  }

  public void Recoil(float f)
  {
    this.curPosition = Mathf.Clamp01(this.curPosition + f);
    this.yOverride = Random.Range(-10f, 10f) * f;
  }
}
