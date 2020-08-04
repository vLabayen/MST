using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public HealthBar healthBar;
    public PlayerStats stats;

    public int health;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.setMaxHealth(stats.health);
        health = stats.health;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            health -= 10;
            healthBar.setHealth(health);
        }
    }
}
