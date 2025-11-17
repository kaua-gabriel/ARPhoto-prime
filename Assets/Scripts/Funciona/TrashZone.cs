using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class TrashZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Imagens da Lixeira")]
    public Image normalImage;      // ícone padrão
    public Image activeImage;      // ícone quando um item é deletado

    [Header("Configurações Visuais")]
    public float flashDuration = 0.3f; // tempo que a imagem "ativa" fica visível

    private void Start()
    {
        // Garante que só a imagem normal aparece no início
        if (normalImage != null) normalImage.enabled = true;
        if (activeImage != null) activeImage.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Efeito simples de hover (ex.: aumentar escala levemente)
        transform.localScale = Vector3.one * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Volta ao tamanho original
        transform.localScale = Vector3.one;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dropped = eventData.pointerDrag;
        if (dropped != null && dropped.CompareTag("IconClone"))
        {
            Destroy(dropped);
            StartCoroutine(FlashDelete());
        }
    }

    private IEnumerator FlashDelete()
    {
        // Mostra a imagem "ativa" por alguns segundos
        if (normalImage != null) normalImage.enabled = false;
        if (activeImage != null) activeImage.enabled = true;

        yield return new WaitForSeconds(flashDuration);

        if (normalImage != null) normalImage.enabled = true;
        if (activeImage != null) activeImage.enabled = false;
    }
}
