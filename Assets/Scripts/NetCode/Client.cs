using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

#pragma warning disable CS0618
public abstract class Client : MonoBehaviour
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
    public delegate void ClientOnMessageReceivedDelegate(NetMsg msg);
    public delegate void ClientOnMessageSentDelegate(NetMsg msg);
    private readonly Dictionary<MessageType, ClientOnMessageReceivedDelegate> onMessageReceivedDelegates = new Dictionary<MessageType, ClientOnMessageReceivedDelegate>();
    private readonly Dictionary<MessageType, ClientOnMessageSentDelegate> onMessageSentDelegates = new Dictionary<MessageType, ClientOnMessageSentDelegate>();

    public event ClientOnMessageReceivedDelegate onAckMessageReceived;
    public event ClientOnMessageSentDelegate onAckMessageSent;

    protected void AddMessageDelegate(MessageType type, ClientOnMessageReceivedDelegate handler){
      if (onMessageReceivedDelegates.ContainsKey(type) == false) onMessageReceivedDelegates.Add(type, handler);
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), type);
        string logMsg = String.Format("OnReceivedHandler already registered for this message : {0}", msgType);
        Debug.LogError(logMsg);
      }
    }
    protected void AddMessageDelegate(MessageType type, ClientOnMessageSentDelegate handler){
      if (onMessageSentDelegates.ContainsKey(type) == false) onMessageSentDelegates.Add(type, handler);
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), type);
        string logMsg = String.Format("OnReceivedHandler already registered for this message : {0}", msgType);
        Debug.LogError(logMsg);
      }
    }

    public void OnReceivedSubscribe(MessageType type, ClientOnMessageReceivedDelegate callback) {
        if (onMessageReceivedDelegates.ContainsKey(type)) onMessageReceivedDelegates[type] += callback;
        else if (SHOW_LOGS) {
          string msgType = Enum.GetName(typeof(MessageType), type);
          string logMsg = String.Format("No delegate found for onReceived MessageType.{0}", msgType);
          Debug.Log(logMsg);
        }
    }
    public void OnSentSubscribe(MessageType type, ClientOnMessageSentDelegate callback) {
      if (onMessageSentDelegates.ContainsKey(type)) onMessageSentDelegates[type] += callback;
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), type);
        string logMsg = String.Format("No delegate found for onSent MessageType.{0}", msgType);
        Debug.Log(logMsg);
      }
    }
    public void OnReceiveUnsubscribe(MessageType type, ClientOnMessageReceivedDelegate callback) {
      if (onMessageReceivedDelegates.ContainsKey(type)) onMessageReceivedDelegates[type] -= callback;
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), type);
        string logMsg = String.Format("No delegate found for onReceived MessageType.{0}", msgType);
        Debug.Log(logMsg);
      }
    }
    public void OnSentUnsubscribe(MessageType type, ClientOnMessageSentDelegate callback) {
      if (onMessageSentDelegates.ContainsKey(type)) onMessageSentDelegates[type] -= callback;
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), type);
        string logMsg = String.Format("No delegate found for onSent MessageType.{0}", msgType);
        Debug.Log(logMsg);
      }
    }

    void Awake(){
      if (Client.instance == null) {
        Client.instance = this;
        DontDestroyOnLoad(this.gameObject);
      } else Destroy(this.gameObject);
    }

    //Start the client
    protected void Init(){
      if (IsStarted) return;

      NetworkTransport.Init();
      ConnectionConfig cc = new ConnectionConfig();
      reliableChannel = cc.AddChannel(QosType.ReliableFragmentedSequenced);
      HostTopology topo = new HostTopology(cc, MAX_USER);
      hostID = NetworkTransport.AddHost(topo, 0);
      connectionID = NetworkTransport.Connect(hostID, SERVER_IP, PORT, 0, out error);

      if (onMessageReceivedDelegates.ContainsKey(MessageType.Ack) == false) AddMessageDelegate(MessageType.Ack, onAckMessageReceived);
      if (onMessageSentDelegates.ContainsKey(MessageType.Ack) == false) AddMessageDelegate(MessageType.Ack, onAckMessageSent);
      IsStarted = true;
      IsConnected = false;

      if (SHOW_LOGS) {
        string logMsg = String.Format("Connecting to {0}:{1}...", SERVER_IP, PORT);
        Debug.Log(logMsg);
      }
    }

    //Stop the client
    protected void Shutdown(){
      if (!IsStarted) return;

      NetworkTransport.Shutdown();
      IsStarted = false;
      IsConnected = false;

      if (SHOW_LOGS) Debug.Log("Client has stopped.");
    }

    //Read messages
    protected void PopMessages(){
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
          OnConnected();
          IsConnected = true;
          if (SHOW_LOGS) Debug.Log("Connected to the server");
          break;

        case NetworkEventType.DisconnectEvent:
          OnDisconnected();
          if ((NetworkError)error == NetworkError.Timeout) Shutdown();
          if (SHOW_LOGS) Debug.Log("Disconnected from the server");
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

    //Read a message and raise the event
    private void ReadMessage(byte[] buffer) {
      BinaryFormatter formatter = new BinaryFormatter();
      MemoryStream ms = new MemoryStream(buffer);
      NetMsg msg = (NetMsg) formatter.Deserialize(ms);

      if (onMessageReceivedDelegates.ContainsKey(msg.type)) onMessageReceivedDelegates[msg.type]?.Invoke(msg);
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), msg.type);
        string logMsg = String.Format("OnReceivedHandler not set for MessageType.{0}", msgType);
        Debug.LogError(logMsg);
      }
    }

    //Send message to the server
    public void SendToServer(NetMsg msg){
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

      if (onMessageReceivedDelegates.ContainsKey(msg.type)) onMessageSentDelegates[msg.type]?.Invoke(msg);
      else if (SHOW_LOGS) {
        string msgType = Enum.GetName(typeof(MessageType), msg.type);
        string logMsg = String.Format("OnSentHandler not set for MessageType.{0}", msgType);
        Debug.LogError(logMsg);
      }
    }

    protected abstract void OnConnected();
    protected abstract void OnDisconnected();
}
#pragma warning restore CS0618
