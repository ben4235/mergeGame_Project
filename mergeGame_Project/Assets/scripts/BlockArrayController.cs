// BlockArrayController.cs
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;   // 新输入系统

public class BlockArrayController : MonoBehaviour
{
    [Header("阵列设置")]
    public int arraySize = 5;          // 顶部数组长度（一般就是 5）
    public Transform[] slots;          // 5 个固定位置，从左到右排好
    public GameObject blockPrefab;     // 顶部显示用的方块 prefab

    [Header("掉落方块")]
    public GameObject fallingBlockPrefab; // 掉落用方块 prefab
    public GridManager gridManager;       // 底部网格管理脚本

    // 内部数据
    private int[] values;                 // 每个格子的数字
    private GameObject[] blockViews;      // 每个格子对应的 GameObject
    private int selectedIndex = 0;        // 当前选中的格子下标

    void Start()
    {
        if (slots == null || slots.Length < arraySize)
        {
            Debug.LogError("slots 数组长度不够，请在 Inspector 里指定 5 个位置的 Transform");
            return;
        }

        values = new int[arraySize];
        blockViews = new GameObject[arraySize];

        // 初始化 5 个格子的数字与显示
        for (int i = 0; i < arraySize; i++)
        {
            values[i] = Random.Range(1, 4);   // 1~3
            CreateBlockView(i);
        }

        UpdateSelectionVisual();
    }

    void Update()
    {
        HandleInput();
    }

    // 处理键盘输入（使用新 Input System）
    private void HandleInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.leftArrowKey.wasPressedThisFrame)
        {
            selectedIndex = (selectedIndex - 1 + arraySize) % arraySize;
            UpdateSelectionVisual();
        }
        else if (keyboard.rightArrowKey.wasPressedThisFrame)
        {
            selectedIndex = (selectedIndex + 1) % arraySize;
            UpdateSelectionVisual();
        }
        else if (keyboard.spaceKey.wasPressedThisFrame)
        {
            OnSelectCurrent();
        }
    }

    // 选中当前格子的逻辑：生成掉落块 + 补位 + merge + 尾部生成新块
    private void OnSelectCurrent()
    {
        if (selectedIndex < 0 || selectedIndex >= arraySize)
            return;

        if (blockViews[selectedIndex] == null)
            return;

        // 1）先取出当前数字，在补位前就拿出来
        int number = values[selectedIndex];
        Vector3 spawnPos = blockViews[selectedIndex].transform.position;

        // 2）原地生成一个向下掉落的方块
        if (fallingBlockPrefab != null)
        {
            var falling = Instantiate(fallingBlockPrefab, spawnPos, Quaternion.identity);
            var fb = falling.GetComponent<FallingBlock>();
            if (fb != null)
            {
                fb.Init(number, gridManager);
            }
        }

        // 3）顶部数组逻辑：删除选中方块，然后右边补位
        Destroy(blockViews[selectedIndex]);
        blockViews[selectedIndex] = null;

        for (int i = selectedIndex; i < arraySize - 1; i++)
        {
            values[i] = values[i + 1];
            blockViews[i] = blockViews[i + 1];

            if (blockViews[i] != null)
            {
                blockViews[i].transform.position = slots[i].position;
            }
        }

        // 现在最右边先清空，等 merge 完后再填新数字
        int lastIndex = arraySize - 1;
        values[lastIndex] = 0;
        blockViews[lastIndex] = null;

        // 4）检测相邻是否 merge（只有 <3 的数字会向上合成一级）
        MergeAdjacentOnce();

        // 5）尾部生成一个新方块（若尾部还没数字）
        if (values[lastIndex] == 0)
        {
            values[lastIndex] = Random.Range(1, 4);
        }
        CreateBlockView(lastIndex);

        // 确保 selectedIndex 合法 & 高亮正确
        selectedIndex = Mathf.Clamp(selectedIndex, 0, arraySize - 1);
        UpdateSelectionVisual();
    }

    /// <summary>
    /// 从左到右扫描一遍：
    /// 如果有相邻相同且数字 < 3，则左边 +1，右边重新随机 1~3
    /// 不做链式多轮合成（你要链式的话可以改成 do-while 循环）
    /// </summary>
    private void MergeAdjacentOnce()
    {
        for (int i = 0; i < arraySize - 1; i++)
        {
            int a = values[i];
            int b = values[i + 1];

            if (a == 0 || b == 0) continue;

            // 相邻相同且 <3 才合成
            if (a == b && a < 3)
            {
                // 左边 +1
                values[i] = a + 1;
                UpdateBlockViewNumber(blockViews[i], values[i]);

                // 右边重新随机一个 1~3
                values[i + 1] = Random.Range(1, 4);

                if (blockViews[i + 1] == null)
                {
                    CreateBlockView(i + 1);
                }
                else
                {
                    UpdateBlockViewNumber(blockViews[i + 1], values[i + 1]);
                }
            }
        }
    }

    // 在 index 位置生成一个顶部方块
    private void CreateBlockView(int index)
    {
        if (blockPrefab == null || slots == null || index < 0 || index >= slots.Length)
            return;

        if (blockViews[index] != null)
        {
            Destroy(blockViews[index]);
            blockViews[index] = null;
        }

        var go = Instantiate(blockPrefab, slots[index].position, Quaternion.identity, transform);
        blockViews[index] = go;

        UpdateBlockViewNumber(go, values[index]);
    }

    // 更新方块显示的数字（兼容 TextMeshPro 和 TextMesh）
    private void UpdateBlockViewNumber(GameObject blockView, int number)
    {
        if (blockView == null) return;

        var tmp = blockView.GetComponentInChildren<TextMeshPro>();
        if (tmp != null)
            tmp.text = number.ToString();

        var textMesh = blockView.GetComponentInChildren<TextMesh>();
        if (textMesh != null)
            textMesh.text = number.ToString();
    }

    // 高亮当前选中的格子
    private void UpdateSelectionVisual()
    {
        for (int i = 0; i < arraySize; i++)
        {
            if (blockViews[i] == null) continue;

            var sr = blockViews[i].GetComponentInChildren<SpriteRenderer>();
            if (sr == null) continue;

            sr.color = (i == selectedIndex) ? Color.yellow : Color.white;
        }
    }
}
