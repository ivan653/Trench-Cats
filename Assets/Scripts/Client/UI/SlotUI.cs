using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private UnityEngine.UI.Image spriteConponent;
    public event Action<int> OnSlotSelected;
    private int slotIdex;

    public void Initialize(int slotIdex)
    {
        this.slotIdex = slotIdex;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteConponent.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotSelected.Invoke(slotIdex);
    }
}

