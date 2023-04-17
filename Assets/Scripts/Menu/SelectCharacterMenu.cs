using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterMenu : MonoBehaviour
{
  public GameObject buttonPrefab;
  
  public delegate void OnCharacterSelectedDelegate(CharacterType type);
  public event OnCharacterSelectedDelegate onCharacterSelected;

  //TODO: El player stats igual hay que hacer que forme parte de un ScriptableObject mas grande
  //y en el tener por un lado los stats, por otro lado los graficos de menus, etc...
  //Aqui solo queremos los graficos de menu
  public void Setup(Dictionary<CharacterType, PlayerStats> characters) {
    foreach(KeyValuePair<CharacterType, PlayerStats> kv in characters) {
      GameObject uiElement = Object.Instantiate(buttonPrefab, this.transform);
      uiElement.GetComponent<Image>().sprite = kv.Value.menuImage;
      uiElement.GetComponent<Button>().onClick.AddListener(() => onCharacterSelected?.Invoke(kv.Key));
    }
  }
}
