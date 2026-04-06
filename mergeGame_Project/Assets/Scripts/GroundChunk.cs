using UnityEngine;

public class GroundChunk : MonoBehaviour
{
    [HideInInspector] public bool countsTowardGoal = false;

    private void Start()
    {
        if (GroundProgressManager.Instance != null)
        {
            GroundProgressManager.Instance.RegisterChunk(this);
        }
    }

    public void DestroyChunk()
    {
        if (GroundProgressManager.Instance != null)
        {
            GroundProgressManager.Instance.NotifyChunkDestroyed(this);
        }

        Destroy(gameObject);
    }
}