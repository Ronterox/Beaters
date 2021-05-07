using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using DG.Tweening;
using Managers;
using Plugins.Tools;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Utilities
{
    [System.Serializable]
    public class SoundMap
    {
        public ushort id;
        public string name;
        public float bpm, startDelay;
        public SerializableAudioClip audioClip;
        public Note[] notes;

        public void SetNotes(Note[] newNotes) => notes = newNotes;

        public void SetBpmDelay(int newBpm, float delay)
        {
            bpm = newBpm;
            startDelay = delay;
        }
    }

    [System.Serializable]
    public struct Note
    {
        public ushort id;
        public float x, y, z;
    }

    [System.Serializable]
    public struct SerializableAudioClip
    {
        public string name;
        [HideInInspector]
        public float[] audioData;
        public int samples, channels, frecuency;

        public SerializableAudioClip(AudioClip audioClip)
        {
            name = audioClip.name;
            samples = audioClip.samples;
            channels = audioClip.channels;
            audioData = new float[samples * channels];
            frecuency = audioClip.frequency;

            audioClip.GetData(audioData, 0);

            for (var i = 0; i < audioData.Length; ++i) audioData[i] = audioData[i] * 0.5f;
        }

        public static implicit operator AudioClip(SerializableAudioClip serializableAudioClip)
        {
            var clip = AudioClip.Create(serializableAudioClip.name, serializableAudioClip.samples, serializableAudioClip.channels, serializableAudioClip.frecuency, false);
            clip.SetData(serializableAudioClip.audioData, 0);
            return clip;
        }

        public static implicit operator SerializableAudioClip(AudioClip audioClip) => new SerializableAudioClip(audioClip);
    }

    [System.Serializable]
    public struct MakerNote
    {
        public ushort id;
        public Image image;
        public NoteObject noteObject;
        [Space]
        public HoldableButton button;
    }

    public class SongMapMaker : MonoBehaviour
    {
        public AudioClip defaultSong;
        [Header("Config")]
        public TMP_Text stateText;
        public MapScroller mapScroller;
        [Space]
        public MakerNote[] makerNotes;
        public LayerMask notesLayer;
        public TMP_InputField bpmInputField, songDelayInputField;
        public TMP_InputField songNameInputField;
        [Space]
        public TMP_Text songNameText;
        public TMP_Dropdown songListDropdown;

        [Header("Feedback Config")]
        public SpriteRenderer preview;
        public ParticleSystem touchParticle;

        [Header("My Maps")]
        public List<SoundMap> soundMaps;
        public bool IsCreating { get; private set; }

        private GameObject m_CurrentMapGameObject;

        private Camera m_MainCamera;
        private Transform m_PreviewTransform;

        private GameObject m_SelectedGameObject;
        private ushort m_SelectedId;

        private const string MAPS_FILE = "soundmaps";
        private const int DEFAULT_BPM = 120;

        private bool m_IsHoldingNote;

        private void Awake()
        {
            m_PreviewTransform = preview.transform;
            m_MainCamera = Camera.main;
        }

#if !UNITY_EDITOR
        private void OnDestroy() => SaveMapsData();
#endif
        //TODO: chek if System.IO.Path works on mobile
        //TODO: check if save songs binary and load work
        private void Start()
        {
#if !UNITY_EDITOR
            LoadMapsData();
#endif
            songListDropdown.onValueChanged.AddListener(option => LoadMap(songListDropdown.options[option].text));

            bpmInputField.onSubmit.AddListener(txt => UpdateBpmDelay(songNameInputField.text));
            songDelayInputField.onSubmit.AddListener(txt => UpdateBpmDelay(songNameInputField.text));
            songNameInputField.onSubmit.AddListener(StartCreating);

            SetState("Not working on mapName");

            foreach (MakerNote makerNote in makerNotes)
            {
                makerNote.button.onButtonDown += () =>
                {
                    if (IsCreating)
                    {
                        preview.sprite = makerNote.image.sprite;
                        preview.color = makerNote.image.color;
                        m_SelectedGameObject = makerNote.noteObject.gameObject;
                        m_SelectedId = makerNote.id;
                        m_IsHoldingNote = true;
                    }
                };
            }
        }

        private void CleanPreview()
        {
            preview.sprite = null;
            m_SelectedGameObject = null;
        }

        private void Update()
        {
            if (!IsCreating) return;

            Vector3 mousePosition = Input.mousePosition;

            //Set the position relative to the camera vision
            mousePosition.z = Mathf.Abs(m_MainCamera.transform.position.z);

            mousePosition = m_MainCamera.ScreenToWorldPoint(mousePosition);

            mousePosition.x = Mathf.RoundToInt(mousePosition.x);
            mousePosition.y = Mathf.RoundToInt(CameraManager.In2D ? mousePosition.y : mousePosition.y + 5);
            mousePosition.z = 0;

            //Position preview where the mouse is supposed to be
            m_PreviewTransform.position = mousePosition;

            //Check to destroy a tile note
            if (Input.GetMouseButtonDown(0) && !m_IsHoldingNote)
            {
                RaycastHit2D result = Physics2D.CircleCast(mousePosition, .4f, Vector2.zero, 0, notesLayer);
                if (result) Destroy(result.collider.gameObject);
            }

            if (!m_IsHoldingNote && Input.GetMouseButtonUp(0)) ShowTouchParticle(mousePosition);

            //Check for holding note and dropping
            if (!m_IsHoldingNote || !Input.GetMouseButtonUp(0)) return;

            if (EventSystem.current.IsPointerOverGameObject() || Physics2D.CircleCast(mousePosition, .4f, Vector2.zero)) return;

            if (m_SelectedGameObject && m_CurrentMapGameObject)
            {
                Instantiate(m_SelectedGameObject,
                            mousePosition,
                            m_SelectedGameObject.transform.rotation,
                            m_CurrentMapGameObject.transform)
                    .GetComponent<NoteObject>().MakerId = m_SelectedId;

                CleanPreview();
            }
            m_IsHoldingNote = false;
        }

        private void ShowTouchParticle(Vector3 position)
        {
            touchParticle.Play();
            touchParticle.transform.position = position;
        }

        public void StartCreating(string mapName)
        {
            if (IsMapNameEmpty(mapName)) return;

            IsCreating = true;
            SetState($"Working on {mapName}");

            if (m_CurrentMapGameObject)
            {
                string currentMap = m_CurrentMapGameObject.name;

                if (currentMap.Equals(mapName)) return;

                SaveMap(currentMap);
                Destroy(m_CurrentMapGameObject);

                CreateMapHolder(mapName);
            }
            else
                CreateMapHolder(mapName);
        }

        private void SetState(string text)
        {
            if (stateText) stateText.text = text;

            const float duration = .2f;
            stateText.transform.DOScale(2, duration).OnComplete(() => stateText.transform.DOScale(1, duration));
        }

        private int GetBpm()
        {
            int.TryParse(bpmInputField.text, out int bpm);
            return bpm;
        }

        private float GetDelay()
        {
            float.TryParse(songDelayInputField.text, out float delay);
            return delay;
        }

        private void UpdateBpmDelay(string mapName)
        {
            SoundMap soundMap = GetSoundMap(mapName);

            if (soundMap != null)
            {
                soundMap.SetBpmDelay(GetBpm(), GetDelay());
                mapScroller.SetSoundMap(soundMap);
            }

            print("Updated bpm and Delay!");
        }

        private void CreateMapHolder(string mapName)
        {
            if (m_CurrentMapGameObject) Destroy(m_CurrentMapGameObject);

            m_CurrentMapGameObject = new GameObject
            {
                name = mapName,
                transform = { parent = mapScroller.transform }
            };

            SoundMap soundMap = GetSoundMap(mapName);

            if (soundMap == null)
            {
                int beatsPerMinutes = GetBpm();
                if (beatsPerMinutes == 0) beatsPerMinutes = DEFAULT_BPM;

                soundMap = new SoundMap
                {
                    id = mapName.GetHashCodeUshort(),
                    name = mapName,
                    startDelay = GetDelay(),
                    bpm = beatsPerMinutes,
                    audioClip = defaultSong
                };
                soundMaps.Add(soundMap);
            }
            mapScroller.SetSoundMap(soundMap);
        }

        public void StopCreating()
        {
            IsCreating = false;
            CleanPreview();
            SetState(mapScroller.IsStarted ? "Playing Map" : "Not working on mapName");
        }

        public void LoadMapsData()
        {
            if (!SaveLoadManager.SaveExists(MAPS_FILE)) return;
            soundMaps.AddRange(SaveLoadManager.Load<SoundMap[]>(MAPS_FILE));
            UpdateSongsList();
        }

        public void SaveMapsData() => SaveLoadManager.Save(soundMaps.ToArray(), MAPS_FILE);

        public void DeleteSoundMap(string mapName)
        {
            if (IsMapNameEmpty(mapName)) return;
            ushort hashName = mapName.GetHashCodeUshort();
            soundMaps.RemoveOne(map => map.id == hashName);
            SetState($"{mapName} was deleted successfully!");
        }

        public void DeleteSoundMap() => DeleteSoundMap(songNameText.text);

        private bool IsMapNameEmpty(string mapName)
        {
            if (!string.IsNullOrEmpty(mapName)) return false;
            SetState("Please, write the mapName name!");
            return true;
        }

        public void LoadMap(string mapName)
        {
            if (IsMapNameEmpty(mapName)) return;

            SoundMap soundMap = GetSoundMap(mapName);

            if (soundMap == null)
            {
                StartCreating(mapName);
            }
            else
            {
                IsCreating = true;
                CreateMapHolder(mapName);
                soundMap.notes?.ForEach(GenerateNote);

                bpmInputField.text = soundMap.bpm + "";
                songDelayInputField.text = soundMap.startDelay + "";
                songNameText.text = soundMap.audioClip.name;

                mapScroller.SetSoundMap(soundMap);

                SetState($"Loaded mapName {mapName} successfully!");
            }

            mapScroller.ResetPos();
        }

        private void GenerateNote(Note note)
        {
            MakerNote makerNote = makerNotes.First(makeNote => makeNote.id == note.id);

            var position = new Vector3 { x = note.x, y = note.y, z = note.z };
            GameObject obj = makerNote.noteObject.gameObject;

            Instantiate(obj, position, obj.transform.rotation, m_CurrentMapGameObject.transform)
                .GetComponent<NoteObject>().MakerId = m_SelectedId;
        }

        public void SaveMap(string mapName)
        {
            if (IsMapNameEmpty(mapName)) return;

            SoundMap soundMap = GetSoundMap(mapName);

            if (soundMap != null)
            {
                NoteObject[] noteObjects = m_CurrentMapGameObject.GetComponentsInChildren<NoteObject>();
                Note[] notes = (
                    from noteObject in noteObjects
                    let position = noteObject.transform.position
                    select new Note
                    {
                        id = noteObject.MakerId,
                        x = position.x,
                        y = position.y,
                        z = position.z
                    }).ToArray();

                soundMap.SetNotes(notes);

                soundMap.SetBpmDelay(GetBpm(), GetDelay());

                soundMap.audioClip = defaultSong;

                UpdateSongsList();

                mapScroller.SetSoundMap(soundMap);
            }
            else StartCreating(mapName);
        }

        private SoundMap GetSoundMap(string mapName)
        {
            ushort hashName = mapName.GetHashCodeUshort();
            return soundMaps.FirstOrDefault(map => map.id == hashName);
        }

        public void LoadMap() => LoadMap(songNameInputField.text);

        public void SaveMap() => SaveMap(songNameInputField.text);

        public void ContinueCreating() => StartCreating(songNameInputField.text);

        private void UpdateSongsList()
        {
            songListDropdown.ClearOptions();
            List<string> mapNames = soundMaps.Select(soundMap => soundMap.name).ToList();
            songListDropdown.AddOptions(mapNames);
        }

        public void AskForSoundFile()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            FileBrowser.Permission permission = FileBrowser.CheckPermission();
            if (permission == FileBrowser.Permission.ShouldAsk) FileBrowser.AskPermissions = true;
#endif
            FileBrowser.SetFilters(false, ".mp3", ".wav");

            FileBrowser.ShowLoadDialog(paths =>
            {
                string path = paths[0];

                if (!FileBrowserHelpers.FileExists(path)) return;

                string extension = Path.GetExtension(path);
                if (extension.Equals(".wav") || extension.Equals(".mp3"))
                {
                    StartCoroutine(GetAudioClip($"file://{path}"));
                }
            }, null, FileBrowser.PickMode.Files);
        }

        private IEnumerator GetAudioClip(string fullPath)
        {
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fullPath, AudioType.MPEG);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError) SetState(www.error);
            else
            {
                defaultSong = DownloadHandlerAudioClip.GetContent(www);
                songNameText.text = defaultSong.name = Path.GetFileNameWithoutExtension(fullPath);
                SaveMap();
            }
        }
    }
}
