using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace General
{
    public class SimpleFeedbackObjectPooler : MonoBehaviour
    {
        public Transform parentCanvas;
        [Space]
        public GameObject prefab;
        public int size;

        private Queue<TMP_Text> m_Queue = new Queue<TMP_Text>();

        private void Start()
        {
            for (var i = 0; i < size; i++)
            {
                var text = Instantiate(prefab).GetComponent<TMP_Text>();
                text.gameObject.transform.SetParent(parentCanvas);
                text.gameObject.SetActive(false);
                m_Queue.Enqueue(text);
            }
        }

        public void ShowText(string text, Color color, Vector3 position)
        {
            TMP_Text tmpText= m_Queue.Dequeue();
            
            tmpText.gameObject.transform.position = position;
            tmpText.color = color;

            tmpText.gameObject.SetActive(false);
            tmpText.gameObject.SetActive(true);

            tmpText.text = text;

            m_Queue.Enqueue(tmpText);
        }
    }
}
