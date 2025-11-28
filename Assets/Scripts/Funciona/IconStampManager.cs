using UnityEngine;
using UnityEngine.UI;

public class IconStampManager : MonoBehaviour
{
    [Header("Referências")]
    public Transform iconContainer;     // onde estão os ícones originais
    public RectTransform dragArea;
    public GameObject iconPrefab;
    public Canvas canvas;

    [Header("Popup")]
    public GameObject popupPrefab;      // prefab do popup
    public RectTransform popupParent;

    [Header("Clones")]
    public Vector2 cloneSize = new Vector2(40, 40);

    private void Start()
    {
        // Adiciona listener para cada ícone
        for (int i = 0; i < iconContainer.childCount; i++)
        {
            Transform child = iconContainer.GetChild(i);
            var iconButton = child.GetComponent<Button>();
            if (iconButton == null)
            {
                iconButton = child.gameObject.AddComponent<Button>();
                iconButton.transition = Selectable.Transition.None;
            }

            // Captura o sprite e Animator do ícone
            Image img = child.GetComponent<Image>();
            Sprite iconSprite = img != null ? img.sprite : null;
            Animator iconAnimator = child.GetComponent<Animator>();

            // Captura o texto do popup, se tiver (pode criar um componente Text ou TMP no próprio ícone)
            string popupText = child.name; // ou outro campo de texto que você queira usar

            iconButton.onClick.AddListener(() =>
            {
                CreateDraggableIcon(iconSprite, iconAnimator);
                ShowPopup(popupText, child.position);
            });
        }
    }

    private void CreateDraggableIcon(Sprite sprite, Animator originalAnimator)
    {
        if (sprite == null) return;

        GameObject newIcon = Instantiate(iconPrefab, dragArea);

        // Tamanho fixo
        RectTransform rect = newIcon.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = cloneSize;
        rect.localScale = Vector3.one;

        // Sprite
        Image img = newIcon.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = sprite;
            img.raycastTarget = true;
        }

        // Animator
        Animator newAnimator = newIcon.GetComponent<Animator>();
        if (newAnimator != null && originalAnimator != null)
        {
            newAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;
        }

        newIcon.SetActive(true);
    }

    private void ShowPopup(string text, Vector3 worldPosition)
    {
        GameObject popup = Instantiate(popupPrefab, popupParent);
        popup.transform.position = worldPosition;

        Text popupText = popup.GetComponentInChildren<Text>();
        if (popupText != null)
            popupText.text = text;

        popup.SetActive(true);
    }
}
