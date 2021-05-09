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

        public void CreateElements(object[] parameters)
        {
            for (var i = 0; i < parameters.Length; i++) CreateElement(i, i == 0).Setup(parameters[i]);
        }
    }
}
