using System.Security.AccessControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plugins.Tools;

[System.Serializable]
public struct LootOption
{
    public GameObject obj;
    public float probability;
}

public class RandomLoot : MonoBehaviour
{
    public LootOption[] table;

    public float total;
    public float randomNumber;
    
    private void Start()
    {
        RandomItem();
    }

    public void RandomItem()
    {
        table.Shuffle();

        foreach(var item in table)
        {
            total += item.probability;
        }

        System.Random rng = new System.Random(UnityEngine.Random.Range(0, (int)total));
        randomNumber = rng.Next((int)total);

        for (int i = 0; i < table.Length; i++)
        {
            if (randomNumber <= table[i].probability)
            {
                table[i].obj.SetActive(true);
                return;
            }
            else
            {
                randomNumber -= table[i].probability;
            }
            
        }
    }
}
