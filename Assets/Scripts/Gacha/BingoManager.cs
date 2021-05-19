using System.Linq;
using Managers;
using Plugins.Tools;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Gacha
{
    public class BingoManager : MonoBehaviour
    {
        public Image bingoCardboard;
        public ScriptableCharacter[] characters;
        [Space]
        public GameObject panelBingo;
        public GameObject[] bingoBoxes;
        public Button redeemReward;

        private const int OPEN_BINGO_GACHA = 6578;

        private void Start()
        {
            redeemReward.onClick.AddListener(TaskOnClick);

            SetCharacterGUI(GetCharacter());

            redeemReward.interactable = false;

            for (var i = 0; i < DataManager.Instance.playerData.bingoBoxes; i++)
            {
                bingoBoxes[i].SetActive(true);
            }

            CheckButton();

            if (GameManager.GetValue() is int value && value == OPEN_BINGO_GACHA)
            {
                panelBingo.SetActive(true);
                GameManager.PutValue(null);
                if (DataManager.Instance.playerData.bingoBoxes == 9)
                {
                    DataManager.Instance.playerData.bingoBoxes = 8;
                }
                bingoBoxes[DataManager.Instance.playerData.bingoBoxes++].SetActive(true);
                CheckButton();
            }
        }

        private void CheckButton()
        {
            if (DataManager.Instance.playerData.bingoBoxes == 9) redeemReward.interactable = true;
        }

        private ScriptableCharacter GetCharacter()
        {
            ScriptableCharacter character = GameManager.GetCharacter();
            if (character) return character;

            ushort randomCharacter = DataManager.GetCharactersIds().GetRandom();

            character = characters.First(c => c.ID == randomCharacter);
            GameManager.PutCharacter(character);

            return character;
        }

        public void SetCharacterGUI(ScriptableCharacter character)
        {
            Palette palette = character.colorPalette;
            bingoCardboard.color = palette.GetColor(character.bingoColor);
        }

        private void TaskOnClick()
        {
            Debug.Log("Give prize");
            DataManager.Instance.playerData.bingoBoxes = 0;
            foreach (GameObject bingoBox in bingoBoxes) bingoBox.SetActive(false);
        }
    }
}
