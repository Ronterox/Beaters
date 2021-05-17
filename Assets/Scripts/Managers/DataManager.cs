using System.Collections.Generic;
using System.Linq;
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
        public List<SerializableItem> currentItems = new List<SerializableItem>();
        public List<SerializableRune> unlockedRunes = new List<SerializableRune>();
        [Space]
        public List<SerializableSong> unlockedSongs = new List<SerializableSong>();
        public List<SerializableCharacter> unlockedCharacters = new List<SerializableCharacter>();
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

        public override string ToString() => $"Id: {itemId}, Quantity: {quantity}";
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

        public bool isCompleted;
        public int highestCombo;

        public Difficulty[] completedDifficulties;
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
        public int CharacterCount => playerData.unlockedCharacters.Count;

        private void Start()
        {
            if (m_Instance != this) return;
            if (SaveLoadManager.SaveExists(PLAYER_FILE)) playerData = SaveLoadManager.Load<PlayerData>(PLAYER_FILE);
        }

        private void OnDestroy()
        {
            if (m_Instance == this) SaveLoadManager.Save(playerData, PLAYER_FILE);
        }

        public static void AddSong(Song song)
        {
            ushort id = song.ID;

            List<SerializableSong> unlockedSongs = m_Instance.playerData.unlockedSongs;

            if (unlockedSongs.Any(sSong => sSong.songId == id)) return;

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

        public static List<ushort> GetCharactersIds() => m_Instance.playerData.unlockedCharacters.Select(character => character.characterId).ToList();

        public static List<ushort> GetRunesIds() => m_Instance.playerData.unlockedRunes.Select(rune => rune.runeId).ToList();

        public static List<ushort> GetSongsIds() => m_Instance.playerData.unlockedSongs.Select(song => song.songId).ToList();

        public static int GetItemQuantity(ushort id) => m_Instance.playerData.currentItems.FirstOrDefault(item => item.itemId == id).quantity;

        public static bool ContainsSong(ushort id) => m_Instance.playerData.unlockedSongs.Any(song => song.songId == id);
    }
}
