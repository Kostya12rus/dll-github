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
    private FOVZoom m_FovKick = new FOVZoom();
    [SerializeField]
    private CurveCtrlBob m_HeadBob = new CurveCtrlBob();
    [SerializeField]
    private LerpCtrlBob m_JumpBob = new LerpCtrlBob();
    private Vector3 m_MoveDir = Vector3.zero;
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
    private bool m_UseHeadBob;
    [SerializeField]
    private float m_StepInterval;
    private Camera m_Camera;
    private bool m_Jump;
    private float m_YRotation;
    private Vector2 m_Input;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;
    private float m_StepCycle;
    private float m_NextStep;
    private bool m_Jumping;

    private void Start()
    {
      this.m_CharacterController = this.GetComponent<CharacterController>();
      this.m_Camera = Camera.main;
      this.m_OriginalCameraPosition = this.m_Camera.transform.localPosition;
      this.m_FovKick.Setup(this.m_Camera);
      this.m_HeadBob.Setup(this.m_Camera, this.m_StepInterval);
      this.m_StepCycle = 0.0f;
      this.m_NextStep = this.m_StepCycle / 2f;
      this.m_Jumping = false;
      this.m_MouseLook.Init(this.transform, this.m_Camera.transform);
    }

    private void Update()
    {
      this.RotateView();
      if (!this.m_Jump)
        this.m_Jump = Input.GetButtonDown("Jump");
      if (!this.m_PreviouslyGrounded && this.m_CharacterController.isGrounded)
      {
        this.StartCoroutine(this.m_JumpBob.DoBobCycle());
        this.m_MoveDir.y = 0.0f;
        this.m_Jumping = false;
      }
      if (!this.m_CharacterController.isGrounded && !this.m_Jumping && this.m_PreviouslyGrounded)
        this.m_MoveDir.y = 0.0f;
      this.m_PreviouslyGrounded = this.m_CharacterController.isGrounded;
    }

    private void FixedUpdate()
    {
      float speed;
      this.GetInput(out speed);
      Vector3 vector = this.transform.forward * this.m_Input.y + this.transform.right * this.m_Input.x;
      RaycastHit hitInfo;
      Physics.SphereCast(this.transform.position, this.m_CharacterController.radius, Vector3.down, out hitInfo, this.m_CharacterController.height / 2f, -1, QueryTriggerInteraction.Ignore);
      Vector3 normalized = Vector3.ProjectOnPlane(vector, hitInfo.normal).normalized;
      this.m_MoveDir.x = normalized.x * speed;
      this.m_MoveDir.z = normalized.z * speed;
      if (this.m_CharacterController.isGrounded)
      {
        this.m_MoveDir.y = -this.m_StickToGroundForce;
        if (this.m_Jump)
        {
          this.m_MoveDir.y = this.m_JumpSpeed;
          this.m_Jump = false;
          this.m_Jumping = true;
        }
      }
      else
        this.m_MoveDir += Physics.gravity * this.m_GravityMultiplier * Time.fixedDeltaTime;
      this.m_CollisionFlags = this.m_CharacterController.Move(this.m_MoveDir * Time.fixedDeltaTime);
      this.ProgressStepCycle(speed);
      this.UpdateCameraPosition(speed);
      this.m_MouseLook.UpdateCursorLock();
    }

    private void ProgressStepCycle(float speed)
    {
      if ((double) this.m_CharacterController.velocity.sqrMagnitude > 0.0 && ((double) this.m_Input.x != 0.0 || (double) this.m_Input.y != 0.0))
        this.m_StepCycle += (this.m_CharacterController.velocity.magnitude + speed * (!this.m_IsWalking ? this.m_RunstepLenghten : 1f)) * Time.fixedDeltaTime;
      if ((double) this.m_StepCycle <= (double) this.m_NextStep)
        return;
      this.m_NextStep = this.m_StepCycle + this.m_StepInterval;
    }

    private void UpdateCameraPosition(float speed)
    {
      if (!this.m_UseHeadBob)
        return;
      Vector3 localPosition;
      if ((double) this.m_CharacterController.velocity.magnitude > 0.0 && this.m_CharacterController.isGrounded)
      {
        this.m_Camera.transform.localPosition = this.m_HeadBob.DoHeadBob(this.m_CharacterController.velocity.magnitude + speed * (!this.m_IsWalking ? this.m_RunstepLenghten : 1f));
        localPosition = this.m_Camera.transform.localPosition;
        localPosition.y = this.m_Camera.transform.localPosition.y - this.m_JumpBob.Offset();
      }
      else
      {
        localPosition = this.m_Camera.transform.localPosition;
        localPosition.y = this.m_OriginalCameraPosition.y - this.m_JumpBob.Offset();
      }
      this.m_Camera.transform.localPosition = localPosition;
    }

    private void GetInput(out float speed)
    {
      float axis1 = Input.GetAxis("Horizontal");
      float axis2 = Input.GetAxis("Vertical");
      bool isWalking = this.m_IsWalking;
      this.m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
      speed = !this.m_IsWalking ? this.m_RunSpeed : this.m_WalkSpeed;
      this.m_Input = new Vector2(axis1, axis2);
      if ((double) this.m_Input.sqrMagnitude > 1.0)
        this.m_Input.Normalize();
      if (this.m_IsWalking == isWalking || !this.m_UseFovKick || (double) this.m_CharacterController.velocity.sqrMagnitude <= 0.0)
        return;
      this.StopAllCoroutines();
      this.StartCoroutine(this.m_IsWalking ? this.m_FovKick.FOVKickDown() : this.m_FovKick.FOVKickUp());
    }

    private void RotateView()
    {
      this.m_MouseLook.LookRotation(this.transform, this.m_Camera.transform);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
      Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
      if (this.m_CollisionFlags == CollisionFlags.Below || (Object) attachedRigidbody == (Object) null || attachedRigidbody.isKinematic)
        return;
      attachedRigidbody.AddForceAtPosition(this.m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }
  }
}
