[System.Serializable]
public abstract class NetMsg
{
  public MessageType type { set; get; }

  public NetMsg(){
    this.type = MessageType.None;
  }
}
