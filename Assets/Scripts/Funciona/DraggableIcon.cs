using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    // ESCALAS MÚLTIPLAS (agora você tem várias opções)
    private float[] scaleLevels;
    private int scaleIndex = 0;

    private float lastClickTime = 0f;
    private float doubleClickTime = 0.25f;

    // controla a suavidade do movimento
    private float dragSmooth = 0.40f; // 0.40 = perfeito no touch

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        float baseSize = rectTransform.sizeDelta.x;

        // tamanhos proporcionais baseados no tamanho original (ex: 5)
        scaleLevels = new float[]
        {
            baseSize * 0.7f,   // menor
            baseSize * 1.0f,   // normal
            baseSize * 1.3f,   // médio
            baseSize * 1.6f,   // grande
            baseSize * 2.0f    // enorme
        };

        rectTransform.sizeDelta = new Vector2(scaleLevels[scaleIndex], scaleLevels[scaleIndex]);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastClickTime < doubleClickTime)
        {
            CycleScale();
        }

        lastClickTime = Time.time;
    }

    private void CycleScale()
    {
        scaleIndex = (scaleIndex + 1) % scaleLevels.Length;
        float size = scaleLevels[scaleIndex];
        rectTransform.sizeDelta = new Vector2(size, size);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.85f;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out pos
        );

        rectTransform.anchoredPosition = pos;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
