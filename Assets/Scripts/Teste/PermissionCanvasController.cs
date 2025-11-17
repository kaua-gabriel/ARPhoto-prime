using UnityEngine;
using UnityEngine.UI;

public class PermissionPanelController : MonoBehaviour
{
    [Header("UI")]
    public Toggle toggleAgree;
    public Button buttonConfirm;

    private const string PREF_KEY = "TermsAndPermissionsAccepted";

    private void Awake()
    {
        // 🔹 Garante que o painel aparece imediatamente quando a cena inicia
        if (PlayerPrefs.GetInt(PREF_KEY, 0) == 0)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (!gameObject.activeSelf)
            return;

        buttonConfirm.interactable = false;

        toggleAgree.onValueChanged.AddListener((isOn) =>
        {
            buttonConfirm.interactable = isOn;
        });

        buttonConfirm.onClick.AddListener(ConfirmPermission);
    }

    private void ConfirmPermission()
    {
        PlayerPrefs.SetInt(PREF_KEY, 1);
        PlayerPrefs.Save();
        gameObject.SetActive(false);
    }

    // 🔹 OPÇÃO EXTRA: usar para testar novamente
    [ContextMenu("Reset Permission (Para Testes)")]
    public void ResetPermission()
    {
        PlayerPrefs.DeleteKey(PREF_KEY);
        Debug.Log("🔄 Permissão resetada!");
    }
}
