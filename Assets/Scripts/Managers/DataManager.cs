using System.Collections.Generic;
using System.Linq;
using Plugins.Properties;
using Plugins.Tools;
using UnityEngine;
using ScriptableObjects;
using UI;
using UnityEditor;
using Settings = UI.Settings;

namespace Managers
{
    [System.Serializable]
    public class PlayerData
    {
        public int tapsDone, money, tickets, bingoBoxes;
        public double timePlayed, timePlayedInGame;
        [Header("Inventory and Progress")]
        public List<SerializableItem> currentItems = new List<SerializableItem>();
        public List<SerializableRune> unlockedRunes = new List<SerializableRune>();
        [Space]
        public List<SerializableSong> unlockedSongs = new List<SerializableSong>();
        public List<SerializableCharacter> unlockedCharacters = new List<SerializableCharacter>();
    }

    [System.Serializable]
    public class SerializableItem
    {
        public ushort itemId;
        public int quantity;

        public void SetItem(SerializableItem item)
        {
            itemId = item.itemId;
            quantity = item.quantity;
        }

        public override string ToString() => $"Id: {itemId}, Quantity: {quantity}";
    }

    [System.Serializable]
    public class SerializableRune
    {
        public ushort runeId;
        public int quantity;
    }

    [System.Serializable]
    public class SerializableSong
    {
        public ushort songId;

        public int highestCombo, highestScore;
        public float accuracy;

        public Difficulty[] completedDifficulties;

        public void UpdateValues(int combo, int score, float precision)
        {
            highestCombo = combo;
            highestScore = score;
            accuracy = precision;
        }

        public void SetId(ushort id) => songId = id;
    }

    [System.Serializable]
    public class SerializableCharacter
    {
        public ushort characterId;
        public int lvl, xp;
    }

    public class DataManager : PersistentSingleton<DataManager>
    {
        public const string PLAYER_FILE = "player.data";

        public PlayerData playerData;
        [ReadOnly]
        public Settings playerSettings = new Settings();

        public int CharacterCount => playerData.unlockedCharacters.Count;
#if UNITY_EDITOR
        public bool resetPlayerPrefs;
#endif

        private void Start()
        {
            if (m_Instance != this) return;

            if (SaveLoadManager.SaveExists(SettingsMenu.SETTINGS_FILE))
            {
                SettingsMenu.SetSettings(playerSettings = SaveLoadManager.Load<Settings>(SettingsMenu.SETTINGS_FILE));
            }

#if UNITY_EDITOR
            if (resetPlayerPrefs) PlayerPrefs.DeleteAll();
            if (!EditorPrefs.HasKey(InformationAttribute.SHOW_INFORMATION_EDITOR_PREF_KEY)) EditorPrefs.SetBool(InformationAttribute.SHOW_INFORMATION_EDITOR_PREF_KEY, true);
#endif
            if (SaveLoadManager.SaveExists(PLAYER_FILE)) playerData = SaveLoadManager.Load<PlayerData>(PLAYER_FILE);
        }

        private void OnDestroy() => SaveData();

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveData();
        }

        public void SaveData()
        {
            if (m_Instance == this) SaveLoadManager.Save(playerData, PLAYER_FILE);
        }

        public static void AddSong(Song song)
        {
            ushort id = song.ID;

            List<SerializableSong> unlockedSongs = m_Instance.playerData.unlockedSongs;

            if (unlockedSongs.Any(srlSong => srlSong.songId == id)) return;

            unlockedSongs.Add(new SerializableSong { songId = id });
        }

        public static void AddItem(ScriptableItem item, int quantity = 1)
        {
            var serializableItem = new SerializableItem
            {
                itemId = item.ID,
                quantity = quantity
            };

            List<SerializableItem> currentItems = m_Instance.playerData.currentItems;

            foreach (SerializableItem sItem in currentItems.Where(sItem => sItem.itemId == serializableItem.itemId))
            {
                serializableItem.quantity += sItem.quantity;

                if (serializableItem.quantity <= 0)
                {
                    currentItems.Remove(sItem);
                }
                else sItem.SetItem(serializableItem);

                return;
            }

            currentItems.Add(serializableItem);
        }

        public static void AddCharacter(ScriptableCharacter character)
        {
            ushort id = character.ID;

            List<SerializableCharacter> characters = m_Instance.playerData.unlockedCharacters;

            if (characters.Any(chara => chara.characterId == id)) return;

            characters.Add(new SerializableCharacter { characterId = character.ID });
        }

        public static void AddRune(ScriptableRune rune)
        {
            ushort id = rune.ID;

            List<SerializableRune> runes = m_Instance.playerData.unlockedRunes;

            if (runes.Any(r => r.runeId == id)) return;

            runes.Add(new SerializableRune { runeId = rune.ID });
        }

        public static SerializableSong GetSong(ushort id) => m_Instance.playerData.unlockedSongs.Find(song => song.songId == id);

        public static List<ushort> GetCharactersIds() => m_Instance.playerData.unlockedCharacters.Select(character => character.characterId).ToList();

        public static List<ushort> GetRunesIds() => m_Instance.playerData.unlockedRunes.Select(rune => rune.runeId).ToList();

        public static List<ushort> GetSongsIds() => m_Instance.playerData.unlockedSongs.Select(song => song.songId).ToList();

        public static int GetItemQuantity(ushort id) => m_Instance.GetItemQuantityWithCheck(id).GetValueOrDefault();

        private int? GetItemQuantityWithCheck(ushort id) => playerData.currentItems.FirstOrDefault(item => item.itemId == id)?.quantity;

        public static bool ContainsCharacter(ushort id) => m_Instance.playerData.unlockedCharacters.Any(character => character.characterId == id);

        public static bool ContainsSong(ushort id) => m_Instance.playerData.unlockedSongs.Any(song => song.songId == id);

        public static bool ContainsRune(ushort id) => m_Instance.playerData.unlockedRunes.Any(rune => rune.runeId == id);

        public static void UpdateSong(SerializableSong serializableSong)
        {
            ushort id = serializableSong.songId;
            List<SerializableSong> unlockedSongs = m_Instance.playerData.unlockedSongs;

            int songIndex = unlockedSongs.FindIndex(song => song.songId == id);

            if (songIndex != -1) unlockedSongs[songIndex] = serializableSong;
            else unlockedSongs.Add(serializableSong);
        }
    }
}
