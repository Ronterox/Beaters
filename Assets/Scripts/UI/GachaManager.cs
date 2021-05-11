using Managers;
using Plugins.Properties;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum Banner { Runes, Characters, Items }

    public class GachaManager : MonoBehaviour
    {
        [Scene]
        public string gachaScene;
        
        [Header("Banner Settings")]
        public Banner banner;
        [Space]
        public Image imageToChange;
        public TMP_Text textToChange;
        [Space]
        public Sprite characterBannerSprite;
        public Sprite runeBannerSprite;

        [Header("Gacha Settings")]
        public TMP_Text ticketText;
        public TMP_Text moneyText;
        [Space]
        public Button ticketButton;
        public Button moneyButton;

        private PlayerData m_Data;

        private void Start()
        {
            m_Data = DataManager.Instance.playerData;

            const int requiredTickets = 1, requiredCoins = 100;
            const string coinName = "Coins", ticketName = "Tickets";

            ticketText.text = GetRequiredItemString(ticketName, m_Data.tickets, requiredTickets);
            moneyText.text = GetRequiredItemString(coinName, m_Data.money, requiredCoins);
            

            ticketButton.onClick.AddListener(() =>
            {
                if (m_Data.tickets < requiredTickets) return;
                m_Data.tickets -= requiredTickets;
                LevelLoadManager.LoadSceneWithTransition(gachaScene, .5f);
            });

            moneyButton.onClick.AddListener(() =>
            {
                if (m_Data.money < requiredCoins) return;
                m_Data.money -= requiredCoins;
                LevelLoadManager.LoadSceneWithTransition(gachaScene, .5f);
            });

            SetBanner();
        }

        private string GetRequiredItemString(string itemName, int quantity, int requiredQuantity) => $"{itemName}\n{quantity}/{requiredQuantity}";

        public void ChangeBannerUI()
        {
            banner = banner == Banner.Characters ? Banner.Runes : Banner.Characters;
            SetBanner();
        }

        public void SetBanner()
        {
            switch (banner)
            {
                case Banner.Runes:
                    textToChange.text = "Rune Banner";
                    imageToChange.sprite = runeBannerSprite;
                    break;
                case Banner.Characters:
                    textToChange.text = "Character Banner";
                    imageToChange.sprite = characterBannerSprite;
                    break;
                case Banner.Items: break;
            }
        }

    }
}
