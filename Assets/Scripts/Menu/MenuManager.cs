using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    public ConnectMenu connectMenu;
    public SelectCharacterMenu selectCharacterMenu;

    public delegate void TryConnectToServerDelegate(string serverAddress);
    public delegate void OnCharacterSelectedDelegate(CharacterType type);
    public event TryConnectToServerDelegate tryConnectToServer;
    public event OnCharacterSelectedDelegate onCharacterSelected;

    public void Awake() {
      if (instance == null) {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        HideMenu();

      } else Destroy(this.gameObject);
    }

    public void Setup(Dictionary<CharacterType, PlayerStats> characters) {
      connectMenu.tryConnectToServer += (string serverAddress) => tryConnectToServer?.Invoke(serverAddress);
      selectCharacterMenu.onCharacterSelected += OnCharacterSelected;

      connectMenu.Setup();
      selectCharacterMenu.Setup(characters);
    }

    private void OnCharacterSelected(CharacterType type) {
      OpenGameMenu();
      onCharacterSelected?.Invoke(type);
    }

    //Change menu screen
    public void HideMenu() {
      connectMenu.gameObject.SetActive(false);
      selectCharacterMenu.gameObject.SetActive(false);
    }
    public void OpenConnectMenu() {
      selectCharacterMenu.gameObject.SetActive(false);
      connectMenu.gameObject.SetActive(true);
    }
    public void OpenSelectCharacterMenu() {
      connectMenu.gameObject.SetActive(false);
      selectCharacterMenu.gameObject.SetActive(true);
    }
    public void OpenGameMenu() {
      connectMenu.gameObject.SetActive(false);
      selectCharacterMenu.gameObject.SetActive(false);
    }

}
