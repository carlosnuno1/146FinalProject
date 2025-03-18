using UnityEngine;

public class bulletScript : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public float lifetime = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    public Vector2 screenBounds;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = moveDirection * speed; // Apply movement to bullet
        }
        PlayerBulletManager.Instance.RegisterBullet();

        // set screen bounds
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    public void Update()
    {
        lifetime -= Time.deltaTime;
        if (
            lifetime <= 0
            || transform.position.x > screenBounds.x
            || transform.position.x < -screenBounds.x
            || transform.position.y > screenBounds.y
            || transform.position.y < -screenBounds.y
        )
        {
            PlayerBulletManager.Instance.UnregisterBullet();
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;

        // Adjust rotation: Rotate 90 degrees counterclockwise
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle); // Adjust if needed
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Shield")
        {
            Debug.Log("Bullet hit: " + other.name);
            Destroy(gameObject);
            PlayerBulletManager.Instance.UnregisterBullet();
        }
        else
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<EnemyHealth>()?.TakeDamage(damage);
                Destroy(gameObject);
                PlayerBulletManager.Instance.UnregisterBullet();
            }
        }
    }
}
