using Plugins.Tools;
using ScriptableObjects;
using UnityEngine;
using Utilities;

namespace Managers
{
    public enum Difficulty { Normal = 2, Hard = 3, Hardcore = 4 }

    public class GameManager : PersistentSingleton<GameManager>
    {
        private SoundMap m_SoundMap;
        private ScriptableObject m_Prize;
        private ScriptableCharacter m_Character;
        private ScriptableRune m_Rune;
        private object m_value;

        public Song Song { get; private set; }

        public static void PutValue(object value) => Instance.m_value = value;

        public static object GetValue() => Instance.m_value;

        public static void PutSoundMap(SoundMap soundMap) => Instance.m_SoundMap = soundMap;

        public static void PutSoundMap(Song song) => Instance.Song = song;

        public static SoundMap GetSoundMap() => Instance.m_SoundMap ?? Instance.Song.soundMap;

        public static void PutRune(ScriptableRune scriptableRune) => Instance.m_Rune = scriptableRune;
        public static ScriptableRune GetRune() => Instance.m_Rune;

        public static void PutCharacter(ScriptableCharacter scriptableCharacter) => Instance.m_Character = scriptableCharacter;

        public static ScriptableCharacter GetCharacter() => Instance.m_Character;

        public static void PutPrize(ScriptableObject scriptableObject)
        {
            Instance.m_Prize = scriptableObject;
            switch (scriptableObject)
            {
                case ScriptableCharacter character:
                    DataManager.AddCharacter(character);
                    break;
                case ScriptableItem item:
                    DataManager.AddItem(item);
                    break;
                case ScriptableRune rune:
                    DataManager.AddRune(rune);
                    break;
            }
        }

        public static ScriptableObject GetPrize() => Instance.m_Prize;
    }
}
