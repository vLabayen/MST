using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    public ConnectMenu connectMenu;
    public SelectCharacterMenu selectCharacterMenu;

    public delegate void OnCharacterSelectedDelegate(CharacterType type);
    public event OnCharacterSelectedDelegate onCharacterSelected;

    public void Awake() {
      if (instance == null) {
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        connectMenu.gameObject.SetActive(false);
        selectCharacterMenu.gameObject.SetActive(false);

      } else Destroy(this.gameObject);
    }

    public void Setup(Dictionary<CharacterType, PlayerStats> characters) {
      GameClient client = GameClient.instance as GameClient;

      client.onConnected += ChangeToSelectCharacter;
      selectCharacterMenu.onCharacterSelected += OnCharacterSelected;

      connectMenu.Setup((string serverAddress) => client.ConnectToServer(serverAddress));
      selectCharacterMenu.Setup(characters);

      ChangeToConnectMenu();
    }


    private void OnCharacterSelected(CharacterType type) {
      connectMenu.gameObject.SetActive(false);
      selectCharacterMenu.gameObject.SetActive(false);
      onCharacterSelected?.Invoke(type);
    }

    public void ChangeToConnectMenu() {
      selectCharacterMenu.gameObject.SetActive(false);
      connectMenu.gameObject.SetActive(true);
    }
    public void ChangeToSelectCharacter() {
      connectMenu.gameObject.SetActive(false);
      selectCharacterMenu.gameObject.SetActive(true);
    }

}
