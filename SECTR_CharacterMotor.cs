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
  private bool canControl;
  private Vector3 lastGroundNormal;
  private Transform cachedTransform;
  private CharacterController cachedController;
  private Vector3 lastFootstepPosition;
  private PhysicMaterial defaultHitMaterial;
  [NonSerialized]
  public Vector3 inputMoveDirection;
  [NonSerialized]
  public bool inputJump;
  [NonSerialized]
  public bool grounded;
  [NonSerialized]
  public Vector3 groundNormal;
  [SECTR_ToolTip("Basic movement properties.")]
  public SECTR_CharacterMotor.CharacterMotorMovement movement;
  [SECTR_ToolTip("Jump specific movement properties.")]
  public SECTR_CharacterMotor.CharacterMotorJumping jumping;
  [SECTR_ToolTip("Platform specific movment properties.")]
  public SECTR_CharacterMotor.CharacterMotorMovingPlatform movingPlatform;
  public SECTR_CharacterMotor.CharacterMotorSliding sliding;

  public SECTR_CharacterMotor()
  {
    base.\u002Ector();
  }

  private void Awake()
  {
    this.cachedController = (CharacterController) ((Component) this).GetComponent<CharacterController>();
    this.cachedTransform = ((Component) this).get_transform();
    this.defaultHitMaterial = new PhysicMaterial();
    this.lastFootstepPosition = this.cachedTransform.get_position();
  }

  private void FixedUpdate()
  {
    if (this.movingPlatform.enabled)
    {
      if (Object.op_Inequality((Object) this.movingPlatform.activePlatform, (Object) null))
      {
        if (!this.movingPlatform.newPlatform)
        {
          SECTR_CharacterMotor.CharacterMotorMovingPlatform movingPlatform = this.movingPlatform;
          Matrix4x4 localToWorldMatrix = this.movingPlatform.activePlatform.get_localToWorldMatrix();
          Vector3 vector3 = Vector3.op_Division(Vector3.op_Subtraction(((Matrix4x4) ref localToWorldMatrix).MultiplyPoint3x4(this.movingPlatform.activeLocalPoint), ((Matrix4x4) ref this.movingPlatform.lastMatrix).MultiplyPoint3x4(this.movingPlatform.activeLocalPoint)), Time.get_deltaTime());
          movingPlatform.platformVelocity = vector3;
        }
        this.movingPlatform.lastMatrix = this.movingPlatform.activePlatform.get_localToWorldMatrix();
        this.movingPlatform.newPlatform = false;
      }
      else
        this.movingPlatform.platformVelocity = Vector3.get_zero();
    }
    Vector3 vector3_1 = this.ApplyGravityAndJumping(this.ApplyInputVelocityChange(this.movement.velocity));
    Vector3.get_zero();
    if (this.MoveWithPlatform())
    {
      Vector3 vector3_2 = Vector3.op_Subtraction(this.movingPlatform.activePlatform.TransformPoint(this.movingPlatform.activeLocalPoint), this.movingPlatform.activeGlobalPoint);
      if (Vector3.op_Inequality(vector3_2, Vector3.get_zero()))
        this.cachedController.Move(vector3_2);
      Quaternion quaternion = Quaternion.op_Multiply(Quaternion.op_Multiply(this.movingPlatform.activePlatform.get_rotation(), this.movingPlatform.activeLocalRotation), Quaternion.Inverse(this.movingPlatform.activeGlobalRotation));
      float y = (float) ((Quaternion) ref quaternion).get_eulerAngles().y;
      if ((double) y != 0.0)
        this.cachedTransform.Rotate(0.0f, y, 0.0f);
    }
    Vector3 position = this.cachedTransform.get_position();
    Vector3 vector3_3 = Vector3.op_Multiply(vector3_1, Time.get_deltaTime());
    double stepOffset = (double) this.cachedController.get_stepOffset();
    Vector3 vector3_4;
    ((Vector3) ref vector3_4).\u002Ector((float) vector3_3.x, 0.0f, (float) vector3_3.z);
    double magnitude = (double) ((Vector3) ref vector3_4).get_magnitude();
    float num1 = Mathf.Max((float) stepOffset, (float) magnitude);
    if (this.grounded)
      vector3_3 = Vector3.op_Subtraction(vector3_3, Vector3.op_Multiply(num1, Vector3.get_up()));
    this.movingPlatform.hitPlatform = (Transform) null;
    this.groundNormal = Vector3.get_zero();
    if (((Collider) this.cachedController).get_enabled())
      this.movement.collisionFlags = this.cachedController.Move(vector3_3);
    this.movement.lastHitPoint = this.movement.hitPoint;
    this.lastGroundNormal = this.groundNormal;
    if (this.movingPlatform.enabled && Object.op_Inequality((Object) this.movingPlatform.activePlatform, (Object) this.movingPlatform.hitPlatform) && Object.op_Inequality((Object) this.movingPlatform.hitPlatform, (Object) null))
    {
      this.movingPlatform.activePlatform = this.movingPlatform.hitPlatform;
      this.movingPlatform.lastMatrix = this.movingPlatform.hitPlatform.get_localToWorldMatrix();
      this.movingPlatform.newPlatform = true;
    }
    Vector3 vector3_5;
    ((Vector3) ref vector3_5).\u002Ector((float) vector3_1.x, 0.0f, (float) vector3_1.z);
    this.movement.velocity = Vector3.op_Division(Vector3.op_Subtraction(this.cachedTransform.get_position(), position), Time.get_deltaTime());
    Vector3 vector3_6;
    ((Vector3) ref vector3_6).\u002Ector((float) this.movement.velocity.x, 0.0f, (float) this.movement.velocity.z);
    if (Vector3.op_Equality(vector3_5, Vector3.get_zero()))
    {
      this.movement.velocity = new Vector3(0.0f, (float) this.movement.velocity.y, 0.0f);
    }
    else
    {
      float num2 = Vector3.Dot(vector3_6, vector3_5) / ((Vector3) ref vector3_5).get_sqrMagnitude();
      this.movement.velocity = Vector3.op_Addition(Vector3.op_Multiply(vector3_5, Mathf.Clamp01(num2)), Vector3.op_Multiply((float) this.movement.velocity.y, Vector3.get_up()));
    }
    if ((double) this.movement.velocity.y < (double) vector3_1.y - 0.001)
    {
      if (this.movement.velocity.y < 0.0)
        this.movement.velocity.y = vector3_1.y;
      else
        this.jumping.holdingJumpButton = false;
    }
    if (this.grounded && !this.IsGroundedTest())
    {
      this.grounded = false;
      if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == SECTR_CharacterMotor.MovementTransferOnJump.InitTransfer || this.movingPlatform.movementTransfer == SECTR_CharacterMotor.MovementTransferOnJump.PermaTransfer))
      {
        this.movement.frameVelocity = this.movingPlatform.platformVelocity;
        SECTR_CharacterMotor.CharacterMotorMovement movement = this.movement;
        movement.velocity = Vector3.op_Addition(movement.velocity, this.movingPlatform.platformVelocity);
      }
      ((Component) this).SendMessage("OnFall", !Object.op_Inequality((Object) this.movement.hitMaterial, (Object) null) ? (object) this.defaultHitMaterial : (object) this.movement.hitMaterial, (SendMessageOptions) 1);
      Transform cachedTransform = this.cachedTransform;
      cachedTransform.set_position(Vector3.op_Addition(cachedTransform.get_position(), Vector3.op_Multiply(num1, Vector3.get_up())));
    }
    else if (!this.grounded && this.IsGroundedTest())
    {
      this.grounded = true;
      this.jumping.jumping = false;
      this.SubtractNewPlatformVelocity();
      ((Component) this).SendMessage("OnLand", !Object.op_Inequality((Object) this.movement.hitMaterial, (Object) null) ? (object) this.defaultHitMaterial : (object) this.movement.hitMaterial, (SendMessageOptions) 1);
    }
    if (this.MoveWithPlatform())
    {
      this.movingPlatform.activeGlobalPoint = Vector3.op_Addition(this.cachedTransform.get_position(), Vector3.op_Multiply(Vector3.get_up(), (float) (this.cachedController.get_center().y - (double) this.cachedController.get_height() * 0.5) + this.cachedController.get_radius()));
      this.movingPlatform.activeLocalPoint = this.movingPlatform.activePlatform.InverseTransformPoint(this.movingPlatform.activeGlobalPoint);
      this.movingPlatform.activeGlobalRotation = this.cachedTransform.get_rotation();
      this.movingPlatform.activeLocalRotation = Quaternion.op_Multiply(Quaternion.Inverse(this.movingPlatform.activePlatform.get_rotation()), this.movingPlatform.activeGlobalRotation);
    }
    if (!this.grounded || this.TooSteep())
      return;
    if ((double) ((Vector3) ref this.inputMoveDirection).get_sqrMagnitude() > 0.0)
    {
      if ((double) Vector3.SqrMagnitude(Vector3.op_Subtraction(position, this.lastFootstepPosition)) < (double) this.movement.footstepDistance * (double) this.movement.footstepDistance)
        return;
      ((Component) this).SendMessage("OnFootstep", !Object.op_Inequality((Object) this.movement.hitMaterial, (Object) null) ? (object) this.defaultHitMaterial : (object) this.movement.hitMaterial, (SendMessageOptions) 1);
      this.lastFootstepPosition = position;
    }
    else
      this.lastFootstepPosition = Vector3.get_zero();
  }

  private Vector3 ApplyInputVelocityChange(Vector3 velocity)
  {
    if (!this.canControl)
      this.inputMoveDirection = Vector3.get_zero();
    Vector3 hVelocity;
    if (this.grounded && this.TooSteep())
    {
      Vector3 vector3_1;
      ((Vector3) ref vector3_1).\u002Ector((float) this.groundNormal.x, 0.0f, (float) this.groundNormal.z);
      Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
      Vector3 vector3_2 = Vector3.Project(this.inputMoveDirection, normalized);
      hVelocity = Vector3.op_Multiply(Vector3.op_Addition(Vector3.op_Addition(normalized, Vector3.op_Multiply(vector3_2, this.sliding.speedControl)), Vector3.op_Multiply(Vector3.op_Subtraction(this.inputMoveDirection, vector3_2), this.sliding.sidewaysControl)), this.sliding.slidingSpeed);
    }
    else
      hVelocity = this.GetDesiredHorizontalVelocity();
    if (this.movingPlatform.enabled && this.movingPlatform.movementTransfer == SECTR_CharacterMotor.MovementTransferOnJump.PermaTransfer)
    {
      hVelocity = Vector3.op_Addition(hVelocity, this.movement.frameVelocity);
      hVelocity.y = (__Null) 0.0;
    }
    if (this.grounded)
      hVelocity = this.AdjustGroundVelocityToNormal(hVelocity, this.groundNormal);
    else
      velocity.y = (__Null) 0.0;
    float num = this.GetMaxAcceleration(this.grounded) * Time.get_deltaTime();
    Vector3 vector3 = Vector3.op_Subtraction(hVelocity, velocity);
    if ((double) ((Vector3) ref vector3).get_sqrMagnitude() > (double) num * (double) num)
      vector3 = Vector3.op_Multiply(((Vector3) ref vector3).get_normalized(), num);
    if (this.grounded || this.canControl)
      velocity = Vector3.op_Addition(velocity, vector3);
    if (this.grounded)
      velocity.y = (__Null) (double) Mathf.Min((float) velocity.y, 0.0f);
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
      this.jumping.lastButtonDownTime = Time.get_time();
    if (this.grounded)
    {
      velocity.y = (__Null) ((double) Mathf.Min(0.0f, (float) velocity.y) - (double) this.movement.gravity * (double) Time.get_deltaTime());
    }
    else
    {
      velocity.y = (__Null) (this.movement.velocity.y - (double) this.movement.gravity * (double) Time.get_deltaTime());
      if (this.jumping.jumping && this.jumping.holdingJumpButton && (double) Time.get_time() < (double) this.jumping.lastStartTime + (double) this.jumping.extraHeight / (double) this.CalculateJumpVerticalSpeed(this.jumping.baseHeight))
        velocity = Vector3.op_Addition(velocity, Vector3.op_Multiply(Vector3.op_Multiply(this.jumping.jumpDir, this.movement.gravity), Time.get_deltaTime()));
      velocity.y = (__Null) (double) Mathf.Max((float) velocity.y, -this.movement.maxFallSpeed);
    }
    if (this.grounded)
    {
      if (this.jumping.enabled && this.canControl && (double) Time.get_time() - (double) this.jumping.lastButtonDownTime < 0.2)
      {
        this.grounded = false;
        this.jumping.jumping = true;
        this.jumping.lastStartTime = Time.get_time();
        this.jumping.lastButtonDownTime = -100f;
        this.jumping.holdingJumpButton = true;
        this.jumping.jumpDir = Vector3.Slerp(Vector3.get_up(), this.groundNormal, !this.TooSteep() ? this.jumping.perpAmount : this.jumping.steepPerpAmount);
        velocity.y = (__Null) 0.0;
        velocity = Vector3.op_Addition(velocity, Vector3.op_Multiply(this.jumping.jumpDir, this.CalculateJumpVerticalSpeed(this.jumping.baseHeight)));
        if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == SECTR_CharacterMotor.MovementTransferOnJump.InitTransfer || this.movingPlatform.movementTransfer == SECTR_CharacterMotor.MovementTransferOnJump.PermaTransfer))
        {
          this.movement.frameVelocity = this.movingPlatform.platformVelocity;
          velocity = Vector3.op_Addition(velocity, this.movingPlatform.platformVelocity);
        }
        ((Component) this).SendMessage("OnJump", !Object.op_Inequality((Object) this.movement.hitMaterial, (Object) null) ? (object) this.defaultHitMaterial : (object) this.movement.hitMaterial, (SendMessageOptions) 1);
      }
      else
        this.jumping.holdingJumpButton = false;
    }
    return velocity;
  }

  private void OnControllerColliderHit(ControllerColliderHit hit)
  {
    if (hit.get_normal().y > 0.0 && hit.get_normal().y > this.groundNormal.y && hit.get_moveDirection().y < 0.0)
    {
      Vector3 vector3 = Vector3.op_Subtraction(hit.get_point(), this.movement.lastHitPoint);
      this.groundNormal = (double) ((Vector3) ref vector3).get_sqrMagnitude() > 0.001 || Vector3.op_Equality(this.lastGroundNormal, Vector3.get_zero()) ? hit.get_normal() : this.lastGroundNormal;
      this.movingPlatform.hitPlatform = ((Component) hit.get_collider()).get_transform();
      this.movement.hitPoint = hit.get_point();
      this.movement.hitMaterial = ((object) hit.get_collider()).GetType() != typeof (TerrainCollider) ? hit.get_collider().get_sharedMaterial() : hit.get_collider().get_material();
      this.movement.frameVelocity = Vector3.get_zero();
    }
    Rigidbody attachedRigidbody = hit.get_collider().get_attachedRigidbody();
    if (!Object.op_Inequality((Object) attachedRigidbody, (Object) null) || attachedRigidbody.get_isKinematic() || hit.get_moveDirection().y < -0.300000011920929)
      return;
    Vector3 vector3_1;
    ((Vector3) ref vector3_1).\u002Ector((float) hit.get_moveDirection().x, 0.0f, (float) hit.get_moveDirection().z);
    attachedRigidbody.set_velocity(Vector3.op_Multiply(vector3_1, this.movement.pushPower));
  }

  [DebuggerHidden]
  private IEnumerator SubtractNewPlatformVelocity()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SECTR_CharacterMotor.\u003CSubtractNewPlatformVelocity\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private bool MoveWithPlatform()
  {
    if (this.movingPlatform.enabled && (this.grounded || this.movingPlatform.movementTransfer == SECTR_CharacterMotor.MovementTransferOnJump.PermaLocked))
      return Object.op_Inequality((Object) this.movingPlatform.activePlatform, (Object) null);
    return false;
  }

  private Vector3 GetDesiredHorizontalVelocity()
  {
    Vector3 desiredMovementDirection = this.cachedTransform.InverseTransformDirection(this.inputMoveDirection);
    float num1 = this.MaxSpeedInDirection(desiredMovementDirection);
    if (this.grounded)
    {
      float num2 = Mathf.Asin((float) ((Vector3) ref this.movement.velocity).get_normalized().y) * 57.29578f;
      num1 *= this.movement.slopeSpeedMultiplier.Evaluate(num2);
    }
    return this.cachedTransform.TransformDirection(Vector3.op_Multiply(desiredMovementDirection, num1));
  }

  private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
  {
    Vector3 vector3 = Vector3.Cross(Vector3.Cross(Vector3.get_up(), hVelocity), groundNormal);
    return Vector3.op_Multiply(((Vector3) ref vector3).get_normalized(), ((Vector3) ref hVelocity).get_magnitude());
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
    return this.groundNormal.y <= (double) Mathf.Cos(this.cachedController.get_slopeLimit() * ((float) Math.PI / 180f));
  }

  private float MaxSpeedInDirection(Vector3 desiredMovementDirection)
  {
    if (!Vector3.op_Inequality(desiredMovementDirection, Vector3.get_zero()))
      return 0.0f;
    float num = (desiredMovementDirection.z <= 0.0 ? this.movement.maxBackwardsSpeed : this.movement.maxForwardSpeed) / this.movement.maxSidewaysSpeed;
    Vector3 vector3_1;
    ((Vector3) ref vector3_1).\u002Ector((float) desiredMovementDirection.x, 0.0f, (float) desiredMovementDirection.z / num);
    Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
    Vector3 vector3_2;
    ((Vector3) ref vector3_2).\u002Ector((float) normalized.x, 0.0f, (float) normalized.z * num);
    return ((Vector3) ref vector3_2).get_magnitude() * this.movement.maxSidewaysSpeed;
  }

  [Serializable]
  public class CharacterMotorMovement
  {
    public float maxForwardSpeed = 3f;
    public float maxSidewaysSpeed = 2f;
    public float maxBackwardsSpeed = 2f;
    public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe[3]
    {
      new Keyframe(-90f, 1f),
      new Keyframe(0.0f, 1f),
      new Keyframe(90f, 0.0f)
    });
    public float maxGroundAcceleration = 30f;
    public float maxAirAcceleration = 20f;
    public float gravity = 9.81f;
    public float maxFallSpeed = 20f;
    public float footstepDistance = 1f;
    public float pushPower = 2f;
    [NonSerialized]
    public Vector3 frameVelocity = Vector3.get_zero();
    [NonSerialized]
    public Vector3 hitPoint = Vector3.get_zero();
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
    public Vector3 jumpDir = Vector3.get_up();
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
