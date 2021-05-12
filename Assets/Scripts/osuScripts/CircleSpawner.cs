using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    public GameObject prefab;
    void Start()
    {
        GenerateCircle();
    }

    public Vector3 GeneratedPosition()
    {
        int x, y;
        x = Random.Range(-5, 5);
        y = Random.Range(-3, 3);
        return new Vector3(x, y, 0);
    }

    public void GenerateCircle()
    {
        Instantiate(prefab, GeneratedPosition(), Quaternion.identity);
    }
}
