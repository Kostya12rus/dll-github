// Decompiled with JetBrains decompiler
// Type: SECTR_Door
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (Animator))]
[AddComponentMenu("SECTR/Audio/SECTR Door")]
public class SECTR_Door : MonoBehaviour
{
  private int controlParam;
  private int canOpenParam;
  private int closedState;
  private int waitingState;
  private int openingState;
  private int openState;
  private int closingState;
  private int lastState;
  private Animator cachedAnimator;
  private int openCount;
  [SECTR_ToolTip("The portal this door affects (if any).")]
  public SECTR_Portal Portal;
  [SECTR_ToolTip("The name of the control param in the door.")]
  public string ControlParam;
  [SECTR_ToolTip("The name of the control param that indicates if we are allowed to open.")]
  public string CanOpenParam;
  [SECTR_ToolTip("The full name (layer and state) of the Open state in the Animation Controller.")]
  public string OpenState;
  [SECTR_ToolTip("The full name (layer and state) of the Closed state in the Animation Controller.")]
  public string ClosedState;
  [SECTR_ToolTip("The full name (layer and state) of the Opening state in the Animation Controller.")]
  public string OpeningState;
  [SECTR_ToolTip("The full name (layer and state) of the Closing state in the Animation Controller.")]
  public string ClosingState;
  [SECTR_ToolTip("The full name (layer and state) of the Wating state in the Animation Controller.")]
  public string WaitingState;

  public SECTR_Door()
  {
    base.\u002Ector();
  }

  public void OpenDoor()
  {
    ++this.openCount;
  }

  public void CloseDoor()
  {
    --this.openCount;
  }

  public bool IsFullyOpen()
  {
    AnimatorStateInfo animatorStateInfo = this.cachedAnimator.GetCurrentAnimatorStateInfo(0);
    return ((AnimatorStateInfo) ref animatorStateInfo).get_fullPathHash() == this.openState;
  }

  public bool IsClosed()
  {
    AnimatorStateInfo animatorStateInfo = this.cachedAnimator.GetCurrentAnimatorStateInfo(0);
    return ((AnimatorStateInfo) ref animatorStateInfo).get_fullPathHash() == this.closedState;
  }

  protected virtual void OnEnable()
  {
    this.cachedAnimator = (Animator) ((Component) this).GetComponent<Animator>();
    this.controlParam = Animator.StringToHash(this.ControlParam);
    this.canOpenParam = Animator.StringToHash(this.CanOpenParam);
    this.closedState = Animator.StringToHash(this.ClosedState);
    this.waitingState = Animator.StringToHash(this.WaitingState);
    this.openingState = Animator.StringToHash(this.OpeningState);
    this.openState = Animator.StringToHash(this.OpenState);
    this.closingState = Animator.StringToHash(this.ClosingState);
  }

  private void Start()
  {
    if (this.controlParam != 0)
      this.cachedAnimator.SetBool(this.controlParam, false);
    if (this.canOpenParam != 0)
      this.cachedAnimator.SetBool(this.canOpenParam, false);
    if (Object.op_Implicit((Object) this.Portal))
      this.Portal.SetFlag(SECTR_Portal.PortalFlags.Closed, true);
    this.openCount = 0;
    this.lastState = this.closedState;
    ((Component) this).SendMessage("OnClose", (SendMessageOptions) 1);
  }

  private void Update()
  {
    bool flag = this.CanOpen();
    if (this.canOpenParam != 0)
      this.cachedAnimator.SetBool(this.canOpenParam, flag);
    if (this.controlParam != 0 && (flag || this.canOpenParam != 0))
    {
      if (this.openCount > 0)
        this.cachedAnimator.SetBool(this.controlParam, true);
      else
        this.cachedAnimator.SetBool(this.controlParam, false);
    }
    AnimatorStateInfo animatorStateInfo = this.cachedAnimator.GetCurrentAnimatorStateInfo(0);
    int fullPathHash = ((AnimatorStateInfo) ref animatorStateInfo).get_fullPathHash();
    if (fullPathHash != this.lastState)
    {
      if (fullPathHash == this.closedState)
        ((Component) this).SendMessage("OnClose", (SendMessageOptions) 1);
      if (fullPathHash == this.waitingState)
        ((Component) this).SendMessage("OnWaiting", (SendMessageOptions) 1);
      else if (fullPathHash == this.openingState)
        ((Component) this).SendMessage("OnOpening", (SendMessageOptions) 1);
      if (fullPathHash == this.openState)
        ((Component) this).SendMessage("OnOpen", (SendMessageOptions) 1);
      else if (fullPathHash == this.closingState)
        ((Component) this).SendMessage("OnClosing", (SendMessageOptions) 1);
      this.lastState = fullPathHash;
    }
    if (!Object.op_Implicit((Object) this.Portal))
      return;
    this.Portal.SetFlag(SECTR_Portal.PortalFlags.Closed, this.IsClosed());
  }

  protected virtual void OnTriggerEnter(Collider other)
  {
    ++this.openCount;
  }

  protected virtual void OnTriggerExit(Collider other)
  {
    --this.openCount;
  }

  protected virtual bool CanOpen()
  {
    return true;
  }
}
