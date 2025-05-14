using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAutoMove : MonoBehaviour
{
    public float standDuration = 2f;      // Durasi player akan berdiri (stand) sebelum mulai bergerak
    public float moveSpeed = 3f;          // Kecepatan pergerakan player
    public float moveDistance = 5f;       // Jarak yang ditempuh untuk bergerak ke kanan atau kiri

    private Animator animator;            // Komponen Animator
    private bool isFacingRight = true;    // Menandakan apakah player sedang menghadap kanan

    private void Start()
    {
        animator = GetComponent<Animator>();  // Ambil komponen Animator
        animator.SetInteger("state", 0);      // Set parameter state ke 0 (stand)
        StartCoroutine(AutoMove());          // Mulai pergerakan otomatis
    }

    private IEnumerator AutoMove()
    {
        while (true) // Loop terus menerus
        {
            // Langkah 1: Player berdiri (stand) untuk beberapa detik
            animator.SetInteger("state", 0); // Set ke state stand
            yield return new WaitForSeconds(standDuration);

            // Langkah 2: Ubah animasi ke idle setelah stand
            animator.SetInteger("state", 1); // Set ke state idle
            
            // Langkah 3: Player bergerak ke kanan atau kiri
            float targetPositionX = isFacingRight ? transform.position.x + moveDistance : transform.position.x - moveDistance;
            
            // Gerakan ke kanan atau kiri
            while ((isFacingRight && transform.position.x < targetPositionX) || (!isFacingRight && transform.position.x > targetPositionX))
            {
                float moveDirection = isFacingRight ? 1 : -1;
                transform.Translate(Vector3.right * moveSpeed * moveDirection * Time.deltaTime);
                yield return null;
            }

            // Langkah 4: Flip arah setelah mencapai batas
            Flip();
        }
    }

    private void Flip()
    {
        // Membalikkan player (flip) dengan mengubah skala X
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1; // Flip skala X
        transform.localScale = theScale;
    }
}
