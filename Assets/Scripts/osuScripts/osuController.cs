using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class osuController : MonoBehaviour
{
    private float m_timer;
    public float timeLimit;
    void Update()
    {
        m_timer += 0.1f;
        if (m_timer > timeLimit)
        {
            Destroy(gameObject);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if(m_timer < timeLimit / 2)
                {
                    Debug.Log("Mal");
                }
                if (m_timer >= timeLimit / 2)
                {
                    Debug.Log("Bien");
                }             
            }
        }
    }
}
