using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Plugins.Tools;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SliderControl : MonoBehaviour
{
    private bool pointerDown;
    private float pointerDownTimer;

    public UnityEvent onLongClick;

    public List<Transform> waypoints;
    public float durationMove;

    void Start()
    {        
        StartCoroutine(Movement());
    }

    public void OnMouseDown()
    {
        pointerDown = true;
        Debug.Log("Manteniendo pulsado");
    }

    public void OnMouseUp()
    {
        Reset();
        Debug.Log("Soltado");
    }

    void Update()
    {
        if (pointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            if(pointerDownTimer >= durationMove)
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

     private IEnumerator Movement(){
      foreach (Transform t in waypoints)
      {
          transform.DOMove(t.position, durationMove);
          yield return new WaitUntil(() => t.position.Approximates(transform.position, 1.3f));
      }
 }
}
