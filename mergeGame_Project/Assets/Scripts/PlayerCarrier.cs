using UnityEngine;
using TMPro;

public class PlayerCarrier : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float leftBound = -7f;
    public float rightBound = 7f;

    [Header("Bomb Drop")]
    public GameObject bombPrefab;
    public Transform holdPoint;
    public Transform dropPoint;

    [Header("Visuals")]
    public SpriteRenderer playerRenderer;
    public Color defaultColor = Color.white;

    [Header("Bomb Limit")]
    public int maxBombs = 10;
    private int bombsUsed = 0;

    [Header("UI")]
    public TMP_Text bombsLeftText;

    private int heldBombLevel = -1;

    private void Start()
    {
        UpdatePlayerColor();
        UpdateBombsUI();
    }

    private void Update()
    {
        HandleMovement();
        HandleDrop();
    }

    private void HandleMovement()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            input = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            input = 1f;

        Vector3 pos = transform.position;
        pos.x += input * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        transform.position = pos;
    }

    private void HandleDrop()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsHoldingBomb())
        {
            DropBomb();
        }
    }

    public bool IsHoldingBomb()
    {
        return heldBombLevel > 0;
    }

    public void GiveBomb(int level)
    {
        heldBombLevel = level;
        UpdatePlayerColor();
    }

    private void DropBomb()
    {
        if (bombsUsed >= maxBombs)
            return;

        GameObject bombObj = Instantiate(bombPrefab, dropPoint.position, Quaternion.identity);
        Bomb bomb = bombObj.GetComponent<Bomb>();

        if (bomb != null)
        {
            bomb.Initialize(heldBombLevel);
        }

        bombsUsed++;

        heldBombLevel = -1;
        UpdatePlayerColor();
        UpdateBombsUI();

        if (GroundProgressManager.Instance != null)
        {
            GroundProgressManager.Instance.ForceRefresh();
        }
    }

    private void UpdateBombsUI()
    {
        if (bombsLeftText != null)
        {
            int bombsRemaining = maxBombs - bombsUsed;
            bombsLeftText.text = $"Bombs Left: {bombsRemaining}";
        }
    }

    public int GetBombsUsed()
    {
        return bombsUsed;
    }

    public int GetMaxBombs()
    {
        return maxBombs;
    }

    public bool HasNoBombsRemaining()
    {
        return bombsUsed >= maxBombs;
    }

    public int GetHeldBombLevel()
    {
        return heldBombLevel;
    }

    private void UpdatePlayerColor()
    {
        if (playerRenderer == null) return;

        if (!IsHoldingBomb())
        {
            playerRenderer.color = defaultColor;
            return;
        }

        playerRenderer.color = GetColorForLevel(heldBombLevel);
    }

    private Color GetColorForLevel(int level)
    {
        switch (level)
        {
            case 1: return Color.white;
            case 2: return Color.green;
            case 3: return Color.yellow;
            case 4: return new Color(1f, 0.5f, 0f);
            default: return Color.red;
        }
    }


}