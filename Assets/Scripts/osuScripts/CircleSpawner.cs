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
        x = Random.Range(-6, 6);
        y = Random.Range(-4, 4);
        return new Vector3(x, y, 0);
    }

    public void GenerateCircle()
    {
        Instantiate(prefab, GeneratedPosition(), Quaternion.identity);
    }
}
