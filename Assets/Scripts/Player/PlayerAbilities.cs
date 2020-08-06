using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    private ManaBar manaBar;
    private PlayerStats stats;
    private float mana;

    public void Setup(ManaBar manaBar, PlayerStats stats)
    {
        this.manaBar = manaBar;
        this.stats = stats;
        mana = stats.maxMana;
        manaBar.setMaxMana((int)stats.maxMana);
        stats.health = mana;
    }    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            mana -= 15f;
            manaBar.setMana((int) mana);
        }
        
        if (stats.maxMana > mana)
        {
            mana += stats.manaRegen * Time.deltaTime;
            manaBar.setMana((int)mana);
        }
    }
}
