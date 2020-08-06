using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClient : Client
{

  [SerializeField] private Text serverAddressText;
  public string serverAddress { get { return serverAddressText.text; } }

  public void ConnectToServer() {
    this.Init(serverAddress);
  }

  public void Update() {
    this.PopMessages();
  }

  protected override void OnConnected() {

  }
  protected override void OnDisconnected() {

  }
}
