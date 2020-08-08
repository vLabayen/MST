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
      menuManager.Setup(characters);
      menuManager.onCharacterSelected += StartGameRequest;
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

      StartCoroutine(StartGame(spawnPosition, spawnRotation, stats, playerID));
    }

    private IEnumerator StartGame(Vector3 pos, Quaternion rot, PlayerStats stats, int playerID) {
      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameScene);
      while (!asyncLoad.isDone) {
        //Aqui se puede poner una barra de cargando o algo
        yield return null;
      }

      SpawnPlayer(pos, rot, stats, playerID);
    }

    private void SpawnPlayer(Vector3 pos, Quaternion rot, PlayerStats stats, int playerID) {
      Player player = Object.Instantiate(playerPrefab, pos, rot).GetComponent<Player>();
      players.Add(playerID, player);
      player.Setup(stats);
    }
}
