using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Vector2 spawnPosition;
    public PlayerStats stats;

    void Start()
    {
      Vector3 pos = new Vector3(spawnPosition.x, spawnPosition.y, 0f);
      Player player = Object.Instantiate(playerPrefab, pos, Quaternion.identity).GetComponent<Player>();
      player.Setup(stats);
    }
}
