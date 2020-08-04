using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Player", menuName = "Stats/Player")]
public class PlayerStats : ScriptableObject {
    public int health;
    public int healthRegen;
    public float mana;
    public float armour;
    public float movementSpeed;
    public float jumpForce;
    public float gravityScale;

    public GameObject bulletPrefab;
    public float shootRate;

}
