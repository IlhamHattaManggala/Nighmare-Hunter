using UnityEngine;

public class SpiderMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveDistance = 3f;

    private Vector3 startPosition;
    private bool movingRight = true;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        startPosition = transform.position;
        animator = GetComponent<Animator>();
        SetState(0); // Awal: stand
    }

    void Update()
    {
        if (isDead) return;

        MoveSpider();
    }

    void MoveSpider()
    {
        if (movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);

            if (transform.position.x >= startPosition.x + moveDistance)
            {
                movingRight = false;
                Flip();
                SetState(1); // idle pas balik
            }
            else
            {
                SetState(0); // jalan terus = stand
            }
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

            if (transform.position.x <= startPosition.x - moveDistance)
            {
                movingRight = true;
                Flip();
                SetState(1); // idle pas balik
            }
            else
            {
                SetState(0); // jalan terus = stand
            }
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        Destroy(gameObject, 1f);
    }

    void SetState(int state)
    {
        if (animator != null && !isDead)
        {
            animator.SetInteger("State", state);
        }
    }
}
