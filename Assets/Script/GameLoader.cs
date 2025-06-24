using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    void Start()
    {
        bool isNewGame = PlayerPrefs.GetInt("IsNewGame", 0) == 1;
        if (!isNewGame)
        {
            LoadGame();
            LoadDeadEnemies();
        }
        else
        {
            Debug.Log("[NEW GAME] Skipping LoadGame & LoadDeadEnemies");
        }
    }

    private void LoadGame()
    {
        int bulletLevel = PlayerPrefs.GetInt("BulletLevel", 1);
        string selectedArmor = PlayerPrefs.GetString("SelectedArmor", "");
        int playerDefense = PlayerPrefs.GetInt("PlayerDefense", 0);
        string purchasedArmors = PlayerPrefs.GetString("PurchasedArmors", "");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && PlayerPrefs.HasKey("PlayerPosX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");

            player.transform.position = new Vector3(x, y, z);
            Debug.Log($"[LOAD POS] Player position loaded to: ({x}, {y}, {z})");
        }

        Debug.Log($"[LOAD GAME] BulletLevel: {bulletLevel}, Armor: {selectedArmor}, Defense: {playerDefense}");
    }

    private void LoadDeadEnemies()
    {
        string deadStr = PlayerPrefs.GetString("DeadEnemies", "");
        if (string.IsNullOrWhiteSpace(deadStr)) return;

        List<string> deadEnemies = new List<string>(deadStr.Split(','));

        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        foreach (var enemy in enemies)
        {
            if (string.IsNullOrEmpty(enemy.enemyID)) continue;

            // ✅ Hanya musuh bertag "Ghost" atau "Spider" yang boleh dihapus
            if (deadEnemies.Contains(enemy.enemyID) &&
                (enemy.CompareTag("Ghost") || enemy.CompareTag("Spider")))
            {
                Debug.Log($"[Destroy] Enemy ID: {enemy.enemyID}, Tag: {enemy.tag}, Name: {enemy.name}");
                Destroy(enemy.gameObject);
            }
            else
            {
                Debug.Log($"[Ignore] {enemy.name} | Tag: {enemy.tag} | ID: {enemy.enemyID}");
            }
        }

        EnemyHealth.deadEnemies = deadEnemies; // Set ulang cache global
    }
}
