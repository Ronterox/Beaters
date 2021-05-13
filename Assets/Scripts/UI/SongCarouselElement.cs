using General;
using Managers;
using Plugins.UI;
using TMPro;
using UnityEngine;
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
                    onClick.AddListener(() =>
                    {
                        GameManager.PutSoundMap(soundMap);
                        LevelLoadManager.LoadArrowGameplayScene();
                    });
                    break;
                case Song song:
                    songImage.sprite = song.songImage;
                    songName.text = song.soundMap.name;

                    //TODO: Show item requirement to unlock song
                    if (parameters[1] is bool isUnlock && isUnlock)
                    {
                        onClick.AddListener(() =>
                        {
                            GameManager.PutSoundMap(song);
                            LevelLoadManager.LoadArrowGameplayScene();
                        });
                    }
                    else
                    {
                        Color defaultColor = songImage.color;
                        songImage.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, .5f);
                    }

                    break;
            }

            return this;
        }
    }
}
