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
        float move = 0f;

        if (Input.GetKey(KeyCode.A))
            move = -1f;
        else if (Input.GetKey(KeyCode.D))
            move = 1f;

        transform.Translate(Vector2.right * move * speed * Time.deltaTime);
    }

    public bool IsHoldingBomb()
    {
        return heldBomb != null;
    }

    void DropBomb()
    {
        if (heldBomb == null) return;

        heldBomb.transform.SetParent(null);

        Rigidbody2D rb = heldBomb.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Bomb bomb = heldBomb.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.StartFuse();
        }

        heldBomb = null;
    }
}