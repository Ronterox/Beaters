using Managers;
using Plugins.Tools;
using Plugins.UI;
using ScriptableObjects;
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
        public GameObject lockImage;

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
                    
                    void PlayMap()
                    {
                        // Show record screen
                        if (parameters[2] is SongRecordScreen recordScreen) recordScreen.ShowRecordScreen(song);
                    }

                    if (parameters[1] is bool isUnlock && isUnlock)
                    {
                        onClick.AddListener(PlayMap);
                    }
                    else
                    {
                        void ShowUnlockPanel()
                        {
                            if (DataManager.ContainsSong(song.ID))
                            {
                                PlayMap();
                            }
                            else if (parameters[3] is LockedSongScreen lockedScreen)
                            {
                                lockedScreen.ShowLockedScreen(song, songImage);
                            }
                        }

                        onClick.AddListener(ShowUnlockPanel);

                        songImage.SetAlpha(.5f);
                        
                        lockImage.SetActive(true);
                    }

                    break;
            }

            return this;
        }
    }
}
