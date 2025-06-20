using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    void Start()
    {
        LoadGame();
        LoadDeadEnemies();
    }

    private void LoadGame()
    {
        int bulletLevel = PlayerPrefs.GetInt("BulletLevel", 1);
        string selectedArmor = PlayerPrefs.GetString("SelectedArmor", "");
        int playerDefense = PlayerPrefs.GetInt("PlayerDefense", 0);
        string purchasedArmors = PlayerPrefs.GetString("PurchasedArmors", "");

        Debug.Log($"[LOAD GAME] BulletLevel: {bulletLevel}, Armor: {selectedArmor}, Defense: {playerDefense}");

        // Apply ke sistemmu (kalau kamu punya sistem pengatur armor/bullet stat)
        // Misalnya:
        // PlayerStats.Instance.SetBulletLevel(bulletLevel);
        // PlayerStats.Instance.SetDefense(playerDefense);
    }

    private void LoadDeadEnemies()
    {
        string deadStr = PlayerPrefs.GetString("DeadEnemies", "");
        if (string.IsNullOrWhiteSpace(deadStr)) return;

        List<string> deadEnemies = new List<string>(deadStr.Split(','));

        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        foreach (var enemy in enemies)
        {
            if (deadEnemies.Contains(enemy.enemyID))
            {
                Destroy(enemy.gameObject);  // Hapus musuh yang sudah mati
            }
        }

        EnemyHealth.deadEnemies = deadEnemies;  // Set ulang cache global
    }
}
