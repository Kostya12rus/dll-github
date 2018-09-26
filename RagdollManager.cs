// Decompiled with JetBrains decompiler
// Type: RagdollManager
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RagdollManager : NetworkBehaviour
{
  public LayerMask inspectionMask;
  private Transform cam;
  private CharacterClassManager ccm;
  private TextMeshProUGUI txt;

  public RagdollManager()
  {
    base.\u002Ector();
  }

  public void SpawnRagdoll(Vector3 pos, Quaternion rot, int classID, PlayerStats.HitInfo ragdollInfo, bool allowRecall, string ownerID, string ownerNick)
  {
    Class @class = this.ccm.klasy[classID];
    if (Object.op_Inequality((Object) @class.model_ragdoll, (Object) null))
    {
      GameObject gameObject = (GameObject) Object.Instantiate<GameObject>((M0) @class.model_ragdoll, Vector3.op_Addition(pos, @class.ragdoll_offset.position), Quaternion.Euler(Vector3.op_Addition(((Quaternion) ref rot).get_eulerAngles(), @class.ragdoll_offset.rotation)));
      NetworkServer.Spawn(gameObject);
      ((Ragdoll) gameObject.GetComponent<Ragdoll>()).SetOwner(new Ragdoll.Info(ownerID, ownerNick, ragdollInfo, classID));
      ((Ragdoll) gameObject.GetComponent<Ragdoll>()).SetRecall(allowRecall);
    }
    if (!ragdollInfo.tool.Contains("SCP") && !(ragdollInfo.tool == "POCKET"))
      return;
    this.RegisterScpFrag();
  }

  private void Start()
  {
    this.txt = (TextMeshProUGUI) GameObject.Find("BodyInspection").GetComponentInChildren<TextMeshProUGUI>();
    this.cam = ((Scp049PlayerScript) ((Component) this).GetComponent<Scp049PlayerScript>()).plyCam.get_transform();
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
  }

  public void Update()
  {
    if (!this.get_isLocalPlayer())
      return;
    string str = string.Empty;
    RaycastHit raycastHit;
    if (Physics.Raycast(new Ray(this.cam.get_position(), this.cam.get_forward()), ref raycastHit, 3f, LayerMask.op_Implicit(this.inspectionMask)))
    {
      Ragdoll componentInParent = (Ragdoll) ((Component) ((RaycastHit) ref raycastHit).get_transform()).GetComponentInParent<Ragdoll>();
      if (Object.op_Inequality((Object) componentInParent, (Object) null))
        str = TranslationReader.Get("Death_Causes", 12).Replace("[user]", componentInParent.owner.steamClientName).Replace("[cause]", RagdollManager.GetCause(componentInParent.owner.deathCause, false)).Replace("[class]", "<color=" + this.GetColor(this.ccm.klasy[componentInParent.owner.charclass].classColor) + ">" + this.ccm.klasy[componentInParent.owner.charclass].fullName + "</color>");
    }
    ((TMP_Text) this.txt).set_text(str);
  }

  public string GetColor(Color c)
  {
    Color32 color32;
    ((Color32) ref color32).\u002Ector((byte) (c.r * (double) byte.MaxValue), (byte) (c.g * (double) byte.MaxValue), (byte) (c.b * (double) byte.MaxValue), byte.MaxValue);
    return "#" + color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2");
  }

  public void RegisterScpFrag()
  {
    ++RoundSummary.kills_by_scp;
  }

  public static string GetCause(PlayerStats.HitInfo info, bool ragdoll)
  {
    string str = TranslationReader.Get("Death_Causes", 11);
    int result = -1;
    if (info.tool == "NUKE")
      str = TranslationReader.Get("Death_Causes", 0);
    else if (info.tool == "FALLDOWN")
      str = TranslationReader.Get("Death_Causes", 1);
    else if (info.tool == "LURE")
      str = TranslationReader.Get("Death_Causes", 2);
    else if (info.tool == "POCKET")
      str = TranslationReader.Get("Death_Causes", 3);
    else if (info.tool == "CONTAIN")
      str = TranslationReader.Get("Death_Causes", 4);
    else if (info.tool == "TESLA")
      str = TranslationReader.Get("Death_Causes", 5);
    else if (info.tool == "WALL")
      str = TranslationReader.Get("Death_Causes", 6);
    else if (info.tool == "DECONT")
      str = TranslationReader.Get("Death_Causes", 15);
    else if (info.tool == "FRAG")
      str = TranslationReader.Get("Death_Causes", 16);
    else if (info.tool.Length > 7 && info.tool.Substring(0, 7) == "Weapon:" && (int.TryParse(info.tool.Remove(0, 7), out result) && result != -1))
    {
      GameObject gameObject = GameObject.Find("Host");
      str = TranslationReader.Get("Death_Causes", 7).Replace("[ammotype]", ((AmmoBox) gameObject.GetComponent<AmmoBox>()).types[((WeaponManager) gameObject.GetComponent<WeaponManager>()).weapons[result].ammoType].label);
    }
    else if (info.tool.Length > 4 && info.tool.Substring(0, 4) == "SCP:" && int.TryParse(info.tool.Remove(0, 4), out result))
    {
      switch (result)
      {
        case 49:
        case 492:
          str = TranslationReader.Get("Death_Causes", 10);
          break;
        case 96:
          str = TranslationReader.Get("Death_Causes", 13);
          break;
        case 106:
          str = TranslationReader.Get("Death_Causes", 9);
          break;
        case 173:
          str = TranslationReader.Get("Death_Causes", 8);
          break;
        case 939:
          str = TranslationReader.Get("Death_Causes", 14);
          break;
      }
    }
    return str;
  }

  private void UNetVersion()
  {
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
