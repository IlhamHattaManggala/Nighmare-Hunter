using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAttack : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Attack Settings")]
    public GameObject fire1;
    public GameObject fire2;
    public Transform fireSpawn;
    public float attackCooldown = 2f;
    public float detectionRange = 5f;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;  // Kecepatan pergerakan Ghost
    public float moveRange = 5f;  // Jarak pergerakan maksimal (gerakan bolak-balik)

    private Transform player;
    private bool canAttack = true;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth enemyHealth;
    private float startX;  // Titik awal posisi X Ghost
    private bool movingRight = true;  // Menyimpan arah gerakan
    private bool isPlayerDetected = false; // Menyimpan status apakah player terdeteksi

    [Header("Sound Settings")]
    public AudioClip fireEffect;
    private AudioSource audioSource;

    private void Start()
    {
        Debug.Log("Detection Range: " + detectionRange);  // Cek nilai saat Start
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();
        audioSource = GetComponent<AudioSource>();

        startX = transform.position.x;  // Menyimpan posisi X awal Ghost
        Debug.Log("Posisi: " + startX);
        // audioSource.PlayOneShot(fireEffect);
    }

    private void Update()
    {
        if (player == null || enemyHealth == null || enemyHealth.isDead) return;

        // Periksa jarak antara Ghost dan Player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Jika Player berada dalam jangkauan deteksi, berhenti bergerak dan tembak
        if (distanceToPlayer <= detectionRange)
        {
            isPlayerDetected = true;
            StopMoving(); // Hentikan pergerakan Ghost
            FacePlayer(); // Arahkan Ghost ke Player

            if (canAttack)
            {
                StartCoroutine(Attack()); // Lakukan serangan
            }
        }
        else
        {
            isPlayerDetected = false;
            MoveGhost(); // Lanjutkan pergerakan bolak-balik jika Player tidak terdeteksi
        }
    }

    private void MoveGhost()
    {
        // Tentukan arah gerakan Ghost jika Player tidak terdeteksi
        if (!isPlayerDetected)
        {
            float targetX = movingRight ? startX + moveRange : startX - moveRange;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetX, transform.position.y), moveSpeed * Time.deltaTime);

            // Balikkan arah jika mencapai batas gerakan
            if (transform.position.x >= startX + moveRange)
            {
                movingRight = false;
                spriteRenderer.flipX = true;  // Mengubah arah sprite saat bergerak ke kiri
            }
            else if (transform.position.x <= startX - moveRange)
            {
                movingRight = true;
                spriteRenderer.flipX = false;  // Mengubah arah sprite saat bergerak ke kanan
            }
        }
    }

    private void StopMoving()
    {
        // Ghost berhenti bergerak
        moveSpeed = 0f;
    }

    private void FacePlayer()
    {
        // Menghadap Player saat deteksi
        if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = false;  // Menghadap kiri jika Player di kiri
        }
        else
        {
            spriteRenderer.flipX = true;   // Menghadap kanan jika Player di kanan
        }
    }

    private IEnumerator Attack()
    {
        canAttack = false;

        GameObject selectedFire = Random.Range(0, 2) == 0 ? fire1 : fire2;

        if (selectedFire != null && fireSpawn != null)
        {
            GameObject fire = Instantiate(selectedFire, fireSpawn.position, Quaternion.identity);
            Debug.Log("Fire instantiated");

            if (fireEffect != null && audioSource != null)
            {
                audioSource.PlayOneShot(fireEffect, 0.7f);
            }

            FireProjectile fireProjectile = fire.GetComponent<FireProjectile>();
            if (fireProjectile != null && player != null)
            {
                Vector2 direction = (player.position - fireSpawn.position).normalized;
                fireProjectile.SetDirection(direction);
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
