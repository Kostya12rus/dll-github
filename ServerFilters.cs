// Decompiled with JetBrains decompiler
// Type: ServerFilters
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using UnityEngine;

public class ServerFilters : MonoBehaviour
{
  private ServerListManager list;
  public string nameFilter;

  public ServerFilters()
  {
    base.\u002Ector();
  }

  public bool AllowToSpawn(string server_name)
  {
    if (this.nameFilter.Length == 0)
      return true;
    this.nameFilter = this.nameFilter.ToUpper();
    int num1 = 0;
    int num2 = 0;
    foreach (char ch in this.nameFilter)
    {
      for (int index = num2; index < server_name.Length; ++index)
      {
        if ((int) server_name.ToUpper()[index] == (int) ch)
        {
          num2 = index;
          ++num1;
          break;
        }
      }
    }
    return num1 == this.nameFilter.Length;
  }

  private void Start()
  {
    this.list = (ServerListManager) ((Component) this).GetComponent<ServerListManager>();
  }
}
