// Decompiled with JetBrains decompiler
// Type: PermissionsHandler
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;

public class PermissionsHandler
{
  private readonly string _overridePassword;
  private readonly string _overrideRole;
  private readonly Dictionary<string, UserGroup> _groups;
  private readonly Dictionary<string, string> _members;
  private readonly Dictionary<string, ulong> _permissions;
  private readonly HashSet<ulong> _raPermissions;
  private readonly YamlConfig _config;
  private ulong _lastPerm;
  private bool _isVerified;
  private readonly bool _staffAccess;
  private readonly bool _managerAccess;
  private readonly bool _banTeamAccess;
  private ulong _fullPerm;

  public PermissionsHandler(ref YamlConfig configuration)
  {
    this._config = configuration;
    this._overridePassword = configuration.GetString("override_password", "none");
    this._overrideRole = configuration.GetString("override_password_role", "owner");
    this._staffAccess = configuration.GetBool("enable_staff_access", false);
    this._managerAccess = configuration.GetBool("enable_manager_access", true);
    this._banTeamAccess = configuration.GetBool("enable_banteam_access", true);
    this._groups = new Dictionary<string, UserGroup>();
    this._raPermissions = new HashSet<ulong>();
    foreach (string key in configuration.GetStringList("Roles"))
    {
      string str1 = configuration.GetString(key + "_badge", string.Empty);
      string str2 = configuration.GetString(key + "_color", string.Empty);
      if (!(str1 == string.Empty) && !(str2 == string.Empty))
        this._groups.Add(key, new UserGroup()
        {
          BadgeColor = str2,
          BadgeText = str1,
          Permissions = 0UL
        });
    }
    this._members = configuration.GetStringDictionary("Members");
    this._lastPerm = 1UL;
    foreach (KeyValuePair<string, string> member in this._members)
    {
      if (!this._groups.ContainsKey(member.Value))
        this._members.Remove(member.Key);
    }
    this._permissions = new Dictionary<string, ulong>();
    foreach (string name in Enum.GetNames(typeof (PlayerPermissions)))
    {
      ulong num = (ulong) Enum.Parse(typeof (PlayerPermissions), name);
      this._fullPerm += num;
      this._permissions.Add(name, num);
      if (num != 4096UL)
        this._raPermissions.Add(num);
      if (num > this._lastPerm)
        this._lastPerm = num;
    }
    this.RefreshPermissions();
  }

  public ulong RegisterPermission(string name, bool remoteAdmin, bool refresh = true)
  {
    this._lastPerm = (ulong) Math.Pow(2.0, Math.Log((double) this._lastPerm, 2.0) + 1.0);
    this._fullPerm += this._lastPerm;
    this._permissions.Add(name, this._lastPerm);
    if (remoteAdmin)
      this._raPermissions.Add(this._lastPerm);
    if (refresh)
      this.RefreshPermissions();
    return this._lastPerm;
  }

  public void RefreshPermissions()
  {
    foreach (KeyValuePair<string, UserGroup> group in this._groups)
      group.Value.Permissions = 0UL;
    Dictionary<string, string> stringDictionary = this._config.GetStringDictionary("Permissions");
    foreach (string key1 in this._permissions.Keys)
    {
      ulong permission = this._permissions[key1];
      if (stringDictionary.ContainsKey(key1))
      {
        foreach (string key2 in YamlConfig.ParseCommaSeparatedString(stringDictionary[key1]))
        {
          if (this._groups.ContainsKey(key2))
            this._groups[key2].Permissions += permission;
        }
      }
    }
  }

  public bool IsRaPermitted(ulong permissions)
  {
    foreach (ulong raPermission in this._raPermissions)
    {
      if (this.IsPermitted(permissions, raPermission))
        return true;
    }
    return false;
  }

  public UserGroup GetGroup(string name)
  {
    if (!this._groups.ContainsKey(name))
      return (UserGroup) null;
    return this._groups[name].Clone();
  }

  public List<string> GetAllGroupsNames()
  {
    return this._groups.Keys.ToList<string>();
  }

  public Dictionary<string, UserGroup> GetAllGroups()
  {
    Dictionary<string, UserGroup> dictionary = new Dictionary<string, UserGroup>();
    foreach (string key in this._groups.Keys)
      dictionary.Add(key, this._groups[key]);
    return dictionary;
  }

  public string GetPermissionName(ulong value)
  {
    return this._permissions.FirstOrDefault<KeyValuePair<string, ulong>>((Func<KeyValuePair<string, ulong>, bool>) (x => (long) x.Value == (long) value)).Key;
  }

  public ulong GetPermissionValue(string name)
  {
    return this._permissions.FirstOrDefault<KeyValuePair<string, ulong>>((Func<KeyValuePair<string, ulong>, bool>) (x => x.Key == name)).Value;
  }

  public List<string> GetAllPermissions()
  {
    return this._permissions.Keys.ToList<string>();
  }

  public void SetServerAsVerified()
  {
    this._isVerified = true;
  }

  public bool IsPermitted(ulong permissions, PlayerPermissions check)
  {
    return this.IsPermitted(permissions, Convert.ToUInt64((object) check));
  }

  public bool IsPermitted(ulong permissions, string check)
  {
    if (this._permissions.ContainsKey(check))
      return this.IsPermitted(permissions, this._permissions[check]);
    return false;
  }

  public bool IsPermitted(ulong permissions, ulong check)
  {
    int num = (int) Math.Log((double) check, 2.0);
    return (permissions >> num) % 2UL == 1UL;
  }

  public byte[] DerivePassword(byte[] serverSalt, byte[] clientSalt)
  {
    return QueryProcessor.DerivePassword(this._overridePassword, serverSalt, clientSalt);
  }

  public UserGroup OverrideGroup
  {
    get
    {
      if (!this.OverrideEnabled)
        return (UserGroup) null;
      if (!this._groups.ContainsKey(this._overrideRole))
        return (UserGroup) null;
      return this._groups[this._overrideRole];
    }
  }

  public bool OverrideEnabled
  {
    get
    {
      if (string.IsNullOrEmpty(this._overridePassword) || this._overridePassword == "none")
        return false;
      if (!this._isVerified)
        return true;
      if (this._overridePassword.Length < 8)
      {
        ServerConsole.AddLog("Override password refused, because it's too short (requirement for verified servers only).");
        return false;
      }
      if (this._overridePassword.ToLower() == this._overridePassword || this._overridePassword.ToUpper() == this._overridePassword)
      {
        ServerConsole.AddLog("Override password refused, because it must contain mixed case chars (requirement for verified servers only).");
        return false;
      }
      if (this._overridePassword.Any<char>((Func<char, bool>) (c => !char.IsLetter(c))))
        return true;
      ServerConsole.AddLog("Override password refused, because it must contain digit or special symbol (requirement for verified servers only).");
      return false;
    }
  }

  public UserGroup GetUserGroup(string steamId)
  {
    if (!this._members.ContainsKey(steamId))
      return (UserGroup) null;
    return this._groups[this._members[steamId]];
  }

  public bool IsVerified
  {
    get
    {
      return this._isVerified;
    }
  }

  public ulong FullPerm
  {
    get
    {
      return this._fullPerm;
    }
  }

  public bool StaffAccess
  {
    get
    {
      return this._staffAccess;
    }
  }

  public bool ManagersAccess
  {
    get
    {
      if (!this._managerAccess && !this._banTeamAccess && !this._staffAccess)
        return this._isVerified;
      return true;
    }
  }

  public bool BanningTeamAccess
  {
    get
    {
      if (!this._banTeamAccess && !this._staffAccess)
        return this._isVerified;
      return true;
    }
  }
}
