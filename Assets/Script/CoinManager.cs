using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    public int coins = 0;
    public TextMeshProUGUI coinText; // Taruh text UI di sini dari inspector
    [Header("Sound Settings")]
    public AudioClip coinEffect;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            if (coinText != null)
            {
                coinText.gameObject.SetActive(false); // Sembunyikan teks coin
            }

            // Jika ingin CoinManager juga nonaktif total:
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true); // Aktifkan kembali kalau scene bukan MainMenu
            coinText.gameObject.SetActive(true);

            GameObject coinObj = GameObject.FindWithTag("CoinText");
            if (coinObj != null)
            {
                coinText = coinObj.GetComponent<TextMeshProUGUI>();
                coinText.gameObject.SetActive(true);
                UpdateCoinUI();
            }
            else
            {
                Debug.LogWarning("CoinText tidak ditemukan di scene: " + scene.name);
            }
        }
    }

    public void ResetCoins()
    {
        coins = 0;
        UpdateCoinUI();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateCoinUI();
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        if (coinEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(coinEffect, 1f); // Bisa atur volume juga
        }
        UpdateCoinUI();
    }

    public void OpenChest()
    {
        int chestCoins = GetChestCoins();
        AddCoin(chestCoins);
    }

    private int GetChestCoins()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        int levelNumber = 1;
        if (currentScene.StartsWith("Level-"))
        {
            string levelStr = currentScene.Substring(6);
            int.TryParse(levelStr, out levelNumber);
        }

        int baseCoins = 20;
        int chestCoins = baseCoins * (int)Mathf.Pow(2, levelNumber - 1);

        return chestCoins;
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
        }
        else
        {
            Debug.LogWarning("Coin Text UI belum diassign di inspector!");
        }
    }
}
