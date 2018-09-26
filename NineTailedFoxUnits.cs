// Decompiled with JetBrains decompiler
// Type: NineTailedFoxUnits
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class NineTailedFoxUnits : NetworkBehaviour
{
  private static int kListlist = -376129279;
  public string[] names;
  [SyncVar(hook = "SetList")]
  public SyncListString list;
  private CharacterClassManager ccm;
  private TextMeshProUGUI txtlist;
  public static NineTailedFoxUnits host;

  public NineTailedFoxUnits()
  {
    base.\u002Ector();
    this.list = new SyncListString();
  }

  private void SetList(SyncListString l)
  {
    this.list = l;
  }

  private void AddUnit(string unit)
  {
    ((SyncList<string>) this.list).Add(unit);
  }

  private string GenerateName()
  {
    return this.names[Random.Range(0, this.names.Length)] + "-" + Random.Range(1, 20).ToString("00");
  }

  private void Start()
  {
    this.ccm = (CharacterClassManager) ((Component) this).GetComponent<CharacterClassManager>();
    this.txtlist = (TextMeshProUGUI) GameObject.Find("NTFlist").GetComponent<TextMeshProUGUI>();
    if (!this.get_isLocalPlayer())
      return;
    if (NetworkServer.get_active())
    {
      this.NewName();
      NineTailedFoxUnits.host = this;
    }
    else
      NineTailedFoxUnits.host = (NineTailedFoxUnits) null;
  }

  private void Update()
  {
    if (!this.get_isLocalPlayer())
      return;
    if (Object.op_Equality((Object) NineTailedFoxUnits.host, (Object) null))
    {
      GameObject gameObject = GameObject.Find("Host");
      if (!Object.op_Inequality((Object) gameObject, (Object) null))
        return;
      NineTailedFoxUnits.host = (NineTailedFoxUnits) gameObject.GetComponent<NineTailedFoxUnits>();
    }
    else
    {
      ((TMP_Text) this.txtlist).set_text(string.Empty);
      if (this.ccm.curClass <= 0 || this.ccm.klasy[this.ccm.curClass].team != Team.MTF)
        return;
      for (int id = 0; id < ((SyncList<string>) NineTailedFoxUnits.host.list).get_Count(); ++id)
      {
        if (id == this.ccm.ntfUnit)
        {
          TextMeshProUGUI txtlist = this.txtlist;
          ((TMP_Text) txtlist).set_text(((TMP_Text) txtlist).get_text() + "<u>" + NineTailedFoxUnits.host.GetNameById(id) + "</u>");
        }
        else
        {
          TextMeshProUGUI txtlist = this.txtlist;
          ((TMP_Text) txtlist).set_text(((TMP_Text) txtlist).get_text() + NineTailedFoxUnits.host.GetNameById(id));
        }
        TextMeshProUGUI txtlist1 = this.txtlist;
        ((TMP_Text) txtlist1).set_text(((TMP_Text) txtlist1).get_text() + "\n");
      }
    }
  }

  public int NewName(out int number, out char letter)
  {
    int num = 0;
    string name;
    for (name = this.GenerateName(); ((SyncList<string>) this.list).Contains(name) && num < 100; name = this.GenerateName())
      ++num;
    letter = name.ToUpper()[0];
    number = int.Parse(name.Split('-')[1]);
    this.AddUnit(name);
    return ((SyncList<string>) this.list).get_Count() - 1;
  }

  public int NewName()
  {
    int number;
    char letter;
    return this.NewName(out number, out letter);
  }

  public string GetNameById(int id)
  {
    return ((SyncList<string>) this.list).get_Item(id);
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeSyncListlist(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.get_active())
      Debug.LogError((object) "SyncList list called on server.");
    else
      ((SyncList<string>) ((NineTailedFoxUnits) obj).list).HandleMsg(reader);
  }

  static NineTailedFoxUnits()
  {
    // ISSUE: method pointer
    NetworkBehaviour.RegisterSyncListDelegate(typeof (NineTailedFoxUnits), NineTailedFoxUnits.kListlist, new NetworkBehaviour.CmdDelegate((object) null, __methodptr(InvokeSyncListlist)));
    NetworkCRC.RegisterBehaviour(nameof (NineTailedFoxUnits), 0);
  }

  private void Awake()
  {
    ((SyncList<string>) this.list).InitializeBehaviour((NetworkBehaviour) this, NineTailedFoxUnits.kListlist);
  }

  public virtual bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      SyncListString.WriteInstance(writer, this.list);
      return true;
    }
    bool flag = false;
    if (((int) this.get_syncVarDirtyBits() & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.get_syncVarDirtyBits());
        flag = true;
      }
      SyncListString.WriteInstance(writer, this.list);
    }
    if (!flag)
      writer.WritePackedUInt32(this.get_syncVarDirtyBits());
    return flag;
  }

  public virtual void OnDeserialize(NetworkReader reader, bool initialState)
  {
    if (initialState)
    {
      SyncListString.ReadReference(reader, this.list);
    }
    else
    {
      if (((int) reader.ReadPackedUInt32() & 1) == 0)
        return;
      SyncListString.ReadReference(reader, this.list);
    }
  }
}
