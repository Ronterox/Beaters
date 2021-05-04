using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/New character")]
public class Character : ScriptableObject
{
    public int id;
    public string name;
    public Skill[] skill;
    public string[] colorPalette;
    public Sprite[] sprites;

    public int getId() => id;
    public string getName() => name;
    public Skill[] getSkill() => skill;
    public string[] getColorPalette() => colorPalette;
    public Sprite[] getSprites() => sprites;

    public void setId(int id)
    {
        this.id = id;
    }

    public void setName(string name)
    {
        this.name = name;
    }

    public void setSkill(Skill[] skills)
    {
        this.skill = skills;
    }

    public void setColorPalette(string[] colorPalette)
    {
        this.colorPalette = colorPalette;
    }

    public void setSprites(Sprite[] sprites)
    {
        this.sprites = sprites;
    }
}
