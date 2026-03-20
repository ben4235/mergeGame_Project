// BlockOnGrid.cs
using UnityEngine;

public class BlockOnGrid : MonoBehaviour
{
    public int gridX;
    public int gridY;
    public GridManager gridManager;

    public void Init(GridManager manager, int x, int y)
    {
        gridManager = manager;
        gridX = x;
        gridY = y;

        if (gridManager != null)
        {
            transform.position = gridManager.GridToWorld(x, y);
        }
    }
}
