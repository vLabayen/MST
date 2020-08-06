using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    private PlayerStats stats;
    private Mana mana;

    public void Setup(PlayerStats stats)
    {
        this.stats =  stats;
        this.mana = this.gameObject.GetComponent<Mana>();
    }

    //Esto habra que hacerlo mejor para dar soporte a varios tipos de habilidades
    private void CastSpell() {
      if (mana.SpendMana(15f)) Debug.Log("Habilidad lanzada");
      else Debug.Log("No hay suficiente mana");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) CastSpell(); //Esto hay que moverlo al input controller
    }
}
