using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasDragger : MonoBehaviour, IDragHandler {
    public RectTransform canvasTransform;
    public float dragSpeed = 1f;

    private Vector2 lastDragPosition;
    public static CanvasDragger instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }
    public void OnDrag(PointerEventData eventData) {
        if (canvasTransform != null) {
            Vector2 dragDelta = eventData.position - lastDragPosition;
            canvasTransform.position += new Vector3(dragDelta.x, dragDelta.y, 0) * dragSpeed;
            lastDragPosition = eventData.position;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        lastDragPosition = eventData.position;
    }
}