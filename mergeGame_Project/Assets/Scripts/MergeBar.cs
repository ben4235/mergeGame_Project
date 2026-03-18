using UnityEngine;

public class MergeBar : MonoBehaviour
{
    public MergeSlot[] slots;
    public GameObject bombPrefab;
    public PlayerController player;

    void Start()
    {
        FillEmptySlots();
        RefreshVisuals();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);

            Collider2D[] hits = Physics2D.OverlapPointAll(mousePos2D);

            Debug.Log("Hits found: " + hits.Length);

            foreach (Collider2D hit in hits)
            {
                Debug.Log("Clicked on: " + hit.name);

                MergeSlot slot = hit.GetComponent<MergeSlot>();
                if (slot != null)
                {
                    TakeBomb(slot);
                    break;
                }
            }
        }
    }

    public void TakeBomb(MergeSlot slot)
    {
        Debug.Log("TakeBomb called on: " + slot.name);

        if (slot == null) return;
        if (slot.level <= 0) return;
        if (player == null || player.IsHoldingBomb()) return;
        if (bombPrefab == null) return;
        if (player.holdPoint == null) return;

        GameObject bomb = Instantiate(bombPrefab, player.holdPoint.position, Quaternion.identity);
        bomb.transform.SetParent(player.holdPoint);
        bomb.transform.localPosition = Vector3.zero;

        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Bomb bombScript = bomb.GetComponent<Bomb>();
        if (bombScript != null)
        {
            bombScript.level = slot.level;
        }

        player.heldBomb = bomb;

        slot.level = 0;

        FillEmptySlots();
        RefreshVisuals();
    }

    void FillEmptySlots()
    {
        foreach (MergeSlot slot in slots)
        {
            if (slot.level == 0)
                slot.level = 1;
        }
    }

    public void RefreshVisuals()
    {
        foreach (MergeSlot slot in slots)
        {
            SpriteRenderer sr = slot.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            sr.color = slot.level > 0 ? Color.yellow : Color.gray;
        }
    }
}