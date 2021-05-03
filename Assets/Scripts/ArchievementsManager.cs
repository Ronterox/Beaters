using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plugins.Tools;
using UnityEngine;

[AddComponentMenu("Penguins Mafia/Managers/Achievement Manager")]
public class ArchievementsManager : Singleton<ArchievementsManager>
{
    [System.Serializable]
    public struct Achievement
    {
        public string id;
        public Sprite image;
        public string title, description;
        public AchievementStatus status;
    }

    [System.Serializable]
    public class AchievementStatus
    {
        public string relatedId;
        public int current, goal;
        public bool unlocked;
    }

    public Achievement[] achievements;

    public GameObject achievementObjTemplate;
        
    [Header("Audio")]
    public AudioClip achievementSound;

    protected override void Awake()
    {
        base.Awake();
            
        achievements.ForEach(achievement => achievement.status.relatedId = achievement.id);
    }

    public List<AchievementStatus> Serialize()
    {
        var list = new List<AchievementStatus>();
        achievements.ForEach(achievement => list.Add(achievement.status));
        return list;
    }

    public void Deserialize(List<AchievementStatus> achievementStatuses) =>
        achievements.ForEach(achievement =>
        {
            achievement.status = achievementStatuses.Find(x => x.relatedId == achievement.id);
        });

    public void UpdateAchievement(string id, int value)
    {
        Achievement achievement = achievements.FirstOrDefault(x => x.id == id);

        if (!achievements.Contains(achievement))
        {
            Debug.LogError($"Achievement by id {id} was not found!");
            return;
        }

        AchievementStatus status = achievement.status;
            
        if (status.unlocked) return;

        status.current += value;

        if (status.current < status.goal) return;

        status.current = status.goal;
        status.unlocked = true;

        //SoundManager.Instance.PlayNonDiegeticSound(achievementSound);
            
        StartCoroutine(ShowAchievement(achievement, 3));
    }

    private IEnumerator ShowAchievement(Achievement achievement, float seconds)
    {
        //GameObject temp = GUIManager.Instance.InstantiateUI(achievementObjTemplate);
           
        // var icon = temp.transform.Find("Image").GetComponent<Image>();
        //icon.sprite = achievement.image;

        //var title = temp.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        //title.text = achievement.title;

        // var description = temp.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        //description.text = achievement.description;
        yield return new WaitForSeconds(seconds);
        //GUIManager.Instance.RemoveUI(temp);
    }
}