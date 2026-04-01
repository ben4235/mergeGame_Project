using UnityEngine;

public class GroundGenerator : MonoBehaviour
{
    [Header("Chunk Prefab")]
    public GameObject groundChunkPrefab;

    [Header("Grid Size")]
    public int columns = 60;
    public int rows = 4;

    [Header("Chunk Spacing")]
    public float chunkWidth = 0.5f;
    public float chunkHeight = 0.5f;

    [Header("World Offset (move whole ground)")]
    public Vector2 worldOffset = new Vector2(0f, -4f);

    [Header("Parenting")]
    public Transform chunkParent;

    private void Start()
    {
        GenerateGround();
    }

    public void GenerateGround()
    {
        if (groundChunkPrefab == null)
        {
            Debug.LogError("GroundGenerator: No groundChunkPrefab assigned.");
            return;
        }

        if (chunkParent == null)
        {
            chunkParent = transform;
        }

        float totalWidth = columns * chunkWidth;
        float totalHeight = rows * chunkHeight;

        Vector2 centerOffset = new Vector2(totalWidth / 2f, totalHeight / 2f);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 spawnPos = new Vector3(
                    col * chunkWidth - centerOffset.x + worldOffset.x,
                    row * chunkHeight - centerOffset.y + worldOffset.y,
                    0f
                );

                GameObject chunk = Instantiate(groundChunkPrefab, spawnPos, Quaternion.identity, chunkParent);
                chunk.name = $"Ground_{col}_{row}";
            }
        }
    }
}