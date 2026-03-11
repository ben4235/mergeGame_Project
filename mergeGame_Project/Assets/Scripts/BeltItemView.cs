using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BeltItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image background;
    public TMP_Text label;

    public RectTransform Rect { get; private set; }
    public int Level { get; private set; }
    public int CurrentSlotIndex { get; set; }
    public bool IsDragging { get; private set; }

    private BeltPanel belt;

    void Awake()
    {
        Rect = GetComponent<RectTransform>();
    }

    public void Init(int level, BeltPanel owner)
    {
        belt = owner;
        SetLevel(level);
    }

    public void SetLevel(int level)
    {
        Level = level;
        if (label) label.text = level.ToString();

        // Optional: quick visual difference by brightness (keep simple)
        if (background)
        {
            float v = Mathf.InverseLerp(1, 10, level);
            background.color = Color.Lerp(new Color(0.2f, 0.2f, 0.2f), Color.white, v);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragging = true;
        belt.MoveToDragLayer(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
        StartCoroutine(belt.TryDropOrReturn(this));
    }
}
