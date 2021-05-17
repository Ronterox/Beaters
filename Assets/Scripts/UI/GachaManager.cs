using Managers;
using Plugins.Properties;
using Plugins.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        public Button ticketButton;
        public Button moneyButton;

        private PlayerData m_Data;
        private float m_Total, m_RandomNumber;
        private int m_BannerIndex;

        private bool m_Payed;

        private void Start()
        {
            if (DataManager.Instance.CharacterCount < 1)
            {
                GetPrizeAndSummon();
                return;
            }
            
            m_Data = DataManager.Instance.playerData;

            const int requiredTickets = 1, requiredCoins = 100;
            const string coinName = "Coins", ticketName = "Tickets";

            ticketText.text = GetRequiredItemString(ticketName, m_Data.tickets, requiredTickets);
            moneyText.text = GetRequiredItemString(coinName, m_Data.money, requiredCoins);

            ticketButton.onClick.AddListener(() =>
            {
                if (m_Payed || m_Data.tickets < requiredTickets)
                {
                    Debug.Log("Not enough tickets!".ToColorString("red"));
                    return;
                }
                m_Payed = true;
                m_Data.tickets -= requiredTickets;
                GetPrizeAndSummon();
            });

            moneyButton.onClick.AddListener(() =>
            {
                if (m_Payed || m_Data.money < requiredCoins)
                {
                    Debug.Log("Not enough money!".ToColorString("red"));
                    return;
                }
                m_Payed = true;
                m_Data.money -= requiredCoins;
                GetPrizeAndSummon();
            });
            
            print("start");

            SetBanner();
        }

        private void GetPrizeAndSummon()
        {
            GameManager.PutPrize(RandomItem());
            LevelLoadManager.LoadSceneWithTransition(gachaScene, .5f);
        }

        public ScriptableObject RandomItem()
        {
            LootOption[] table = banners[m_BannerIndex].lootTable;
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

        private string GetRequiredItemString(string itemName, int quantity, int requiredQuantity) => $"{itemName}\n{quantity}/{requiredQuantity}";

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
