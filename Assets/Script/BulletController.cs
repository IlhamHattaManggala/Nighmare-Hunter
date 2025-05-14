using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update
    public int bulletLevel = 1;
    public float lifetime = 3f;
    public int baseDamage = 10;
    public float speed = 10f;
    private Vector2 moveDirection = Vector2.right;
    private int damage;

    private void Start()
    {
        damage = baseDamage * bulletLevel;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ghost"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    private void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
    }

    public void SetLevel(int level)
    {
        bulletLevel = level;
    }
}
