using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypewriterLines : MonoBehaviour
{
    private bool isTypingIntro = true;
    private bool isTypingNow = false;
    private Coroutine currentTypingCoroutine;
    private Coroutine ghostTypingCoroutine;
    private bool skipRequested = false;

    public TextMeshProUGUI textUI;
    public GameObject parentText;
    public float typingSpeed = 0.05f;
    public float lineDelay = 2f;
    public static bool IsTyping = false; // ✅ Global flag untuk blok input
    public Button skipButton;


    [TextArea(3, 10)]
    public string[] lines = new string[]
    {
        "Defeat enemies to collect coins!",
        "Open chests for extra rewards!",
        "Dodge traps and overcome obstacles in your path!",
        "Upgrade your armor to endure tougher enemies.",
        "Enhance your bullets to strike harder and faster!",
        "The deeper you go, the stronger the nightmares become..."
    };

    [TextArea(3, 10)]
    public string[] ghostLines = new string[]
    {
        "A Ghost appears... its eyes pierce through your soul!",
        "Do not let it touch you. Stay back and strike fast!",
        "The air grows colder — it is not alone..."
    };

    private bool hasPlayedGhostLines = false;

    private void Start()
    {
        StartCoroutine(PlayTypewriter());
    }

    IEnumerator PlayTypewriter()
    {
        isTypingIntro = true;
        isTypingNow = true;
        IsTyping = true;
        Time.timeScale = 0;
        textUI.text = "";
        parentText.SetActive(true);

        foreach (string line in lines)
        {
            currentTypingCoroutine = StartCoroutine(TypeLine(line));
            yield return currentTypingCoroutine;

            if (skipRequested)
            {
                skipRequested = false;
                continue; // lanjut ke baris berikutnya
            }

            yield return new WaitForSecondsRealtime(lineDelay);
        }

        yield return new WaitForSecondsRealtime(3f);
        parentText.SetActive(false);
        Time.timeScale = 1;
        isTypingIntro = false;
        isTypingNow = false;
        IsTyping = false;
    }

    IEnumerator TypeLine(string line)
    {
        textUI.text = "";
        skipRequested = false;

        foreach (char c in line)
        {
            if (skipRequested)
            {
                textUI.text = line;
                yield break;
            }

            textUI.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    public void TriggerGhostLines()
    {
        if (!isTypingIntro && !hasPlayedGhostLines && !isTypingNow)
        {
            hasPlayedGhostLines = true;
            parentText.SetActive(true);
            StartCoroutine(PlayGhostLines());
        }
    }

    IEnumerator PlayGhostLines()
    {
        isTypingNow = true;
        IsTyping = true;
        Time.timeScale = 0;
        textUI.text = "";
        parentText.SetActive(true);
        skipRequested = false;

        foreach (string line in ghostLines)
        {
            ghostTypingCoroutine = StartCoroutine(TypeLine(line));
            yield return ghostTypingCoroutine;

            if (skipRequested)
            {
                skipRequested = false;
                continue;
            }

            yield return new WaitForSecondsRealtime(lineDelay);
        }

        yield return new WaitForSecondsRealtime(3f);
        parentText.SetActive(false);
        Time.timeScale = 1;
        isTypingNow = false;
        IsTyping = false;
    }

    public void OnSkipPressed()
    {
        if (!isTypingNow) return;

        skipRequested = true;
    }

}
