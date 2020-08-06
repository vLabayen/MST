using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    private HealthBar healthBar;
    private PlayerStats stats;
    private float health;
    
    public void Setup(HealthBar healthBar, PlayerStats stats)
    {
        this.healthBar = healthBar;
        this.stats = stats;
        healthBar.setMaxHealth((int)stats.maxHealth);
        health = stats.maxHealth;
        stats.health = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            healthBar.setHealth((int)(health - 10f));
            health -= 10f;
        }
        if (stats.maxHealth > health)
        {
            health += stats.healthRegen * Time.deltaTime;
            healthBar.setHealth((int)health);
        }
    }
}
