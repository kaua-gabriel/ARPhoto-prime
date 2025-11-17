using UnityEngine;
using UnityEngine.UI;

public class PermissionPanelController : MonoBehaviour
{
    [Header("UI")]
    public Toggle toggleAgree;
    public Button buttonConfirm;

    private const string PREF_KEY = "TermsAndPermissionsAccepted";

    private void Start()
    {
        // Se o usuário já aceitou antes → não mostrar mais
        if (PlayerPrefs.GetInt(PREF_KEY, 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        // Começa com o botão desligado
        buttonConfirm.interactable = false;

        // Listener do toggle
        toggleAgree.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        // Só libera o botão quando marcado
        buttonConfirm.interactable = isOn;
    }

    public void OnConfirmPressed()
    {
        // Marca como aceito para nunca mais aparecer
        PlayerPrefs.SetInt(PREF_KEY, 1);
        PlayerPrefs.Save();

        // Esconde o painel
        gameObject.SetActive(false);

        Debug.Log("Permissões aceitas! Painel não aparecerá novamente.");
    }
}
