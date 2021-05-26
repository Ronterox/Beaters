using System.Collections.Generic;
using System.Linq;
using Managers;
using Plugins.Tools;
using Plugins.UI;
using ScriptableObjects;
using UnityEngine.EventSystems;
using Utilities;

namespace UI
{
    public class SongCarousel : UICarousel
    {
        public Song[] songs, defaultSongs;

        public SongRecordScreen songRecordScreen;
        public LockedSongScreen lockedSongScreen;

        public void SetupElements()
        {
            const string SONG_FOLDER = "Songs";

            CreateElements(defaultSongs, true);
            CreateElements(songs);

            if (SaveLoadManager.SaveFolderInGameDirectoryExists(SONG_FOLDER))
            {
                SoundMap[] savedSoundMaps = SaveLoadManager.LoadMultipleJsonFromFolderInGameDirectory<SoundMap>(SONG_FOLDER).ToArray();
                CreateElements(savedSoundMaps);
            }

            EventSystem.current.SetSelectedGameObject(SelectedElement.gameObject);
        }

        protected override void ScrollToElement(UICarouselElement element) { }

        public void CreateElements(SoundMap[] parameters)
        {
            for (var i = 0; i < parameters.Length; i++) CreateElement(i, i == 0).Setup(parameters[i]);
        }

        public void CreateElements(Song[] parameters, bool unlock = false)
        {
            List<ushort> songIds = DataManager.GetSongsIds();
            for (var i = 0; i < parameters.Length; i++)
            {
                Song song = parameters[i];
                
                bool unlockedSong = songIds.Contains(song.ID);
                bool isUnlock = unlock || unlockedSong;
                
                if(unlock && !unlockedSong) DataManager.AddSong(song);
                
                CreateElement(i, i == 0).Setup(song, isUnlock, songRecordScreen, isUnlock ? null : lockedSongScreen);
            }
        }
    }
}
