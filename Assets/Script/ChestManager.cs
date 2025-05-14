using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isOpened = false;
    private Animator animator;  // Tambahkan variabel untuk Animator

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

            // Trigger animasi untuk membuka peti
            animator.SetBool("isOpened", true);

            // Mulai coroutine untuk menunggu animasi selesai sebelum menghapus peti
            StartCoroutine(WaitForAnimationToEnd());

            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.StartAutoMoveToNextLevel();
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
