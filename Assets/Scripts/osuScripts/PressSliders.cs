using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PressSliders : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pointerDown;
    private float pointerDownTimer;

    public float requiredHoldTime;

    public UnityEvent onLongClick;
    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
        Debug.Log("Manteniendo pulsado");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();
        Debug.Log("Soltado");
    }

    void Update()
    {
        if (pointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            if(pointerDownTimer >= requiredHoldTime)
            {
                if(onLongClick != null)
                {
                    onLongClick.Invoke();
                }
                Reset();
            }
        }
    }

    private void Reset()
    {
        pointerDown = false;
        pointerDownTimer = 0;
    }
}
