using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

#pragma warning disable CS0618
public class Server : MonoBehaviour
{
    //Server config
    private const int MAX_USER = 100;
    private const int PORT = 9200;
    private const bool SHOW_LOGS = true;
    private const int BUFF_SIZE = 2048;

    //Internal variables
    private byte reliableChannel;
    private int hostID;
    private byte error; //https://docs.unity3d.com/ScriptReference/Networking.NetworkError.html

    //Class fields
    public static Server instance { get; private set; }
    public bool isStarted { get; private set; }
    public int connectedPlayers { get { return clients.Count(); } }
    public readonly Dictionary<int, SClient> clients = new Dictionary<int, SClient>();

    //Dictionary to store data handlers
    public delegate void ServerMessageHandlerDelegate(NetMsg msg, int connectionID);
    private Dictionary<NetOP, ServerMessageHandlerDelegate> dataHandlers = new Dictionary<NetOP, ServerMessageHandlerDelegate>();

    public void AddDataHandler(NetOP OP, ServerMessageHandlerDelegate handler){
      if (dataHandlers.ContainsKey(OP) == false) dataHandlers.Add(OP, handler);
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(NetOP), OP);
        string logMsg = String.Format("Handler already registered for this message : {0}", msgType);
        Debug.LogError(logMsg);
      }
    }

    // ------ Unity callbacks
    void Awake(){
      if (Server.instance == null) {
        Server.instance = this;
        DontDestroyOnLoad(this.gameObject);
      } else Destroy(this.gameObject);
    }

    void Start(){
      Init();
    }

    void Update(){
      UpdateMessagePump();
      // ConsoleThread();
    }
    // ------- Unity callbacks


    //Start the server
    void Init(){
      if (isStarted) return;

      //Init the LLAPI
      NetworkTransport.Init();
      ConnectionConfig cc = new ConnectionConfig();
      reliableChannel = cc.AddChannel(QosType.ReliableFragmentedSequenced);
      HostTopology topo = new HostTopology(cc, MAX_USER);

      //Start the server
      hostID = NetworkTransport.AddHost(topo, PORT, null);
      isStarted = true;

      //SHOW_LOGS
      if (SHOW_LOGS) {
        string logMsg = string.Format("Opening socket on port {0}...", PORT);
        Debug.Log(logMsg);
      }
    }


    //Stop the server
    void Shutdown(){
      if (!isStarted) return;

      NetworkTransport.Shutdown();
      isStarted = false;

      if (SHOW_LOGS) Debug.Log("Closing server socket.");
    }

    //Debug clients
    void ConsoleThread(){
      if (Input.GetKeyDown("space")){
        foreach(SClient c in clients.Values) c.ShowInfo();
      }
    }

    //Read messages
    void UpdateMessagePump(){
      if (!isStarted) return;

      int recHostID;
      int connectionID;
      int channelID;
      byte[] recBuffer = new byte[BUFF_SIZE];
      int dataSize;

      NetworkEventType type = NetworkTransport.Receive(
        out recHostID,    //client platform id
        out connectionID, //client id
        out channelID,    //channel id
        recBuffer,
        BUFF_SIZE,
        out dataSize,
        out error
      );

      switch (type)
      {
        case NetworkEventType.ConnectEvent:
          if (SHOW_LOGS) Debug.Log(string.Format("Connected user {0}", connectionID));
          clients.Add(connectionID, new SClient(connectionID));
          break;

        case NetworkEventType.DisconnectEvent:
          if (SHOW_LOGS) Debug.Log(string.Format("Disconnected user {0}", connectionID));
          clients.Remove(connectionID);
          break;

        case NetworkEventType.DataEvent:
          ReadMessage(recBuffer, connectionID);
          break;

        default:
        case NetworkEventType.Nothing:
        case NetworkEventType.BroadcastEvent:
          break;
      }
    }

    //Send message to a client
    public void SendClient(NetMsg msg, int connectionID){
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

    //Broadcast message
    public void SendBroadcast(NetMsg msg) {
      foreach(KeyValuePair<int, SClient> keyValuePair in clients) SendClient(msg, keyValuePair.Key);
    }


    public void ReadMessage(byte[] buffer, int connectionID) {
      BinaryFormatter formatter = new BinaryFormatter();
      MemoryStream ms = new MemoryStream(buffer);
      NetMsg msg = (NetMsg) formatter.Deserialize(ms);

      foreach(KeyValuePair<NetOP, ServerMessageHandlerDelegate> keyValuePair in dataHandlers) {
        if (keyValuePair.Key == msg.OP) {
          keyValuePair.Value?.Invoke(msg, connectionID);
          return;
        }
      }
      if (SHOW_LOGS) Debug.LogError(string.Format("NetOP : {0}. Handler not set", msg.OP));
    }
}
#pragma warning restore CS0618s
