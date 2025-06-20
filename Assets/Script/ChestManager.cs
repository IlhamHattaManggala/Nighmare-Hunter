using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isOpened = false;
    private Animator animator;  // Tambahkan variabel untuk Animator
    public GameObject bgSuccessful;

    private void Start()
    {
        animator = GetComponent<Animator>();  // Ambil komponen Animator dari peti
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isOpened && collision.CompareTag("Player"))
        {
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.OpenChest();
            }

            isOpened = true;
            animator.SetBool("isOpened", true);
            StartCoroutine(WaitForAnimationToEnd());

            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.StartAutoMoveToNextLevel();
            }

            // Cek nama scene aktif
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName == "Level-7" && bgSuccessful != null)
            {
                bgSuccessful.SetActive(true);
            }
        }
    }

    private IEnumerator WaitForAnimationToEnd()
    {
        // Tunggu sampai animasi "ChestOpen" selesai
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        // Tunggu selama durasi animasi
        yield return new WaitForSeconds(animationLength);

        // Hapus peti setelah animasi selesai
        Destroy(gameObject);
    }
}
