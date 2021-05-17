using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class CharacterScriptableSelector : ScriptableSelector<ScriptableCharacter>
    {
        public GUIManager guiManager;
        [Space]
        public TMP_Text characterName;
        public TMP_Text characterDescription;
        [Space]
        public Image activeSkillImage;
        public Image passiveSkillImage;
        
        public TMP_Text activeSkillText, passiveSkillText;

        protected override List<ushort> GetObjectIds() => DataManager.GetCharactersIds();

        protected override void SetObject()
        {
            ScriptableCharacter character = m_PlayerObjects[m_Index];

            objectImage.sprite = character.sprites[0];
            
            characterName.text = character.characterName;
            characterDescription.text = character.description;

            activeSkillText.text = character.activeSkill.effectDescription;
            passiveSkillText.text = character.passiveSkill.effectDescription;

            activeSkillImage.sprite = character.activeSkill.skillImage;
            passiveSkillImage.sprite = character.passiveSkill.skillImage;

            GameManager.PutCharacter(character);
            guiManager.SetCharacterGUI(character);
        }

        protected override void SetStartIndex() => m_Index = m_PlayerObjects.FindIndex(character => character.ID == GameManager.GetCharacter().ID);
    }
}
