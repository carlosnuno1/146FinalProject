using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    float vertical;
    float horizontal;
    
    public float moveSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, vertical * moveSpeed);
    }
}
