using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/New character")]
public class Character : ScriptableObject
{
    public string characterName;
    public Skill[] skill;
    public Color[] colorPalette;
    public Sprite[] sprites;
    public Item[] items;
    public float hp;
    public int level;
    public ushort ID { get; private set; }

    private void Awake() => ID = (ushort)characterName.GetHashCode();
}
