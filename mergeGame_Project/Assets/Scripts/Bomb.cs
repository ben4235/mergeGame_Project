using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int level = 1;
    public float explodeTime = 3f;

    private bool triggered = false;

    public void StartFuse()
    {
        CancelInvoke();
        Invoke(nameof(Explode), explodeTime);
    }

    void Explode()
    {
        if (triggered) return;
        triggered = true;

        Debug.Log("BOOM level " + level);
        Destroy(gameObject);
    }
}