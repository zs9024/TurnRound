using UnityEngine;
using UnityEngine.EventSystems;

class UGUIDragHandler: MonoBehaviour, 
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    public delegate void PointerEvetCallBackFunc(GameObject target, PointerEventData eventData);

    public event PointerEvetCallBackFunc onBeginDrag;
    public event PointerEvetCallBackFunc onDrag;
    public event PointerEvetCallBackFunc onEndDrag;
    public event PointerEvetCallBackFunc onDrop;

    public void OnBeginDrag(PointerEventData eventData)     { if (onBeginDrag != null) onBeginDrag(gameObject, eventData); }
    public void OnDrag(PointerEventData eventData)          { if (onDrag != null) onDrag(gameObject, eventData); }
    public void OnEndDrag(PointerEventData eventData)       { if (onEndDrag != null) onEndDrag(gameObject, eventData); }
    public void OnDrop(PointerEventData eventData)          { if (onDrop != null) onDrop(gameObject, eventData); }

    public static UGUIDragHandler Get(GameObject go)
    {
        UGUIDragHandler listener = go.GetComponent<UGUIDragHandler>();
        if (listener == null) 
            listener = go.AddComponent<UGUIDragHandler>();
        return listener;
    }

    public static UGUIDragHandler Get(Transform tran)
    {
        return Get(tran.gameObject);
    }
}
