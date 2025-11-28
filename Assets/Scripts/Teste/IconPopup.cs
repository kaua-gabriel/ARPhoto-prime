using UnityEngine;
using UnityEngine.UI;

public class IconPopup : MonoBehaviour
{
    public GameObject popup;      // Arrasta o popup correspondente aqui
    public Button closeButton;    // Botão X do popup

    private void Awake()
    {
        // Esconde o popup no início
        if (popup != null)
            popup.SetActive(false);

        // Configura o botão X
        if (closeButton != null)
            closeButton.onClick.AddListener(() => popup.SetActive(false));

        // Configura clique no ícone
        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(() => ShowPopup());
    }

    private void ShowPopup()
    {
        if (popup != null)
            popup.SetActive(true);
    }
}
