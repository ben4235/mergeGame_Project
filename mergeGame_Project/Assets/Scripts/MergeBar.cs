using UnityEngine;

public class MergeBar : MonoBehaviour
{
    public MergeSlot[] slots;
    public GameObject bombPrefab;
    public PlayerController player;

    void Start()
    {
        FillBar();
    }

    void FillBar()
    {
        foreach (MergeSlot slot in slots)
        {
            if (slot.level == 0)
            {
                slot.level = 1;
                slot.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }
    }

    public void TakeBomb(MergeSlot slot)
    {
        if (player.heldBomb != null) return;

        GameObject bomb = Instantiate(
            bombPrefab,
            player.holdPoint.position,
            Quaternion.identity
        );

        bomb.transform.parent = player.holdPoint;

        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
        rb.simulated = false;

        bomb.GetComponent<Bomb>().level = slot.level;

        player.heldBomb = bomb;

        slot.level = 0;
        slot.GetComponent<SpriteRenderer>().color = Color.gray;

        FillBar();
    }
}