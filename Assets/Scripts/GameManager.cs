using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MenuManager menuManager;
    public SceneField gameScene;
    public GameObject playerPrefab;

    private GameClient client;
    private Dictionary<CharacterType, PlayerStats> characters = new Dictionary<CharacterType, PlayerStats>();
    private Dictionary<int, Player> players = new Dictionary<int, Player>();

    void Awake() { //Use awake to load all the runtime resources
      if (instance == null) {
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        //Load resources ------------------------
        LoadCharacters();
        //Load resources ------------------------

      } else Destroy(this.gameObject);
    }

    void Start() {
      client = (GameClient.instance as GameClient);

      menuManager.Setup(characters);
      menuManager.tryConnectToServer += (string serverAddress) => client.ConnectToServer(serverAddress);
      client.onConnected += menuManager.OpenSelectCharacterMenu;
      menuManager.onCharacterSelected += StartGameRequest;

      menuManager.OpenConnectMenu();
    }


    //Load all the PlayerStats assets into a dictionary indexed by character type
    private void LoadCharacters() {
      Object[] resources = Resources.LoadAll("Stats/Players", typeof(PlayerStats));
      foreach(Object r in resources) {
        PlayerStats stats = r as PlayerStats;
        if (!characters.ContainsKey(stats.type)) characters.Add(stats.type, stats);
      }
    }

    public void StartGameRequest(CharacterType type) {
      //Client -> Server -> Client
      Vector3 spawnPosition = new Vector3(-8f, 0f, 0f);
      Quaternion spawnRotation = Quaternion.identity;
      PlayerStats stats = characters[type];
      int playerID = 1;
      bool isLocalPlayer = true;

      StartCoroutine(StartGame(spawnPosition, spawnRotation, stats, playerID, isLocalPlayer));
    }

    private IEnumerator StartGame(Vector3 pos, Quaternion rot, PlayerStats stats, int playerID, bool isLocalPlayer) {
      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameScene);
      while (!asyncLoad.isDone) {
        //Aqui se puede poner una barra de cargando o algo
        yield return null;
      }

      SpawnPlayer(pos, rot, stats, playerID, isLocalPlayer);
    }

    private void SpawnPlayer(Vector3 pos, Quaternion rot, PlayerStats stats, int playerID, bool isLocalPlayer) {
      Player player = Object.Instantiate(playerPrefab, pos, rot).GetComponent<Player>();
      players.Add(playerID, player);
      player.Setup(stats, isLocalPlayer);
      if (isLocalPlayer) player.gameObject.GetComponent<InputController>().onInputActive +=
        (PlayerInputMsg msg) => SyncPlayerInput(playerID, msg);
    }

    private void SyncPlayerInput(int playerID, PlayerInputMsg msg) {
      Debug.Log(System.String.Format("Player {0} sync input", playerID));
    }
}
