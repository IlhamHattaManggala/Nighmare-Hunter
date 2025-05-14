using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public HealthBar healthBar;
    private Animator anim;
    public bool isDead { get; private set; }

    private void Start()
    {
        isDead = false;
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Sisa Darah Ghost: " + currentHealth);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Ghost Dead");

        if (anim != null)
        {
            anim.SetTrigger("Death");
        }

        isDead = true;

        StartCoroutine(DestroyAfterDeath());
        CoinManager coinManager = FindObjectOfType<CoinManager>();
        if (coinManager != null)
        {
            coinManager.AddCoin(1); // Setiap musuh mati, dapat 1 koin
        }
    }

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(1.0f); // Delay sesuai animasi
        Destroy(gameObject);
    }
}
