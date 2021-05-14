using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plugins.Audio;
using Plugins.Tools;
using UnityEngine;

namespace Managers
{
    [AddComponentMenu("Penguins Mafia/Managers/Achievement Manager")]
    public class ArchievementsManager : PersistentSingleton<ArchievementsManager>
    {
        [System.Serializable]
        public class Achievement
        {
            public Sprite image;
            public string title, description;
            public int goal;
            public AchievementStatus status;
            public string ID => status.relatedId;
        }

        [System.Serializable]
        public struct AchievementStatus
        {
            public string relatedId;
            public int current;
            public bool unlocked;

            public void Increment(int value) => current += value;

            public void Unlock() => unlocked = true;
        }

        public Achievement[] achievements;

        public GameObject achievementObjTemplate;

        [Header("Audio")]
        public AudioClip achievementSound;

        public static List<AchievementStatus> Serialize()
        {
            var list = new List<AchievementStatus>();
            m_Instance.achievements.ForEach(achievement => list.Add(achievement.status));
            return list;
        }

        public static void Deserialize(List<AchievementStatus> achievementStatuses) =>
            m_Instance.achievements.ForEach(achievement =>
            {
                achievement.status = achievementStatuses.Find(x => x.relatedId == achievement.ID);
            });

        /// <summary>
        /// Updates the achievement by the passed id, and increments it, if it reaches the goal. Completes the achievement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public void UpdateAchievement(string id, int value)
        {
            Achievement achievement = achievements.First(x => x.ID == id);

            if (!achievements.Contains(achievement))
            {
                Debug.LogError($"Achievement by id {id} was not found!");
                return;
            }

            AchievementStatus status = achievement.status;

            if (status.unlocked) return;

            status.Increment(value);

            if (status.current < achievement.goal) return;

            status.Unlock();

            if (achievementSound) SoundManager.Instance.PlayNonDiegeticSound(achievementSound);

            StartCoroutine(ShowAchievement(achievement, 3));
        }

        /// <summary>
        /// Coroutine for showing the achievement on screen
        /// </summary>
        /// <param name="achievement"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
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
}
