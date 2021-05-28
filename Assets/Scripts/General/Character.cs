using DG.Tweening;
using Managers;
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
        public Slider hpSlider;
        public Image hpBarFill;

        [Header("Animations")]
        public RectTransform objectiveTransformPosition;
        public SimpleFeedbackObjectPooler feedbackPooler;

        public bool IsDead { get; private set; }

        public delegate void CharacterEvent();

        public event CharacterEvent onDie;

        private bool m_IsLowHp;

        public void SetCharacter(ScriptableCharacter scriptableCharacter, GameplayManager manager)
        {
            //Set serialized exp and lvl
            character = scriptableCharacter;

            characterImage.sprite = scriptableCharacter.sprites[0];

            //TEMPORALLY UNTIL SAVING LEVEL
            level = 1;

            maxHp = Mathf.RoundToInt(character.hp + level * character.multiplier);

            hpSlider.maxValue = hpSlider.value = currentHp = maxHp;

            scriptableCharacter.passiveSkill.UseSkill(manager);

            UpdateText();
        }

        public void TakeDamage(int damage)
        {
            if (IsDead) return;

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

        public void MoveToPosition(float duration) => transform.DOMove(objectiveTransformPosition.position, duration);

        public void Heal(float healingPercentage)
        {
            float percentage = currentHp * healingPercentage;
            currentHp += (int)percentage;
        }

        public void Revive(int health)
        {
            currentHp = health;
            IsDead = false;
            Heal(1);
        }

        private void UpdateText()
        {
            if (currentHp > maxHp * .75f) hpBarFill.color = Color.green;
            else if (currentHp > maxHp * .5f) hpBarFill.color = Color.yellow;
            else hpBarFill.color = Color.red;

            hpText.text = currentHp + "";
            hpSlider.value = currentHp;
        }

        public void UsePower(GameplayManager gameplayManager) => character.activeSkill.UseSkill(gameplayManager);

        public void Die()
        {
            IsDead = true;
            characterImage.DOColor(Color.clear, 5f);
            onDie?.Invoke();
        }
    }
}
