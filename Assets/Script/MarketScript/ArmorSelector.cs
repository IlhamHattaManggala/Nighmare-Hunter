using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ArmorSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectArmor(string armorName)
    {
        int defenseValue = 0;

        // Tentukan nilai defense berdasarkan armor yang dipilih
        switch (armorName)
        {
            case "DefaultArmor":
                defenseValue = 5;
                break;
            case "SilverArmor":
                defenseValue = 10;
                break;
            case "BlueArmor":
                defenseValue = 15;
                break;
            case "KnightArmor":
                defenseValue = 20;
                break;
        }

        // Simpan data ke PlayerPrefs
        PlayerPrefs.SetString("SelectedArmor", armorName);
        PlayerPrefs.SetInt("PlayerDefense", defenseValue);
        PlayerPrefs.Save();

        Debug.Log($"Armor '{armorName}' dipilih. Defense: {defenseValue}");
    }

    public void GoBackToLastScene()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", "MainScene"); // fallback
        PlayerPrefs.DeleteKey("LastScene");
        SceneManager.LoadScene(lastScene);
    }
}
