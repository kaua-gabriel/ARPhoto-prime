using UnityEngine;
using UnityEngine.UI;

public class IconPopup : MonoBehaviour
{
    [Header("Referências")]
    public GameObject popup;      // O popup correspondente a este ícone
    public Button closeButton;    // O botão X que fecha o popup

    private void Awake()
    {
        // Garante que o popup começa desativado
        if (popup != null)
            popup.SetActive(false);

        // Configura o botão de fechar
        if (closeButton != null)
            closeButton.onClick.AddListener(HidePopup);

        // Configura o clique no ícone (este objeto)
        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(ShowPopup);
    }

    private void ShowPopup()
    {
        if (popup != null)
            popup.SetActive(true);

        if (closeButton != null)
            closeButton.gameObject.SetActive(true); // ativa o X
    }

    private void HidePopup()
    {
        if (popup != null)
            popup.SetActive(false);

        if (closeButton != null)
            closeButton.gameObject.SetActive(false); // desativa o X
    }
}
