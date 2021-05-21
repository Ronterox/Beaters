using Managers;
using ScriptableObjects;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Gacha
{
    public class BingoManager : MonoBehaviour
    {
        public GachaManager gachaManager;
        [Space]
        public Image bingoCardboard;
        [Space]
        public GameObject panelBingo;
        public GameObject[] bingoBoxes;
        public Button redeemReward;

        private const int OPEN_BINGO_GACHA = 6578;

        private void Start()
        {
            int userBoxes = DataManager.Instance.playerData.bingoBoxes;

            for (var i = 0; i < userBoxes; i++) bingoBoxes[i].SetActive(true);

            redeemReward.onClick.AddListener(TaskOnClick);
            redeemReward.interactable = userBoxes >= bingoBoxes.Length;

            if (GameManager.GetValue() is int value && value == OPEN_BINGO_GACHA)
            {
                panelBingo.SetActive(true);
                GameManager.PutValue(null);

                if (!redeemReward.interactable)
                {
                    int box = DataManager.Instance.playerData.bingoBoxes++;
                    if (box < bingoBoxes.Length) bingoBoxes[box].SetActive(true);
                    else redeemReward.interactable = true;
                }
            }

            SetCharacterGUI(GameManager.GetCharacter());
        }

        public void SetCharacterGUI(ScriptableCharacter character)
        {
            Palette palette = character.colorPalette;
            bingoCardboard.color = palette.GetColor(character.bingoColor);
        }

        private void TaskOnClick()
        {
            DataManager.Instance.playerData.bingoBoxes = 0;
            foreach (GameObject bingoBox in bingoBoxes) bingoBox.SetActive(false);
            gachaManager.GetPrizeAndSummon();
        }
    }
}
