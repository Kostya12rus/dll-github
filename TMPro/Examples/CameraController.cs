// Decompiled with JetBrains decompiler
// Type: TMPro.Examples.CameraController
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

namespace TMPro.Examples
{
  public class CameraController : MonoBehaviour
  {
    public float FollowDistance = 30f;
    public float MaxFollowDistance = 100f;
    public float MinFollowDistance = 2f;
    public float ElevationAngle = 30f;
    public float MaxElevationAngle = 85f;
    public bool MovementSmoothing = true;
    public float MovementSmoothingValue = 25f;
    public float RotationSmoothingValue = 5f;
    public float MoveSensitivity = 2f;
    private Vector3 currentVelocity = Vector3.zero;
    private Transform cameraTransform;
    private Transform dummyTarget;
    public Transform CameraTarget;
    public float MinElevationAngle;
    public float OrbitalAngle;
    public CameraController.CameraModes CameraMode;
    public bool RotationSmoothing;
    private bool previousSmoothing;
    private Vector3 desiredPosition;
    private float mouseX;
    private float mouseY;
    private Vector3 moveVector;
    private float mouseWheel;
    private const string event_SmoothingValue = "Slider - Smoothing Value";
    private const string event_FollowDistance = "Slider - Camera Zoom";

    private void Awake()
    {
      Application.targetFrameRate = QualitySettings.vSyncCount <= 0 ? -1 : 60;
      if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        Input.simulateMouseWithTouches = false;
      this.cameraTransform = this.transform;
      this.previousSmoothing = this.MovementSmoothing;
    }

    private void Start()
    {
      if (!((Object) this.CameraTarget == (Object) null))
        return;
      this.dummyTarget = new GameObject("Camera Target").transform;
      this.CameraTarget = this.dummyTarget;
    }

    private void LateUpdate()
    {
      this.GetPlayerInput();
      if (!((Object) this.CameraTarget != (Object) null))
        return;
      if (this.CameraMode == CameraController.CameraModes.Isometric)
        this.desiredPosition = this.CameraTarget.position + Quaternion.Euler(this.ElevationAngle, this.OrbitalAngle, 0.0f) * new Vector3(0.0f, 0.0f, -this.FollowDistance);
      else if (this.CameraMode == CameraController.CameraModes.Follow)
        this.desiredPosition = this.CameraTarget.position + this.CameraTarget.TransformDirection(Quaternion.Euler(this.ElevationAngle, this.OrbitalAngle, 0.0f) * new Vector3(0.0f, 0.0f, -this.FollowDistance));
      this.cameraTransform.position = !this.MovementSmoothing ? this.desiredPosition : Vector3.SmoothDamp(this.cameraTransform.position, this.desiredPosition, ref this.currentVelocity, this.MovementSmoothingValue * Time.fixedDeltaTime);
      if (this.RotationSmoothing)
        this.cameraTransform.rotation = Quaternion.Lerp(this.cameraTransform.rotation, Quaternion.LookRotation(this.CameraTarget.position - this.cameraTransform.position), this.RotationSmoothingValue * Time.deltaTime);
      else
        this.cameraTransform.LookAt(this.CameraTarget);
    }

    private void GetPlayerInput()
    {
      this.moveVector = Vector3.zero;
      this.mouseWheel = Input.GetAxis("Mouse ScrollWheel");
      float touchCount = (float) Input.touchCount;
      if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || (double) touchCount > 0.0)
      {
        this.mouseWheel *= 10f;
        if (Input.GetKeyDown(KeyCode.I))
          this.CameraMode = CameraController.CameraModes.Isometric;
        if (Input.GetKeyDown(KeyCode.F))
          this.CameraMode = CameraController.CameraModes.Follow;
        if (Input.GetKeyDown(KeyCode.S))
          this.MovementSmoothing = !this.MovementSmoothing;
        if (Input.GetMouseButton(1))
        {
          this.mouseY = Input.GetAxis("Mouse Y");
          this.mouseX = Input.GetAxis("Mouse X");
          if ((double) this.mouseY > 0.00999999977648258 || (double) this.mouseY < -0.00999999977648258)
          {
            this.ElevationAngle -= this.mouseY * this.MoveSensitivity;
            this.ElevationAngle = Mathf.Clamp(this.ElevationAngle, this.MinElevationAngle, this.MaxElevationAngle);
          }
          if ((double) this.mouseX > 0.00999999977648258 || (double) this.mouseX < -0.00999999977648258)
          {
            this.OrbitalAngle += this.mouseX * this.MoveSensitivity;
            if ((double) this.OrbitalAngle > 360.0)
              this.OrbitalAngle -= 360f;
            if ((double) this.OrbitalAngle < 0.0)
              this.OrbitalAngle += 360f;
          }
        }
        if ((double) touchCount == 1.0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
          Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;
          if ((double) deltaPosition.y > 0.00999999977648258 || (double) deltaPosition.y < -0.00999999977648258)
          {
            this.ElevationAngle -= deltaPosition.y * 0.1f;
            this.ElevationAngle = Mathf.Clamp(this.ElevationAngle, this.MinElevationAngle, this.MaxElevationAngle);
          }
          if ((double) deltaPosition.x > 0.00999999977648258 || (double) deltaPosition.x < -0.00999999977648258)
          {
            this.OrbitalAngle += deltaPosition.x * 0.1f;
            if ((double) this.OrbitalAngle > 360.0)
              this.OrbitalAngle -= 360f;
            if ((double) this.OrbitalAngle < 0.0)
              this.OrbitalAngle += 360f;
          }
        }
        RaycastHit hitInfo;
        if (Input.GetMouseButton(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 300f, 23552))
        {
          if ((Object) hitInfo.transform == (Object) this.CameraTarget)
          {
            this.OrbitalAngle = 0.0f;
          }
          else
          {
            this.CameraTarget = hitInfo.transform;
            this.OrbitalAngle = 0.0f;
            this.MovementSmoothing = this.previousSmoothing;
          }
        }
        if (Input.GetMouseButton(2))
        {
          if ((Object) this.dummyTarget == (Object) null)
          {
            this.dummyTarget = new GameObject("Camera Target").transform;
            this.dummyTarget.position = this.CameraTarget.position;
            this.dummyTarget.rotation = this.CameraTarget.rotation;
            this.CameraTarget = this.dummyTarget;
            this.previousSmoothing = this.MovementSmoothing;
            this.MovementSmoothing = false;
          }
          else if ((Object) this.dummyTarget != (Object) this.CameraTarget)
          {
            this.dummyTarget.position = this.CameraTarget.position;
            this.dummyTarget.rotation = this.CameraTarget.rotation;
            this.CameraTarget = this.dummyTarget;
            this.previousSmoothing = this.MovementSmoothing;
            this.MovementSmoothing = false;
          }
          this.mouseY = Input.GetAxis("Mouse Y");
          this.mouseX = Input.GetAxis("Mouse X");
          this.moveVector = this.cameraTransform.TransformDirection(this.mouseX, this.mouseY, 0.0f);
          this.dummyTarget.Translate(-this.moveVector, Space.World);
        }
      }
      if ((double) touchCount == 2.0)
      {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);
        float num = (touch1.position - touch1.deltaPosition - (touch2.position - touch2.deltaPosition)).magnitude - (touch1.position - touch2.position).magnitude;
        if ((double) num > 0.00999999977648258 || (double) num < -0.00999999977648258)
        {
          this.FollowDistance += num * 0.25f;
          this.FollowDistance = Mathf.Clamp(this.FollowDistance, this.MinFollowDistance, this.MaxFollowDistance);
        }
      }
      if ((double) this.mouseWheel >= -0.00999999977648258 && (double) this.mouseWheel <= 0.00999999977648258)
        return;
      this.FollowDistance -= this.mouseWheel * 5f;
      this.FollowDistance = Mathf.Clamp(this.FollowDistance, this.MinFollowDistance, this.MaxFollowDistance);
    }

    public enum CameraModes
    {
      Follow,
      Isometric,
      Free,
    }
  }
}
