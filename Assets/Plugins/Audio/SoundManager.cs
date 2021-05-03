using System.Collections;
using Plugins.Tools;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Plugins.Audio
{
    public readonly struct SoundItem
    {
        public readonly AudioClip clip;

        public readonly AudioMixerGroup audioMixerGroup;
        public readonly float volume;
        public readonly float spatialBlend;
        public readonly bool loop;
        public readonly float minDistance;
        public readonly float maxDistance;
        public readonly AudioRolloffMode audioRollOffMode;
        public readonly float randomPitchRange;

        public SoundItem(AudioClip clip, AudioMixerGroup audioMixerGroup, float volume = 1f, float randomPitchRange = .1f, float spatialBlend = 0f, bool loop = false, float minDistance = 1f, float maxDistance = 500f, AudioRolloffMode audioRollOffMode = AudioRolloffMode.Logarithmic)
        {
            this.clip = clip;
            this.audioMixerGroup = audioMixerGroup;
            this.volume = volume;
            this.spatialBlend = spatialBlend;
            this.loop = loop;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            this.audioRollOffMode = audioRollOffMode;
            this.randomPitchRange = randomPitchRange;
        }
    }

    [AddComponentMenu("Penguins Mafia/Sound Manager")]
    [RequireComponent(typeof(SoundPooler))]
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        [Header("Mixer")]
        public AudioMixer audioMixer;
        public AudioMixerGroup inGameAudioMixer;
        public AudioMixerGroup uiAudioMixer;

        [Header("InGame Pooler:")]
        public GameObject inGamePoolerPrefab;

        public float MasterVolume => GetVolume(MASTER_VOLUME_PARAM);
        public float MusicVolume => GetVolume(MUSIC_VOLUME_PARAM);

        public float VoiceVolume => GetVolume(VOICE_VOLUME_PARAM);
        public float SFXVolume => GetVolume(SFX_VOLUME_PARAM);

        /// Support variables
        private AudioSource m_BackgroundMusic;
        private SoundPooler m_SoundsPool;
        private SoundPooler m_CurrentInGamePooler;

        // Volume Params
        private const string MASTER_VOLUME_PARAM = "Master_Volume";
        
        private const string MUSIC_VOLUME_PARAM = "Music_Volume";
        private const string VOICE_VOLUME_PARAM = "Voice_Volume";
        private const string SFX_VOLUME_PARAM = "SFX_Volume";

        private const string SOUND_OBJECT_POOL = "SoundFXPool_Item";

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_BackgroundMusic = gameObject.GetComponentSafely<AudioSource>();
            m_SoundsPool = GetComponent<SoundPooler>();
        }

        /// <summary>
        /// On Enable
        /// </summary>
        private void OnEnable() => SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        /// <summary>
        /// On Disable
        /// </summary>
        private void OnDisable() => SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;

        /// <summary>
        /// Active Scene change
        /// </summary>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            if (m_CurrentInGamePooler != null) Destroy(m_CurrentInGamePooler.gameObject);
            m_CurrentInGamePooler = Instantiate(inGamePoolerPrefab).GetComponent<SoundPooler>();
        }

        /// <summary>
        /// Plays a background music.
        /// Only one background music can be active at a time.
        /// </summary>
        /// <param name="clip">Your audio clip.</param>
        /// <param name="fadeDuration"></param>
        public IEnumerator _PlayBackgroundMusic(AudioClip clip, float fadeDuration = 1f)
        {
            if (m_BackgroundMusic.clip == clip) yield break;

            yield return StartCoroutine(FadeMixerVolume(m_BackgroundMusic.outputAudioMixerGroup.audioMixer, MUSIC_VOLUME_PARAM, fadeDuration, 0f));

            m_BackgroundMusic.clip = clip;
            // we set the loop setting to true, the music will loop forever
            m_BackgroundMusic.loop = true;

            // we start playing the background music
            m_BackgroundMusic.Play();

            yield return StartCoroutine(FadeMixerVolume(m_BackgroundMusic.outputAudioMixerGroup.audioMixer, MUSIC_VOLUME_PARAM, fadeDuration, MusicVolume));
        }

        /// <summary>
        /// Fade mixer volume
        /// </summary>
        /// <param name="adMixer"></param>
        /// <param name="exposedParam"></param>
        /// <param name="duration"></param>
        /// <param name="targetVolume"></param>
        /// <returns></returns>
        private IEnumerator FadeMixerVolume(AudioMixer adMixer, string exposedParam, float duration, float targetVolume)
        {
            float currentTime = 0;
            adMixer.GetFloat(exposedParam, out float currentVol);
            currentVol = Mathf.Pow(10, currentVol / 20);

            float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
                adMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
                yield return null;
            }
        }

        /// <summary>
        /// Start Coroutine for play music
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="fadeDuration"></param>
        public void PlayBackgroundMusic(AudioClip clip, float fadeDuration = 1) => StartCoroutine(_PlayBackgroundMusic(clip, fadeDuration));

        /// <summary>
        /// Set master volume
        /// </summary>
        /// <param name="volume"></param>
        public void SetMasterVolume(float volume) => SetVolume(MASTER_VOLUME_PARAM, volume);

        /// <summary>
        /// Set music volume
        /// </summary>
        /// <param name="volume"></param>
        public void SetMusicVolume(float volume) => SetVolume(MUSIC_VOLUME_PARAM, volume);

        /// <summary>
        /// Set voice volume
        /// </summary>
        /// <param name="volume"></param>
        public void SetVoiceVolume(float volume) => SetVolume(VOICE_VOLUME_PARAM, volume);

        /// <summary>
        /// Set SFX volume
        /// </summary>
        /// <param name="volume"></param>
        public void SetSFXVolume(float volume) => SetVolume(SFX_VOLUME_PARAM, volume);

        private void SetVolume(string param, float volume) => audioMixer.SetFloat(param, volume.ToDecibels());

        private float GetVolume(string param) => audioMixer.GetFloat(param, out float decibels) ? decibels.ToVolume() : 0f;

        /// <summary>
        /// Stop a background music
        /// </summary>
        public void StopBackgroundMusic() => m_BackgroundMusic.Stop();

        /// <summary>
        /// Stop a background music
        /// </summary>
        public void ResumeBackgroundMusic() => m_BackgroundMusic.Play();

        /// <summary>
        /// Pause background music
        /// </summary>
        public void PauseBackgroundMusic() => m_BackgroundMusic.Pause();

        /// <summary>
        /// Resume background music
        /// </summary>
        public void UnPauseBackgroundMusic() => m_BackgroundMusic.UnPause();

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="sfx">The sound clip you want to play.</param>
        /// <param name="location">The location of the sound.</param>
        /// <param name="volume"></param>
        /// <param name="loop">If set to true, the sound will loop.</param>
        /// <param name="minDistance"></param>
        /// <param name="maxDistance"></param>
        /// <param name="delay"></param>
        public void PlaySound(AudioClip sfx, Vector3 location, float volume = 1f, bool loop = false, float minDistance = 1f, float maxDistance = 4f, float delay = 0f) => PlaySoundRandomPitch(sfx, location, volume, 0f, loop, minDistance, maxDistance, delay);

        /// <summary>
        /// Plays a sound with random pitch
        /// </summary>
        /// <param name="sfx">The sound clip you want to play.</param>
        /// <param name="location">The location of the sound.</param>
        /// <param name="volume"></param>
        /// <param name="randomPitchRange">A range to be played between the pitch</param>
        /// <param name="loop">If set to true, the sound will loop.</param>
        /// <param name="minDistance"></param>
        /// <param name="maxDistance"></param>
        /// <param name="delay"></param>
        public void PlaySoundRandomPitch(AudioClip sfx, Vector3 location, float volume = 1f, float randomPitchRange = .1f, bool loop = false, float minDistance = 1f, float maxDistance = 4f, float delay = 0f)
        {
            if (!m_CurrentInGamePooler) return;

            var sound = new SoundItem(sfx, inGameAudioMixer, volume, randomPitchRange, 1f, loop, minDistance, maxDistance, AudioRolloffMode.Linear);
            m_CurrentInGamePooler.Trigger(SOUND_OBJECT_POOL, location, delay, sound);
        }

        /// <summary>
        /// Plays a sound effect without taking position into account. Also, sound is not destroyed on scene load,
        /// as long as the SoundManager isn't destroyed either.
        /// </summary>
        /// <param name="sfx">Sound Effect to be played.</param>
        /// <param name="volume"></param>
        /// <param name="loop"></param>
        /// <param name="delay">Delay time waited before playing.</param>
        public void PlayNonDiegeticSound(AudioClip sfx, float volume = 1f, bool loop = false, float delay = 0f) => PlayNonDiegeticRandomPitchSound(sfx, volume, 0f, loop, delay);

        /// <summary>
        /// Plays a sound effect with random pitch without taking position into account. Also, sound is not destroyed on scene load,
        /// as long as the SoundManager isn't destroyed either.
        /// </summary>
        /// <param name="sfx">Sound Effect to be played.</param>
        /// <param name="volume"></param>
        /// <param name="randomPitchRange">A range to be played between the pitch</param>
        /// <param name="loop"></param>
        /// <param name="delay">Delay time waited before playing.</param>
        public void PlayNonDiegeticRandomPitchSound(AudioClip sfx, float volume = 1f, float randomPitchRange = .1f, bool loop = false, float delay = 0f)
        {
            var sound = new SoundItem(sfx, uiAudioMixer, volume, randomPitchRange, 0f, loop);
            m_SoundsPool.Trigger(SOUND_OBJECT_POOL, Vector3.zero, delay, sound);
        }

        /// <summary>
        /// Apply sound/music modification values
        /// </summary>
        /// <param name="musicValue"></param>
        /// <param name="sfxValue"></param>
        public void ApplySoundValues(float musicValue, float sfxValue)
        {
            SetMusicVolume(musicValue);
            SetSFXVolume(sfxValue);
        }

        /// <summary>
        /// Mute all current sfx
        /// </summary>
        public void StopAllSfx()
        {
            m_SoundsPool.Stop();
            m_CurrentInGamePooler.Stop();
        }

        /// <summary>
        /// Change volume to half for menus
        /// </summary>
        public void OpenMenuVolume()
        {
            audioMixer.SetFloat(MUSIC_VOLUME_PARAM, (MusicVolume * 0.5f).ToDecibels());
            audioMixer.SetFloat(SFX_VOLUME_PARAM, (SFXVolume * 0.5f).ToDecibels());
        }

        /// <summary>
        /// Change volume to default for closed menus
        /// </summary>
        public void CloseMenuVolume()
        {
            audioMixer.SetFloat(MUSIC_VOLUME_PARAM, MusicVolume.ToDecibels());
            audioMixer.SetFloat(SFX_VOLUME_PARAM, SFXVolume.ToDecibels());
        }
    }
}
