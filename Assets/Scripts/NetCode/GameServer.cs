using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServer : Server
{

  public void StartServer() {
    this.Init();
  }

  public void Update() {
    this.PopMessages();
  }

  protected override void OnClientConnected(SClient client) {
    Debug.Log("Client connected");
  }
  protected override void OnClientDisconnected(SClient client) {
    Debug.Log("Client disconnected");
  }

}
