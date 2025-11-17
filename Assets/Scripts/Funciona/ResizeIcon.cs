using UnityEngine;

public class IconScaler : MonoBehaviour
{
    private RectTransform rect;
    private float scale = 1f;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Só funciona quando o mouse está por cima do item
        if (!RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
            return;

        // Scroll aumenta/diminui
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            scale += scroll * 0.1f;
            scale = Mathf.Clamp(scale, 0.3f, 3f);
            rect.localScale = Vector3.one * scale;
        }

        // Botão direito reseta tamanho
        if (Input.GetMouseButtonDown(1))
        {
            scale = 1f;
            rect.localScale = Vector3.one;
        }
    }
}
