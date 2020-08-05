[System.Serializable]
public abstract class NetMsg
{
  public MessageType type { set; get; }

  public NetMsg(MessageType type){
    this.type = type;
  }
}
