using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Slider slider;

    public void setMaxMana(int maxMana)
    {
        slider.maxValue = maxMana;
        slider.value = maxMana;
    }

    public void setHealth(int mana)
    {
        slider.value = mana;
    }
}
