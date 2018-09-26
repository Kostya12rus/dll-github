// Decompiled with JetBrains decompiler
// Type: MainMenuCamera
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
  public float borderWidthPercent;
  private float rotSpeed;

  private void Update()
  {
    float num = (float) Screen.width * (this.borderWidthPercent / 100f);
    Vector3 zero = Vector3.zero;
    Vector3 mousePosition = Input.mousePosition;
    if ((double) mousePosition.x < (double) num && (double) this.transform.localRotation.eulerAngles.y > 41.0)
      zero += Vector3.down;
    if ((double) mousePosition.x > (double) Screen.width - (double) num && (double) this.transform.localRotation.eulerAngles.y < 74.0)
      zero += Vector3.up;
    if (zero == Vector3.zero)
    {
      this.rotSpeed = 0.0f;
    }
    else
    {
      this.rotSpeed += Time.deltaTime * 200f;
      this.rotSpeed = Mathf.Clamp(this.rotSpeed, 0.0f, 120f);
    }
    zero.Normalize();
    this.transform.localRotation = Quaternion.Euler(this.transform.localRotation.eulerAngles + zero * Time.deltaTime * this.rotSpeed);
    if (!Input.GetKeyDown(KeyCode.Mouse0))
      return;
    this.Raycast();
  }

  private void Raycast()
  {
    RaycastHit hitInfo;
    if (!Physics.Raycast(this.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hitInfo))
      return;
    this.ElementChoosen(hitInfo.transform.name);
  }

  public void ElementChoosen(string id)
  {
    if (id == "EXIT")
      Application.Quit();
    if (!(id == "PLAY"))
      return;
    Object.FindObjectOfType<NetManagerValueSetter>().HostGame();
  }
}
