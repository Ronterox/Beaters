using System.Collections.Generic;
using Managers;
using Plugins.Tools;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace General
{
    public class CharacterSelector : MonoBehaviour
    {
        public Image characterImage;
        public TMP_Text characterName;

        public ScriptableCharacter[] characters;
        private readonly List<ScriptableCharacter> m_PlayerCharacters = new List<ScriptableCharacter>();

        private int m_Index;

        private void Start()
        {
            List<ushort> characterIds = DataManager.GetCharactersIds();

            characters.ForEach(character =>
            {
                if (characterIds.Contains(character.ID)) m_PlayerCharacters.Add(character);
            });

            SetCharacter();
        }

        public void TravelCharacters(int index)
        {
            m_Index.ChangeValueLoop(index, m_PlayerCharacters.Count);
            SetCharacter();
        }

        private void SetCharacter()
        {
            ScriptableCharacter character = m_PlayerCharacters[m_Index];

            characterImage.sprite = character.sprites[0];
            characterName.text = character.characterName;

            GameManager.PutCharacter(character);
        }
    }
}
