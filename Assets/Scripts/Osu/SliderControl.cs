using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Plugins.Tools;
using UnityEngine;

namespace Osu
{
    public class SliderControl : MonoBehaviour
    {
        public GameObject great, bad;

        private Vector3 m_Gposition = new Vector3(-8f, -1.5f, 0f),
                        m_Bposition = new Vector3(8f, -1.5f, 0f);

        private bool pointerDown;
        private float pointerDownTimer;

        public List<Transform> waypoints;
        public float durationMove;

        private void Start() => StartCoroutine(Movement());

        public void OnMouseDown()
        {
            pointerDown = true;
            Debug.Log("Holding");
        }

        public void OnMouseUp()
        {
            Reset();
            Debug.Log("Stop Holding");
        }

        private void Update()
        {
            if (pointerDownTimer >= durationMove)
            {
                Instantiate(great, m_Gposition, Quaternion.identity);
                Reset();
            }
            else if (pointerDown) pointerDownTimer += Time.deltaTime;
        }

        private void Reset()
        {
            pointerDown = false;
            pointerDownTimer = 0;
        }

        private IEnumerator Movement()
        {
            yield return new WaitForSeconds(durationMove);
            foreach (Transform t in waypoints)
            {
                transform.DOMove(t.position, durationMove);
                yield return new WaitUntil(() => t.position.Approximates(transform.position, 1.3f));
            }
        }
    }
}
