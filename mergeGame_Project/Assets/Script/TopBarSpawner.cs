using UnityEngine;

public class TopBarSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject bombPrefab;
    public SpriteRenderer topBarRenderer;
    public Transform[] slots;

    [Header("Random Level Range")]
    public int minLevel = 0;
    public int maxLevel = 1;

    [Header("Respawn Settings")]
    public float respawnDelay = 0.2f;

    private GameObject[] currentBombs;
    private float[] respawnTimers;

    void Start()
    {
        currentBombs = new GameObject[slots.Length];
        respawnTimers = new float[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            SpawnBombAtSlot(i);
        }
    }

    void Update()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (currentBombs[i] == null)
            {
                if (respawnTimers[i] <= 0f)
                {
                    respawnTimers[i] = respawnDelay;
                }
                else
                {
                    respawnTimers[i] -= Time.deltaTime;

                    if (respawnTimers[i] <= 0f)
                    {
                        SpawnBombAtSlot(i);
                    }
                }
            }
            else
            {
                respawnTimers[i] = 0f;
            }
        }
    }

    void SpawnBombAtSlot(int slotIndex)
    {
        if (slots[slotIndex] == null || bombPrefab == null) return;
        if (currentBombs[slotIndex] != null) return;

        GameObject newBomb = Instantiate(bombPrefab, slots[slotIndex].position, Quaternion.identity);
        currentBombs[slotIndex] = newBomb;

        Bomb bombScript = newBomb.GetComponent<Bomb>();
        if (bombScript != null)
        {
            int randomLevel = Random.Range(minLevel, maxLevel + 1);
            bombScript.SetLevel(randomLevel);
            bombScript.isTopBarBomb = true;
        }

        DraggableBomb dragScript = newBomb.GetComponent<DraggableBomb>();
        if (dragScript != null)
        {
            dragScript.topBarRenderer = topBarRenderer;
            dragScript.spawner = this;
            dragScript.slotIndex = slotIndex;
            dragScript.homeSlot = slots[slotIndex];
        }
    }

    public void ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= currentBombs.Length) return;
        currentBombs[slotIndex] = null;
    }

    public bool TryMergeDraggedBomb(DraggableBomb draggedBomb)
    {
        if (draggedBomb == null) return false;
        if (draggedBomb.slotIndex < 0 || draggedBomb.slotIndex >= currentBombs.Length) return false;

        GameObject draggedObj = draggedBomb.gameObject;
        Bomb draggedBombData = draggedObj.GetComponent<Bomb>();
        Collider2D draggedCol = draggedObj.GetComponent<Collider2D>();

        if (draggedBombData == null || draggedCol == null) return false;

        for (int i = 0; i < currentBombs.Length; i++)
        {
            GameObject otherObj = currentBombs[i];

            if (otherObj == null) continue;
            if (otherObj == draggedObj) continue;

            Bomb otherBombData = otherObj.GetComponent<Bomb>();
            Collider2D otherCol = otherObj.GetComponent<Collider2D>();

            if (otherBombData == null || otherCol == null) continue;

            // only merge same level
            if (draggedBombData.level != otherBombData.level) continue;

            // max level cannot merge further
            if (draggedBombData.level >= 3) continue;

            // check overlap without physics collision
            if (!draggedCol.bounds.Intersects(otherCol.bounds)) continue;

            // choose which slot keeps the merged bomb
            int keepSlot = Mathf.Min(draggedBomb.slotIndex, i);
            int clearSlot = Mathf.Max(draggedBomb.slotIndex, i);

            GameObject keepObj = currentBombs[keepSlot];
            GameObject removeObj = currentBombs[clearSlot];

            if (keepObj == null || removeObj == null) return false;

            Bomb keepBomb = keepObj.GetComponent<Bomb>();
            if (keepBomb == null) return false;

            keepBomb.SetLevel(keepBomb.level + 1);

            // snap merged bomb back to its slot
            keepObj.transform.position = slots[keepSlot].position;

            Destroy(removeObj);
            currentBombs[clearSlot] = null;

            return true;
        }

        return false;
    }
}
