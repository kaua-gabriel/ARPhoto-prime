using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TrashArea : MonoBehaviour, IDropHandler
{
    [Header("Imagens da Lixeira")]
    public GameObject idleImage;
    public GameObject activeImage;

    [Header("Animação")]
    public float feedbackTime = 0.3f;
    public float scaleUpAmount = 1.2f;      // quanto aumenta
    public float scaleSpeed = 0.15f;        // velocidade da animação

    private bool isAnimating = false;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var icon = eventData.pointerDrag;

        if (icon != null && icon.GetComponent<DraggableIcon>() != null)
        {
            Destroy(icon.gameObject);  // <-- CORRETO
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

        // troca para sprite ativa
        idleImage.SetActive(false);
        activeImage.SetActive(true);

        // anima: aumenta
        yield return StartCoroutine(ScaleTo(scaleUpAmount));

        yield return new WaitForSeconds(feedbackTime);

        // anima: volta ao normal
        yield return StartCoroutine(ScaleTo(1f));

        // volta pro idle
        activeImage.SetActive(false);
        idleImage.SetActive(true);

        isAnimating = false;
    }

    private IEnumerator ScaleTo(float targetMultiplier)
    {
        Vector3 targetScale = originalScale * targetMultiplier;

        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * (1f / scaleSpeed)
            );

            yield return null;
        }

        transform.localScale = targetScale;
    }
}
