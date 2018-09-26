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

  public MainMenuCamera()
  {
    base.\u002Ector();
  }

  private void Update()
  {
    float num = (float) Screen.get_width() * (this.borderWidthPercent / 100f);
    Vector3 vector3 = Vector3.get_zero();
    Vector3 mousePosition = Input.get_mousePosition();
    if (mousePosition.x < (double) num)
    {
      Quaternion localRotation = ((Component) this).get_transform().get_localRotation();
      if (((Quaternion) ref localRotation).get_eulerAngles().y > 41.0)
        vector3 = Vector3.op_Addition(vector3, Vector3.get_down());
    }
    if (mousePosition.x > (double) Screen.get_width() - (double) num)
    {
      Quaternion localRotation = ((Component) this).get_transform().get_localRotation();
      if (((Quaternion) ref localRotation).get_eulerAngles().y < 74.0)
        vector3 = Vector3.op_Addition(vector3, Vector3.get_up());
    }
    if (Vector3.op_Equality(vector3, Vector3.get_zero()))
    {
      this.rotSpeed = 0.0f;
    }
    else
    {
      this.rotSpeed += Time.get_deltaTime() * 200f;
      this.rotSpeed = Mathf.Clamp(this.rotSpeed, 0.0f, 120f);
    }
    ((Vector3) ref vector3).Normalize();
    Transform transform = ((Component) this).get_transform();
    Quaternion localRotation1 = ((Component) this).get_transform().get_localRotation();
    Quaternion quaternion = Quaternion.Euler(Vector3.op_Addition(((Quaternion) ref localRotation1).get_eulerAngles(), Vector3.op_Multiply(Vector3.op_Multiply(vector3, Time.get_deltaTime()), this.rotSpeed)));
    transform.set_localRotation(quaternion);
    if (!Input.GetKeyDown((KeyCode) 323))
      return;
    this.Raycast();
  }

  private void Raycast()
  {
    RaycastHit raycastHit;
    if (!Physics.Raycast(((Camera) ((Component) this).GetComponent<Camera>()).ScreenPointToRay(Input.get_mousePosition()), ref raycastHit))
      return;
    this.ElementChoosen(((Object) ((RaycastHit) ref raycastHit).get_transform()).get_name());
  }

  public void ElementChoosen(string id)
  {
    if (id == "EXIT")
      Application.Quit();
    if (!(id == "PLAY"))
      return;
    ((NetManagerValueSetter) Object.FindObjectOfType<NetManagerValueSetter>()).HostGame();
  }
}
