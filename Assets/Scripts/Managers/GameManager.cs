using System.Collections.Generic;
using Plugins.Tools;
using UnityEngine;

namespace Managers
{
    [System.Serializable]
    public struct PlayerData
    {
        public int tapsDone;
        public double timePlayed;
        public SerializableItem[] unlockedItems;
        public SerializableSong[] completedSongs, unlockedSongs;
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
    }

    public enum Difficulty
    {
        Normal, Hard, Hardcore
    }

    public class GameManager : Singleton<GameManager>
    {
        public double startPlayingTime;
        private const string PLAYER_FILE = "player.data";
        
        public PlayerData playerData;
        public List<SerializableSong> songsList;
        public List<SerializableItem> itemsList;

        public void OnEnable()
        {
            startPlayingTime = Time.time;
            if (SaveLoadManager.SaveExists(PLAYER_FILE)) playerData = SaveLoadManager.Load<PlayerData>(PLAYER_FILE);
        }

        private void OnDisable() => SavePlayerData();

        private void SavePlayerData()
        {
            playerData.timePlayed += Time.time - startPlayingTime;

            var completedSongs = new List<SerializableSong>();
            songsList.ForEach(song =>
            {
                if(song.isCompleted) completedSongs.Add(song);
            });

            playerData.unlockedItems = itemsList.ToArray();
            playerData.completedSongs = completedSongs.ToArray();
            playerData.unlockedSongs = songsList.ToArray();
            
            SaveLoadManager.Save(playerData, PLAYER_FILE);
        }

        public void MissArrow()
        {
            playerData.tapsDone++;
        }

        public void HitArrow()
        {
            playerData.tapsDone++;
        }
    }
}
