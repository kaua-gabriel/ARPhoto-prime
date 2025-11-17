using UnityEngine;
using UnityEngine.EventSystems;

public class StampDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform rt;
    Canvas canvas;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        // move direto para a posição do toque (simples e responsivo)
        rt.anchoredPosition += eventData.delta / (canvas ? canvas.scaleFactor : 1f);
    }

    public void OnEndDrag(PointerEventData eventData) { }
}
