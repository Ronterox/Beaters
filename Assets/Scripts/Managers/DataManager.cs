using System.Collections.Generic;
using System.Linq;
using General;
using Plugins.Tools;
using UnityEngine;
using ScriptableObjects;

namespace Managers
{
    [System.Serializable]
    public class PlayerData
    {
        public int tapsDone, money, tickets;
        public double timePlayed, timePlayedInGame;
        [Header("Inventory and Progress")]
        public SerializableItem[] currentItems;
        public SerializableRune[] unlockedRunes;
        [Space]
        public SerializableSong[] unlockedSongs;
        public SerializableCharacter[] unlockedCharacters;
    }

    [System.Serializable]
    public struct SerializableItem
    {
        public ushort itemId;
        public int quantity;

        public void SetItem(SerializableItem item)
        {
            itemId = item.itemId;
            quantity = item.quantity;
        }
    }

    [System.Serializable]
    public struct SerializableRune
    {
        public ushort runeId;
        public int quantity;
    }

    [System.Serializable]
    public struct SerializableSong
    {
        public ushort songId;

        public Difficulty[] completedDifficulties;
        public bool isCompleted;

        public int highestCombo;
    }

    [System.Serializable]
    public struct SerializableCharacter
    {
        public ushort characterId;
        public int lvl, xp;
    }

    public class DataManager : PersistentSingleton<DataManager>
    {
        private const string PLAYER_FILE = "player.data";

        public PlayerData playerData;
        [Space]
        private readonly List<SerializableSong> m_SongsList = new List<SerializableSong>();
        private readonly List<SerializableItem> m_ItemsList = new List<SerializableItem>();
        private readonly List<SerializableCharacter> m_CharactersList = new List<SerializableCharacter>();
        private readonly List<SerializableRune> m_RunesList = new List<SerializableRune>();

        private void Start()
        {
            if (m_Instance != this) return;
            if (SaveLoadManager.SaveExists(PLAYER_FILE)) playerData = SaveLoadManager.Load<PlayerData>(PLAYER_FILE);
        }

        private void OnDestroy()
        {
            if (m_Instance == this) SavePlayerData();
        }

        private static void SavePlayerData()
        {
            m_Instance.playerData.timePlayed += Time.realtimeSinceStartupAsDouble;

            m_Instance.playerData.currentItems = m_Instance.m_ItemsList.ToArray();

            m_Instance.playerData.unlockedSongs = m_Instance.m_SongsList.ToArray();

            m_Instance.playerData.unlockedCharacters = m_Instance.m_CharactersList.ToArray();

            m_Instance.playerData.unlockedRunes = m_Instance.m_RunesList.ToArray();

            SaveLoadManager.Save(m_Instance.playerData, PLAYER_FILE);
        }

        public static void AddSong(Song song)
        {
            ushort id = song.ID;

            if (m_Instance.m_SongsList.Any(sSong => sSong.songId == id)) return;

            m_Instance.m_SongsList.Add(new SerializableSong { songId = id });
        }

        public static void AddItem(ScriptableItem item, int quantity = 1)
        {
            var serializableItem = new SerializableItem
            {
                itemId = item.ID,
                quantity = quantity
            };

            foreach (SerializableItem sItem in m_Instance.m_ItemsList.Where(sItem => sItem.itemId == serializableItem.itemId))
            {
                serializableItem.quantity += sItem.quantity;

                if (serializableItem.quantity <= 0)
                {
                    m_Instance.m_ItemsList.Remove(sItem);
                }
                else sItem.SetItem(serializableItem);

                return;
            }
            m_Instance.m_ItemsList.Add(serializableItem);
        }

        public static void AddCharacter(ScriptableCharacter character)
        {
            ushort id = character.ID;

            if (m_Instance.m_CharactersList.Any(chara => chara.characterId == id)) return;

            m_Instance.m_CharactersList.Add(new SerializableCharacter { characterId = character.ID });
        }

        public static void AddRune(ScriptableRune rune)
        {
            ushort id = rune.ID;

            if (m_Instance.m_RunesList.Any(r => r.runeId == id)) return;

            m_Instance.m_RunesList.Add(new SerializableRune { runeId = rune.ID });
        }

        public static List<ushort> GetCharactersIds()
        {
            List<SerializableCharacter> serializableCharacters = m_Instance.m_CharactersList;

            var characters = new List<ushort>(serializableCharacters.Count);

            for (var i = 0; i < characters.Count; i++) characters[i] = serializableCharacters[i].characterId;

            return characters;
        }
        
        public static List<ushort> GetRunesIds()
        {
            List<SerializableRune> serializableRunes = m_Instance.m_RunesList;

            var runes = new List<ushort>(serializableRunes.Count);

            for (var i = 0; i < runes.Count; i++) runes[i] = serializableRunes[i].runeId;

            return runes;
        }
        
        public static List<ushort> GetSongsIds()
        {
            List<SerializableSong> serializableSongs = m_Instance.m_SongsList;

            var songs = new List<ushort>(serializableSongs.Count);

            for (var i = 0; i < songs.Count; i++) songs[i] = serializableSongs[i].songId;

            return songs;
        }
    }
}
