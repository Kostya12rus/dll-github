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
    private Transform cameraTransform;
    private Transform dummyTarget;
    public Transform CameraTarget;
    public float FollowDistance;
    public float MaxFollowDistance;
    public float MinFollowDistance;
    public float ElevationAngle;
    public float MaxElevationAngle;
    public float MinElevationAngle;
    public float OrbitalAngle;
    public CameraController.CameraModes CameraMode;
    public bool MovementSmoothing;
    public bool RotationSmoothing;
    private bool previousSmoothing;
    public float MovementSmoothingValue;
    public float RotationSmoothingValue;
    public float MoveSensitivity;
    private Vector3 currentVelocity;
    private Vector3 desiredPosition;
    private float mouseX;
    private float mouseY;
    private Vector3 moveVector;
    private float mouseWheel;
    private const string event_SmoothingValue = "Slider - Smoothing Value";
    private const string event_FollowDistance = "Slider - Camera Zoom";

    public CameraController()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      if (QualitySettings.get_vSyncCount() > 0)
        Application.set_targetFrameRate(60);
      else
        Application.set_targetFrameRate(-1);
      if (Application.get_platform() == 8 || Application.get_platform() == 11)
        Input.set_simulateMouseWithTouches(false);
      this.cameraTransform = ((Component) this).get_transform();
      this.previousSmoothing = this.MovementSmoothing;
    }

    private void Start()
    {
      if (!Object.op_Equality((Object) this.CameraTarget, (Object) null))
        return;
      this.dummyTarget = new GameObject("Camera Target").get_transform();
      this.CameraTarget = this.dummyTarget;
    }

    private void LateUpdate()
    {
      this.GetPlayerInput();
      if (!Object.op_Inequality((Object) this.CameraTarget, (Object) null))
        return;
      if (this.CameraMode == CameraController.CameraModes.Isometric)
        this.desiredPosition = Vector3.op_Addition(this.CameraTarget.get_position(), Quaternion.op_Multiply(Quaternion.Euler(this.ElevationAngle, this.OrbitalAngle, 0.0f), new Vector3(0.0f, 0.0f, -this.FollowDistance)));
      else if (this.CameraMode == CameraController.CameraModes.Follow)
        this.desiredPosition = Vector3.op_Addition(this.CameraTarget.get_position(), this.CameraTarget.TransformDirection(Quaternion.op_Multiply(Quaternion.Euler(this.ElevationAngle, this.OrbitalAngle, 0.0f), new Vector3(0.0f, 0.0f, -this.FollowDistance))));
      if (this.MovementSmoothing)
        this.cameraTransform.set_position(Vector3.SmoothDamp(this.cameraTransform.get_position(), this.desiredPosition, ref this.currentVelocity, this.MovementSmoothingValue * Time.get_fixedDeltaTime()));
      else
        this.cameraTransform.set_position(this.desiredPosition);
      if (this.RotationSmoothing)
        this.cameraTransform.set_rotation(Quaternion.Lerp(this.cameraTransform.get_rotation(), Quaternion.LookRotation(Vector3.op_Subtraction(this.CameraTarget.get_position(), this.cameraTransform.get_position())), this.RotationSmoothingValue * Time.get_deltaTime()));
      else
        this.cameraTransform.LookAt(this.CameraTarget);
    }

    private void GetPlayerInput()
    {
      this.moveVector = Vector3.get_zero();
      this.mouseWheel = Input.GetAxis("Mouse ScrollWheel");
      float touchCount = (float) Input.get_touchCount();
      if (Input.GetKey((KeyCode) 304) || Input.GetKey((KeyCode) 303) || (double) touchCount > 0.0)
      {
        this.mouseWheel *= 10f;
        if (Input.GetKeyDown((KeyCode) 105))
          this.CameraMode = CameraController.CameraModes.Isometric;
        if (Input.GetKeyDown((KeyCode) 102))
          this.CameraMode = CameraController.CameraModes.Follow;
        if (Input.GetKeyDown((KeyCode) 115))
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
        if ((double) touchCount == 1.0)
        {
          Touch touch1 = Input.GetTouch(0);
          if (((Touch) ref touch1).get_phase() == 1)
          {
            Touch touch2 = Input.GetTouch(0);
            Vector2 deltaPosition = ((Touch) ref touch2).get_deltaPosition();
            if (deltaPosition.y > 0.00999999977648258 || deltaPosition.y < -0.00999999977648258)
            {
              this.ElevationAngle -= (float) (deltaPosition.y * 0.100000001490116);
              this.ElevationAngle = Mathf.Clamp(this.ElevationAngle, this.MinElevationAngle, this.MaxElevationAngle);
            }
            if (deltaPosition.x > 0.00999999977648258 || deltaPosition.x < -0.00999999977648258)
            {
              this.OrbitalAngle += (float) (deltaPosition.x * 0.100000001490116);
              if ((double) this.OrbitalAngle > 360.0)
                this.OrbitalAngle -= 360f;
              if ((double) this.OrbitalAngle < 0.0)
                this.OrbitalAngle += 360f;
            }
          }
        }
        RaycastHit raycastHit;
        if (Input.GetMouseButton(0) && Physics.Raycast(Camera.get_main().ScreenPointToRay(Input.get_mousePosition()), ref raycastHit, 300f, 23552))
        {
          if (Object.op_Equality((Object) ((RaycastHit) ref raycastHit).get_transform(), (Object) this.CameraTarget))
          {
            this.OrbitalAngle = 0.0f;
          }
          else
          {
            this.CameraTarget = ((RaycastHit) ref raycastHit).get_transform();
            this.OrbitalAngle = 0.0f;
            this.MovementSmoothing = this.previousSmoothing;
          }
        }
        if (Input.GetMouseButton(2))
        {
          if (Object.op_Equality((Object) this.dummyTarget, (Object) null))
          {
            this.dummyTarget = new GameObject("Camera Target").get_transform();
            this.dummyTarget.set_position(this.CameraTarget.get_position());
            this.dummyTarget.set_rotation(this.CameraTarget.get_rotation());
            this.CameraTarget = this.dummyTarget;
            this.previousSmoothing = this.MovementSmoothing;
            this.MovementSmoothing = false;
          }
          else if (Object.op_Inequality((Object) this.dummyTarget, (Object) this.CameraTarget))
          {
            this.dummyTarget.set_position(this.CameraTarget.get_position());
            this.dummyTarget.set_rotation(this.CameraTarget.get_rotation());
            this.CameraTarget = this.dummyTarget;
            this.previousSmoothing = this.MovementSmoothing;
            this.MovementSmoothing = false;
          }
          this.mouseY = Input.GetAxis("Mouse Y");
          this.mouseX = Input.GetAxis("Mouse X");
          this.moveVector = this.cameraTransform.TransformDirection(this.mouseX, this.mouseY, 0.0f);
          this.dummyTarget.Translate(Vector3.op_UnaryNegation(this.moveVector), (Space) 0);
        }
      }
      if ((double) touchCount == 2.0)
      {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);
        Vector2 vector2_1 = Vector2.op_Subtraction(Vector2.op_Subtraction(((Touch) ref touch1).get_position(), ((Touch) ref touch1).get_deltaPosition()), Vector2.op_Subtraction(((Touch) ref touch2).get_position(), ((Touch) ref touch2).get_deltaPosition()));
        float magnitude1 = ((Vector2) ref vector2_1).get_magnitude();
        Vector2 vector2_2 = Vector2.op_Subtraction(((Touch) ref touch1).get_position(), ((Touch) ref touch2).get_position());
        float magnitude2 = ((Vector2) ref vector2_2).get_magnitude();
        float num = magnitude1 - magnitude2;
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
