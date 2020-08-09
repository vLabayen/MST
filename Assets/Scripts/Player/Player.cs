using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerShoot), typeof(InputController))]
[RequireComponent(typeof(Health), typeof(Mana), typeof(PlayerAbilities))]
public class Player : MonoBehaviour
{
    private PlayerStats stats;
    public Transform shootPoint;
    public LayerMask floorLayers;
    public Camera playerCamera;
    public Slider healthBar;
    public Slider manaBar;

  public void Setup(PlayerStats stats) {
    this.stats = stats;
    this.gameObject.GetComponent<PlayerMovement>().Setup(stats, floorLayers);
    this.gameObject.GetComponent<PlayerShoot>().Setup(stats, shootPoint);
    this.gameObject.GetComponent<InputController>().Setup(playerCamera);
    this.gameObject.GetComponent<Health>().Setup(stats, healthBar);
    this.gameObject.GetComponent<Mana>().Setup(stats, manaBar);
    this.gameObject.GetComponent<PlayerAbilities>().Setup(stats);
  }
}
