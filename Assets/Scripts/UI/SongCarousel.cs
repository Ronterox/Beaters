using System.Collections.Generic;
using System.Linq;
using General;
using Managers;
using Plugins.Tools;
using Plugins.UI;
using UnityEngine.EventSystems;
using Utilities;

namespace UI
{
    public class SongCarousel : UICarousel
    {
        public Song[] songs;

        public SongRecordScreen songRecordScreen;
        public LockedSongScreen lockedSongScreen;

        public void SetupElements()
        {
            const string SONG_FOLDER = "Songs";

            CreateElements(songs);

            if (!SaveLoadManager.SaveFolderInGameDirectoryExists(SONG_FOLDER))
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

        public void CreateElements(Song[] parameters)
        {
            List<ushort> songIds = DataManager.GetSongsIds();
            for (var i = 0; i < parameters.Length; i++)
            {
                Song song = parameters[i];
                bool isUnlock = songIds.Contains(song.ID);
                CreateElement(i, i == 0).Setup(song, isUnlock, songRecordScreen, isUnlock ? null : lockedSongScreen);
            }
        }
    }
}
