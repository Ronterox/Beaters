using DG.Tweening;
using Managers;
using Plugins.Properties;
using Plugins.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public enum BannerType { Runes, Characters, Items }

    [System.Serializable]
    public struct Banner
    {
        public BannerType type;
        public LootOption[] lootTable;
    }

    [System.Serializable]
    public struct LootOption
    {
        public ScriptableObject loot;
        public float probability;
    }

    public class GachaManager : MonoBehaviour
    {
        [Scene]
        public string gachaScene;

        [Header("Banner Settings")]
        public Banner[] banners;
        [Space]
        public Image imageToChange;
        public TMP_Text textToChange;
        [Space]
        public Sprite characterBannerSprite;
        public Sprite runeBannerSprite, itemBannerSprite;

        [Header("Gacha Settings")]
        public TMP_Text ticketText;
        public TMP_Text moneyText;
        [Space]
        public Button bannerButton;
        public HoldableButton ticketButton, moneyButton;
        [Header("Feedback")]
        public PopUp moneyPopUp;
        public PopUp ticketPopUp;

        private PlayerData m_Data;
        private float m_Total, m_RandomNumber;
        private int m_BannerIndex, m_ThrowMultiplier = 1;

        private bool m_AnimatingCoins, m_AnimatingTickets;

        private void Start()
        {
            m_Data = DataManager.Instance.playerData;

            const int requiredTickets = 1, requiredCoins = 100;
            const string coinName = "Coins", ticketName = "Tickets";

            void UpdateTexts()
            {
                ticketText.text = GetRequiredItemString(ticketName, m_Data.tickets, requiredTickets);
                moneyText.text = GetRequiredItemString(coinName, m_Data.money, requiredCoins);
            }
            
            UpdateTexts();

            void IncrementThrowQuantity()
            {
                m_ThrowMultiplier.ChangeValueLimited(1, 11);
                m_ThrowMultiplier = Mathf.Max(1, m_ThrowMultiplier);
                UpdateTexts();
            }
            
            ticketButton.onButtonDown += ticketPopUp.Show;
            ticketButton.onButtonUp += ticketPopUp.Hide;
            ticketButton.onClick.AddListener(IncrementThrowQuantity);
            
            moneyButton.onButtonDown += moneyPopUp.Show;
            moneyButton.onButtonUp += moneyPopUp.Hide;
            moneyButton.onClick.AddListener(IncrementThrowQuantity);

            bannerButton.onClick.AddListener(() =>
            {
                const float animationDuration = .8f, animationStrength = .5f;
                const int animationVibrato = 7;

                int requiredMoneyQty = requiredCoins * m_ThrowMultiplier,
                    requiredTicketQty = requiredTickets * m_ThrowMultiplier;
                
                if (m_Data.money < requiredMoneyQty)
                {
                    if (!m_AnimatingCoins)
                    {
                        m_AnimatingCoins = true;
                        ticketButton.transform.DOShakeScale(animationDuration, animationStrength, animationVibrato).OnComplete(() => m_AnimatingCoins = false);
                    }
                }
                else
                {
                    m_Data.money -= requiredMoneyQty;
                    GetPrizeAndSummon();
                    return;
                }

                if (m_Data.tickets < requiredTicketQty)
                {
                    if (m_AnimatingTickets) return;
                    
                    m_AnimatingTickets = true;
                    moneyButton.transform.DOShakeScale(animationDuration, animationStrength, animationVibrato).OnComplete(() => m_AnimatingTickets = false);
                }
                else
                {
                    m_Data.tickets -= requiredTicketQty;
                    GetPrizeAndSummon();
                }
            });

            SetBanner();
        }

        public void GetPrizeAndSummon()
        {
            bannerButton.interactable = false;

            var prizes = new ScriptableObject[m_ThrowMultiplier];

            for (var i = 0; i < prizes.Length; i++) prizes[i] = RandomItem();

            GameManager.PutPrizes(prizes);
            LevelLoadManager.LoadSceneWithTransition(gachaScene, .5f);
        }

        public ScriptableObject RandomItem()
        {
            LootOption[] table = banners[m_BannerIndex].lootTable;
            
            m_Total = 0;
            table.Shuffle();

            foreach (LootOption item in table) m_Total += item.probability;

            var totalProbabilityWeight = (int)m_Total;

            var rng = new System.Random(Random.Range(0, totalProbabilityWeight));
            m_RandomNumber = rng.Next(totalProbabilityWeight);

            for (var i = 0; i < table.Length; i++)
            {
                float probability = table[i].probability;

                if (m_RandomNumber <= probability) return table[i].loot;

                m_RandomNumber -= probability;
            }

            return null;
        }

        private string GetRequiredItemString(string itemName, int quantity, int requiredQuantity) => $"{itemName}\n{quantity}/{requiredQuantity * m_ThrowMultiplier}(x{m_ThrowMultiplier})";

        public void TravelBanners(int index)
        {
            m_BannerIndex.ChangeValueLoop(index, banners.Length);
            SetBanner();
        }

        public void SetBanner()
        {
            switch (banners[m_BannerIndex].type)
            {
                case BannerType.Runes:
                    textToChange.text = "Runes Banner";
                    imageToChange.sprite = runeBannerSprite;
                    break;
                case BannerType.Characters:
                    textToChange.text = "Characters Banner";
                    imageToChange.sprite = characterBannerSprite;
                    break;
                case BannerType.Items:
                    textToChange.text = "Items Banner";
                    imageToChange.sprite = itemBannerSprite;
                    break;
            }
        }

    }
}
