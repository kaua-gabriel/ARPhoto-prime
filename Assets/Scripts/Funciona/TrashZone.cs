using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class TrashZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Imagens da Lixeira")]
    public Image normalImage;      // ícone normal
    public Image activeImage;      // ícone quando algo é deletado

    [Header("Configurações")]
    public float flashDuration = 0.25f; // tempo do "piscar" ao deletar

    private void Start()
    {
        if (normalImage != null) normalImage.enabled = true;
        if (activeImage != null) activeImage.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // efeito leve
        transform.localScale = Vector3.one * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        if (dropped == null) return;

        // Só deleta ícones arrastáveis
        if (dropped.CompareTag("IconClone"))
        {
            Destroy(dropped);
            StartCoroutine(FlashDelete());
        }
    }

    private IEnumerator FlashDelete()
    {
        // Pisca na cor "ativa"
        if (normalImage != null) normalImage.enabled = false;
        if (activeImage != null) activeImage.enabled = true;

        yield return new WaitForSeconds(flashDuration);

        if (normalImage != null) normalImage.enabled = true;
        if (activeImage != null) activeImage.enabled = false;
    }
}
