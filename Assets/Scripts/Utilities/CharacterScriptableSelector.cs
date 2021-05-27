using Managers;
using Plugins.Tools;
using ScriptableObjects;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class CharacterScriptableSelector : MonoBehaviour
    {
        public Image objectImage;
        public ScriptableCharacter[] objectList;
        [Space]
        public GUIManager guiManager;
        [Space]
        public TMP_Text characterName;
        public TMP_Text characterDescription;
        [Space]
        public Image activeSkillImage;
        public Image passiveSkillImage;
        
        public HoldableButton characterButton;
        public HoldableButton passiveButton;
        public HoldableButton activeButton;
        public PopUp activePopup;
        public PopUp passivePopup;
        public PopUp characterPopup;

        public TMP_Text activeSkillText, passiveSkillText;
        private int m_Index;

        private void Start()
        {
            characterButton.onButtonDown += characterPopup.Show;
            characterButton.onButtonDown += () => print("hallow");
            characterButton.onButtonUp += characterPopup.Hide;
            passiveButton.onButtonDown += passivePopup.Show;
            passiveButton.onButtonUp += passivePopup.Hide;
            activeButton.onButtonDown += activePopup.Show;
            activeButton.onButtonUp += activePopup.Hide;
            CheckAndSetObject();
         }

        private void CheckAndSetObject()
        {
            if(objectList.Length < 1) return;
            SetStartIndex();
            SetObject();
        }
        
        public void TravelObjects(int index)
        {
            m_Index.ChangeValueLoop(index, objectList.Length);
            SetObject();
        }

        private void SetObject()
        {
            ScriptableCharacter character = objectList[m_Index];

            characterName.text = character.characterName;
            characterDescription.text = character.description;

            activeSkillText.text = character.activeSkill.effectDescription;
            passiveSkillText.text = character.passiveSkill.effectDescription;

            activeSkillImage.sprite = character.activeSkill.skillImage;
            passiveSkillImage.sprite = character.passiveSkill.skillImage;

            if (DataManager.ContainsCharacter(character.ID))
            {
                objectImage.sprite = character.sprites[0];
                GameManager.PutCharacter(character);
                guiManager.SetCharacterGUI(character);
            }
            else
                objectImage.sprite = character.gachaButton;
        }

        private void SetStartIndex() => m_Index = objectList.FindIndex(character => character.ID == GameManager.GetCharacter().ID);
    }
}
