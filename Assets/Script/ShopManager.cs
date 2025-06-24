using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject ShopBg;
    // Start is called before the first frame update
    public void openShop()
    {
        // Simpan posisi pemain
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 pos = player.transform.position;
            PlayerPrefs.SetFloat("PlayerPosX", pos.x);
            PlayerPrefs.SetFloat("PlayerPosY", pos.y);
            PlayerPrefs.SetFloat("PlayerPosZ", pos.z);
        }

        // Simpan daftar musuh yang sudah mati
        string deadEnemiesStr = string.Join(",", EnemyHealth.deadEnemies.ToArray());
        PlayerPrefs.SetString("DeadEnemies", deadEnemiesStr);

        // Simpan scene saat ini
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastScene", currentScene);

        // Simpan koin jika ada CoinManager
        if (CoinManager.Instance != null)
        {
            PlayerPrefs.SetInt("PlayerCoins", CoinManager.Instance.coins);
        }

        PlayerPrefs.Save();

        // Buka shop
        Time.timeScale = 0;
        ShopBg.SetActive(true);

        Debug.Log("Data disimpan saat membuka shop.");
    }
    public void closeShop()
    {
        ShopBg.SetActive(false);
        Time.timeScale = 1;

        // Restore posisi pemain
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX", player.transform.position.x);
            float y = PlayerPrefs.GetFloat("PlayerPosY", player.transform.position.y);
            float z = PlayerPrefs.GetFloat("PlayerPosZ", player.transform.position.z);
            player.transform.position = new Vector3(x, y, z);
        }

        // Hapus musuh yang sudah dibunuh
        string deadEnemiesStr = PlayerPrefs.GetString("DeadEnemies", "");
        List<string> deadEnemyNames = new List<string>(deadEnemiesStr.Split(','));

        string[] enemyTags = { "Ghost", "Spider" };

        foreach (string tag in enemyTags)
        {
            GameObject[] enemies;
            try
            {
                enemies = GameObject.FindGameObjectsWithTag(tag);
            }
            catch (UnityException e)
            {
                Debug.LogWarning($"Tag '{tag}' tidak ditemukan di project: {e.Message}");
                continue;
            }

            if (enemies.Length == 0)
            {
                Debug.Log($"Tidak ada musuh dengan tag '{tag}' di scene.");
                continue;
            }

            foreach (GameObject enemy in enemies)
            {
                if (deadEnemyNames.Contains(enemy.name))
                {
                    Destroy(enemy);
                    Debug.Log($"Musuh '{enemy.name}' dengan tag '{tag}' dihapus dari scene.");
                }
            }
        }

        Debug.Log("Shop ditutup, musuh yang sudah dibunuh dihapus dari scene.");
    }
}
