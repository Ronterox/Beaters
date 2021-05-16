using Managers;
using Plugins.Audio;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LockedSongScreen : MonoBehaviour
    {
        public Image itemSprite1, itemSprite2;
        public TMP_Text itemText1, itemText2;
        [Space]
        public Button unlockButton;
        public Image lockedSongImage;

        private Image m_SongImage;
        private Song m_Song;

        private bool m_CanUnlock;

        private void Start() => unlockButton.onClick.AddListener(UnlockSong);
        
        private void UnlockSong()
        {
            if (DataManager.ContainsSong(m_Song.ID)) return;

            //Check for item requirement and then unlock
            if (m_CanUnlock)
            {
                DataManager.AddSong(m_Song);

                Color color = m_SongImage.color;
                color.a = 1f;

                m_SongImage.color = color;

                gameObject.SetActive(false);
            }
        }

        public void ShowLockedScreen(Song song, Image songImage)
        {
            ScriptableItem required1 = song.requiredItem1, required2 = song.requiredItem2;
            itemSprite1.sprite = required1.itemSprite;
            itemSprite2.sprite = required2.itemSprite;

            //Get item quantity
            int quantity1 = DataManager.GetItemQuantity(required1.ID), quantity2 = DataManager.GetItemQuantity(required2.ID);

            itemText1.text = $"{quantity1}/{song.requiredQuantityItem1}";
            itemText2.text = $"{quantity2}/{song.requiredQuantityItem2}";

            m_CanUnlock = quantity1 >= song.requiredQuantityItem1 && quantity2 >= song.requiredQuantityItem2;

            m_Song = song;
            m_SongImage = songImage;

            lockedSongImage.sprite = songImage.sprite;

            gameObject.SetActive(true);

            SoundManager.Instance.PlayBackgroundMusicNoFade(song.soundMap.audioClip);
        }
    }
}
