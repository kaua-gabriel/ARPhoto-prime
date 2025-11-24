using UnityEngine;
using UnityEngine.EventSystems;

public class TrashZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var icon = eventData.pointerDrag;

        if (icon != null && icon.GetComponent<DraggableIcon>() != null)
        {
            Destroy(icon);
            Debug.Log(" Ícone deletado");
        }
    }
}
