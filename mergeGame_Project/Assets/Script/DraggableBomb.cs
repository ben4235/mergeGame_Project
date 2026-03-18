using UnityEngine;

public class DraggableBomb : MonoBehaviour
{
    private Rigidbody2D rb;
    private Camera mainCam;
    private SpriteRenderer sr;

    private bool isDragging = false;
    private bool hasDropped = false;

    [Header("Bar Reference")]
    public SpriteRenderer topBarRenderer;

    [HideInInspector] public TopBarSpawner spawner;
    [HideInInspector] public int slotIndex = -1;
    [HideInInspector] public Transform homeSlot;

    private float minX;
    private float maxX;
    private float barCenterY;
    private float barBottomY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
        sr = GetComponent<SpriteRenderer>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (topBarRenderer != null)
        {
            Bounds barBounds = topBarRenderer.bounds;
            minX = barBounds.min.x;
            maxX = barBounds.max.x;
            barCenterY = barBounds.center.y;
            barBottomY = barBounds.min.y;
        }
        else
        {
            Debug.LogError("Top Bar Renderer is not assigned!");
        }

        SnapBackToSlot();
    }

    void OnMouseDown()
    {
        if (hasDropped || topBarRenderer == null) return;

        isDragging = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    void OnMouseDrag()
    {
        if (!isDragging || hasDropped || topBarRenderer == null) return;

        Vector3 mouseWorld = GetMouseWorldPosition();

        float clampedX = Mathf.Clamp(mouseWorld.x, minX, maxX);
        float dragY = Mathf.Min(mouseWorld.y, barCenterY);

        transform.position = new Vector3(clampedX, dragY, 0f);

        // check top bar merge by overlap, not physics
        if (spawner != null)
        {
            bool merged = spawner.TryMergeDraggedBomb(this);
            if (merged)
            {
                isDragging = false;
                return;
            }
        }
    }

    void OnMouseUp()
    {
        if (!isDragging || hasDropped || topBarRenderer == null) return;

        isDragging = false;

        float bombBottom = sr.bounds.min.y;

        if (bombBottom < barBottomY)
        {
            hasDropped = true;

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1f;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            Bomb bomb = GetComponent<Bomb>();
            if (bomb != null)
            {
                bomb.isTopBarBomb = false;
            }

            if (spawner != null)
            {
                spawner.ClearSlot(slotIndex);
            }
        }
        else
        {
            SnapBackToSlot();
        }
    }

    void SnapBackToSlot()
    {
        if (homeSlot != null)
        {
            transform.position = homeSlot.position;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, barCenterY, 0f);
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(mainCam.transform.position.z);
        Vector3 worldPos = mainCam.ScreenToWorldPoint(mouseScreen);
        worldPos.z = 0f;
        return worldPos;
    }
}
