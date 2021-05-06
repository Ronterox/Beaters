using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Character[] associatedCharacter;
    public int value;
    public float probability;
    public ushort ID { get; private set; }

    private void Awake() => ID = (ushort)itemName.GetHashCode();
}
