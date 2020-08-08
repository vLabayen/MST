using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameClient : Client
{

  public delegate void OnConnectedToServer();
  public event OnConnectedToServer onConnected;

  void FixedUpdate() {
    this.PopMessages();
  }

  public void ConnectToServer(string serverAddress) {
    this.Init(serverAddress);
  }

  protected override void OnConnected() {
    onConnected?.Invoke();
  }
  protected override void OnDisconnected() {

  }
}
