using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using TMPro;

namespace General
{
    public class CharacterScriptableSelector : ScriptableSelector<ScriptableCharacter>
    {
        public TMP_Text characterName;

        protected override List<ushort> GetObjectIds() => DataManager.GetCharactersIds();

        protected override void SetObject()
        {
            ScriptableCharacter character = m_PlayerObjects[m_Index];

            objectImage.sprite = character.sprites[0];
            characterName.text = character.characterName;

            GameManager.PutCharacter(character);
        }
    }
}
