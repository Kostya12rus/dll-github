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
    this.list = new SyncListString();
  }

  private void SetList(SyncListString l)
  {
    this.list = l;
  }

  private void AddUnit(string unit)
  {
    this.list.Add(unit);
  }

  private string GenerateName()
  {
    return this.names[Random.Range(0, this.names.Length)] + "-" + Random.Range(1, 20).ToString("00");
  }

  private void Start()
  {
    this.ccm = this.GetComponent<CharacterClassManager>();
    this.txtlist = GameObject.Find("NTFlist").GetComponent<TextMeshProUGUI>();
    if (!this.isLocalPlayer)
      return;
    if (NetworkServer.active)
    {
      this.NewName();
      NineTailedFoxUnits.host = this;
    }
    else
      NineTailedFoxUnits.host = (NineTailedFoxUnits) null;
  }

  private void Update()
  {
    if (!this.isLocalPlayer)
      return;
    if ((Object) NineTailedFoxUnits.host == (Object) null)
    {
      GameObject gameObject = GameObject.Find("Host");
      if (!((Object) gameObject != (Object) null))
        return;
      NineTailedFoxUnits.host = gameObject.GetComponent<NineTailedFoxUnits>();
    }
    else
    {
      this.txtlist.text = string.Empty;
      if (this.ccm.curClass <= 0 || this.ccm.klasy[this.ccm.curClass].team != Team.MTF)
        return;
      for (int id = 0; id < NineTailedFoxUnits.host.list.Count; ++id)
      {
        if (id == this.ccm.ntfUnit)
        {
          TextMeshProUGUI txtlist = this.txtlist;
          txtlist.text = txtlist.text + "<u>" + NineTailedFoxUnits.host.GetNameById(id) + "</u>";
        }
        else
          this.txtlist.text += NineTailedFoxUnits.host.GetNameById(id);
        this.txtlist.text += "\n";
      }
    }
  }

  public int NewName(out int number, out char letter)
  {
    int num = 0;
    string name;
    for (name = this.GenerateName(); this.list.Contains(name) && num < 100; name = this.GenerateName())
      ++num;
    letter = name.ToUpper()[0];
    number = int.Parse(name.Split('-')[1]);
    this.AddUnit(name);
    return this.list.Count - 1;
  }

  public int NewName()
  {
    int number;
    char letter;
    return this.NewName(out number, out letter);
  }

  public string GetNameById(int id)
  {
    return this.list[id];
  }

  private void UNetVersion()
  {
  }

  protected static void InvokeSyncListlist(NetworkBehaviour obj, NetworkReader reader)
  {
    if (!NetworkClient.active)
      Debug.LogError((object) "SyncList list called on server.");
    else
      ((NineTailedFoxUnits) obj).list.HandleMsg(reader);
  }

  static NineTailedFoxUnits()
  {
    NetworkBehaviour.RegisterSyncListDelegate(typeof (NineTailedFoxUnits), NineTailedFoxUnits.kListlist, new NetworkBehaviour.CmdDelegate(NineTailedFoxUnits.InvokeSyncListlist));
    NetworkCRC.RegisterBehaviour(nameof (NineTailedFoxUnits), 0);
  }

  private void Awake()
  {
    this.list.InitializeBehaviour((NetworkBehaviour) this, NineTailedFoxUnits.kListlist);
  }

  public override bool OnSerialize(NetworkWriter writer, bool forceAll)
  {
    if (forceAll)
    {
      SyncListString.WriteInstance(writer, this.list);
      return true;
    }
    bool flag = false;
    if (((int) this.syncVarDirtyBits & 1) != 0)
    {
      if (!flag)
      {
        writer.WritePackedUInt32(this.syncVarDirtyBits);
        flag = true;
      }
      SyncListString.WriteInstance(writer, this.list);
    }
    if (!flag)
      writer.WritePackedUInt32(this.syncVarDirtyBits);
    return flag;
  }

  public override void OnDeserialize(NetworkReader reader, bool initialState)
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
