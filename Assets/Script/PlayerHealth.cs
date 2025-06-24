using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public HealthBar healthBar;
    public GameObject bgDeath;
    private int playerDefense;


    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
        playerDefense = PlayerPrefs.GetInt("PlayerDefense", 5); // Ambil dari PlayerPrefs
        Debug.Log("Defense loaded in PlayerHealth: " + playerDefense);
    }

    public void TakeDamage(int damage)
    {
        playerDefense = PlayerPrefs.GetInt("PlayerDefense", 5); // ambil ulang setiap kali kena damage

        int finalDamage = Mathf.Max(damage - playerDefense, 0);
        Debug.Log($"Terima damage {damage}, setelah armor (def {playerDefense}): {finalDamage}");

        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water")) // pastikan AirTilemap kamu punya tag "Water"
        {
            Debug.Log("Player jatuh ke air!");
            currentHealth = 0;
            Die();
        }
    }


    private void Die()
    {
        Debug.Log("Player Dead");
        if (bgDeath != null)
        {
            bgDeath.SetActive(true);
        }
        Time.timeScale = 0;
        GetComponent<PlayerMovement>().enabled = false;
    }
}
