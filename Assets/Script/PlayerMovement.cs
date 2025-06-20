using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 15f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerController playerController;

    private Vector2 moveInputArrow = Vector2.zero;
    private Vector2 moveInputASWD = Vector2.zero;
    private Vector2 moveInput = Vector2.zero;

    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isShooting = false;
    private bool isPaused = false;
    private bool isStandingInput = false;
    private bool wasRunningBeforeJump = false;

    private enum MovementState { stand, idle, run, jump, fall, hurt, crouch, shoot, crouchShoot }

    [Header("Jump Settings")]
    [SerializeField] private LayerMask jumpableGround;
    private BoxCollider2D coll;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public Transform bulletSpawnShoot;
    public float bulletSpeed = 10f;
    private float shootCooldown = 0.4f;
    private float shootTimer = 0f;
    private int currentBulletLevel = 1;

    private bool isAutoMoving = false;
    private float autoMoveDistance = 2f;
    private Vector3 autoMoveStartPos;
    private float autoMoveSpeed = 2f;
    // Untuk input dari button UI
    private float mobileInputX = 0f;

    [Header("Sound Settings")]
    public AudioClip shootClip;
    private AudioSource audioSource;

    public GameObject backgroundMenu;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

        playerController = new PlayerController();
    }

    private void OnEnable()
    {
        playerController.Enable();

        playerController.Movement.MoveArrow.performed += ctx => moveInputArrow = ctx.ReadValue<Vector2>();
        playerController.Movement.MoveASWD.performed += ctx => moveInputASWD = ctx.ReadValue<Vector2>();

        playerController.Movement.Jump.performed += ctx => Jump();

        playerController.Movement.Crouch.performed += ctx => isCrouching = true;
        playerController.Movement.Crouch.canceled += ctx => isCrouching = false;

        playerController.Movement.Stand.performed += ctx => isStandingInput = true;
        playerController.Movement.Stand.canceled += ctx => isStandingInput = false;

        playerController.Movement.Shoot.performed += ctx => isShooting = true;
        playerController.Movement.Shoot.canceled += ctx => isShooting = false;

        playerController.Movement.CrouchShoot.performed += ctx => { isCrouching = true; isShooting = true; };
        playerController.Movement.CrouchShoot.canceled += ctx => { isCrouching = false; isShooting = false; };

        playerController.Movement.Pause.performed += ctx => TogglePause();
    }

    private void OnDisable()
    {
        playerController.Disable();
    }

    private void FixedUpdate()
    {
        if (isPaused) return;

        if (isAutoMoving)
        {
            rb.velocity = new Vector2(autoMoveSpeed, rb.velocity.y);
            if (Vector3.Distance(transform.position, autoMoveStartPos) >= autoMoveDistance)
            {
                rb.velocity = Vector2.zero;
                isAutoMoving = false;
                GoToNextScene();
            }
            return;
        }
        moveInput = moveInputArrow + moveInputASWD;
        Vector2 targetVelocity = new Vector2((moveInput.x + mobileInputX) * moveSpeed, rb.velocity.y);
        rb.velocity = targetVelocity;

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        MovementState state;

        float horizontal = moveInput.x != 0 ? moveInput.x : mobileInputX;

        if (isPaused)
        {
            anim.speed = 0;
            return;
        }
        else
        {
            anim.speed = 1;
        }

        // Paling penting: periksa status udara dulu
        if (!isGrounded())
        {
            if (rb.velocity.y > 0.1f)
            {
                state = MovementState.jump;
            }
            else if (rb.velocity.y < -0.1f)
            {
                state = MovementState.fall;
            }
            else
            {
                state = MovementState.idle; // fallback kalau tidak bergerak vertikal
            }
        }
        // Cek kombinasi crouch dan shoot
        else if (isCrouching && isShooting)
        {
            state = MovementState.crouchShoot;
        }
        else if (isCrouching)
        {
            state = MovementState.crouch;
        }
        else if (isShooting)
        {
            state = MovementState.shoot;
        }
        else if (Mathf.Abs(horizontal) > 0.1f)
        {
            state = MovementState.run;
            sprite.flipX = horizontal < 0;
        }
        else if (isStandingInput)
        {
            state = MovementState.stand;
        }
        else
        {
            state = MovementState.idle;
        }

        switch (state)
        {
            case MovementState.stand:
                anim.SetInteger("state", 0);
                break;
            case MovementState.idle:
                anim.SetInteger("state", 1);
                break;
            case MovementState.run:
                anim.SetInteger("state", 2);
                break;
            case MovementState.jump:
                anim.SetInteger("state", 3);
                break;
            case MovementState.fall:
                anim.SetInteger("state", 4);
                break;
            case MovementState.hurt:
                anim.SetInteger("state", 5);
                break;
            case MovementState.crouch:
                anim.SetInteger("state", 6);
                break;
            case MovementState.shoot:
                anim.SetInteger("state", 7);
                break;
            case MovementState.crouchShoot:
                anim.SetInteger("state", 8);
                break;
        }
    }


    private void Jump()
    {
        if (isGrounded() && !isCrouching)
        {
            wasRunningBeforeJump = moveInput.x != 0;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        if (backgroundMenu != null)
        {
            backgroundMenu.SetActive(isPaused);
        }
    }



    public void ShootBullet()
    {
        if (bulletPrefab == null) return;

        isShooting = true; // tambahkan ini agar animasi trigger

        // Pilih spawn point berdasarkan apakah player sedang crouch
        Transform spawnPoint = isCrouching ? bulletSpawn : bulletSpawnShoot;
        if (spawnPoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
        float direction = sprite.flipX ? -1f : 1f;

        BulletController bulletCtrl = bullet.GetComponent<BulletController>();
        if (bulletCtrl != null)
        {
            bulletCtrl.SetLevel(currentBulletLevel);
            bulletCtrl.SetDirection(new Vector2(direction, 0f));
            bulletCtrl.speed = bulletSpeed;
        }

        if (shootClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootClip);
        }

        // Reset isShooting dalam waktu singkat supaya animasi bisa kembali ke idle
        StartCoroutine(ResetShootingFlag());
    }

    private IEnumerator ResetShootingFlag()
    {
        yield return new WaitForSeconds(0.1f); // waktu cukup singkat
        isShooting = false;
    }

    public void UpgradeBullet()
    {
        currentBulletLevel++;
    }

    private void Update()
    {
        if (isPaused) return;

        shootTimer -= Time.deltaTime;
        // Jika menggunakan mobile input, pakai itu
        if (Application.isMobilePlatform)
        {
            moveInput = new Vector2(mobileInputX, 0f);
        }
        else
        {
            // Kalau bukan mobile, pakai Input System
            moveInput = playerController.Movement.MoveArrow.ReadValue<Vector2>();
        }

        // Cek apakah sedang menembak (termasuk crouch shoot)
        if ((isShooting || (isCrouching && isShooting)) && shootTimer <= 0f)
        {
            ShootBullet();
            shootTimer = shootCooldown;
        }
    }

    public void StartAutoMoveToNextLevel()
    {
        isAutoMoving = true;
        autoMoveStartPos = transform.position;
    }

    private void GoToNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }
    // Dipanggil ketika tombol crouch + shoot diklik (once)
    public void CrouchShoot()
    {
        if (isGrounded())
        {
            isCrouching = true;
            isShooting = true;

            if (shootTimer <= 0f)
            {
                ShootBullet();
                shootTimer = shootCooldown;
            }

            StartCoroutine(ResetCrouchShoot());
        }
    }

    private IEnumerator ResetCrouchShoot()
    {
        yield return new WaitForSeconds(0.1f);
        isCrouching = false;
        isShooting = false;
    }


    // Fungsi ini dipanggil saat tombol kanan ditekan
    public void MoveRight(bool isPressed)
    {
        if (isPressed)
            mobileInputX = 1f;
        else if (mobileInputX == 1f)
            mobileInputX = 0f;
    }

    public void MoveLeft(bool isPressed)
    {
        if (isPressed)
            mobileInputX = -1f;
        else if (mobileInputX == -1f)
            mobileInputX = 0f;
    }

    // Fungsi ini dipanggil saat tombol lompat ditekan
    public void MobileJump()
    {
        if (isGrounded())
        {
            Jump();
        }
    }
    // Dipanggil saat tombol crouch+shoot ditekan (onPointerDown)
    public void SetCrouchShoot(bool isPressed)
    {
        isCrouching = isPressed;
        isShooting = isPressed;
    }

}
