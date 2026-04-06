using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bomb : MonoBehaviour
{
    [Header("Stats")]
    public int level = 1;
    public float baseExplosionRadius = 0.6f;
    public float explosionGrowthPerLevel = 0.2f;

    [Header("Visuals")]
    public SpriteRenderer sr;
    public TextMeshPro levelLabel;

    [Header("Merge Settings")]
    public float mergeCooldown = 0.1f;

    [Header("Sprites")]
    public Sprite[] levelSprites;

    private Rigidbody2D rb;
    private bool isMerging = false;
    private bool hasMergedRecently = false;

    public void Initialize(int newLevel)
    {
        level = newLevel;
        RefreshVisual();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        RefreshVisual();
        BombManager.Instance?.RegisterBomb(this);
    }

    private void OnDestroy()
    {
        if (BombManager.HasInstance)
            BombManager.Instance.UnregisterBomb(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryMergeWithCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryMergeWithCollision(collision);
    }

    private void TryMergeWithCollision(Collision2D collision)
    {
        if (isMerging || hasMergedRecently)
            return;

        Bomb other = collision.collider.GetComponent<Bomb>();

        if (other == null)
            return;

        if (other == this)
            return;

        if (other.isMerging || other.hasMergedRecently)
            return;

        if (other.level != level)
            return;

        // Prevent both bombs from trying to merge each other at the same time.
        if (GetInstanceID() > other.GetInstanceID())
            return;

        StartCoroutine(MergeIntoThis(other));
    }

    private IEnumerator MergeIntoThis(Bomb other)
    {
        if (other == null)
            yield break;

        isMerging = true;
        other.isMerging = true;

        Vector3 mergePoint = (transform.position + other.transform.position) * 0.5f;

        // Stop motion before merging so the result feels cleaner.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Rigidbody2D otherRb = other.GetComponent<Rigidbody2D>();
        if (otherRb != null)
        {
            otherRb.linearVelocity = Vector2.zero;
            otherRb.angularVelocity = 0f;
        }

        yield return new WaitForSeconds(0.03f);

        transform.position = mergePoint;
        level += 1;
        RefreshVisual();

        hasMergedRecently = true;

        Destroy(other.gameObject);

        isMerging = false;

        yield return new WaitForSeconds(mergeCooldown);
        hasMergedRecently = false;
    }

    public void Explode()
    {
        float radius = baseExplosionRadius + (level - 1) * explosionGrowthPerLevel;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            GroundChunk chunk = hit.GetComponent<GroundChunk>();
            if (chunk != null)
            {
                chunk.DestroyChunk();
            }
        }

        Destroy(gameObject);
    }

    private void RefreshVisual()
    {
        if (levelLabel != null)
            levelLabel.text = level.ToString();

        if (sr != null)
        {
            int spriteIndex = level - 1;

            if (levelSprites != null && spriteIndex >= 0 && spriteIndex < levelSprites.Length && levelSprites[spriteIndex] != null)
            {
                sr.sprite = levelSprites[spriteIndex];
                sr.color = Color.white; // keep sprite colors normal
            }
            else
            {
                sr.color = GetColorForLevel(level); // fallback if sprite missing
            }
        }

        transform.localScale = Vector3.one * (1f + (level - 1) * 0.15f);
    }

    private Color GetColorForLevel(int lvl)
    {
        switch (lvl)
        {
            case 1: return Color.white;
            case 2: return Color.green;
            case 3: return Color.yellow;
            case 4: return new Color(1f, 0.5f, 0f);
            default: return Color.red;
        }
    }

    private void OnDrawGizmosSelected()
    {
        float radius = baseExplosionRadius + (level - 1) * explosionGrowthPerLevel;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}