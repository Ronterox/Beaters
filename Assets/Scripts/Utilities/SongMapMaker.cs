using System.Collections.Generic;
using System.Linq;
using Core;
using Plugins.Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities
{
    [System.Serializable]
    public struct SoundMap
    {
        public ushort id;
        public float bpm, startDelay;
        //public SerializedAudioClip mapSong;
        public AudioClip audioClip;
        public Note[] notes;

        public void SetNotes(Note[] newNotes) => notes = newNotes;
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
        public MakerNote[] makerNotes;
        private Camera m_MainCamera;
        public LayerMask notesLayer;

        [Header("Feedback Config")]
        public SpriteRenderer preview;
        private Transform m_PreviewTransform;

        private GameObject m_SelectedGameObject;
        private ushort m_SelectedId;

        [Header("My Maps")]
        public List<SoundMap> soundMaps;
        public bool IsCreating { get; private set; }
        private GameObject m_CurrentMapGameObject;

        private const string MAPS_FILE = "soundmaps.data";

        private void Awake()
        {
            m_PreviewTransform = preview.transform;
            m_MainCamera = Camera.main;

            IsCreating = true;
        }

        private void Start()
        {
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

            StartCreating("Test");
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

        private void CreateMapHolder(string map)
        {
            if (m_CurrentMapGameObject) Destroy(m_CurrentMapGameObject);
            
            m_CurrentMapGameObject = new GameObject { name = map };
            //TODO: add audio clip somehow
            soundMaps.Add(new SoundMap { id = (ushort)map.GetHashCode() });
        }

        public void StopCreating()
        {
            IsCreating = false;
            preview.sprite = null;
            m_SelectedGameObject = null;
            if (m_CurrentMapGameObject) SaveMap(m_CurrentMapGameObject.name);
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
            var hashName = (ushort)mapName.GetHashCode();
            soundMaps.RemoveOne(map => map.id == hashName);
        }


        public void LoadMap(string mapName)
        {
            var hashName = (ushort)mapName.GetHashCode();
            SoundMap soundMap = soundMaps.FirstOrDefault(map => map.id == hashName);
            soundMap.notes.ForEach(GenerateNote);
        }

        private void GenerateNote(Note note)
        {
            foreach (MakerNote makerNote in makerNotes)
            {
                if (note.id == makerNote.id)
                {
                    var position = new Vector3 { x = note.x, y = note.y, z = note.z };
                    GameObject obj = makerNote.noteObject.gameObject;
                    Instantiate(obj,
                                position,
                                obj.transform.rotation,
                                m_CurrentMapGameObject.transform)
                        .GetComponent<NoteObject>().MakerId = m_SelectedId;
                    return;
                }
            }
        }

        public void SaveMap(string mapName)
        {
            var hashName = (ushort)mapName.GetHashCode();

            NoteObject[] noteObjects = m_CurrentMapGameObject.GetComponentsInChildren<NoteObject>();

            List<Note> notes = (from noteObject in noteObjects let position = noteObject.transform.position select new Note { id = noteObject.MakerId, x = position.x, y = position.y, z = position.z }).ToList();

            for (var i = 0; i < soundMaps.Count; i++)
            {
                if (soundMaps[i].id == hashName)
                {
                    soundMaps[i].SetNotes(notes.ToArray());
                    return;
                }
            }
        }
    }
}
