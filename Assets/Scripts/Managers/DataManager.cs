using System.Collections.Generic;
using General;
using Plugins.Tools;
using UnityEngine;
using ScriptableObjects;

namespace Managers
{
    [System.Serializable]
    public struct PlayerData
    {
        public int tapsDone;
        public double timePlayed;
        public SerializableItem[] unlockedItems;
        public SerializableSong[] completedSongs, unlockedSongs;
        public SerializableCharacter[] unlockedCharacters;
    }

    [System.Serializable]
    public struct SerializableItem
    {
        public ushort itemId;
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
        public static PlayerData playerData;

        private static double startPlayingTime;
        private const string PLAYER_FILE = "player.data";


        private static List<SerializableSong> m_SongsList = new List<SerializableSong>();
        private static List<SerializableItem> m_ItemsList = new List<SerializableItem>();
        private static List<SerializableCharacter> m_CharactersList = new List<SerializableCharacter>();

        public static int lvl, xp;

        public void OnEnable()
        {
            if (Instance != this) return;

            startPlayingTime = Time.time;
            if (SaveLoadManager.SaveExists(PLAYER_FILE)) playerData = SaveLoadManager.Load<PlayerData>(PLAYER_FILE);
        }

        private void OnDisable()
        {
            if (Instance == this) SavePlayerData();
        }

        private static void SavePlayerData()
        {
            playerData.timePlayed += Time.time - startPlayingTime;

            var completedSongs = new List<SerializableSong>();
            m_SongsList.ForEach(song =>
            {
                if (song.isCompleted) completedSongs.Add(song);
            });

            playerData.unlockedItems = m_ItemsList.ToArray();

            playerData.completedSongs = completedSongs.ToArray();
            playerData.unlockedSongs = m_SongsList.ToArray();

            playerData.unlockedCharacters = m_CharactersList.ToArray();

            SaveLoadManager.Save(playerData, PLAYER_FILE);
        }

        public static void AddSong(Song song)
        {
            var serializableSong = new SerializableSong { songId = (ushort)song.name.GetHashCode(), isCompleted = song.isCompleted, highestCombo = song.highestCombo, completedDifficulties = song.completedDifficulties };

            for (var i = 0; i < m_SongsList.Count; i++)
            {
                if (m_SongsList[i].songId == serializableSong.songId)
                {
                    m_SongsList[i] = serializableSong;
                    return;
                }
            }

            m_SongsList.Add(serializableSong);
        }

        public static void AddItem(ScriptableItem item, int quantity = 1)
        {
            var serializableItem = new SerializableItem { itemId = item.ID, quantity = quantity};
            
            for (var i = 0; i < m_ItemsList.Count; i++)
            {
                SerializableItem sItem = m_ItemsList[i];
                if (sItem.itemId == serializableItem.itemId)
                {
                    serializableItem.quantity += sItem.quantity;
                    
                    if (serializableItem.quantity <= 0) m_ItemsList.Remove(sItem);
                    else m_ItemsList[i] = serializableItem;
                    
                    return;
                }
            }
            m_ItemsList.Add(serializableItem);
        }

        public static void AddCharacter(ScriptableCharacter character)
        {
            var serializableCharacter = new SerializableCharacter { characterId = (ushort)character.name.GetHashCode(), lvl = lvl, xp = xp };

            for (var i = 0; i < m_CharactersList.Count; i++)
            {
                SerializableCharacter sChar = m_CharactersList[i];
                if (sChar.characterId == serializableCharacter.characterId)
                {
                    m_CharactersList[i] = serializableCharacter;
                    return;
                }
            }
            m_CharactersList.Add(serializableCharacter);
        }
    }
}
