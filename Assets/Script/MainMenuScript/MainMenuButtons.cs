using UnityEngine;
using UnityEngine.SceneManagement;  // Untuk mengelola scene
using UnityEngine.UI;
using System.Collections.Generic;

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


    public void ChangeScene(string name)
    {
        Time.timeScale = 1f;

        if (name == "Market")
        {
            // Simpan nama scene saat ini sebelum berpindah
            PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        }

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
    }
}
