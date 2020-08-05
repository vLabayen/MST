using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

#pragma warning disable CS0618
public abstract class Server : MonoBehaviour
{
    //Server config
    private const int MAX_USER = 100;
    private const int PORT = 9200;
    private const bool SHOW_LOGS = true;
    private const int BUFF_SIZE = 2048;

    //Internal variables <-- Wrapper for more than one cannel?
    private byte reliableChannel;
    private int hostID;
    private byte error; //https://docs.unity3d.com/ScriptReference/Networking.NetworkError.html

    //Class fields
    public static Server instance { get; private set; }
    public bool isStarted { get; private set; }
    public int connectedPlayers { get { return clients.Count(); } }
    public readonly Dictionary<int, SClient> clients = new Dictionary<int, SClient>(); //Key : connectionID, value : ServerClient

    //Dictionary to store data handlers
    public delegate void ServerOnMessageReceivedDelegate(NetMsg msg, int connectionID);
    public delegate void ServerOnMessageSentDelegate(NetMsg msg, int connectionID);
    private readonly Dictionary<MessageType, ServerOnMessageReceivedDelegate> onMessageReceivedDelegates = new Dictionary<MessageType, ServerOnMessageReceivedDelegate>();
    private readonly Dictionary<MessageType, ServerOnMessageSentDelegate> onMessageSentDelegates = new Dictionary<MessageType, ServerOnMessageSentDelegate>();

    private event ServerOnMessageReceivedDelegate onAckMessageReceived;
    private event ServerOnMessageSentDelegate onAckMessageSent;

    protected void AddMessageDelegate(MessageType type, ServerOnMessageReceivedDelegate handler){
      if (onMessageReceivedDelegates.ContainsKey(type) == false) onMessageReceivedDelegates.Add(type, handler);
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), type);
        string logMsg = String.Format("OnReceivedHandler already registered for this message : {0}", msgType);
        Debug.LogError(logMsg);
      }
    }
    protected void AddMessageDelegate(MessageType type, ServerOnMessageSentDelegate handler){
      if (onMessageSentDelegates.ContainsKey(type) == false) onMessageSentDelegates.Add(type, handler);
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), type);
        string logMsg = String.Format("OnSentHandler already registered for this message : {0}", msgType);
        Debug.LogError(logMsg);
      }
    }

    public void OnReceivedSubscribe(MessageType type, ServerOnMessageReceivedDelegate callback) {
        if (onMessageReceivedDelegates.ContainsKey(type)) onMessageReceivedDelegates[type] += callback;
        else if (SHOW_LOGS) {
          string msgType = Enum.GetName(typeof(MessageType), type);
          string logMsg = String.Format("No delegate found for onReceived MessageType.{0}", msgType);
          Debug.Log(logMsg);
        }
    }
    public void OnSentSubscribe(MessageType type, ServerOnMessageSentDelegate callback) {
      if (onMessageSentDelegates.ContainsKey(type)) onMessageSentDelegates[type] += callback;
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), type);
        string logMsg = String.Format("No delegate found for onSent MessageType.{0}", msgType);
        Debug.Log(logMsg);
      }
    }
    public void OnReceiveUnsubscribe(MessageType type, ServerOnMessageReceivedDelegate callback) {
      if (onMessageReceivedDelegates.ContainsKey(type)) onMessageReceivedDelegates[type] -= callback;
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), type);
        string logMsg = String.Format("No delegate found for onReceived MessageType.{0}", msgType);
        Debug.Log(logMsg);
      }
    }
    public void OnSentUnsubscribe(MessageType type, ServerOnMessageSentDelegate callback) {
      if (onMessageSentDelegates.ContainsKey(type)) onMessageSentDelegates[type] -= callback;
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), type);
        string logMsg = String.Format("No delegate found for onSent MessageType.{0}", msgType);
        Debug.Log(logMsg);
      }
    }

    void Awake(){
      if (Server.instance == null) {
        Server.instance = this;
        DontDestroyOnLoad(this.gameObject);
      } else Destroy(this.gameObject);
    }

    //Start the server
    protected void Init(){
      if (isStarted) return;

      //Init the LLAPI
      NetworkTransport.Init();
      ConnectionConfig cc = new ConnectionConfig();
      reliableChannel = cc.AddChannel(QosType.ReliableFragmentedSequenced);
      HostTopology topo = new HostTopology(cc, MAX_USER);
      hostID = NetworkTransport.AddHost(topo, PORT, null);

      if (onMessageReceivedDelegates.ContainsKey(MessageType.Ack) == false) AddMessageDelegate(MessageType.Ack, onAckMessageReceived);
      if (onMessageSentDelegates.ContainsKey(MessageType.Ack) == false) AddMessageDelegate(MessageType.Ack, onAckMessageSent);
      isStarted = true;

      //SHOW_LOGS
      if (SHOW_LOGS) {
        string logMsg = String.Format("Opening socket on port {0}...", PORT);
        Debug.Log(logMsg);
      }
    }

    //Stop the server
    protected void Shutdown(){
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
    protected void PopMessages(){
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
          SClient newclient = new SClient(connectionID);
          clients.Add(connectionID, newclient);
          OnClientConnected(newclient);

          if (SHOW_LOGS) {
            string logMsg = String.Format("Connected user {0}", connectionID);
            Debug.Log(logMsg);
          }
          break;

        case NetworkEventType.DisconnectEvent:
          SClient oldclient = clients[connectionID];
          clients.Remove(connectionID);
          OnClientDisconnected(oldclient);

          if (SHOW_LOGS) {
            string logMsg = String.Format("Disconnected user {0}", connectionID);
            Debug.Log(logMsg);
          }
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

    //Read a message and raise the event
    private void ReadMessage(byte[] buffer, int connectionID) {
      BinaryFormatter formatter = new BinaryFormatter();
      MemoryStream ms = new MemoryStream(buffer);
      NetMsg msg = (NetMsg) formatter.Deserialize(ms);

      if (onMessageReceivedDelegates.ContainsKey(msg.type)) onMessageReceivedDelegates[msg.type]?.Invoke(msg, connectionID);
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), msg.type);
        string logMsg = String.Format("OnReceivedHandler not set for MessageType.{0}", msgType);
        Debug.LogError(logMsg);
      }
    }

    //Send message to a client
    public void SendToClient(NetMsg msg, int connectionID){
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

      if (onMessageReceivedDelegates.ContainsKey(msg.type)) onMessageSentDelegates[msg.type]?.Invoke(msg, connectionID);
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), msg.type);
        string logMsg = String.Format("OnSentHandler not set for MessageType.{0}", msgType);
        Debug.LogError(logMsg);
      }
    }

    //Broadcast message
    public void SendBroadcast(NetMsg msg) {
      foreach(KeyValuePair<int, SClient> keyValuePair in clients) SendToClient(msg, keyValuePair.Key);
    }

    //Broadcast message except for a player
    public void SendBroadcast(NetMsg msg, int connectionID) {
      foreach(KeyValuePair<int, SClient> keyValuePair in clients)
        if (keyValuePair.Key != connectionID) SendToClient(msg, keyValuePair.Key);
    }

    protected abstract void OnClientConnected(SClient client);
    protected abstract void OnClientDisconnected(SClient client);
}
#pragma warning restore CS0618s
