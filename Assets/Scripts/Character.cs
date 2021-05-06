using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/New character")]
public class Character : ScriptableObject
{
    public int id;
    public string name;
    public Skill[] skill;
    public Color[] colorPalette;
    public Sprite[] sprites;
    public Item[] items;
    public float hp;
    public int level;
}
