using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int level = 1;
    public float explodeTime = 3f;

    bool triggered = false;

    public void StartFuse()
    {
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