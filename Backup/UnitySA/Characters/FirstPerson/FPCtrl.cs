// Decompiled with JetBrains decompiler
// Type: UnitySA.Characters.FirstPerson.FPCtrl
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;
using UnitySA.Utility;

namespace UnitySA.Characters.FirstPerson
{
  [RequireComponent(typeof (CharacterController))]
  [RequireComponent(typeof (AudioSource))]
  public class FPCtrl : MonoBehaviour
  {
    [SerializeField]
    private bool m_IsWalking;
    [SerializeField]
    private float m_WalkSpeed;
    [SerializeField]
    private float m_RunSpeed;
    [SerializeField]
    [Range(0.0f, 1f)]
    private float m_RunstepLenghten;
    [SerializeField]
    private float m_JumpSpeed;
    [SerializeField]
    private float m_StickToGroundForce;
    [SerializeField]
    private float m_GravityMultiplier;
    [SerializeField]
    private MLook m_MouseLook;
    [SerializeField]
    private bool m_UseFovKick;
    [SerializeField]
    private FOVZoom m_FovKick;
    [SerializeField]
    private bool m_UseHeadBob;
    [SerializeField]
    private CurveCtrlBob m_HeadBob;
    [SerializeField]
    private LerpCtrlBob m_JumpBob;
    [SerializeField]
    private float m_StepInterval;
    private Camera m_Camera;
    private bool m_Jump;
    private float m_YRotation;
    private Vector2 m_Input;
    private Vector3 m_MoveDir;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;
    private float m_StepCycle;
    private float m_NextStep;
    private bool m_Jumping;

    public FPCtrl()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      this.m_CharacterController = (CharacterController) ((Component) this).GetComponent<CharacterController>();
      this.m_Camera = Camera.get_main();
      this.m_OriginalCameraPosition = ((Component) this.m_Camera).get_transform().get_localPosition();
      this.m_FovKick.Setup(this.m_Camera);
      this.m_HeadBob.Setup(this.m_Camera, this.m_StepInterval);
      this.m_StepCycle = 0.0f;
      this.m_NextStep = this.m_StepCycle / 2f;
      this.m_Jumping = false;
      this.m_MouseLook.Init(((Component) this).get_transform(), ((Component) this.m_Camera).get_transform());
    }

    private void Update()
    {
      this.RotateView();
      if (!this.m_Jump)
        this.m_Jump = Input.GetButtonDown("Jump");
      if (!this.m_PreviouslyGrounded && this.m_CharacterController.get_isGrounded())
      {
        this.StartCoroutine(this.m_JumpBob.DoBobCycle());
        this.m_MoveDir.y = (__Null) 0.0;
        this.m_Jumping = false;
      }
      if (!this.m_CharacterController.get_isGrounded() && !this.m_Jumping && this.m_PreviouslyGrounded)
        this.m_MoveDir.y = (__Null) 0.0;
      this.m_PreviouslyGrounded = this.m_CharacterController.get_isGrounded();
    }

    private void FixedUpdate()
    {
      float speed;
      this.GetInput(out speed);
      Vector3 vector3_1 = Vector3.op_Addition(Vector3.op_Multiply(((Component) this).get_transform().get_forward(), (float) this.m_Input.y), Vector3.op_Multiply(((Component) this).get_transform().get_right(), (float) this.m_Input.x));
      RaycastHit raycastHit;
      Physics.SphereCast(((Component) this).get_transform().get_position(), this.m_CharacterController.get_radius(), Vector3.get_down(), ref raycastHit, this.m_CharacterController.get_height() / 2f, -1, (QueryTriggerInteraction) 1);
      Vector3 vector3_2 = Vector3.ProjectOnPlane(vector3_1, ((RaycastHit) ref raycastHit).get_normal());
      Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
      this.m_MoveDir.x = (__Null) (normalized.x * (double) speed);
      this.m_MoveDir.z = (__Null) (normalized.z * (double) speed);
      if (this.m_CharacterController.get_isGrounded())
      {
        this.m_MoveDir.y = (__Null) -(double) this.m_StickToGroundForce;
        if (this.m_Jump)
        {
          this.m_MoveDir.y = (__Null) (double) this.m_JumpSpeed;
          this.m_Jump = false;
          this.m_Jumping = true;
        }
      }
      else
      {
        FPCtrl fpCtrl = this;
        fpCtrl.m_MoveDir = Vector3.op_Addition(fpCtrl.m_MoveDir, Vector3.op_Multiply(Vector3.op_Multiply(Physics.get_gravity(), this.m_GravityMultiplier), Time.get_fixedDeltaTime()));
      }
      this.m_CollisionFlags = this.m_CharacterController.Move(Vector3.op_Multiply(this.m_MoveDir, Time.get_fixedDeltaTime()));
      this.ProgressStepCycle(speed);
      this.UpdateCameraPosition(speed);
      this.m_MouseLook.UpdateCursorLock();
    }

    private void ProgressStepCycle(float speed)
    {
      Vector3 velocity1 = this.m_CharacterController.get_velocity();
      if ((double) ((Vector3) ref velocity1).get_sqrMagnitude() > 0.0 && (this.m_Input.x != 0.0 || this.m_Input.y != 0.0))
      {
        FPCtrl fpCtrl = this;
        double stepCycle = (double) fpCtrl.m_StepCycle;
        Vector3 velocity2 = this.m_CharacterController.get_velocity();
        double num = ((double) ((Vector3) ref velocity2).get_magnitude() + (double) speed * (!this.m_IsWalking ? (double) this.m_RunstepLenghten : 1.0)) * (double) Time.get_fixedDeltaTime();
        fpCtrl.m_StepCycle = (float) (stepCycle + num);
      }
      if ((double) this.m_StepCycle <= (double) this.m_NextStep)
        return;
      this.m_NextStep = this.m_StepCycle + this.m_StepInterval;
    }

    private void UpdateCameraPosition(float speed)
    {
      if (!this.m_UseHeadBob)
        return;
      Vector3 velocity1 = this.m_CharacterController.get_velocity();
      Vector3 localPosition;
      if ((double) ((Vector3) ref velocity1).get_magnitude() > 0.0 && this.m_CharacterController.get_isGrounded())
      {
        Transform transform = ((Component) this.m_Camera).get_transform();
        CurveCtrlBob headBob = this.m_HeadBob;
        Vector3 velocity2 = this.m_CharacterController.get_velocity();
        double num = (double) ((Vector3) ref velocity2).get_magnitude() + (double) speed * (!this.m_IsWalking ? (double) this.m_RunstepLenghten : 1.0);
        Vector3 vector3 = headBob.DoHeadBob((float) num);
        transform.set_localPosition(vector3);
        localPosition = ((Component) this.m_Camera).get_transform().get_localPosition();
        localPosition.y = (__Null) (((Component) this.m_Camera).get_transform().get_localPosition().y - (double) this.m_JumpBob.Offset());
      }
      else
      {
        localPosition = ((Component) this.m_Camera).get_transform().get_localPosition();
        localPosition.y = (__Null) (this.m_OriginalCameraPosition.y - (double) this.m_JumpBob.Offset());
      }
      ((Component) this.m_Camera).get_transform().set_localPosition(localPosition);
    }

    private void GetInput(out float speed)
    {
      float axis1 = Input.GetAxis("Horizontal");
      float axis2 = Input.GetAxis("Vertical");
      bool isWalking = this.m_IsWalking;
      this.m_IsWalking = !Input.GetKey((KeyCode) 304);
      speed = !this.m_IsWalking ? this.m_RunSpeed : this.m_WalkSpeed;
      this.m_Input = new Vector2(axis1, axis2);
      if ((double) ((Vector2) ref this.m_Input).get_sqrMagnitude() > 1.0)
        ((Vector2) ref this.m_Input).Normalize();
      if (this.m_IsWalking == isWalking || !this.m_UseFovKick)
        return;
      Vector3 velocity = this.m_CharacterController.get_velocity();
      if ((double) ((Vector3) ref velocity).get_sqrMagnitude() <= 0.0)
        return;
      this.StopAllCoroutines();
      this.StartCoroutine(this.m_IsWalking ? this.m_FovKick.FOVKickDown() : this.m_FovKick.FOVKickUp());
    }

    private void RotateView()
    {
      this.m_MouseLook.LookRotation(((Component) this).get_transform(), ((Component) this.m_Camera).get_transform());
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
      Rigidbody attachedRigidbody = hit.get_collider().get_attachedRigidbody();
      if (this.m_CollisionFlags == 4 || Object.op_Equality((Object) attachedRigidbody, (Object) null) || attachedRigidbody.get_isKinematic())
        return;
      attachedRigidbody.AddForceAtPosition(Vector3.op_Multiply(this.m_CharacterController.get_velocity(), 0.1f), hit.get_point(), (ForceMode) 1);
    }
  }
}
