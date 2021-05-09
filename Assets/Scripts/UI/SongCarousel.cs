using General;
using Plugins.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public class SongCarousel : UICarousel
    {
        public Song[] songs;

        protected override void Start() => SetupElements();

        public void SetupElements()
        {
            for (var i = 0; i < songs.Length; i++) CreateElement(i, i == 0).Setup(songs[i]);
            
            EventSystem eventSystem = EventSystem.current;
            if(eventSystem && SelectedElement) EventSystem.current.SetSelectedGameObject(SelectedElement.gameObject);
        }
    }
}
