using System.Collections.Generic;
using System.Linq;
using Core;
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
        //public SerializedAudioClip mapSong;
        public AudioClip audioClip;
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
    public class SerializedAudioClip
    {
        public string name;
        public int sampleRate = 44100;
        public float frequency = 440;

        /*
        public AudioClip GetAudio()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            float[] samples = new float[audioSource.clip.samples * audioSource.clip.channels];
            audioSource.clip.GetData(samples, 0);

            for (int i = 0; i < samples.Length; ++i)
            {
                samples[i] = samples[i] * 0.5f;
            }

            audioSource.clip.SetData(samples, 0);
            
            AudioClip myClip = AudioClip.Create("MySinusoid", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
        }
        */
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

        private const string MAPS_FILE = "soundmaps.data";
        private const int DEFAULT_BPM = 120;

        private void Awake()
        {
            m_PreviewTransform = preview.transform;
            m_MainCamera = Camera.main;
        }

        private void Start()
        {
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
            }
        }

        private void CreateMapHolder(string map)
        {
            if (m_CurrentMapGameObject) Destroy(m_CurrentMapGameObject);

            bpmInputField.onSubmit.AddListener(UpdateBpmDelay);
            songDelayInputField.onSubmit.AddListener(UpdateBpmDelay);

            m_CurrentMapGameObject = new GameObject
            {
                name = map,
                transform = { parent = mapScroller.transform }
            };

            int beatsPerMinutes = GetBpm();
            if (beatsPerMinutes == 0) beatsPerMinutes = DEFAULT_BPM;

            //TODO: add audio clip somehow
            var soundMap = new SoundMap
            {
                id = map.GetHashCodeUshort(),
                startDelay = GetDelay(),
                bpm = beatsPerMinutes
            };

            mapScroller.soundMap = soundMap;
            soundMaps.Add(soundMap);
        }

        public void StopCreating()
        {
            IsCreating = false;
            preview.sprite = null;
            m_SelectedGameObject = null;
            if (m_CurrentMapGameObject)
            {
                SaveMap(m_CurrentMapGameObject.name);

                bpmInputField.onSubmit.RemoveListener(UpdateBpmDelay);
                songDelayInputField.onSubmit.RemoveListener(UpdateBpmDelay);
            }

            SetState(mapScroller.IsStarted ? "Playing Map" : "Not working on map");
        }

        public void LoadMapsData()
        {
            if (SaveLoadManager.SaveExists(MAPS_FILE))
            {
                soundMaps.AddRange(SaveLoadManager.Load<SoundMap[]>(MAPS_FILE));
            }
        }

        public void SaveMapsData() => SaveLoadManager.Save(soundMaps, MAPS_FILE);

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
            if (soundMap.id != hashName)
            {
                StartCreating(mapName);
            }
            else
            {
                soundMap.notes.ForEach(GenerateNote);
                SetState($"Loaded map {mapName} successfully!");
            }
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
                List<Note> notes = (
                    from noteObject in noteObjects
                    let position = noteObject.transform.position
                    select new Note { id = noteObject.MakerId, x = position.x, y = position.y, z = position.z }).ToList();

                soundMap.SetNotes(notes.ToArray());

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
