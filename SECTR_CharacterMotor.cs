// Decompiled with JetBrains decompiler
// Type: SECTR_CharacterMotor
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("SECTR/Demos/SECTR Character Motor")]
[RequireComponent(typeof (CharacterController))]
public class SECTR_CharacterMotor : MonoBehaviour
{
  private bool canControl = true;
  private Vector3 lastGroundNormal = Vector3.zero;
  private Vector3 lastFootstepPosition = Vector3.zero;
  [NonSerialized]
  public Vector3 inputMoveDirection = Vector3.zero;
  [NonSerialized]
  public bool grounded = true;
  [NonSerialized]
  public Vector3 groundNormal = Vector3.zero;
  [SECTR_ToolTip("Basic movement properties.")]
  public SECTR_CharacterMotor.CharacterMotorMovement movement = new SECTR_CharacterMotor.CharacterMotorMovement();
  [SECTR_ToolTip("Jump specific movement properties.")]
  public SECTR_CharacterMotor.CharacterMotorJumping jumping = new SECTR_CharacterMotor.CharacterMotorJumping();
  [SECTR_ToolTip("Platform specific movment properties.")]
  public SECTR_CharacterMotor.CharacterMotorMovingPlatform movingPlatform = new SECTR_CharacterMotor.CharacterMotorMovingPlatform();
  public SECTR_CharacterMotor.CharacterMotorSliding sliding = new SECTR_CharacterMotor.CharacterMotorSliding();
  private Transform cachedTransform;
  private CharacterController cachedController;
  private PhysicMaterial defaultHitMaterial;
  [NonSerialized]
  public bool inputJump;

  private void Awake()
  {
    this.cachedController = this.GetComponent<CharacterController>();
    this.cachedTransform = this.transform;
    this.defaultHitMaterial = new PhysicMaterial();
    this.lastFootstepPosition = this.cachedTransform.position;
  }

  private void FixedUpdate()
  {
    if (this.movingPlatform.enabled)
    {
      if ((Object) this.movingPlatform.activePlatform != (Object) null)
      {
        if (!this.movingPlatform.newPlatform)
          this.movingPlatform.platformVelocity = (this.movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint) - this.movingPlatform.lastMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint)) / Time.deltaTime;
        this.movingPlatform.lastMatrix = this.movingPlatform.activePlatform.localToWorldMatrix;
        this.movingPlatform.newPlatform = false;
      }
      else
        this.movingPlatform.platformVelocity = Vector3.zero;
    }
    Vector3 vector3 = this.ApplyGravityAndJumping(this.ApplyInputVelocityChange(this.movement.velocity));
    Vector3 zero = Vector3.zero;
    if (this.MoveWithPlatform())
    {
      Vector3 motion = this.movingPlatform.activePlatform.TransformPoint(this.movingPlatform.activeLocalPoint) - this.movingPlatform.activeGlobalPoint;
      if (motion != Vector3.zero)
      {
        int num = (int) this.cachedController.Move(motion);
      }
      float y = (this.movingPlatform.activePlatform.rotation * this.movingPlatform.activeLocalRotation * Quaternion.Inverse(this.movingPlatform.activeGlobalRotation)).eulerAngles.y;
      if ((double) y != 0.0)
        this.cachedTransform.Rotate(0.0f, y, 0.0f);
    }
    Vector3 position = this.cachedTransform.position;
    Vector3 motion1 = vector3 * Time.deltaTime;
    float num1 = Mathf.Max(this.cachedController.stepOffset, new Vector3(motion1.x, 0.0f, motion1.z).magnitude);
    if (this.grounded)
      motion1 -= num1 * Vector3.up;
    this.movingPlatform.hitPlatform = (Transform) null;
    this.groundNormal = Vector3.zero;
    if (this.cachedController.enabled)
      this.movement.collisionFlags = this.cachedController.Move(motion1);
    this.movement.lastHitPoint = this.movement.hitPoint;
    this.lastGroundNormal = this.groundNormal;
    if (this.movingPlatform.enabled && (Object) this.movingPlatform.activePlatform != (Object) this.movingPlatform.hitPlatform && (Object) this.movingPlatform.hitPlatform != (Object) null)
    {
      this.movingPlatform.activePlatform = this.movingPlatform.hitPlatform;
      this.movingPlatform.lastMatrix = this.movingPlatform.hitPlatform.localToWorldMatrix;
      this.movingPlatform.newPlatform = true;
    }
    Vector3 rhs = new Vector3(vector3.x, 0.0f, vector3.z);
    this.movement.velocity = (this.cachedTransform.position - position) / Time.deltaTime;
    Vector3 lhs = new Vector3(this.movement.velocity.x, 0.0f, this.movement.velocity.z);
    if (rhs == Vector3.zero)
    {
      this.movement.velocity = new Vector3(0.0f, this.movement.velocity.y, 0.0f);
    }
    else
    {
      float num2 = Vector3.Dot(lhs, rhs) / rhs.sqrMagnitude;
      this.movement.velocity = rhs * Mathf.Clamp01(num2) + this.movement.velocity.y * Vector3.up;
    }
    if ((double) this.movement.velocity.y < (double) vector3.y - 0.001)
    {
      if ((double) this.movement.velocity.y < 0.0)
        this.movement.velocity.y = vector3.y;
      else
        this.jumping.holdingJumpButton = false;
    }
    if (this.grounded && !this.IsGroundedTest())
    {
      this.grounded = false;
      if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == SECTR_CharacterMotor.MovementTransferOnJump.InitTransfer || this.movingPlatform.movementTransfer == SECTR_CharacterMotor.MovementTransferOnJump.PermaTransfer))
      {
        this.movement.frameVelocity = this.movingPlatform.platformVelocity;
        this.movement.velocity += this.movingPlatform.platformVelocity;
      }
      this.SendMessage("OnFall", !((Object) this.movement.hitMaterial != (Object) null) ? (object) this.defaultHitMaterial : (object) this.movement.hitMaterial, SendMessageOptions.DontRequireReceiver);
      this.cachedTransform.position += num1 * Vector3.up;
    }
    else if (!this.grounded && this.IsGroundedTest())
    {
      this.grounded = true;
      this.jumping.jumping = false;
      this.SubtractNewPlatformVelocity();
      this.SendMessage("OnLand", !((Object) this.movement.hitMaterial != (Object) null) ? (object) this.defaultHitMaterial : (object) this.movement.hitMaterial, SendMessageOptions.DontRequireReceiver);
    }
    if (this.MoveWithPlatform())
    {
      this.movingPlatform.activeGlobalPoint = this.cachedTransform.position + Vector3.up * (this.cachedController.center.y - this.cachedController.height * 0.5f + this.cachedController.radius);
      this.movingPlatform.activeLocalPoint = this.movingPlatform.activePlatform.InverseTransformPoint(this.movingPlatform.activeGlobalPoint);
      this.movingPlatform.activeGlobalRotation = this.cachedTransform.rotation;
      this.movingPlatform.activeLocalRotation = Quaternion.Inverse(this.movingPlatform.activePlatform.rotation) * this.movingPlatform.activeGlobalRotation;
    }
    if (!this.grounded || this.TooSteep())
      return;
    if ((double) this.inputMoveDirection.sqrMagnitude > 0.0)
    {
      if ((double) Vector3.SqrMagnitude(position - this.lastFootstepPosition) < (double) this.movement.footstepDistance * (double) this.movement.footstepDistance)
        return;
      this.SendMessage("OnFootstep", !((Object) this.movement.hitMaterial != (Object) null) ? (object) this.defaultHitMaterial : (object) this.movement.hitMaterial, SendMessageOptions.DontRequireReceiver);
      this.lastFootstepPosition = position;
    }
    else
      this.lastFootstepPosition = Vector3.zero;
  }

  private Vector3 ApplyInputVelocityChange(Vector3 velocity)
  {
    if (!this.canControl)
      this.inputMoveDirection = Vector3.zero;
    Vector3 hVelocity;
    if (this.grounded && this.TooSteep())
    {
      Vector3 normalized = new Vector3(this.groundNormal.x, 0.0f, this.groundNormal.z).normalized;
      Vector3 vector3 = Vector3.Project(this.inputMoveDirection, normalized);
      hVelocity = (normalized + vector3 * this.sliding.speedControl + (this.inputMoveDirection - vector3) * this.sliding.sidewaysControl) * this.sliding.slidingSpeed;
    }
    else
      hVelocity = this.GetDesiredHorizontalVelocity();
    if (this.movingPlatform.enabled && this.movingPlatform.movementTransfer == SECTR_CharacterMotor.MovementTransferOnJump.PermaTransfer)
    {
      hVelocity += this.movement.frameVelocity;
      hVelocity.y = 0.0f;
    }
    if (this.grounded)
      hVelocity = this.AdjustGroundVelocityToNormal(hVelocity, this.groundNormal);
    else
      velocity.y = 0.0f;
    float num = this.GetMaxAcceleration(this.grounded) * Time.deltaTime;
    Vector3 vector3_1 = hVelocity - velocity;
    if ((double) vector3_1.sqrMagnitude > (double) num * (double) num)
      vector3_1 = vector3_1.normalized * num;
    if (this.grounded || this.canControl)
      velocity += vector3_1;
    if (this.grounded)
      velocity.y = Mathf.Min(velocity.y, 0.0f);
    return velocity;
  }

  private Vector3 ApplyGravityAndJumping(Vector3 velocity)
  {
    if (!this.inputJump || !this.canControl)
    {
      this.jumping.holdingJumpButton = false;
      this.jumping.lastButtonDownTime = -100f;
    }
    if (this.inputJump && (double) this.jumping.lastButtonDownTime < 0.0 && this.canControl)
      this.jumping.lastButtonDownTime = Time.time;
    if (this.grounded)
    {
      velocity.y = Mathf.Min(0.0f, velocity.y) - this.movement.gravity * Time.deltaTime;
    }
    else
    {
      velocity.y = this.movement.velocity.y - this.movement.gravity * Time.deltaTime;
      if (this.jumping.jumping && this.jumping.holdingJumpButton && (double) Time.time < (double) this.jumping.lastStartTime + (double) this.jumping.extraHeight / (double) this.CalculateJumpVerticalSpeed(this.jumping.baseHeight))
        velocity += this.jumping.jumpDir * this.movement.gravity * Time.deltaTime;
      velocity.y = Mathf.Max(velocity.y, -this.movement.maxFallSpeed);
    }
    if (this.grounded)
    {
      if (this.jumping.enabled && this.canControl && (double) Time.time - (double) this.jumping.lastButtonDownTime < 0.2)
      {
        this.grounded = false;
        this.jumping.jumping = true;
        this.jumping.lastStartTime = Time.time;
        this.jumping.lastButtonDownTime = -100f;
        this.jumping.holdingJumpButton = true;
        this.jumping.jumpDir = Vector3.Slerp(Vector3.up, this.groundNormal, !this.TooSteep() ? this.jumping.perpAmount : this.jumping.steepPerpAmount);
        velocity.y = 0.0f;
        velocity += this.jumping.jumpDir * this.CalculateJumpVerticalSpeed(this.jumping.baseHeight);
        if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == SECTR_CharacterMotor.MovementTransferOnJump.InitTransfer || this.movingPlatform.movementTransfer == SECTR_CharacterMotor.MovementTransferOnJump.PermaTransfer))
        {
          this.movement.frameVelocity = this.movingPlatform.platformVelocity;
          velocity += this.movingPlatform.platformVelocity;
        }
        this.SendMessage("OnJump", !((Object) this.movement.hitMaterial != (Object) null) ? (object) this.defaultHitMaterial : (object) this.movement.hitMaterial, SendMessageOptions.DontRequireReceiver);
      }
      else
        this.jumping.holdingJumpButton = false;
    }
    return velocity;
  }

  private void OnControllerColliderHit(ControllerColliderHit hit)
  {
    if ((double) hit.normal.y > 0.0 && (double) hit.normal.y > (double) this.groundNormal.y && (double) hit.moveDirection.y < 0.0)
    {
      this.groundNormal = (double) (hit.point - this.movement.lastHitPoint).sqrMagnitude > 0.001 || this.lastGroundNormal == Vector3.zero ? hit.normal : this.lastGroundNormal;
      this.movingPlatform.hitPlatform = hit.collider.transform;
      this.movement.hitPoint = hit.point;
      this.movement.hitMaterial = hit.collider.GetType() != typeof (TerrainCollider) ? hit.collider.sharedMaterial : hit.collider.material;
      this.movement.frameVelocity = Vector3.zero;
    }
    Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
    if (!((Object) attachedRigidbody != (Object) null) || attachedRigidbody.isKinematic || (double) hit.moveDirection.y < -0.300000011920929)
      return;
    Vector3 vector3 = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);
    attachedRigidbody.velocity = vector3 * this.movement.pushPower;
  }

  [DebuggerHidden]
  private IEnumerator SubtractNewPlatformVelocity()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SECTR_CharacterMotor.\u003CSubtractNewPlatformVelocity\u003Ec__Iterator0() { \u0024this = this };
  }

  private bool MoveWithPlatform()
  {
    if (this.movingPlatform.enabled && (this.grounded || this.movingPlatform.movementTransfer == SECTR_CharacterMotor.MovementTransferOnJump.PermaLocked))
      return (Object) this.movingPlatform.activePlatform != (Object) null;
    return false;
  }

  private Vector3 GetDesiredHorizontalVelocity()
  {
    Vector3 desiredMovementDirection = this.cachedTransform.InverseTransformDirection(this.inputMoveDirection);
    float num = this.MaxSpeedInDirection(desiredMovementDirection);
    if (this.grounded)
    {
      float time = Mathf.Asin(this.movement.velocity.normalized.y) * 57.29578f;
      num *= this.movement.slopeSpeedMultiplier.Evaluate(time);
    }
    return this.cachedTransform.TransformDirection(desiredMovementDirection * num);
  }

  private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
  {
    return Vector3.Cross(Vector3.Cross(Vector3.up, hVelocity), groundNormal).normalized * hVelocity.magnitude;
  }

  private bool IsGroundedTest()
  {
    return (double) this.groundNormal.y > 0.01;
  }

  private float GetMaxAcceleration(bool grounded)
  {
    if (grounded)
      return this.movement.maxGroundAcceleration;
    return this.movement.maxAirAcceleration;
  }

  private float CalculateJumpVerticalSpeed(float targetJumpHeight)
  {
    return Mathf.Sqrt(2f * targetJumpHeight * this.movement.gravity);
  }

  private bool TooSteep()
  {
    return (double) this.groundNormal.y <= (double) Mathf.Cos(this.cachedController.slopeLimit * ((float) Math.PI / 180f));
  }

  private float MaxSpeedInDirection(Vector3 desiredMovementDirection)
  {
    if (!(desiredMovementDirection != Vector3.zero))
      return 0.0f;
    float num = ((double) desiredMovementDirection.z <= 0.0 ? this.movement.maxBackwardsSpeed : this.movement.maxForwardSpeed) / this.movement.maxSidewaysSpeed;
    Vector3 normalized = new Vector3(desiredMovementDirection.x, 0.0f, desiredMovementDirection.z / num).normalized;
    return new Vector3(normalized.x, 0.0f, normalized.z * num).magnitude * this.movement.maxSidewaysSpeed;
  }

  [Serializable]
  public class CharacterMotorMovement
  {
    public float maxForwardSpeed = 3f;
    public float maxSidewaysSpeed = 2f;
    public float maxBackwardsSpeed = 2f;
    public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe[3]{ new Keyframe(-90f, 1f), new Keyframe(0.0f, 1f), new Keyframe(90f, 0.0f) });
    public float maxGroundAcceleration = 30f;
    public float maxAirAcceleration = 20f;
    public float gravity = 9.81f;
    public float maxFallSpeed = 20f;
    public float footstepDistance = 1f;
    public float pushPower = 2f;
    [NonSerialized]
    public Vector3 frameVelocity = Vector3.zero;
    [NonSerialized]
    public Vector3 hitPoint = Vector3.zero;
    [NonSerialized]
    public Vector3 lastHitPoint = new Vector3(float.PositiveInfinity, 0.0f, 0.0f);
    [NonSerialized]
    public CollisionFlags collisionFlags;
    [NonSerialized]
    public Vector3 velocity;
    [NonSerialized]
    public PhysicMaterial hitMaterial;
  }

  public enum MovementTransferOnJump
  {
    None,
    InitTransfer,
    PermaTransfer,
    PermaLocked,
  }

  [Serializable]
  public class CharacterMotorJumping
  {
    public bool enabled = true;
    public float baseHeight = 1f;
    public float extraHeight = 4.1f;
    public float steepPerpAmount = 0.5f;
    [NonSerialized]
    public float lastButtonDownTime = -100f;
    [NonSerialized]
    public Vector3 jumpDir = Vector3.up;
    public float perpAmount;
    [NonSerialized]
    public bool jumping;
    [NonSerialized]
    public bool holdingJumpButton;
    [NonSerialized]
    public float lastStartTime;
  }

  [Serializable]
  public class CharacterMotorMovingPlatform
  {
    public bool enabled = true;
    public SECTR_CharacterMotor.MovementTransferOnJump movementTransfer = SECTR_CharacterMotor.MovementTransferOnJump.PermaTransfer;
    [NonSerialized]
    public Transform hitPlatform;
    [NonSerialized]
    public Transform activePlatform;
    [NonSerialized]
    public Vector3 activeLocalPoint;
    [NonSerialized]
    public Vector3 activeGlobalPoint;
    [NonSerialized]
    public Quaternion activeLocalRotation;
    [NonSerialized]
    public Quaternion activeGlobalRotation;
    [NonSerialized]
    public Matrix4x4 lastMatrix;
    [NonSerialized]
    public Vector3 platformVelocity;
    [NonSerialized]
    public bool newPlatform;
  }

  [Serializable]
  public class CharacterMotorSliding
  {
    public bool enabled = true;
    public float slidingSpeed = 15f;
    public float sidewaysControl = 1f;
    public float speedControl = 0.4f;
  }
}
