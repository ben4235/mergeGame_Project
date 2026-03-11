using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltPanel : MonoBehaviour
{
    [Header("World Bomb")]
    public GameObject bombPrefab;
    public Transform bombSpawnPoint;

    [Header("Slots (left -> right)")]
    public RectTransform[] slots = new RectTransform[5];

    [Header("Prefabs / Parents")]
    public BeltItemView itemPrefab;
    public RectTransform itemsParent;   // usually BeltPanel
    public RectTransform dragLayer;     // UI layer on top for dragging
    public RectTransform dropZone;      // where items can be dropped

    [Header("Spawning")]
    public int minSpawnLevel = 1;
    public int maxSpawnLevel = 4;

    [Header("Conveyor Movement (optional)")]
    public bool autoAdvance = true;
    public float stepInterval = 1.0f;

    [Header("Animation")]
    public float moveDuration = 0.15f;

    private readonly List<BeltItemView> belt = new List<BeltItemView>(5);
    private bool isAnimating;

    void Start()
    {
        if (itemsParent == null) itemsParent = (RectTransform)transform;

        // Fill belt with 5 items
        for (int i = 0; i < 5; i++)
            belt.Add(SpawnNewItemOffscreenRight());

        SnapAllToSlots();

        if (autoAdvance)
            StartCoroutine(AutoAdvanceLoop());
    }

    IEnumerator AutoAdvanceLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(stepInterval);
            if (isAnimating) continue;
            if (AnyDragging()) continue;

            // Advance one step: leftmost exits, everyone shifts left, new enters right
            RemoveLeftmost();
            yield return ShiftLeftAndRefill();
            yield return ResolveMergesAnimated();
        }
    }

    bool AnyDragging()
    {
        for (int i = 0; i < belt.Count; i++)
        {
            if (belt[i] != null && belt[i].IsDragging)
                return true;
        }
        return false;
    }

    BeltItemView SpawnNewItemOffscreenRight()
    {
        int level = Random.Range(minSpawnLevel, maxSpawnLevel + 1);

        BeltItemView item = Instantiate(itemPrefab, itemsParent);
        item.Init(level, this);

        // Put it just off the right side of the last slot
        item.Rect.position = slots[4].position + new Vector3(200f, 0f, 0f);
        return item;
    }

    void SnapAllToSlots()
    {
        for (int i = 0; i < 5; i++)
        {
            belt[i].Rect.position = slots[i].position;
            belt[i].CurrentSlotIndex = i;
        }
    }

    void RemoveLeftmost()
    {
        if (belt[0] != null)
            Destroy(belt[0].gameObject);

        belt.RemoveAt(0);
    }

    IEnumerator ShiftLeftAndRefill()
    {
        // Shift existing items one to the left in the list
        // Then add a new one to the right to keep size 5
        BeltItemView newItem = SpawnNewItemOffscreenRight();
        belt.Add(newItem);

        yield return AnimateAllToSlots();
    }

    IEnumerator AnimateAllToSlots()
    {
        isAnimating = true;

        Vector3[] start = new Vector3[5];
        Vector3[] target = new Vector3[5];

        for (int i = 0; i < 5; i++)
        {
            start[i] = belt[i].Rect.position;
            target[i] = slots[i].position;
            belt[i].CurrentSlotIndex = i;
        }

        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / moveDuration);
            for (int i = 0; i < 5; i++)
                belt[i].Rect.position = Vector3.Lerp(start[i], target[i], a);

            yield return null;
        }

        for (int i = 0; i < 5; i++)
            belt[i].Rect.position = target[i];

        isAnimating = false;
    }

    // Called by item when it begins drag
    public void MoveToDragLayer(BeltItemView item)
    {
        item.Rect.SetParent(dragLayer, worldPositionStays: true);
    }

    // Called by item when it ends drag
    public IEnumerator TryDropOrReturn(BeltItemView item)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(dropZone, Input.mousePosition))
        {
            int index = item.CurrentSlotIndex;

            // Spawn bomb in world
            SpawnBomb(item.Level);

            Destroy(item.gameObject);
            belt[index] = null;

            yield return CollapseLeftAndRefill(index);
            yield return ResolveMergesAnimated();
        }
    }

    IEnumerator CollapseLeftAndRefill(int removedIndex)
    {
        // Collapse: shift items right of removedIndex left by 1
        for (int i = removedIndex; i < 4; i++)
            belt[i] = belt[i + 1];

        // New item enters at the right
        belt[4] = SpawnNewItemOffscreenRight();

        yield return AnimateAllToSlots();
    }
    void SpawnBomb(int level)
    {
        GameObject bomb = Instantiate(bombPrefab, bombSpawnPoint.position, Quaternion.identity);

        // optional scaling based on level
        bomb.transform.localScale *= 1f + (level * 0.1f);
    }

    IEnumerator ResolveMergesAnimated()
    {
        // Keep merging until stable (chain merges)
        bool merged;
        do
        {
            merged = false;

            for (int i = 0; i < 4; i++)
            {
                var a = belt[i];
                var b = belt[i + 1];
                if (a == null || b == null) continue;

                if (a.Level == b.Level)
                {
                    // Merge into left (a)
                    a.SetLevel(a.Level + 1);
                    Destroy(b.gameObject);

                    // Remove b and collapse
                    for (int j = i + 1; j < 4; j++)
                        belt[j] = belt[j + 1];

                    belt[4] = SpawnNewItemOffscreenRight();

                    merged = true;
                    yield return AnimateAllToSlots();
                    break; // restart scanning from left for clean chain behavior
                }
            }
        }
        while (merged);
    }
}
