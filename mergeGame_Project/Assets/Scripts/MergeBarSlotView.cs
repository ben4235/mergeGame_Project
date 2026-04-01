using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MergeBarSlotView : MonoBehaviour
{
    public Button button;
    public Image background;
    public TMP_Text levelText;

    private MergeBarManager manager;
    private int slotIndex;

    public void SetSlot(MergeBarManager owningManager, int index, int level)
    {
        manager = owningManager;
        slotIndex = index;

        levelText.text = level.ToString();
        background.color = GetColorForLevel(level);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        manager.TryTakeBombFromSlot(slotIndex);
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