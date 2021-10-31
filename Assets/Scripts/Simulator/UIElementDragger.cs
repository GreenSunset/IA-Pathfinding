using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIElementDragger : EventTrigger
{

    private bool dragging;
    private Vector2 dragPoint;

    public void Update()
    {
        if (dragging)
        {
            transform.position = new Vector2(Input.mousePosition.x + dragPoint.x, Input.mousePosition.y + dragPoint.y);
            Debug.Log(transform.position.x + ", " + transform.position.y);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        dragPoint = new Vector2(transform.position.x - Input.mousePosition.x, transform.position.y - Input.mousePosition.y);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        //if (transform.position.x > X)
    }
}