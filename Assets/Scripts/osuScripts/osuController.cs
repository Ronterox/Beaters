using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class osuController : MonoBehaviour
{
    public GameObject great;
    public GameObject bad;
    private float m_timer;
    public float timeLimit;
    public GameObject spawner;
    private Vector3 m_Gposition = new Vector3(-8f,-1.5f,0f);
    private Vector3 m_Bposition = new Vector3(8f,-1.5f,0f);

    void Update()
    {
        m_timer += 0.1f;
        if (m_timer > timeLimit)
        {
            spawner.GetComponent<CircleSpawner>().GenerateCircle();
            Destroy(gameObject);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if(m_timer < timeLimit * 0.5)
                {
                    Instantiate(bad, m_Bposition, Quaternion.identity);
                    spawner.GetComponent<CircleSpawner>().GenerateCircle();
                    Destroy(gameObject);
                }

                if (m_timer >= timeLimit * 0.5)
                {
                    Instantiate(great, m_Gposition, Quaternion.identity);
                    spawner.GetComponent<CircleSpawner>().GenerateCircle();
                    Destroy(gameObject);
                }             
            }
        }
    }
}
