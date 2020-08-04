using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

#pragma warning disable CS0618
public class Client : MonoBehaviour
{
    //Client config
    private const int MAX_USER = 100;
    private const int PORT = 9200;
    private const bool SHOW_LOGS = true;
    private const string SERVER_IP = "127.0.0.1";
    private const int BUFF_SIZE = 2048;

    //Class fields
    private byte reliableChannel;
    private int hostID;
    private int connectionID;
    private byte error; //https://docs.unity3d.com/ScriptReference/Networking.NetworkError.html

    public static Client instance;
    public bool IsStarted { get; private set; }
    public bool IsConnected { get; private set; }

    //Dictionary to store data handlers
    public delegate void ClientMessageHandlerDelegate(NetMsg msg);
    private Dictionary<NetOP, ClientMessageHandlerDelegate> dataHandlers = new Dictionary<NetOP, ClientMessageHandlerDelegate>();

    public void AddDataHandler(NetOP OP, ClientMessageHandlerDelegate handler){
      if (dataHandlers.ContainsKey(OP) == false) dataHandlers.Add(OP, handler);
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(NetOP), OP);
        string logMsg = String.Format("Handler already registered for this message : {0}", msgType);
        Debug.LogError(logMsg);
      }
    }

    // ------ Unity callbacks
    void Awake(){
      if (Client.instance == null) {
        Client.instance = this;
        DontDestroyOnLoad(this.gameObject);
      } else Destroy(this.gameObject);
    }

    void Start(){
      Init();
    }

    void Update(){
      UpdateMessagePump();
    }
    // ------- Unity callbacks


    //Start the client
    public void Init(){
      if (IsStarted) return;

      NetworkTransport.Init();
      ConnectionConfig cc = new ConnectionConfig();
      reliableChannel = cc.AddChannel(QosType.ReliableFragmentedSequenced);
      HostTopology topo = new HostTopology(cc, MAX_USER);

      hostID = NetworkTransport.AddHost(topo, 0);
      connectionID = NetworkTransport.Connect(hostID, SERVER_IP, PORT, 0, out error);
      IsStarted = true;
      IsConnected = false;

      if (SHOW_LOGS) Debug.Log(string.Format("Connecting to {0}:{1}...", SERVER_IP, PORT));
    }

    //Stop the client
    void Shutdown(){
      if (!IsStarted) return;

      NetworkTransport.Shutdown();
      IsStarted = false;
      IsConnected = false;

      if (SHOW_LOGS) Debug.Log("Client has stopped.");
    }

    //Read messages
    void UpdateMessagePump(){
      if (!IsStarted) return;

      int recHostID;
      int recConnectionID;
      int channelID;
      byte[] recBuffer = new byte[BUFF_SIZE];
      int dataSize;

      NetworkEventType type = NetworkTransport.Receive(
        out recHostID,
        out recConnectionID,
        out channelID,
        recBuffer,
        BUFF_SIZE,
        out dataSize,
        out error
      );

      switch (type) {
        case NetworkEventType.ConnectEvent:
          IsConnected = true;
          if (SHOW_LOGS) Debug.Log("Connected to the server");
          break;

        case NetworkEventType.DisconnectEvent:
          if (SHOW_LOGS) Debug.Log("Disconnected from the server");
          if ((NetworkError)error == NetworkError.Timeout) Shutdown();
          break;

        case NetworkEventType.DataEvent:
          ReadMessage(recBuffer);
          break;

        default:
        case NetworkEventType.Nothing:
        case NetworkEventType.BroadcastEvent:
          break;
      }
    }
    public void SendServer(NetMsg msg){
      if (!IsStarted) return;

      byte[] buffer = new byte[BUFF_SIZE];

      BinaryFormatter formatter = new BinaryFormatter();
      MemoryStream ms = new MemoryStream(buffer);
      formatter.Serialize(ms, msg);

      NetworkTransport.Send(
        hostID,
        connectionID,
        reliableChannel,
        buffer,
        BUFF_SIZE,
        out error
      );
    }

    public void ReadMessage(byte[] buffer) {
      BinaryFormatter formatter = new BinaryFormatter();
      MemoryStream ms = new MemoryStream(buffer);
      NetMsg msg = (NetMsg) formatter.Deserialize(ms);

      foreach(KeyValuePair<NetOP, ClientMessageHandlerDelegate> keyValuePair in dataHandlers) {
        if (keyValuePair.Key == msg.OP) {
          keyValuePair.Value?.Invoke(msg);
          return;
        }
      }
      if (SHOW_LOGS) Debug.LogError(string.Format("NetOP : {0}. Handler not set", msg.OP));
    }
}
#pragma warning restore CS0618
