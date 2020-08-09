using UnityEngine;

﻿[System.Serializable]
public class PlayerInputMsg : NetMsg
{
  public PlayerInputMsg(){ this.type = MessageType.PlayerInputMsg; }
  List<NetMsg> inputChanges = new List<NetMsg>();
}

[System.Serializable] public class PlayerInputHorizontal : NetMsg { public float x { get; set; } }
[System.Serializable] public class PlayerInputJump : NetMsg { public bool jump { get; set; } }
[System.Serializable] public class PlayerInputFire : NetMsg {
  public bool fire { get; set; }
  public Vector2 fireDirection { get; set; }
}
