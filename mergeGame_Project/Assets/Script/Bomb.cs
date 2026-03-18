using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int level = 0; // 0=White, 1=Blue, 2=Green, 3=Purple
    public bool isTopBarBomb = false;

    private SpriteRenderer sr;
    private bool isMerging = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    public void SetLevel(int newLevel)
    {
        level = newLevel;
        UpdateColor();
    }

    void UpdateColor()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        switch (level)
        {
            case 0:
                sr.color = Color.white;
                break;
            case 1:
                sr.color = Color.blue;
                break;
            case 2:
                sr.color = Color.green;
                break;
            case 3:
                sr.color = new Color(0.5f, 0f, 0.8f); // purple
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isTopBarBomb) return;
        if (isMerging) return;

        Bomb other = collision.gameObject.GetComponent<Bomb>();
        if (other == null) return;

        if (other.isTopBarBomb) return;
        if (other.isMerging) return;
        if (other.level != level) return;
        if (level >= 3) return;

        if (GetInstanceID() < other.GetInstanceID())
        {
            MergeWith(other);
        }
    }

    void MergeWith(Bomb other)
    {
        isMerging = true;
        other.isMerging = true;

        Vector3 mergePosition = (transform.position + other.transform.position) / 2f;

        GameObject newBomb = Instantiate(gameObject, mergePosition, Quaternion.identity);

        DraggableBomb drag = newBomb.GetComponent<DraggableBomb>();
        if (drag != null)
        {
            Destroy(drag);
        }

        Bomb newBombScript = newBomb.GetComponent<Bomb>();
        newBombScript.isMerging = false;
        newBombScript.SetLevel(level + 1);

        Rigidbody2D newRb = newBomb.GetComponent<Rigidbody2D>();
        if (newRb != null)
        {
            newRb.gravityScale = 1f;
        }

        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
