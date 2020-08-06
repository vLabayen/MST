using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    private Slider manaBar;
    private PlayerStats stats;
    private float mana;

    public void Setup(PlayerStats stats, Slider manaBar)
    {
        this.stats = stats;
        this.manaBar = manaBar;
        SetMana(stats.maxMana);
    }

    private void SetMana(float m) {
      if (m == mana) return; //Evitamos updatear si no hay cambios para que no se refresque la UI

      mana = m;
      manaBar.value = mana / stats.maxMana;
    }
    public bool CanSpendMana(float amount) => (mana - amount >= 0);
    public bool SpendMana(float amount) {
      if (amount < 0) {
        Debug.LogError("Mana.SpendMana(float amount) received a negative amount value");
        return false;
      }
      if (!CanSpendMana(amount)) return false;

      SetMana(mana - amount);
      return true;
    }
    private void RegenMana(float amount) {
      if (amount < 0) {
        Debug.LogError("Mana.RegenMana(float amount) received a negative amount value");
        return;
      }
      SetMana(Mathf.Min(stats.maxMana, mana + amount));
    }

    //La regeneracion de mana es mejor ponerla en FixedUpdate que en Update
    void FixedUpdate() {
      RegenMana(stats.manaRegen * Time.fixedDeltaTime);
    }
}
