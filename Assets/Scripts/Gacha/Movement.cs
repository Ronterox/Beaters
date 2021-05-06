using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    public float speed2;
    public Transform target;


    void Update()
    {
        if(transform.position != target.transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
        else if(transform.localScale != new Vector3(20f, 20f, 20f))
        {
            transform.localScale += new Vector3(1, 1, 1) * speed2;
        }
        
    }


}
