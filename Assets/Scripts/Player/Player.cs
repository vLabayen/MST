using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerShoot), typeof(InputController))]
[RequireComponent(typeof(Health), typeof(Mana), typeof(PlayerAbilities))]
public class Player : MonoBehaviour
{
    private PlayerStats stats;
    private bool isLocalPlayer;

    public Transform shootPoint;
    public LayerMask floorLayers;
    public Camera playerCamera;
    public Slider healthBar;
    public Slider manaBar;

  public void Setup(PlayerStats stats, bool isLocalPlayer) {
    this.stats = stats;
    this.isLocalPlayer = isLocalPlayer;
    this.gameObject.GetComponent<PlayerMovement>().Setup(stats, floorLayers);
    this.gameObject.GetComponent<PlayerShoot>().Setup(stats, shootPoint);
    this.gameObject.GetComponent<InputController>().Setup(playerCamera);
    this.gameObject.GetComponent<Health>().Setup(stats, healthBar);
    this.gameObject.GetComponent<Mana>().Setup(stats, manaBar);
    this.gameObject.GetComponent<PlayerAbilities>().Setup(stats);
  }
}
