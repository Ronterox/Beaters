using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum Banner { Runes, Characters, Items }

    public class GachaManager : MonoBehaviour
    {
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

            void UpdateTicketText() => ticketText.text = GetRequiredItemString(ticketName, m_Data.tickets, requiredTickets);
            void UpdateMoneyText() => moneyText.text = GetRequiredItemString(coinName, m_Data.money, requiredCoins);
            
            UpdateTicketText();
            UpdateMoneyText();

            ticketButton.onClick.AddListener(() =>
            {
                //if (m_Data.tickets < requiredTickets) return;
                m_Data.tickets -= requiredTickets;
                UpdateTicketText();
            });

            moneyButton.onClick.AddListener(() =>
            {
                //if (m_Data.money < requiredCoins) return;
                m_Data.money -= requiredCoins;
                UpdateMoneyText();
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
