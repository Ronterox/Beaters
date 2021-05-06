using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;

public class Character : MonoBehaviour
{
    public ScriptableCharacter character;

    public int level;
    public int exp;

    public int calculateHP(int lvl){
        return Mathf.RoundToInt(character.hp + lvl * character.multiplier);
    }
    
}
