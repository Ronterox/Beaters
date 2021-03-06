using System;
using System.Collections;
using Core.Arrow_Game;
using General;
using MK.Glow.Legacy;
using Plugins.Audio;
using Plugins.Properties;
using Plugins.Tools;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{
    public enum HitType { Good = 3, Perfect = 5, TooSlow = 2, TooSoon = 2 }

    public enum GameMode { Hero, Push, Defender }

    public class GameplayManager : Singleton<GameplayManager>
    {
        public MapScroller mapScroller;

        [Header("Config")]
        public Character currentCharacter;
        public SimpleFeedbackObjectPooler feedbackTextPooler, skillGainTextPooler;
        [Space]
        public Transform gameCanvas;
        public GameObject endGamePanel;
        [Information("Prefab will not be instantiated, this has to be reference from the scene", InformationAttribute.InformationType.Info, false)]
        public GameObject pauseMenu;

        [Header("About Song")]
        public Slider songTimeBar;
        public TMP_Text starsCounter;

        [Header("Combo and Score feedback")]
        public Slider scoreBar;
        public TMP_Text comboText, scoreText;

        [Header("Skill feedback")]
        public SwipeDetector swipeDetector;
        public Slider skillBarSlider;
        [Space]
        public TimerUI skillsTimer;

        [Header("Song State")]
        public Timer songTimer;
        public TMP_Text finishText;

        [Header("Timer Feedback")]
        public Image feedbackImage;
        public TMP_Text skillText;

        [Header("Combo Feedback")]
        public MKGlow mkGlow;

        [Header("Sfx")]
        public AudioClip missSfx;
        public AudioClip winSfx, loseSfx;

        protected int m_Combo, m_Score, m_StarsCount, m_Taps;
        private bool m_OnGoing, m_Ended, m_IsPaused, m_Started;

        protected int m_ComboPrizeCounter, m_HighestCombo, m_NotesHit;
        private int m_SongFactorialScore, m_FactorialCombo = 1;

        protected PlayerData m_Data;
        protected SoundManager m_SoundManager;

        private float m_StartTime;
        protected Vector3 m_SkillSliderPosition;

        private GameObject m_AnimationPrefab;

        private int Combo
        {
            get => m_Combo;
            set
            {
                float bloom = (m_Combo = value) * .3f / m_FactorialCombo;
                mkGlow.bloomIntensity = Mathf.Max(.1f, bloom);
            }
        }

        public bool CanLose { get; set; } = true;

        protected int m_MaxMoneyGain, m_MinMoneyGain;

        //POWERS RELATED VARIABLES
        public bool CanMiss { get; set; } = true;
        public float Multiplier
        {
            get => m_Multiplier;
            set => m_Multiplier = value < 1f ? 1f : value;
        }
        private float m_Multiplier = 1f;
        public float DurationIncrement { get; set; }

        public float MoneyMultiplier { get; set; } = .25f;
        public int MinimumDamage { get; set; } = 9;
        public bool EveryNoteGivesMoney { get; set; }
        public bool ComboTimeHeal { get; set; }
        public float HealingValueComboTime { get; set; }
        public int MinimumCombo { get; set; }

        public int FreeTapsCount { get; set; }

        private int m_MissedTaps;

        //------------------------

        protected override void Awake()
        {
            base.Awake();
            m_StartTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Sets the gameplay values
        /// </summary>
        protected virtual void Start()
        {
            m_Data = DataManager.Instance.playerData;
            m_SoundManager = SoundManager.Instance;

            m_SkillSliderPosition = skillBarSlider.transform.position;

            SetGameplayCharacter();

            ResetValues();

            ScriptableRune rune = GameManager.GetRune();
            if (rune) rune.ActivateRune(this);

            feedbackImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// Saves the time played on the scene and the taps done, if not ended
        /// </summary>
        private void OnDestroy()
        {
            //Save the time played
            m_Data.timePlayedInGame += Time.realtimeSinceStartup - m_StartTime;
            //If it didn't see the final screen then save the tapsDone
            if (!m_Ended) m_Data.tapsDone += m_Taps;
        }

        /// <summary>
        /// Updates the song time ui
        /// </summary>
        private void Update()
        {
            if (!m_OnGoing) return;
            songTimeBar.value = songTimer.CurrentTime;
        }

        /// <summary>
        /// Starts the whole gameplay
        /// </summary>
        public virtual void StartMap()
        {
            m_Ended = false;
            m_OnGoing = CanMiss = m_Started = true;

            SoundMap soundMap = GameManager.GetSoundMap();

            float audioClipLength = soundMap.audioClip.length;
            int notesLength = soundMap.notes.Length;

            const float short_song_length = 10, medium_song_length = 30;

            if (audioClipLength < short_song_length)
            {
                m_MaxMoneyGain = 34;
                m_MinMoneyGain = 17;
            }
            else if (audioClipLength < medium_song_length)
            {
                m_MaxMoneyGain = 50;
                m_MinMoneyGain = 25;
            }
            else
            {
                m_MaxMoneyGain = 100;
                m_MinMoneyGain = 50;
            }

            SetSongValues(audioClipLength, notesLength);
            songTimer.StartTimer();

            mapScroller.SetSoundMap(soundMap, true);
            mapScroller.StartMap();
        }

        /// <summary>
        /// Free taps
        /// </summary>
        /// <param name="taps"></param>
        public void SetFreeTaps(int taps)
        {
            FreeTapsCount = m_MissedTaps + taps;

            UpdateFreeTapsText();

            feedbackImage.fillAmount = 1f;
            feedbackImage.gameObject.SetActive(true);
        }

        /// <summary>
        /// Updates the free taps skill text
        /// </summary>
        private void UpdateFreeTapsText() => skillText.text = $"{FreeTapsCount - m_MissedTaps}";

        /// <summary>
        /// Sets the skill timer with feedback for it
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onStop"></param>
        public void SetSkillTimer(float duration, Timer.TimerEvent onStop)
        {
            feedbackImage.gameObject.SetActive(true);

            onStop += () => feedbackImage.gameObject.SetActive(false);

            skillsTimer.SetEvents(null, onStop);
            skillsTimer.timerTime = duration + DurationIncrement;

            skillsTimer.StartTimer();
        }

        /// <summary>
        /// Alternates between pause and unpause
        /// </summary>
        public void PauseUnpausePress()
        {
            m_IsPaused = !m_IsPaused;
            if (m_IsPaused) PauseMap();
            else ResumeMap();
        }

        /// <summary>
        /// Activates the passive and sets the active skill
        /// </summary>
        private void SetGameplayCharacter()
        {
            currentCharacter.onDie += Lose;

            currentCharacter.SetCharacter(GameManager.GetCharacter(), this);

            skillBarSlider.maxValue = currentCharacter.character.activeSkill.rechargeQuantity;

            swipeDetector.onSwipe += swipe =>
            {
                if (skillBarSlider.value < skillBarSlider.maxValue) return;

                //Use active only when available
                currentCharacter.UsePower(this);
                skillBarSlider.value = 0;
            };
        }

        /// <summary>
        /// Loses the game and calls everything related
        /// </summary>
        public void Lose() =>
            SlowTime(2f, () =>
            {
                mapScroller.StopMap();
                ShowEndGameplayPanel(gameCanvas, false);
                SoundManager.Instance.backgroundAudioSource.Stop();
                mapScroller.gameObject.SetActive(false);
            });

        /// <summary>
        /// Slows the game time
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="afterSlowingTime"></param>
        public void SlowTime(float duration, Action afterSlowingTime = null)
        {
            feedbackImage.gameObject.SetActive(false);
            StartCoroutine(SlowTimeCoroutine(duration, afterSlowingTime));
        }

        public void SlowTimeReverse(float duration, Action afterSlowingTime = null) => StartCoroutine(SlowTimeReverseCoroutine(duration, afterSlowingTime));

        /// <summary>
        /// Slow time coroutine
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="afterSlowingTime"></param>
        /// <returns></returns>
        private IEnumerator SlowTimeCoroutine(float duration, Action afterSlowingTime)
        {
            AudioSource sound = SoundManager.Instance.backgroundAudioSource;

            var currentTime = 0f;
            float startValue = Time.timeScale;

            const float objective = 0f;
            while (currentTime < duration)
            {
                sound.pitch = Time.timeScale = Mathf.Lerp(startValue, objective, currentTime / duration);
                currentTime += Time.unscaledDeltaTime;
                yield return null;
            }

            sound.pitch = Time.timeScale = 1f;

            afterSlowingTime?.Invoke();
        }

        /// <summary>
        /// Slow time coroutine
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="afterSlowingTime"></param>
        /// <returns></returns>
        private IEnumerator SlowTimeReverseCoroutine(float duration, Action afterSlowingTime)
        {
            AudioSource sound = SoundManager.Instance.backgroundAudioSource;

            float currentTime = duration;
            float startValue = Time.timeScale;

            feedbackImage.gameObject.SetActive(true);

            const float objective = 0f;
            while (currentTime > 0)
            {
                sound.pitch = Time.timeScale = Mathf.Lerp(startValue, objective, currentTime / duration);
                currentTime -= Time.unscaledDeltaTime;

                UpdateTimerFeedback(currentTime, duration);
                yield return null;
            }

            feedbackImage.gameObject.SetActive(false);

            sound.pitch = Time.timeScale = 1f;

            afterSlowingTime?.Invoke();
        }

        public void UpdateTimerFeedback(float currentTime, float duration)
        {
            feedbackImage.fillAmount = currentTime.GetPercentageValue(duration);
            skillText.text = $"{currentTime:N2}";
        }

        /// <summary>
        /// Resets all the values to 0, for a replay
        /// </summary>
        protected void ResetValues()
        {
            //Reset slider and private values to 0
            m_Ended = m_Started = m_OnGoing = CanMiss = false;

            Combo = m_Score = m_StarsCount = m_Taps = m_ComboPrizeCounter = m_HighestCombo = m_NotesHit = 0;
            scoreBar.value = songTimeBar.value = skillBarSlider.value = 0;

            starsCounter.text = "0";
            comboText.text = "x0";
            scoreText.text = "0";
        }

        /// <summary>
        /// Sets the initial songs values for the ui
        /// </summary>
        /// <param name="time"></param>
        /// <param name="notes"></param>
        private void SetSongValues(float time, int notes)
        {
            time = Mathf.Ceil(time) + 1f; //We add extra length to assure ending

            songTimer.SetTimer(new TimerOptions(time, TimerType.Progressive, false));
            songTimer.onTimerStop += StopMap;

            songTimeBar.maxValue = time;

            int songLog = Mathf.RoundToInt(Mathf.Log(notes));

            m_FactorialCombo = notes;

            scoreBar.maxValue = notes < 1 ? 0 : m_SongFactorialScore = songLog.FactorialSum();
            m_SongFactorialScore *= songLog;
        }

        /// <summary>
        /// Pauses gameplay and song timer but not time
        /// </summary>
        public void PauseMap(bool showMenu = true)
        {
            m_OnGoing = false;
            songTimer.PauseTimer();
            mapScroller.StopMap();
            pauseMenu.SetActive(showMenu);
        }

        /// <summary>
        /// Resumes gameplay and song timer
        /// </summary>
        public void ResumeMap()
        {
            if (m_Started)
            {
                m_OnGoing = true;
                songTimer.UnpauseTimer();
                mapScroller.ResumeMap();
            }
            m_IsPaused = false;
            pauseMenu.SetActive(false);
        }

        /// <summary>
        /// Ends the gameplay, and shows the end screen
        /// </summary>
        public void StopMap()
        {
            if (m_Ended) return;

            m_OnGoing = false;
            songTimer.onTimerStop -= StopMap;

            ShowEndGameplayPanel(gameCanvas, true);
        }

        /// <summary>
        /// Shows the end gameplay panel
        /// </summary>
        /// <param name="parentCanvas"></param>
        /// <param name="win"></param>
        protected virtual void ShowEndGameplayPanel(Transform parentCanvas, bool win)
        {
            m_Ended = true;

            mkGlow.bloomIntensity = .1f;

            CheckHighestCombo();

            float accuracy = m_NotesHit * 100f / mapScroller.MapNotesQuantity;

            float rng = Random.Range(m_MinMoneyGain, m_MaxMoneyGain);
            const float percentageGainLimiter = .50f;

            int accuracyGain = Mathf.RoundToInt((accuracy * percentageGainLimiter + rng) * MoneyMultiplier),
                comboGain = Mathf.RoundToInt((m_HighestCombo * percentageGainLimiter + rng) * MoneyMultiplier);

            m_Data.money += accuracyGain + comboGain;
            m_Data.tapsDone += m_Taps;

            SoundMap soundMap = mapScroller.SoundMap;
            ScriptableCharacter character = GameManager.GetCharacter();

            SerializableSong songData = DataManager.GetSong(soundMap.ID);
            if (songData.songId == 0) songData.SetId(soundMap.ID);

            int oldHighScore = songData.highestScore;

            var gameOverPanel = Instantiate(endGamePanel, parentCanvas).GetComponent<GameOverPanel>();
            //Set end panel values
            gameOverPanel.SetSongName(soundMap.name);
            gameOverPanel.SetCharacterVisuals(character);
            gameOverPanel.SetCharacterBonus(character.characterGenre, soundMap.genre);

            gameOverPanel.SetScore(songData.highestScore, m_Score);
            gameOverPanel.SetStars(m_StarsCount, character.fullStar, character.emptyStar);
            gameOverPanel.SetAccuracy(soundMap.notes.Length, m_NotesHit, accuracy);

            gameOverPanel.SetMapMaker(soundMap.mapCreator);
            gameOverPanel.SetGroupName(soundMap.genre);

            gameOverPanel.SetHighestCombo(m_HighestCombo);
            gameOverPanel.SetRewardsText(comboGain, accuracyGain);

            gameOverPanel.replaySongButton.onClick.AddListener(() =>
            {
                m_SoundManager.StopAllSfx();
                LevelLoadManager.LoadArrowGameplayScene();
            });

            gameOverPanel.gameObject.SetActive(false);

            Action activateEndScreen = () => gameOverPanel.gameObject.SetActive(true);

            finishText.text = win ? "You WIN!" : "You LOSE!";
            finishText.gameObject.SetActive(true);

            activateEndScreen.DelayAction(2);

            m_SoundManager.PlayNonDiegeticSound(win ? winSfx : loseSfx);

            //Show prizes

            if (m_Score <= oldHighScore) return;

            songData.UpdateValues(m_Combo, m_Score, accuracy);
            DataManager.UpdateSong(songData);
        }

        /// <summary>
        /// Static caller for instance method
        /// </summary>
        public static void MissArrow()
        {
            if (m_Instance) m_Instance.MissArrowLogic();
        }

        /// <summary>
        /// Logic for missing an arrow without tapping
        /// </summary>
        private void MissArrowLogic()
        {
            // Can miss check in case of power
            if (!CanMiss || m_IsPaused) return;

            if (m_Combo > MinimumCombo)
            {
                CheckHighestCombo();
                //Reset combo to 0
                comboText.text = $"x{Combo = 0}";
            }

            if (FreeTapsCount >= ++m_MissedTaps)
            {
                UpdateFreeTapsText();
                return;
            }

            if (CanLose && !currentCharacter.IsDead) currentCharacter.TakeDamage(MinimumDamage);

            m_SoundManager.PlayNonDiegeticRandomPitchSound(missSfx);
        }

        /// <summary>
        /// Saves highest combo
        /// </summary>
        protected void CheckHighestCombo()
        {
            int combo = m_Combo;
            if (combo > m_HighestCombo) m_HighestCombo = combo;
        }

        /// <summary>
        /// Logic for missing an arrow by tapping
        /// </summary>
        public static void MissArrowTap()
        {
            if (!m_Instance) return;

            m_Instance.m_Taps++;
            MissArrow();
        }

        /// <summary>
        /// Static caller for hitting an arrow
        /// </summary>
        /// <param name="hitType"></param>
        /// <param name="feedbackPosition"></param>
        /// <param name="feedbackColor"></param>
        /// <param name="isCombo"></param>
        /// <param name="comboLength"></param>
        public static void HitArrow(HitType hitType, Vector3 feedbackPosition, Color feedbackColor, bool isCombo = false, int comboLength = 0)
        {
            if (m_Instance) m_Instance.HitArrowLogic(hitType, feedbackPosition, feedbackColor, isCombo, comboLength);
        }

        /// <summary>
        /// Logic for hitting an arrow correctly
        /// </summary>
        /// <param name="hitType"></param>
        /// <param name="feedbackPosition"></param>
        /// <param name="feedbackColor"></param>
        /// <param name="isCombo"></param>
        /// <param name="comboLength"></param>
        private void HitArrowLogic(HitType hitType, Vector3 feedbackPosition, Color feedbackColor, bool isCombo, int comboLength)
        {
            //Increment combo, taps, notes hit for player stats
            m_Taps++;
            m_NotesHit++;
            skillBarSlider.value++;

            skillGainTextPooler.ShowText($"+1", m_SkillSliderPosition);

            //Get the points multiply by the combo and multiplier and finally rounded
            int points = Mathf.RoundToInt((int)hitType * ++Combo * Multiplier / m_SongFactorialScore);

            //TODO: print increment on screen feedback

            scoreText.text = $"{m_Score += points}";
            comboText.text = $"x{m_Combo}";

            float newScoreValue = scoreBar.value + points;
            scoreBar.value = newScoreValue % scoreBar.maxValue;

            //Increment the start count if went upper the score limit
            if (newScoreValue >= scoreBar.maxValue) starsCounter.text = $"{++m_StarsCount}";

            if (EveryNoteGivesMoney) m_Data.money += (int)(Random.Range(m_MinMoneyGain, m_MaxMoneyGain) * .10f * MoneyMultiplier);

            //Check if is combo or if is in middle of a combo and give prize
            if (isCombo)
            {
                if (ComboTimeHeal) currentCharacter.Heal(HealingValueComboTime);

                bool completedCombo = ++m_ComboPrizeCounter >= comboLength;

                float rng = Random.Range(m_MinMoneyGain, m_MaxMoneyGain);

                float totalGain = completedCombo ? rng * comboLength * .05f : rng * .05f;

                skillBarSlider.value += totalGain;

                skillGainTextPooler.ShowText($"+{totalGain}", m_SkillSliderPosition);
            }
            else
                m_ComboPrizeCounter = 0;

            //Hit Feedback
            feedbackTextPooler.ShowText(hitType.ToString(), feedbackColor, feedbackPosition);
        }

        /// <summary>
        /// Sets and plays the character animation prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="animationSound"></param>
        /// <param name="onAnimationEnd"></param>
        public void PlayAnimationPrefab(GameObject prefab, AudioClip animationSound, Action onAnimationEnd = null)
        {
            if (!m_AnimationPrefab || !prefab.name.Equals(m_AnimationPrefab.name))
            {
                m_AnimationPrefab = Instantiate(prefab, gameCanvas);
                m_AnimationPrefab.name = prefab.name;
            }

            m_AnimationPrefab.SetActive(true);
            PauseMap(false);

            Action deactivateAnimation = () =>
            {
                m_AnimationPrefab.SetActive(false);
                ResumeMap();
                onAnimationEnd?.Invoke();
            };

            m_SoundManager.PlayNonDiegeticSound(animationSound);

            deactivateAnimation.DelayAction(1f);
        }
    }
}
