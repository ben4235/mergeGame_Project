using UnityEngine;

public class bombBehaviour : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float dropGravity = 3f;

    Rigidbody2D rb;
    bool isHeld = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void Update()
    {
        if (!isHeld) return;

        HandleMovement();
        HandleDrop();
    }

    void HandleMovement()
    {
        float move = 0;

        if (Input.GetKey(KeyCode.A))
            move = -1;

        if (Input.GetKey(KeyCode.D))
            move = 1;

        transform.position += Vector3.right * move * moveSpeed * Time.deltaTime;
    }

    void HandleDrop()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Drop();
        }
    }

    void Drop()
    {
        isHeld = false;
        rb.gravityScale = dropGravity;
    }
}