using System.Collections.Generic;
using System.Linq;
using Core;
using DG.Tweening;
using Plugins.Properties;
using Plugins.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities
{
    [System.Serializable]
    public class SoundMap
    {
        public ushort id;
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
        public Button button;
    }

    public class SongMapMaker : MonoBehaviour
    {
        [Information("This is temporal", InformationAttribute.InformationType.Info, true)]
        public AudioClip defaultSong;
        [Header("Config")]
        public TMP_Text stateText;
        public MapScroller mapScroller;
        [Space]
        public MakerNote[] makerNotes;
        public LayerMask notesLayer;
        public TMP_InputField bpmInputField, songDelayInputField;
        public TMP_InputField songNameInputField;

        [Header("Feedback Config")]
        public SpriteRenderer preview;

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

        private void Awake()
        {
            m_PreviewTransform = preview.transform;
            m_MainCamera = Camera.main;
        }

        private void OnEnable() => LoadMapsData();

        private void OnDisable() => SaveMapsData();

        private void Start()
        {
            bpmInputField.onSubmit.AddListener(txt => UpdateBpmDelay(songNameInputField.text));
            songDelayInputField.onSubmit.AddListener(txt => UpdateBpmDelay(songNameInputField.text));
            songNameInputField.onSubmit.AddListener(StartCreating);

            SetState("Not working on map");

            foreach (MakerNote makerNote in makerNotes)
            {
                makerNote.button.onClick.AddListener(() =>
                {
                    if (IsCreating)
                    {
                        preview.sprite = makerNote.image.sprite;
                        preview.color = makerNote.image.color;
                        m_SelectedGameObject = makerNote.noteObject.gameObject;
                        m_SelectedId = makerNote.id;
                    }
                });
            }
        }

        private void Update()
        {
            if (!IsCreating) return;

            Vector3 mousePosition = Input.mousePosition;

            mousePosition.z = Mathf.Abs(m_MainCamera.transform.position.z);

            mousePosition = m_MainCamera.ScreenToWorldPoint(mousePosition);

            mousePosition.x = Mathf.RoundToInt(mousePosition.x);
            mousePosition.y = Mathf.RoundToInt(mousePosition.y);
            mousePosition.z = 0;

            m_PreviewTransform.position = mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject() || Physics2D.CircleCast(mousePosition, .4f, Vector2.zero)) return;

                if (m_SelectedGameObject && m_CurrentMapGameObject)
                {
                    Instantiate(m_SelectedGameObject,
                                mousePosition,
                                m_SelectedGameObject.transform.rotation,
                                m_CurrentMapGameObject.transform)
                        .GetComponent<NoteObject>().MakerId = m_SelectedId;
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                RaycastHit2D result = Physics2D.CircleCast(mousePosition, .4f, Vector2.zero, 0, notesLayer);
                if (result)
                {
                    Destroy(result.collider.gameObject);
                }
            }
        }

        public void StartCreating(string map)
        {
            IsCreating = true;
            SetState($"Working on {map}");

            if (m_CurrentMapGameObject)
            {
                string currentMap = m_CurrentMapGameObject.name;

                if (currentMap.Equals(map)) return;

                SaveMap(currentMap);
                Destroy(m_CurrentMapGameObject);

                CreateMapHolder(map);
            }
            else
                CreateMapHolder(map);
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
            ushort hashName = mapName.GetHashCodeUshort();
            foreach (SoundMap soundMap in soundMaps.Where(soundMap => soundMap.id == hashName))
            {
                soundMap.SetBpmDelay(GetBpm(), GetDelay());
                mapScroller.SetSoundMap(soundMap);
            }

            print("Updated bpm and Delay!");
        }

        private void CreateMapHolder(string map)
        {
            if (m_CurrentMapGameObject) Destroy(m_CurrentMapGameObject);

            m_CurrentMapGameObject = new GameObject
            {
                name = map,
                transform = { parent = mapScroller.transform }
            };

            int beatsPerMinutes = GetBpm();
            if (beatsPerMinutes == 0) beatsPerMinutes = DEFAULT_BPM;

            //TODO: upload a song file.

            var soundMap = new SoundMap
            {
                id = map.GetHashCodeUshort(),
                startDelay = GetDelay(),
                bpm = beatsPerMinutes,
                audioClip = defaultSong
            };

            mapScroller.SetSoundMap(soundMap);
            soundMaps.Add(soundMap);
        }

        public void StopCreating()
        {
            IsCreating = false;
            preview.sprite = null;
            m_SelectedGameObject = null;
            SetState(mapScroller.IsStarted ? "Playing Map" : "Not working on map");
        }

        public void LoadMapsData()
        {
            if (SaveLoadManager.SaveExists(MAPS_FILE))
            {
                soundMaps.AddRange(SaveLoadManager.Load<SoundMap[]>(MAPS_FILE));
            }
        }

        public void SaveMapsData() => SaveLoadManager.Save(soundMaps.ToArray(), MAPS_FILE);

        public void DeleteMap(string mapName)
        {
            ushort hashName = mapName.GetHashCodeUshort();
            soundMaps.RemoveOne(map => map.id == hashName);
        }


        public void LoadMap(string mapName)
        {
            if (string.IsNullOrEmpty(mapName))
            {
                SetState("Please, write the map name!");
                return;
            }

            ushort hashName = mapName.GetHashCodeUshort();
            SoundMap soundMap = soundMaps.FirstOrDefault(map => map.id == hashName);

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
                
                SetState($"Loaded map {mapName} successfully!");
            }

            mapScroller.ResetPos();
        }

        private void GenerateNote(Note note)
        {
            foreach (MakerNote makerNote in makerNotes.Where(mNote => mNote.id == note.id))
            {
                var position = new Vector3 { x = note.x, y = note.y, z = note.z };
                GameObject obj = makerNote.noteObject.gameObject;
                Instantiate(obj,
                            position,
                            obj.transform.rotation,
                            m_CurrentMapGameObject.transform)
                    .GetComponent<NoteObject>().MakerId = m_SelectedId;
            }
        }

        public void SaveMap(string mapName)
        {
            if (string.IsNullOrEmpty(mapName))
            {
                SetState("Please, write the map name!");
                return;
            }

            ushort hashName = mapName.GetHashCodeUshort();
            foreach (SoundMap soundMap in soundMaps.Where(soundMap => soundMap.id == hashName))
            {
                NoteObject[] noteObjects = m_CurrentMapGameObject.GetComponentsInChildren<NoteObject>();
                Note[] notes = (
                    from noteObject in noteObjects
                    let position = noteObject.transform.position
                    select new Note { id = noteObject.MakerId, x = position.x, y = position.y, z = position.z }).ToArray();

                soundMap.SetNotes(notes);

                soundMap.SetBpmDelay(GetBpm(), GetDelay());
                return;
            }

            StartCreating(mapName);
        }

        public void LoadMap() => LoadMap(songNameInputField.text);

        public void SaveMap() => SaveMap(songNameInputField.text);

        public void ContinueCreating() => StartCreating(songNameInputField.text);
    }
}
