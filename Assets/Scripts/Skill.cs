using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/New skill")]
public class Skill : ScriptableObject
{
    private int id;
    private string kind;
    private string effect;
    private int? duration;

    public int getId() => id;
    public string getKind() => kind;
    public string getEffect() => effect;
    public int? getDuration() => duration;

    public void setId(int id)
    {
        this.id = id;
    }
    public void setKind(string kind)
    {
        this.kind = kind;
    }
    public void setEffect(string effect)
    {
        this.effect = effect;
    }
    public void getDuration(int? duration)
    {
        this.duration = duration;
    }
}
