using UnityEngine;

public class DetonateButtonUI : MonoBehaviour
{
    public void DetonateAllBombs()
    {
        if (BombManager.Instance != null)
            BombManager.Instance.DetonateAll();
    }
}