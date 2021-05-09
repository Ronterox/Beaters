using General;
using Plugins.UI;
using TMPro;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class SongCarouselElement : UICarouselElement
    {
        public Image songImage;
        public TMP_Text songName;

        public override UICarouselElement Setup(params object[] parameters)
        {
            object value = parameters?[0];
            switch (value)
            {
                case SoundMap soundMap:
                    songName.text = soundMap.name;
                    break;
                case Song song:
                    songImage.sprite = song.songImage;
                    songName.text = song.soundMap.name;
                    break;
            }
            return this;
        }
    }
}
