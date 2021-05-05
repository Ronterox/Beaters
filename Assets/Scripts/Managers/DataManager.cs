using System.Collections.Generic;
using System.Linq;
using General;
using Plugins.Tools;
using UnityEngine;

namespace Managers
{
    [System.Serializable]
    public class PlayerData
    {
        public int tapsDone;
        public double timePlayed;
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
        public static PlayerData playerData;

        private static double startPlayingTime;
        private const string PLAYER_FILE = "player.data";

        private static List<SerializableSong> m_SongsList = new List<SerializableSong>();
        private static List<SerializableItem> m_ItemsList = new List<SerializableItem>();
        private static List<SerializableCharacter> m_CharactersList = new List<SerializableCharacter>();

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
            var serializableSong = new SerializableSong
            {
                songId = song.name.GetHashCodeUshort(),
                isCompleted = song.isCompleted,
                highestCombo = song.highestCombo,
                completedDifficulties = song.completedDifficulties
            };

            if (m_SongsList.Where(sSong => sSong.songId == serializableSong.songId).Select(sSong => serializableSong).Any())
            {
                return;
            }
            m_SongsList.Add(serializableSong);
        }

        public static void AddItem(Item item, int quantity = 1)
        {
            var serializableItem = new SerializableItem
            {
                itemId = item.id,
                quantity = quantity
            };

            foreach (SerializableItem sItem in m_ItemsList.Where(sItem => sItem.itemId == serializableItem.itemId))
            {
                serializableItem.quantity += sItem.quantity;

                if (serializableItem.quantity <= 0)
                {
                    m_ItemsList.Remove(sItem);
                }
                else sItem.SetItem(serializableItem);

                return;
            }
            m_ItemsList.Add(serializableItem);
        }

        public static void AddCharacter(Character character)
        {
            var serializableCharacter = new SerializableCharacter
            {
                characterId = character.name.GetHashCodeUshort(),
                lvl = character.lvl,
                xp = character.xp
            };

            if (m_CharactersList.Where(chara => chara.characterId == serializableCharacter.characterId).Select(sChar => serializableCharacter).Any())
            {
                return;
            }
            m_CharactersList.Add(serializableCharacter);
        }
    }
}
