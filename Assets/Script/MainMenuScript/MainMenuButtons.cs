using UnityEngine;
using UnityEngine.SceneManagement;  // Untuk mengelola scene
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MainMenuButtons : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public AudioSource fireballEffect;
    public AudioSource fireballEffect1;
    public AudioSource fireballEffect2;
    public AudioSource shootEffect;
    public AudioSource coinEffect;

    public GameObject soundOnButton;   // Referensi ke tombol SoundOn di UI
    public GameObject soundOffButton;

    private List<AudioSource> dynamicAudioSources = new List<AudioSource>();
    private bool isSoundOn = true;

    [Header("Bullet Upgrade")]
    public TextMeshProUGUI bulletLevelText;
    public Image upgradeCostImage;
    public Sprite[] upgradeCostSprites; // Sprite untuk cost upgrade level 1→2, 2→3, dst

    private int maxBulletLevel = 5;
    private int[] upgradeCosts = { 20, 40, 60, 80 };


    public void ChangeScene(string name)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(name);
    }

    // Fungsi untuk tombol "Quit Game"
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void paused()
    {
        Time.timeScale = 0;
    }

    public void resume()
    {
        Time.timeScale = 1;
    }

    public void SoundOff()
    {
        isSoundOn = false;
        backgroundMusic.mute = true;
        fireballEffect.mute = true;
        fireballEffect1.mute = true;
        fireballEffect2.mute = true;
        dynamicAudioSources.RemoveAll(audio => audio == null);
        foreach (var audio in dynamicAudioSources)
        {
            audio.mute = true;
        }
        shootEffect.mute = true;
        coinEffect.mute = true;
        // soundOffButton.SetActive(false);
        // soundOnButton.SetActive(true);
    }

    public void SoundOn()
    {
        isSoundOn = true;
        backgroundMusic.mute = false;
        fireballEffect.mute = false;
        fireballEffect1.mute = false;
        fireballEffect2.mute = false;
        dynamicAudioSources.RemoveAll(audio => audio == null);
        foreach (var audio in dynamicAudioSources)
        {
            audio.mute = false;
        }
        shootEffect.mute = false;
        coinEffect.mute = false;
        // soundOffButton.SetActive(true);
        // soundOnButton.SetActive(false);
    }

    public void RegisterDynamicAudio(AudioSource audio)
    {
        if (!dynamicAudioSources.Contains(audio))
        {
            dynamicAudioSources.Add(audio);
        }
        audio.mute = !isSoundOn;
    }

    public void NewGame()
    {
        if (CoinManager.Instance != null && CoinManager.Instance.gameObject.activeInHierarchy)
        {
            CoinManager.Instance.ResetCoins();
        }
        PlayerPrefs.SetInt("BulletLevel", 1);

        // Tandai sebagai permainan baru
        PlayerPrefs.SetInt("IsNewGame", 1);
        PlayerPrefs.SetInt("PlayerDefense", 0); // Reset defense saat masuk ke armor selector
        PlayerPrefs.SetInt("BulletLevel", 1); // Reset defense saat masuk ke armor selector
        PlayerPrefs.SetString("SelectedArmor", ""); // Reset selected armor saat masuk ke armor selector
        PlayerPrefs.SetString("PurchasedArmors", "");

        // Simpan perubahan
        PlayerPrefs.Save();
        ChangeScene("Level-1");
    }
    public void ContinueGame()
    {
        PlayerPrefs.SetInt("IsNewGame", 0);
        string lastScene = PlayerPrefs.GetString("LastScene", "Level-1");
        SceneManager.LoadScene(lastScene);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void SaveGame()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastScene", currentScene);

        // Simpan daftar musuh yang mati
        string deadEnemiesStr = string.Join(",", EnemyHealth.deadEnemies.ToArray());
        PlayerPrefs.SetString("DeadEnemies", deadEnemiesStr);

        // Simpan bullet level
        int bulletLevel = PlayerPrefs.GetInt("BulletLevel", 1);
        PlayerPrefs.SetInt("BulletLevel", bulletLevel);

        // Simpan armor dan defense
        string selectedArmor = PlayerPrefs.GetString("SelectedArmor", "");
        int playerDefense = PlayerPrefs.GetInt("PlayerDefense", 0);
        string purchasedArmors = PlayerPrefs.GetString("PurchasedArmors", "");

        PlayerPrefs.SetString("SelectedArmor", selectedArmor);
        PlayerPrefs.SetInt("PlayerDefense", playerDefense);
        PlayerPrefs.SetString("PurchasedArmors", purchasedArmors);

        PlayerPrefs.Save();
        ChangeScene("MainMenu");

        Debug.Log($"Game disimpan. Scene: {currentScene}, BulletLevel: {bulletLevel}, Armor: {selectedArmor}, Defense: {playerDefense}");
    }

    public void UpgradeBullet()
    {
        int currentLevel = PlayerPrefs.GetInt("BulletLevel", 1);

        if (currentLevel >= maxBulletLevel)
        {
            Debug.Log("Bullet sudah level maksimal.");
            return;
        }

        int cost = upgradeCosts[currentLevel - 1];

        if (CoinManager.Instance.SpendCoins(cost))
        {
            currentLevel++;
            PlayerPrefs.SetInt("BulletLevel", currentLevel);
            PlayerPrefs.Save();

            Debug.Log($"Berhasil upgrade bullet ke level {currentLevel}. Biaya: {cost} koin");
            UpdateUI();
        }
        else
        {
            Debug.Log("Koin tidak cukup untuk upgrade.");
        }
    }

    private void UpdateUI()
    {
        int currentLevel = PlayerPrefs.GetInt("BulletLevel", 1);
        bulletLevelText.text = $"{currentLevel}";

        if (currentLevel >= maxBulletLevel)
        {
            upgradeCostImage.enabled = false; // sembunyikan icon upgrade cost
        }
        else
        {
            upgradeCostImage.enabled = true;
            int spriteIndex = currentLevel - 1;

            if (spriteIndex >= 0 && spriteIndex < upgradeCostSprites.Length)
            {
                upgradeCostImage.sprite = upgradeCostSprites[spriteIndex];
            }
            else
            {
                Debug.LogWarning("Upgrade cost sprite tidak ditemukan untuk level: " + currentLevel);
                upgradeCostImage.sprite = null;
            }
        }
    }
}
