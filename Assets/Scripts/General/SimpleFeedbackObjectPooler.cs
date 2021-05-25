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
        [Space]
        public Color defaultColor;

        private readonly Queue<TMP_Text> m_Queue = new Queue<TMP_Text>();

        private void Start()
        {
            for (var i = 0; i < size; i++)
            {
                var text = Instantiate(prefab).GetComponent<TMP_Text>();
                
                text.transform.SetParent(parentCanvas);
                text.gameObject.SetActive(false);
                
                text.color = defaultColor;
                
                m_Queue.Enqueue(text);
            }
        }

        public void ShowText(string text, Color color, Vector3 position)
        {
            TMP_Text tmpText = GetText(text, position);

            tmpText.color = color;

            m_Queue.Enqueue(tmpText);
        }

        public TMP_Text GetText(string text, Vector3 position)
        {
            TMP_Text tmpText = m_Queue.Dequeue();

            tmpText.gameObject.transform.position = position;

            tmpText.gameObject.SetActive(false);
            tmpText.gameObject.SetActive(true);
            
            tmpText.text = text;

            return tmpText;
        }

        public void ShowText(string text, Vector3 position) => m_Queue.Enqueue(GetText(text, position));
    }
}
