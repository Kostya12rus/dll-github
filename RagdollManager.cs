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

  public void SpawnRagdoll(Vector3 pos, Quaternion rot, int classID, PlayerStats.HitInfo ragdollInfo, bool allowRecall, string ownerID, string ownerNick)
  {
    Class @class = this.ccm.klasy[classID];
    if ((Object) @class.model_ragdoll != (Object) null)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(@class.model_ragdoll, pos + @class.ragdoll_offset.position, Quaternion.Euler(rot.eulerAngles + @class.ragdoll_offset.rotation));
      NetworkServer.Spawn(gameObject);
      gameObject.GetComponent<Ragdoll>().SetOwner(new Ragdoll.Info(ownerID, ownerNick, ragdollInfo, classID));
      gameObject.GetComponent<Ragdoll>().SetRecall(allowRecall);
    }
    if (!ragdollInfo.tool.Contains("SCP") && !(ragdollInfo.tool == "POCKET"))
      return;
    this.RegisterScpFrag();
  }

  private void Start()
  {
    this.txt = GameObject.Find("BodyInspection").GetComponentInChildren<TextMeshProUGUI>();
    this.cam = this.GetComponent<Scp049PlayerScript>().plyCam.transform;
    this.ccm = this.GetComponent<CharacterClassManager>();
  }

  public void Update()
  {
    if (!this.isLocalPlayer)
      return;
    string str = string.Empty;
    RaycastHit hitInfo;
    if (Physics.Raycast(new Ray(this.cam.position, this.cam.forward), out hitInfo, 3f, (int) this.inspectionMask))
    {
      Ragdoll componentInParent = hitInfo.transform.GetComponentInParent<Ragdoll>();
      if ((Object) componentInParent != (Object) null)
        str = TranslationReader.Get("Death_Causes", 12).Replace("[user]", componentInParent.owner.steamClientName).Replace("[cause]", RagdollManager.GetCause(componentInParent.owner.deathCause, false)).Replace("[class]", "<color=" + this.GetColor(this.ccm.klasy[componentInParent.owner.charclass].classColor) + ">" + this.ccm.klasy[componentInParent.owner.charclass].fullName + "</color>");
    }
    this.txt.text = str;
  }

  public string GetColor(Color c)
  {
    Color32 color32 = new Color32((byte) ((double) c.r * (double) byte.MaxValue), (byte) ((double) c.g * (double) byte.MaxValue), (byte) ((double) c.b * (double) byte.MaxValue), byte.MaxValue);
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
      str = TranslationReader.Get("Death_Causes", 7).Replace("[ammotype]", gameObject.GetComponent<AmmoBox>().types[gameObject.GetComponent<WeaponManager>().weapons[result].ammoType].label);
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

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    bool flag;
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
  {
  }
}
