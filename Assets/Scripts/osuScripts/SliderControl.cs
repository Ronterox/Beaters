using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Plugins.Tools;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SliderControl : MonoBehaviour
{
    public GameObject great;
    public GameObject bad;
    
    private Vector3 m_Gposition = new Vector3(-8f,-1.5f,0f);
    private Vector3 m_Bposition = new Vector3(8f,-1.5f,0f);

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
                Instantiate(great, m_Gposition, Quaternion.identity);
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
      yield return new WaitForSeconds(durationMove);
      foreach (Transform t in waypoints)
      {
          transform.DOMove(t.position, durationMove);
          yield return new WaitUntil(() => t.position.Approximates(transform.position, 1.3f));
      }
 }
}
