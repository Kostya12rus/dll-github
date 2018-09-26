// Decompiled with JetBrains decompiler
// Type: UBER_MouseOrbit_DynamicDistance
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("UBER/Mouse Orbit - Dynamic Distance")]
public class UBER_MouseOrbit_DynamicDistance : MonoBehaviour
{
  public GameObject target;
  public Transform targetFocus;
  public float distance;
  [Range(0.1f, 4f)]
  public float ZoomWheelSpeed;
  public float minDistance;
  public float maxDistance;
  public float xSpeed;
  public float ySpeed;
  public float xObjSpeed;
  public float yObjSpeed;
  public float yMinLimit;
  public float yMaxLimit;
  private float x;
  private float y;
  private float normal_angle;
  private float cur_distance;
  private float cur_xSpeed;
  private float cur_ySpeed;
  private float req_xSpeed;
  private float req_ySpeed;
  private float cur_ObjxSpeed;
  private float cur_ObjySpeed;
  private float req_ObjxSpeed;
  private float req_ObjySpeed;
  private bool DraggingObject;
  private bool lastLMBState;
  private Collider[] surfaceColliders;
  private float bounds_MaxSize;
  [HideInInspector]
  public bool disableSteering;

  public UBER_MouseOrbit_DynamicDistance()
  {
    base.\u002Ector();
  }

  private void Start()
  {
    Vector3 eulerAngles = ((Component) this).get_transform().get_eulerAngles();
    this.x = (float) eulerAngles.y;
    this.y = (float) eulerAngles.x;
    this.Reset();
  }

  public void DisableSteering(bool state)
  {
    this.disableSteering = state;
  }

  public void Reset()
  {
    this.lastLMBState = Input.GetMouseButton(0);
    this.disableSteering = false;
    this.cur_distance = this.distance;
    this.cur_xSpeed = 0.0f;
    this.cur_ySpeed = 0.0f;
    this.req_xSpeed = 0.0f;
    this.req_ySpeed = 0.0f;
    this.surfaceColliders = (Collider[]) null;
    this.cur_ObjxSpeed = 0.0f;
    this.cur_ObjySpeed = 0.0f;
    this.req_ObjxSpeed = 0.0f;
    this.req_ObjySpeed = 0.0f;
    if (!Object.op_Implicit((Object) this.target))
      return;
    Renderer[] componentsInChildren = (Renderer[]) this.target.GetComponentsInChildren<Renderer>();
    Bounds bounds = (Bounds) null;
    bool flag = false;
    foreach (Renderer renderer in componentsInChildren)
    {
      if (!flag)
      {
        flag = true;
        bounds = renderer.get_bounds();
      }
      else
        ((Bounds) ref bounds).Encapsulate(renderer.get_bounds());
    }
    Vector3 size = ((Bounds) ref bounds).get_size();
    float num = size.x <= size.y ? (float) size.y : (float) size.x;
    this.bounds_MaxSize = size.z <= (double) num ? num : (float) (double) size.z;
    this.cur_distance += this.bounds_MaxSize * 1.2f;
    this.surfaceColliders = (Collider[]) this.target.GetComponentsInChildren<Collider>();
  }

  private void LateUpdate()
  {
    if (!Object.op_Implicit((Object) this.target) || !Object.op_Implicit((Object) this.targetFocus))
      return;
    if (!this.lastLMBState && Input.GetMouseButton(0))
    {
      this.DraggingObject = false;
      if (this.surfaceColliders != null)
      {
        RaycastHit raycastHit = (RaycastHit) null;
        Ray ray = Camera.get_main().ScreenPointToRay(Input.get_mousePosition());
        foreach (Collider surfaceCollider in this.surfaceColliders)
        {
          if (surfaceCollider.Raycast(ray, ref raycastHit, float.PositiveInfinity))
          {
            this.DraggingObject = true;
            break;
          }
        }
      }
    }
    else if (this.lastLMBState && !Input.GetMouseButton(0))
      this.DraggingObject = false;
    this.lastLMBState = Input.GetMouseButton(0);
    if (this.DraggingObject)
    {
      if (Input.GetMouseButton(0) && !this.disableSteering)
      {
        this.req_ObjxSpeed += (float) (((double) Input.GetAxis("Mouse X") * (double) this.xObjSpeed * 0.0199999995529652 - (double) this.req_ObjxSpeed) * (double) Time.get_deltaTime() * 10.0);
        this.req_ObjySpeed += (float) (((double) Input.GetAxis("Mouse Y") * (double) this.yObjSpeed * 0.0199999995529652 - (double) this.req_ObjySpeed) * (double) Time.get_deltaTime() * 10.0);
      }
      else
      {
        this.req_ObjxSpeed += (float) ((0.0 - (double) this.req_ObjxSpeed) * (double) Time.get_deltaTime() * 4.0);
        this.req_ObjySpeed += (float) ((0.0 - (double) this.req_ObjySpeed) * (double) Time.get_deltaTime() * 4.0);
      }
      this.req_xSpeed += (float) ((0.0 - (double) this.req_xSpeed) * (double) Time.get_deltaTime() * 4.0);
      this.req_ySpeed += (float) ((0.0 - (double) this.req_ySpeed) * (double) Time.get_deltaTime() * 4.0);
    }
    else
    {
      if (Input.GetMouseButton(0) && !this.disableSteering)
      {
        this.req_xSpeed += (float) (((double) Input.GetAxis("Mouse X") * (double) this.xSpeed * 0.0199999995529652 - (double) this.req_xSpeed) * (double) Time.get_deltaTime() * 10.0);
        this.req_ySpeed += (float) (((double) Input.GetAxis("Mouse Y") * (double) this.ySpeed * 0.0199999995529652 - (double) this.req_ySpeed) * (double) Time.get_deltaTime() * 10.0);
      }
      else
      {
        this.req_xSpeed += (float) ((0.0 - (double) this.req_xSpeed) * (double) Time.get_deltaTime() * 4.0);
        this.req_ySpeed += (float) ((0.0 - (double) this.req_ySpeed) * (double) Time.get_deltaTime() * 4.0);
      }
      this.req_ObjxSpeed += (float) ((0.0 - (double) this.req_ObjxSpeed) * (double) Time.get_deltaTime() * 4.0);
      this.req_ObjySpeed += (float) ((0.0 - (double) this.req_ObjySpeed) * (double) Time.get_deltaTime() * 4.0);
    }
    this.distance -= Input.GetAxis("Mouse ScrollWheel") * this.ZoomWheelSpeed;
    this.distance = Mathf.Clamp(this.distance, this.minDistance, this.maxDistance);
    this.cur_ObjxSpeed += (float) (((double) this.req_ObjxSpeed - (double) this.cur_ObjxSpeed) * (double) Time.get_deltaTime() * 20.0);
    this.cur_ObjySpeed += (float) (((double) this.req_ObjySpeed - (double) this.cur_ObjySpeed) * (double) Time.get_deltaTime() * 20.0);
    this.target.get_transform().RotateAround(this.targetFocus.get_position(), Vector3.Cross(Vector3.op_Subtraction(this.targetFocus.get_position(), ((Component) this).get_transform().get_position()), ((Component) this).get_transform().get_right()), -this.cur_ObjxSpeed);
    this.target.get_transform().RotateAround(this.targetFocus.get_position(), Vector3.Cross(Vector3.op_Subtraction(this.targetFocus.get_position(), ((Component) this).get_transform().get_position()), ((Component) this).get_transform().get_up()), -this.cur_ObjySpeed);
    this.cur_xSpeed += (float) (((double) this.req_xSpeed - (double) this.cur_xSpeed) * (double) Time.get_deltaTime() * 20.0);
    this.cur_ySpeed += (float) (((double) this.req_ySpeed - (double) this.cur_ySpeed) * (double) Time.get_deltaTime() * 20.0);
    this.x += this.cur_xSpeed;
    this.y -= this.cur_ySpeed;
    this.y = UBER_MouseOrbit_DynamicDistance.ClampAngle(this.y, this.yMinLimit + this.normal_angle, this.yMaxLimit + this.normal_angle);
    if (this.surfaceColliders != null)
    {
      RaycastHit raycastHit = (RaycastHit) null;
      Vector3 vector3 = Vector3.Normalize(Vector3.op_Subtraction(this.targetFocus.get_position(), ((Component) this).get_transform().get_position()));
      float num = 0.01f;
      bool flag = false;
      foreach (Collider surfaceCollider in this.surfaceColliders)
      {
        if (surfaceCollider.Raycast(new Ray(Vector3.op_Subtraction(((Component) this).get_transform().get_position(), Vector3.op_Multiply(vector3, this.bounds_MaxSize)), vector3), ref raycastHit, float.PositiveInfinity))
        {
          num = Mathf.Max(Vector3.Distance(((RaycastHit) ref raycastHit).get_point(), this.targetFocus.get_position()) + this.distance, num);
          flag = true;
        }
      }
      if (flag)
        this.cur_distance += (float) (((double) num - (double) this.cur_distance) * (double) Time.get_deltaTime() * 4.0);
    }
    Quaternion quaternion = Quaternion.Euler(this.y, this.x, 0.0f);
    Vector3 vector3_1 = Vector3.op_Addition(Quaternion.op_Multiply(quaternion, new Vector3(0.0f, 0.0f, -this.cur_distance)), this.targetFocus.get_position());
    ((Component) this).get_transform().set_rotation(quaternion);
    ((Component) this).get_transform().set_position(vector3_1);
  }

  private static float ClampAngle(float angle, float min, float max)
  {
    if ((double) angle < -360.0)
      angle += 360f;
    if ((double) angle > 360.0)
      angle -= 360f;
    return Mathf.Clamp(angle, min, max);
  }

  public void set_normal_angle(float a)
  {
    this.normal_angle = a;
  }
}
