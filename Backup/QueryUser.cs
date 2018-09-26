// Decompiled with JetBrains decompiler
// Type: QueryUser
// Assembly: Assembly-CSharp, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 51F4D31F-B166-4C43-9BCF-DD08031E944E
// Assembly location: C:\Users\Kostya12rus\Desktop\Cheat\TextureLoger\Assembly-CSharp.dll

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

internal class QueryUser
{
  private readonly NetworkStream _s;
  private readonly QueryServer _server;
  private readonly Thread _thr;
  private readonly Thread _sol;
  private int _lastping;
  private bool _closing;
  private int _invalidPackets;
  private readonly string _querypassword;
  internal readonly string Ip;
  private bool _authenticated;
  private readonly UTF8Encoding _encoder;

  internal QueryUser(QueryServer s, TcpClient c, string ip)
  {
    this._s = c.GetStream();
    this._server = s;
    this.Ip = ip;
    this.Send("Welcome to SCP Secret Laboratory Query Protocol");
    this._thr = new Thread(new ThreadStart(this.Receive))
    {
      IsBackground = true
    };
    this._thr.Start();
    this._encoder = new UTF8Encoding();
    this._querypassword = ConfigFile.ServerConfig.GetString("administrator_query_password", string.Empty);
    this._lastping = Convert.ToInt32(this._server.Stopwatch.Elapsed.TotalSeconds) + 5;
    this._authenticated = false;
  }

  internal bool IsConnected()
  {
    return this._server.Stopwatch.Elapsed.TotalSeconds - (double) this._lastping < (double) this._server.TimeoutThreshold;
  }

  private void Receive()
  {
    this._s.ReadTimeout = 200;
    this._s.WriteTimeout = 200;
    while (!this._closing)
    {
      try
      {
        byte[] numArray = new byte[4096];
        int num;
        try
        {
          num = this._s.Read(numArray, 0, 4096);
        }
        catch
        {
          num = -1;
          Thread.Sleep(5);
        }
        if (num > -1)
        {
          foreach (byte[] bytes in AuthenticatedMessage.Decode(numArray))
          {
            string str = this._encoder.GetString(bytes);
            AuthenticatedMessage authenticatedMessage = (AuthenticatedMessage) null;
            string message1;
            try
            {
              message1 = str.Substring(0, str.LastIndexOf(';'));
            }
            catch
            {
              ++this._invalidPackets;
              message1 = str.TrimEnd(new char[1]);
              if (message1.EndsWith(";"))
                message1 = message1.Substring(0, message1.Length - 1);
            }
            if (this._invalidPackets >= 5)
            {
              if (!this._closing)
              {
                this.Send("Too many invalid packets sent.");
                ServerConsole.AddLog("Query connection from " + this.Ip + " dropped due to too many invalid packets sent.");
                this._server.Users.Remove(this);
                this.CloseConn(false);
                break;
              }
              break;
            }
            try
            {
              authenticatedMessage = AuthenticatedMessage.AuthenticateMessage(message1, TimeBehaviour.CurrentTimestamp(), this._querypassword);
            }
            catch (MessageAuthenticationFailureException ex)
            {
              this.Send("Message can't be authenticated - " + ex.Message);
              ServerConsole.AddLog("Query command from " + this.Ip + " can't be authenticated - " + ex.Message);
            }
            catch (MessageExpiredException ex)
            {
              this.Send("Message expired");
              ServerConsole.AddLog("Query command from " + this.Ip + " is expired.");
            }
            catch (Exception ex)
            {
              this.Send("Error during processing your message.");
              ServerConsole.AddLog("Query command from " + this.Ip + " can't be processed - " + ex.Message + ".");
            }
            if (authenticatedMessage != null)
            {
              if (!this._authenticated && authenticatedMessage.Administrator)
                this._authenticated = true;
              string message2 = authenticatedMessage.Message;
              string[] strArray = new string[0];
              if (message2.Contains(" "))
              {
                message2 = message2.Split(' ')[0];
                strArray = authenticatedMessage.Message.Substring(message2.Length + 1).Split(' ');
              }
              string lower1 = message2.ToLower();
              if (authenticatedMessage.Message == "Ping")
              {
                this._invalidPackets = 0;
                this._lastping = Convert.ToInt32(this._server.Stopwatch.Elapsed.TotalSeconds);
                this.Send("Pong");
              }
              else if (lower1 == "roundrestart")
              {
                if (this.AdminCheck(authenticatedMessage.Administrator))
                {
                  foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
                  {
                    PlayerStats component = (PlayerStats) gameObject.GetComponent<PlayerStats>();
                    if (component.get_isLocalPlayer() && component.get_isServer())
                      component.Roundrestart();
                  }
                  this.Send("Round restarted.");
                }
              }
              else if (lower1 == "shutdown")
              {
                if (this.AdminCheck(authenticatedMessage.Administrator))
                {
                  this.Send("Server is shutting down...");
                  Application.Quit();
                }
              }
              else if (lower1 == "warhead")
              {
                AlphaWarheadController host = AlphaWarheadController.host;
                if (strArray.Length == 0)
                {
                  this.Send("Synax: warhead (status|detonate|cancel|enable|disable)");
                }
                else
                {
                  string lower2 = strArray[0].ToLower();
                  if (lower2 == "status")
                  {
                    if (host.detonated || (double) host.timeToDetonation == 0.0)
                      this.Send("Warhead has been detonated.");
                    else if (host.inProgress)
                      this.Send("Detonation is in progress.");
                    else if (!AlphaWarheadOutsitePanel.nukeside.enabled)
                      this.Send("Warhead is disabled.");
                    else if ((double) host.timeToDetonation > (double) AlphaWarheadController.host.RealDetonationTime())
                      this.Send("Warhead is restarting.");
                    else
                      this.Send("Warhead is ready to detonation.");
                  }
                  else if (lower2 == "detonate")
                  {
                    if (this.AdminCheck(authenticatedMessage.Administrator))
                    {
                      AlphaWarheadController.host.StartDetonation();
                      this.Send("Detonation sequence started.");
                    }
                  }
                  else if (lower2 == "cancel")
                  {
                    if (this.AdminCheck(authenticatedMessage.Administrator))
                    {
                      AlphaWarheadController.host.CancelDetonation((GameObject) null);
                      this.Send("Detonation has been canceled.");
                    }
                  }
                  else if (lower2 == "enable")
                  {
                    if (this.AdminCheck(authenticatedMessage.Administrator))
                    {
                      AlphaWarheadOutsitePanel.nukeside.Networkenabled = true;
                      this.Send("Warhead has been enabled.");
                    }
                  }
                  else if (lower2 == "disable")
                  {
                    if (this.AdminCheck(authenticatedMessage.Administrator))
                    {
                      AlphaWarheadOutsitePanel.nukeside.Networkenabled = false;
                      this.Send("Warhead has been disabled.");
                    }
                  }
                  else
                    this.Send("WARHEAD: Unknown subcommand.");
                }
              }
              else
                this.Send("Command not found");
            }
          }
        }
      }
      catch (SocketException ex)
      {
        ServerConsole.AddLog("Query connection from " + this.Ip + " dropped (SocketException).");
        if (this._closing)
          break;
        this._server.Users.Remove(this);
        this.CloseConn(false);
        break;
      }
      catch
      {
        ServerConsole.AddLog("Query connection from " + this.Ip + " dropped.");
        if (this._closing)
          break;
        this._server.Users.Remove(this);
        this.CloseConn(false);
        break;
      }
    }
  }

  private bool AdminCheck(bool admin)
  {
    if (!admin)
      this.Send("Access denied! You need to have administrator permissions.");
    return admin;
  }

  public void CloseConn(bool shuttingdown = false)
  {
    this._closing = true;
    if (shuttingdown)
      this.Send("Server is shutting down...");
    this._s.Close();
    if (this._thr == null)
      return;
    this._thr.Abort();
  }

  public void Send(string msg)
  {
    msg = !this._authenticated || this._querypassword == string.Empty || (this._querypassword == "none" || this._querypassword == null) ? AuthenticatedMessage.GenerateNonAuthenticatedMessage(msg) : AuthenticatedMessage.GenerateAuthenticatedMessage(msg, TimeBehaviour.CurrentTimestamp(), this._querypassword);
    this.Send(Utf8.GetBytes(msg));
  }

  public void Send(byte[] msg)
  {
    try
    {
      byte[] buffer = AuthenticatedMessage.Encode(msg);
      this._s.Write(buffer, 0, buffer.Length);
    }
    catch (Exception ex)
    {
      ServerConsole.AddLog("Can't send query response to " + this.Ip + ": " + ex.StackTrace);
    }
  }
}
