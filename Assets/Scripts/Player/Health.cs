using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private Slider healthBar;
    private PlayerStats stats;
    private float health;

    public delegate void OnDiedDelegate(Health diedUnit);
    private event OnDiedDelegate onDied;

    public void Setup(PlayerStats stats, Slider healthBar)
    {
        this.stats = stats;
        this.healthBar = healthBar;
        SetHealth(stats.maxHealth);
    }

    public void OnDiedSubscribe(OnDiedDelegate callback) {
      onDied += callback;
    }

    private void SetHealth(float h) {
      if (h == health) return; //Evitamos updatear si no hay cambios para que no se refresque la UI

      health = h;
      healthBar.value = health / stats.maxHealth;
    }
    private void ReceiveDmg(float dmg) {
      if (dmg < 0) {
        Debug.LogError("Health.ReceiveDmg(float dmg) received a negative dmg value");
        return;
      }
      SetHealth(Mathf.Max(0, health - dmg));
      if (health == 0) onDied?.Invoke(this);
    }
    private void Heal(float heal) {
      if (heal < 0) {
        Debug.LogError("Health.Heal(float heal) received a negative heal value");
        return;
      }
      SetHealth(Mathf.Min(stats.maxHealth, health + heal));
    }

    void Update() {
      if (Input.GetKeyDown(KeyCode.F)) ReceiveDmg(10f);
    }

    //La regeneracion de vida es mejor ponerla en FixedUpdate que en Update
    void FixedUpdate() {
      Heal(stats.healthRegen * Time.fixedDeltaTime);
    }
}
