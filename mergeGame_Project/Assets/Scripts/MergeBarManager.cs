using System.Collections.Generic;
using UnityEngine;

public class MergeBarManager : MonoBehaviour
{
    [Header("Bar Settings")]
    public int slotCount = 5;
    public int minLevel = 1;
    public int maxLevel = 4;

    [Header("References")]
    public MergeBarSlotView[] slotViews;
    public PlayerCarrier player;

    private List<int> slots = new List<int>();

    private void Start()
    {
        InitializeBar();
        RefreshUI();
    }

    private void InitializeBar()
    {
        slots.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(Random.Range(minLevel, maxLevel + 1));
        }

        ResolveBar();
    }

    public void TryTakeBombFromSlot(int index)
    {
        if (index < 0 || index >= slots.Count) return;
        if (player == null) return;
        if (player.IsHoldingBomb()) return;

        int level = slots[index];
        if (level <= 0) return;

        player.GiveBomb(level);
        slots[index] = 0;

        ResolveBar();
        RefreshUI();
    }

    private void ResolveBar()
    {
        ShiftLeftToFillEmpties();
        FillRightSide();
        MergeAdjacent();
        ShiftLeftToFillEmpties();
        FillRightSide();
    }

    private void ShiftLeftToFillEmpties()
    {
        List<int> nonZero = new List<int>();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] > 0)
                nonZero.Add(slots[i]);
        }

        slots.Clear();
        slots.AddRange(nonZero);

        while (slots.Count < slotCount)
        {
            slots.Add(0);
        }
    }

    private void FillRightSide()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == 0)
                slots[i] = Random.Range(minLevel, maxLevel + 1);
        }
    }

    private void MergeAdjacent()
    {
        bool mergedSomething;

        do
        {
            mergedSomething = false;

            for (int i = 0; i < slotCount - 1; i++)
            {
                if (slots[i] == slots[i + 1] && slots[i] > 0)
                {
                    slots[i] += 1;      // merged result stays on the LEFT
                    slots[i + 1] = 0;   // right slot becomes empty
                    mergedSomething = true;
                }
            }

            if (mergedSomething)
            {
                ShiftLeftToFillEmpties();
                FillRightSide();
            }

        } while (mergedSomething);
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slotViews.Length; i++)
        {
            slotViews[i].SetSlot(this, i, slots[i]);
        }
    }
}