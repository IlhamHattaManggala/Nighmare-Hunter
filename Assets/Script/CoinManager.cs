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

            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);

            GameObject coinObj = FindInactiveObjectByTag("CoinText");
            if (coinObj != null)
            {
                coinObj.SetActive(true); // Aktifkan jika belum aktif
                coinText = coinObj.GetComponent<TextMeshProUGUI>();
                UpdateCoinUI();
            }
            else
            {
                Debug.LogWarning("CoinText tidak ditemukan bahkan dalam objek yang nonaktif.");
            }
        }
    }

    private GameObject FindInactiveObjectByTag(string tag)
    {
        GameObject[] objs = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in objs)
        {
            if (obj.CompareTag(tag))
            {
                return obj;
            }
        }
        return null;
    }


    public void ResetCoins()
    {
        coins = 0;
        PlayerPrefs.SetInt("PlayerCoins", coins);
        UpdateCoinUI();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        coins = PlayerPrefs.GetInt("PlayerCoins", 0);
        UpdateCoinUI();
    }

    private void Update()
    {
        UpdateCoinUI();
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt("PlayerCoins", coins);
        PlayerPrefs.Save();
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

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            PlayerPrefs.SetInt("PlayerCoins", coins);
            PlayerPrefs.Save();
            UpdateCoinUI();
            return true;
        }
        else
        {
            Debug.Log("Koin tidak cukup untuk pengurangan.");
            return false;
        }
    }

}
