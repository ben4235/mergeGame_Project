using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FallingBlock : MonoBehaviour
{
    [Header("数字（只读，用来调试）")]
    public int number;

    private GridManager gridManager;

    private TextMeshPro tmp;
    private TextMesh legacyText;

    private BlockOnGrid target;
    void Awake()
    {
        tmp = GetComponentInChildren<TextMeshPro>();
        legacyText = GetComponentInChildren<TextMesh>();
    }

    /// <summary>
    /// 由 BlockArrayController 生成时调用
    /// </summary>
    public void Init(int n, GridManager manager)
    {
        number = n;
        gridManager = manager;

        RefreshText();
        Debug.Log($"[FallingBlock] Init number={number}, gridManager={(gridManager ? gridManager.name : "NULL")}");
    }

    private void RefreshText()
    {
        if (tmp != null) tmp.text = number.ToString();
        if (legacyText != null) legacyText.text = number.ToString();
    }

	private void Update()
	{
        if (Input.GetKeyDown(KeyCode.Return))
        {
			if (target != null && gridManager != null)
			{
				Debug.Log($"[FallingBlock] Hit BlockOnGrid ({target.gridX},{target.gridY}), do explosion, number={number}");
				gridManager.ExplosionAt(target, number);
				Destroy(gameObject);
			}
			else
			{
				if (target == null)
					Debug.Log("[FallingBlock] Collider上没有 BlockOnGrid 组件，检查你的 grid prefab 是否挂了 BlockOnGrid");
				if (gridManager == null)
					Debug.Log("[FallingBlock] gridManager 是 null，检查 BlockArrayController 里有没有把 GridManager 拖进去并且 Init 有传");
			}
		}
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[FallingBlock] OnCollisionEnter2D with {collision.collider.name}");

        target = collision.collider.GetComponent<BlockOnGrid>();
       
    }

    // 如果你把任一 collider 设成了 IsTrigger=true，那就用这个而不是上面的
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[FallingBlock] OnTriggerEnter2D with {other.name}");

        var block = other.GetComponent<BlockOnGrid>();
        if (block != null && gridManager != null)
        {
            Debug.Log($"[FallingBlock] (Trigger) Hit BlockOnGrid ({block.gridX},{block.gridY}), do explosion, number={number}");
            gridManager.ExplosionAt(block, number);
            Destroy(gameObject);
        }
    }
}
