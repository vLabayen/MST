using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public HealthBar healthBar;
    public ManaBar manaBar;
    public PlayerStats stats;
    private float health;
    private float healthRegen;
    private float maxHealth;
    private float mana;
    private float manaRegen;
    private float maxMana;

    // Start is called before the first frame update
    void Start()
    {
        //  Setting Health
        healthBar.setMaxHealth((int)stats.maxHealth);
        health = stats.maxHealth;
        stats.health = health;
        healthRegen = stats.healthRegen;
        maxHealth = health;

        //  Setting Mana
        manaBar.setMaxMana((int)stats.maxMana);
        mana = stats.maxMana;
        stats.mana = mana;
        manaRegen = stats.manaRegen;
        maxMana = mana;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            healthBar.setHealth((int)(health - 10f));
            health -= 10f;
        }
        if (maxHealth > health)
        {
            health += manaRegen * Time.deltaTime;
            healthBar.setHealth((int)health);
        }
    }
}
