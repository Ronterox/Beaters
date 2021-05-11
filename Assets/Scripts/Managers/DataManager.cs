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
        public SerializableItem[] unlockedItems;
        public SerializableSong[] completedSongs, unlockedSongs;
        public SerializableCharacter[] unlockedCharacters;
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
    }

    [System.Serializable]
    public class SerializableSong
    {
        public ushort songId;

        public Difficulty[] completedDifficulties;
        public bool isCompleted;

        public int highestCombo;
    }

    [System.Serializable]
    public class SerializableCharacter
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

            var completedSongs = new List<SerializableSong>();
            m_Instance.m_SongsList.ForEach(song =>
            {
                if (song.isCompleted) completedSongs.Add(song);
            });

            m_Instance.playerData.unlockedItems = m_Instance.m_ItemsList.ToArray();

            m_Instance.playerData.completedSongs = completedSongs.ToArray();
            m_Instance.playerData.unlockedSongs = m_Instance.m_SongsList.ToArray();

            m_Instance.playerData.unlockedCharacters = m_Instance.m_CharactersList.ToArray();

            SaveLoadManager.Save(m_Instance.playerData, PLAYER_FILE);
        }

        public static void AddSong(Song song)
        {
            var serializableSong = new SerializableSong
            {
                songId = song.name.GetHashCodeUshort()
            };

            if (m_Instance.m_SongsList.Where(sSong => sSong.songId == serializableSong.songId).Select(sSong => serializableSong).Any())
            {
                return;
            }
            m_Instance.m_SongsList.Add(serializableSong);
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
            var serializableCharacter = new SerializableCharacter
            {
                characterId = character.name.GetHashCodeUshort()
            };

            if (m_Instance.m_CharactersList.Where(chara => chara.characterId == serializableCharacter.characterId).Select(sChar => serializableCharacter).Any())
            {
                return;
            }
            m_Instance.m_CharactersList.Add(serializableCharacter);
        }
    }
}
