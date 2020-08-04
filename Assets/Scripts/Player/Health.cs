using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public HealthBar healthBar;
    public PlayerStats stats;
    private int health;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.setMaxHealth(stats.maxHealth);
        health = stats.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            healthBar.setHealth(stats.health -= 10);
        }


    }
}
