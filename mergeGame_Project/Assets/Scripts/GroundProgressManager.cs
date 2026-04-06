using TMPro;
using UnityEngine;

public class GroundProgressManager : MonoBehaviour
{
    public static GroundProgressManager Instance { get; private set; }

    [Header("UI")]
    public TMP_Text progressText;

    [Header("Goal")]
    [Range(0f, 100f)]
    public float targetPercent = 75f;

    [Header("References")]
    public PlayerCarrier player;

    private int totalVisibleChunks = 0;
    private int destroyedVisibleChunks = 0;

    private Camera mainCam;

    private void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
    }

    public void RegisterChunk(GroundChunk chunk)
    {
        if (chunk == null) return;

        bool countsTowardGoal = IsChunkVisibleOnScreen(chunk);

        chunk.countsTowardGoal = countsTowardGoal;

        if (countsTowardGoal)
        {
            totalVisibleChunks++;
            UpdateUI();
        }
    }

    public void NotifyChunkDestroyed(GroundChunk chunk)
    {
        if (chunk == null) return;
        if (!chunk.countsTowardGoal) return;

        destroyedVisibleChunks++;
        UpdateUI();
    }

    public float GetDestroyedPercent()
    {
        if (totalVisibleChunks <= 0) return 0f;
        return (destroyedVisibleChunks / (float)totalVisibleChunks) * 100f;
    }

    private bool gameEnded = false;

    private void UpdateUI()
    {
        if (gameEnded) return;

        float percent = GetDestroyedPercent();

        if (progressText != null)
        {
            progressText.text = $"Terrain Destroyed: {percent:0}%";
        }

        if (percent >= targetPercent)
        {
            Debug.Log("WIN");
            gameEnded = true;
            Time.timeScale = 0f;
            return;
        }

        if (
            player != null &&
            player.HasNoBombsRemaining() &&
            !player.IsHoldingBomb() &&
            BombManager.Instance != null &&
            BombManager.Instance.GetActiveBombCount() == 0 &&
            percent < targetPercent
        )
        {
            Debug.Log("LOSE");
            gameEnded = true;
            Time.timeScale = 0f;
        }
    }

    private bool IsChunkVisibleOnScreen(GroundChunk chunk)
    {
        if (mainCam == null)
            mainCam = Camera.main;

        if (mainCam == null) return false;

        SpriteRenderer sr = chunk.GetComponent<SpriteRenderer>();
        if (sr == null) return false;

        Bounds bounds = sr.bounds;

        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        Vector3 camPos = mainCam.transform.position;

        Bounds cameraBounds = new Bounds(
            new Vector3(camPos.x, camPos.y, 0f),
            new Vector3(camWidth, camHeight, 100f)
        );

        return bounds.Intersects(cameraBounds);
    }

    public void ForceRefresh()
    {
        UpdateUI();
    }
}