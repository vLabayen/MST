using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameClient : Client
{
  //Connection - Disconnection events
  public delegate void OnConnectedToServer();
  public delegate void OnDisconnectedFromServer();
  public event OnConnectedToServer onConnected;
  public event OnDisconnectedFromServer onDisconnected;

  //NetMsg events

  void Start() {

  }

  void FixedUpdate() {
    this.PopMessages();
  }

  public void ConnectToServer(string serverAddress) {
    this.Init(serverAddress);
  }

  protected override void OnConnected() => onConnected?.Invoke();
  protected override void OnDisconnected() => onDisconnected?.Invoke();
}
