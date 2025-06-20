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
        PlayerPrefs.SetInt("PlayerDefense", 0); // Reset defense saat masuk ke armor selector
        PlayerPrefs.SetString("SelectedArmor", ""); // Reset selected armor saat masuk ke armor selector
        PlayerPrefs.SetString("PurchasedArmors", ""); // Reset purchased armors saat masuk ke armor selector
        LoadPurchasedArmors();
        UpdateArmorButtonsUI();
    }

    public void SelectArmor(string armorName)
    {
        ArmorData selectedArmor = null;

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

        // Jika belum dibeli, beli dulu
        if (!purchasedArmors.Contains(selectedArmor.name))
        {
            if (!CoinManager.Instance.SpendCoins(selectedArmor.price))
            {
                StartCoroutine(ShowNotification("Koin tidak cukup untuk membeli armor ini!"));
                return;
            }

            purchasedArmors.Add(selectedArmor.name);
            SavePurchasedArmors();
            StartCoroutine(ShowNotification($"Berhasil membeli armor: {selectedArmor.name}!"));
        }
        else
        {
            StartCoroutine(ShowNotification($"Armor {selectedArmor.name} dipakai!"));
        }

        PlayerPrefs.SetString("SelectedArmor", selectedArmor.name);
        PlayerPrefs.SetInt("PlayerDefense", selectedArmor.defense);
        PlayerPrefs.Save();

        UpdateArmorButtonsUI();
    }
    IEnumerator ShowNotification(string message)
    {
        bgNotifikasi.SetActive(true);
        textUI.text = message;
        Debug.Log("Notifikasi Muncul...");
        yield return new WaitForSecondsRealtime(notifikasiDurasi);
        bgNotifikasi.SetActive(false);
        Debug.Log("Notifikasi Hilang...");
    }
    private void UpdateArmorButtonsUI()
    {
        string selected = PlayerPrefs.GetString("SelectedArmor", "");

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
    [System.Serializable]
    private class ArmorList
    {
        public List<string> armors;
        public HashSet<string> ToHashSet() => new HashSet<string>(armors ?? new List<string>());
    }
}

[System.Serializable]
public class ArmorButton
{
    public string armorName;
    public Button button;
    public Image buttonImage;
}
