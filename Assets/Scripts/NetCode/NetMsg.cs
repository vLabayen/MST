[System.Serializable]
public class NetMsg
{
  public NetOP OP { set; get; }

  public NetMsg(){
    this.OP = NetOP.None;
  }
}
