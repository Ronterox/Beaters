using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string name;
    public Character[] associatedCharacter;
    public int value;
    public float probability;
}
