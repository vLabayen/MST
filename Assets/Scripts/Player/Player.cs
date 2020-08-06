using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerShoot), typeof(InputController))]
public class Player : MonoBehaviour
{
    public PlayerStats stats;
    public Transform shootPoint;
    public LayerMask floorLayers;
    public Camera playerCamera;
    public HealthBar healthBar;
    public ManaBar manaBar;

  void Start () {
    this.gameObject.GetComponent<PlayerMovement>().Setup(stats, floorLayers);
    this.gameObject.GetComponent<PlayerShoot>().Setup(stats, shootPoint);
    this.gameObject.GetComponent<InputController>().Setup(playerCamera);
    this.gameObject.GetComponent<Health>().Setup(healthBar,stats);
    this.gameObject.GetComponent<PlayerAbilities>().Setup(manaBar, stats);
  }
}
