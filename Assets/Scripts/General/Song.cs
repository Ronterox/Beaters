using Managers;
using UnityEngine;
using Utilities;

namespace General
{
    public class Song : ScriptableObject
    {
        public SoundMap soundMap;
        public Difficulty[] completedDifficulties;
        public bool isCompleted;
        public int highestCombo;
    }
}
