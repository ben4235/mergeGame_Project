using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MergeBarSlotView : MonoBehaviour
{
    public Button button;
    public Image background;
    public TMP_Text levelText;

    [Header("Slot Sprites")]
    public Sprite[] levelSprites;

    private MergeBarManager manager;
    private int slotIndex;

    public void SetSlot(MergeBarManager owningManager, int index, int level)
    {
        manager = owningManager;
        slotIndex = index;

        if (levelText != null)
            levelText.text = level.ToString();

        if (background != null)
        {
            int spriteIndex = level - 1;

            if (levelSprites != null &&
                spriteIndex >= 0 &&
                spriteIndex < levelSprites.Length &&
                levelSprites[spriteIndex] != null)
            {
                background.sprite = levelSprites[spriteIndex];
                background.color = Color.white;
                background.type = Image.Type.Simple;
            }
            else
            {
                background.color = GetColorForLevel(level);
            }
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        if (manager != null)
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
            case 5: return Color.red;
            case 6: return new Color(0.6f, 0f, 1f);
            default: return Color.gray;
        }
    }
}