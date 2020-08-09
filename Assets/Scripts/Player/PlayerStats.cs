using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu (fileName = "New Player", menuName = "Stats/Player")]
public class PlayerStats : ScriptableObject {
    public CharacterType type;
  
    public float maxHealth;
    public float health;
    public float healthRegen;

    public float maxMana;
    public float mana;
    public float manaRegen;

    public float armour;

    public float movementSpeed;
    public float jumpForce;
    public float gravityScale;

    public GameObject bulletPrefab;
    public float shootRate;

    public Sprite menuImage;
}
