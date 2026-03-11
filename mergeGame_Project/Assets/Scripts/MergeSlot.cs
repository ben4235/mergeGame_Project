using UnityEngine;

public class MergeSlot : MonoBehaviour
{
    public int level = 0;
    public MergeBar mergeBar;

    void OnMouseDown()
    {
        if (level == 0) return;

        mergeBar.TakeBomb(this);
    }
}