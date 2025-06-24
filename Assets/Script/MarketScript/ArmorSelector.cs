using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArmorSelector : MonoBehaviour
{
    [System.Serializable]
    public class ArmorData
    {
        public string name;
        public int defense;
        public int price;
    }

    [Header("Armor UI")]
    public ArmorButton[] armorButtons;
    public Sprite usedSprite;
    public Sprite notUsedSprite;

    public TextMeshProUGUI textUI;
    public GameObject bgNotifikasi;
    public ArmorData[] armors;
    public float notifikasiDurasi = 2f;

    private HashSet<string> purchasedArmors;

    private void Start()
    {
        bool isNewGame = PlayerPrefs.GetInt("IsNewGame", 0) == 1;

        if (isNewGame)
        {
            PlayerPrefs.SetString("SelectedArmor", "Default Armor"); // Set default selected
            PlayerPrefs.SetInt("PlayerDefense", 5); // Defense untuk Default Armor
            PlayerPrefs.SetString("PurchasedArmors", JsonUtility.ToJson(new ArmorList
            {
                armors = new List<string> { "Default Armor" } // Tandai Default Armor sebagai sudah dibeli
            }));
            PlayerPrefs.Save();
        }

        LoadPurchasedArmors();
        UpdateArmorButtonsUI();
    }

    public void SelectArmor(string armorName)
    {
        ArmorData selectedArmor = null;
        Debug.Log($"[DEBUG] Current Coins: {(CoinManager.Instance != null ? CoinManager.Instance.coins.ToString() : "NULL")}");

        foreach (var armor in armors)
        {
            if (armor.name == armorName)
            {
                selectedArmor = armor;
                break;
            }
        }

        if (selectedArmor == null)
        {
            Debug.LogWarning("Armor tidak ditemukan!");
            return;
        }
        Debug.Log($"[DEBUG] Trying to buy {selectedArmor.name}, Price: {selectedArmor.price}, Coins: {CoinManager.Instance.coins}");

        // Jika belum dibeli, beli dulu
        if (!purchasedArmors.Contains(selectedArmor.name))
        {
            if (!CoinManager.Instance.SpendCoins(selectedArmor.price))
            {
                NotificationManager.Instance?.Show("Not enough coins to buy this armor!.");
                return;
            }

            purchasedArmors.Add(selectedArmor.name);
            SavePurchasedArmors();
            NotificationManager.Instance?.Show($"Successfully purchased {selectedArmor.name}!");
        }
        else
        {
            NotificationManager.Instance?.Show($"{selectedArmor.name} is used!");
        }

        PlayerPrefs.SetString("SelectedArmor", selectedArmor.name);
        PlayerPrefs.SetInt("PlayerDefense", selectedArmor.defense);
        PlayerPrefs.Save();

        UpdateArmorButtonsUI();
    }
    private void UpdateArmorButtonsUI()
    {
        string selected = PlayerPrefs.GetString("SelectedArmor", "");
        Debug.Log($"[DEBUG] Selected Armor: {selected}");

        foreach (var button in armorButtons)
        {
            if (purchasedArmors.Contains(button.armorName))
            {
                if (button.armorName == selected)
                {
                    button.buttonImage.sprite = usedSprite; // Armor yang dipakai
                }
                else
                {
                    Debug.Log($"Armor {button.armorName} sudah dibeli tapi tidak dipakai.");
                    button.buttonImage.sprite = notUsedSprite; // Armor yang sudah dibeli tapi tidak dipakai
                    Debug.Log("Mengubah sprite untuk armor yang sudah dibeli tapi tidak dipakai.");
                }
            }
        }
    }

    private void LoadPurchasedArmors()
    {
        string json = PlayerPrefs.GetString("PurchasedArmors", "");

        if (string.IsNullOrEmpty(json))
        {
            purchasedArmors = new HashSet<string>();
            return;
        }

        try
        {
            ArmorList list = JsonUtility.FromJson<ArmorList>(json);
            purchasedArmors = list?.ToHashSet() ?? new HashSet<string>();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Gagal load purchased armors, reset ke default: " + e.Message);
            purchasedArmors = new HashSet<string>();
        }
    }
    private void SavePurchasedArmors()
    {
        ArmorList list = new ArmorList { armors = new List<string>(purchasedArmors) };
        string json = JsonUtility.ToJson(list);
        Debug.Log("Saving Purchased Armors: " + json);
        PlayerPrefs.SetString("PurchasedArmors", json);
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class ArmorButton
{
    public string armorName;
    public Button button;
    public Image buttonImage;
}
