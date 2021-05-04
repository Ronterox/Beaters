using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
public class Item : ScriptableObject
{
    public ushort id;
    public int damage, health;

    private void Awake() => id = (ushort)name.GetHashCode();
}
