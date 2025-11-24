using UnityEngine;
using UnityEngine.UI;

public class PermissionPanelController : MonoBehaviour
{
    [Header("UI")]
    public Toggle toggleAgree;
    public Button buttonConfirm;

    private void Awake()
    {
        // Sempre mostra quando o app é aberto
        gameObject.SetActive(true);
    }

    private void Start()
    {
        // Se o painel estiver ativo, bloqueia o botão
        buttonConfirm.interactable = false;

        toggleAgree.onValueChanged.AddListener((isOn) =>
        {
            buttonConfirm.interactable = isOn;
        });

        buttonConfirm.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
