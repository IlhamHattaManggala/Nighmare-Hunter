using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Tambahkan di atas

public class FireProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 5f;
    public int damage = 1;
    private Vector2 direction;


    void Start()
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.Play(); // Mainkan suara saat peluru muncul
        }

        GameObject menu = GameObject.Find("MenuCanvas"); // atau objek yang menyimpan MainMenuButtons
        menu.GetComponent<MainMenuButtons>().RegisterDynamicAudio(audio);
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        damage = sceneIndex * 5;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name); // Ini buat cek object apa yang kena

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Health after damage: " + playerHealth.currentHealth);
            }
            else
            {
                Debug.LogWarning("PlayerHealth component not found on Player!");
            }
            Destroy(gameObject);
        }
    }

}
