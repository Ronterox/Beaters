using Core;
using Managers;
using UnityEngine;

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
