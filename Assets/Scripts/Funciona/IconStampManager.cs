using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IconStampManager : MonoBehaviour
{
    [Header("Referências")]
    public ScrollRect iconScroll;      // ScrollView_Icons
    public RectTransform dragArea;     // Área onde os ícones podem ser colocados
    public GameObject iconPrefab;      // Prefab do ícone que será clonado
    public Canvas canvas;              // Canvas principal (arraste o Canvas_IconStamp)

    private void Start()
    {
        // Adiciona evento de clique em todos os ícones dentro do Content
        foreach (Transform child in iconScroll.content)
        {
            var iconButton = child.GetComponent<Button>();
            if (iconButton == null)
            {
                iconButton = child.gameObject.AddComponent<Button>();
                iconButton.transition = Selectable.Transition.None;
            }

            // Captura o sprite do ícone e associa ao evento
            Sprite iconSprite = child.GetComponent<Image>().sprite;
            iconButton.onClick.AddListener(() => CreateDraggableIcon(iconSprite));
        }
    }

    private void CreateDraggableIcon(Sprite sprite)
    {
        // Cria uma nova cópia do prefab
        GameObject newIcon = Instantiate(iconPrefab, dragArea);

        Image img = newIcon.GetComponent<Image>();
        img.sprite = sprite;
        img.raycastTarget = true;

        RectTransform rect = newIcon.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero; // Começa no centro da área

        // Garante que o novo ícone pode ser arrastado independentemente
        newIcon.SetActive(true);
    }
}
