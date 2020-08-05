using UnityEngine;
using System;

public class SClient{

  public SClient(int cnn){
    this.CnnID = cnn;
  }

  public int CnnID { get; private set; }

  public void ShowInfo() {
    Debug.Log(String.Format("Client : {0}", this.CnnID));
  }
}
