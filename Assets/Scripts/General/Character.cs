using Managers;
using Plugins.DOTween.Modules;
using Plugins.Properties;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace General
{
    public class Character : MonoBehaviour
    {
        [ReadOnly]
        public ScriptableCharacter character;
        [Space]
        public Image characterImage;
        [Space]
        public int maxHp;
        public int currentHp;
        [Space]
        public int level;
        public int exp;

        [Header("Feedback")]
        public TMP_Text hpText;
        public Transform feedbackTextPositionTransform;
        [Space]
        public SimpleFeedbackObjectPooler feedbackPooler;

        private bool m_IsDead;

        public delegate void CharacterEvent();

        public event CharacterEvent onDie;

        public bool CanTakeDamage { get; set; } = true;
        private bool m_IsLowHp;

        public void SetCharacter(ScriptableCharacter scriptableCharacter, GameplayManager manager)
        {
            //Set serialized exp and lvl
            character = scriptableCharacter;

            characterImage.sprite = scriptableCharacter.sprites[0];

            //TEMPORALLY UNTIL SAVING LEVEL
            level = 1;

            maxHp = Mathf.RoundToInt(character.hp + level * character.multiplier);
            currentHp = maxHp;

            scriptableCharacter.passiveSkill.UseSkill(manager);

            UpdateText();
        }

        public void TakeDamage(int damage)
        {
            if (m_IsDead) return;

            currentHp -= damage;
            if (currentHp <= 0) Die();
            else
            {
                m_IsLowHp = currentHp < maxHp * .5f;

                //Temporally until animations
                if (m_IsLowHp) characterImage.DOColor(Color.red, 2f);
            }

            feedbackPooler.ShowText($"-{damage}", feedbackTextPositionTransform.position);
            UpdateText();
        }

        public void Heal(float healing){
            float percentage = currentHp * healing;
            currentHp += (int) percentage;
        }

        private void UpdateText()
        {
            if (currentHp > maxHp * .75f) hpText.color = Color.green;
            else if (currentHp > maxHp * .5f) hpText.color = Color.yellow;
            else hpText.color = Color.red;

            hpText.text = $"Current hp: {currentHp}";
        }

        public void UsePower(GameplayManager gameplayManager) => character.activeSkill.UseSkill(gameplayManager);

        public void Die()
        {
            m_IsDead = true;
            characterImage.DOColor(Color.clear, 5f);
            onDie?.Invoke();
        }
    }
}
