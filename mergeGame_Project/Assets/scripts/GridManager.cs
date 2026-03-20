using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("网格设置")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public Vector2 origin = Vector2.zero;

    [Header("底部格子 Prefab")]
    public GameObject gridBlockPrefab;

    private BlockOnGrid[,] blocks;

    void Awake()
    {
        blocks = new BlockOnGrid[width, height];
    }

    void Start()
    {
        AutoFillGrid();
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public Vector3 GridToWorld(int x, int y)
    {
        return (Vector3)origin + new Vector3(x * cellSize, y * cellSize, 0f);
    }

    // 由 GridManager 自己生成格子时调用
    public BlockOnGrid SpawnBlock(int x, int y)
    {
        if (!InBounds(x, y) || gridBlockPrefab == null)
            return null;

        if (blocks[x, y] != null)
        {
            Destroy(blocks[x, y].gameObject);
        }

        GameObject go = Instantiate(gridBlockPrefab, GridToWorld(x, y), Quaternion.identity, transform);
        BlockOnGrid b = go.GetComponent<BlockOnGrid>();
        if (b == null)
        {
            Debug.LogError("gridBlockPrefab 上没有 BlockOnGrid 组件！");
        }
        else
        {
            b.Init(this, x, y);
            blocks[x, y] = b;
        }

        return b;
    }

    private void AutoFillGrid()
    {
        if (gridBlockPrefab == null)
        {
            Debug.LogError("GridManager: gridBlockPrefab 没设置，无法生成底部格子");
            return;
        }

        Debug.Log($"[GridManager] AutoFillGrid width={width}, height={height}");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SpawnBlock(x, y);
            }
        }
    }

    // 核心：曼哈顿爆炸
    public void ExplosionAt(BlockOnGrid centerBlock, int number)
    {
        if (centerBlock == null)
        {
            Debug.LogWarning("[GridManager] ExplosionAt: centerBlock 为 null");
            return;
        }

        int cx = centerBlock.gridX;
        int cy = centerBlock.gridY;
        int radius = number;

        Debug.Log($"[GridManager] ExplosionAt center=({cx},{cy}), number={number}, radius={radius}");

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                if (Mathf.Abs(dx) + Mathf.Abs(dy) <= radius)
                {
                    int nx = cx + dx;
                    int ny = cy + dy;

                    if (InBounds(nx, ny) && blocks[nx, ny] != null)
                    {
                        Debug.Log($"[GridManager] Destroy block at ({nx},{ny})");
                        Destroy(blocks[nx, ny].gameObject);
                        blocks[nx, ny] = null;
                    }
                }
            }
        }
    }
}
