using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Import this to access Slider

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;

    // Method untuk update slider
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth / maxHealth;
    }
}
