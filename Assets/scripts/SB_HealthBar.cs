using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SB_HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public Image background;

    public void SetHealth(int health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);

        if(slider.maxValue == slider.value)
        {
            fill.enabled = false;
            background.enabled = false;
        }
        else
        {
            fill.enabled = true;
            background.enabled = true;
        }
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);

        fill.enabled = false;
        background.enabled = false;
    }
}
