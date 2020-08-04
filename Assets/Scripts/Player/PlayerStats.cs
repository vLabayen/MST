using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Player", menuName = "Stats/Player")]
public class PlayerStats : ScriptableObject {
    public int maxHealth;
    public int health;
    public int healthRegen;
    public int maxMana;
    public int mana;
    public int manaRegen;
    public int armour;
    public float movementSpeed;
    public float jumpForce;
    public float gravityScale;

    public GameObject bulletPrefab;
    public float shootRate;
}
