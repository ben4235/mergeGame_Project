using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;

    public Transform holdPoint;
    public GameObject heldBomb;

    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DropBomb();
        }
    }

    void Move()
    {
        float move = 0;

        if (Input.GetKey(KeyCode.A))
            move = -1;

        if (Input.GetKey(KeyCode.D))
            move = 1;

        transform.Translate(Vector2.right * move * speed * Time.deltaTime);
    }

    void DropBomb()
    {
        if (heldBomb == null) return;

        Rigidbody2D rb = heldBomb.GetComponent<Rigidbody2D>();

        heldBomb.transform.parent = null;

        rb.simulated = true;

        heldBomb.GetComponent<Bomb>().StartFuse();

        heldBomb = null;
    }
}