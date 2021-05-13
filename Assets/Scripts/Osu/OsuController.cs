using UnityEngine;

namespace Osu
{
    public class OsuController : MonoBehaviour
    {
        public GameObject great, bad;

        public float timeLimit;
        private float m_timer;

        public CircleSpawner spawner;

        private Vector3 m_Gposition = new Vector3(-8f, -1.5f, 0f),
                        m_Bposition = new Vector3(8f, -1.5f, 0f);

        private Camera m_MainCamera;

        private void Start() => m_MainCamera = Camera.main;

        private void GenerateAndDestroy()
        {
            spawner.GenerateCircle();
            gameObject.SetActive(false);
        }

        private void Miss()
        {
            Instantiate(bad, m_Bposition, Quaternion.identity);
            GenerateAndDestroy();
        }

        private void Hit()
        {
            Instantiate(great, m_Gposition, Quaternion.identity);
            GenerateAndDestroy();
        }

        private void Update()
        {
            m_timer += Time.deltaTime;

            if (m_timer >= timeLimit)
            {
                Miss();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
                var mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                if (!hit.collider) return;

                float halfTime = timeLimit * 0.5f;

                if (m_timer < halfTime) Miss();
                else if (m_timer >= halfTime) Hit();
            }
        }
    }
}
