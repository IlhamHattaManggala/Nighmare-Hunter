using System.Collections;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    public GameObject notificationPanel;           // Panel yang akan dimunculkan
    public TextMeshProUGUI notificationText;       // Isi teksnya
    public float displayDuration = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: kalau kamu mau notifikasi tetap ada di semua scene
        }
        else
        {
            Destroy(gameObject);
        }

        if (notificationPanel != null)
            notificationPanel.SetActive(false); // Pastikan panel disembunyikan awal
    }

    public void Show(string message)
    {
        if (!gameObject.activeInHierarchy) return;

        // StopAllCoroutines(); // Kalau ada notifikasi sebelumnya belum selesai, hentikan
        StartCoroutine(ShowNotification(message));
    }

    private IEnumerator ShowNotification(string message)
    {
        notificationText.text = message;
        notificationPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(displayDuration);
        notificationPanel.SetActive(false);
    }
}
