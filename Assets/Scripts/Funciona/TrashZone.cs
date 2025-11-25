using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TrashArea : MonoBehaviour, IDropHandler
{
    [Header("Imagens da Lixeira")]
    public GameObject idleImage;
    public GameObject activeImage;

    [Header("Configuração")]
    public float feedbackTime = 0.3f;

    private bool isAnimating = false;

    public void OnDrop(PointerEventData eventData)
    {
        var icon = eventData.pointerDrag;
        if (icon != null && icon.GetComponent<DraggableIcon>() != null)
        {
            Destroy(icon.gameObject);
            TriggerFeedback();
            Debug.Log("Ícone deletado");
        }
    }

    public void TriggerFeedback()
    {
        if (!isAnimating)
            StartCoroutine(PlayFeedback());
    }

    private IEnumerator PlayFeedback()
    {
        isAnimating = true;
        idleImage.SetActive(false);
        activeImage.SetActive(true);

        yield return new WaitForSeconds(feedbackTime);

        activeImage.SetActive(false);
        idleImage.SetActive(true);
        isAnimating = false;
    }
}
