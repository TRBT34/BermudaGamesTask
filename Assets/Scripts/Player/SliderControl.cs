using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SliderControl : MonoBehaviour
{
    public Slider slider;


    // Can barýn deðerlerini ayarlamak için kullanýlýr.
    public void SetMaxHealth(int health) 
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
