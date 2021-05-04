using System.Collections.Generic;
using Plugins.Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities
{
    [System.Serializable]
    public struct SoundMap
    {
        public string name;
        public float bpm;
        //public SerializedAudioClip mapSong;
        public AudioClip audioClip;
        public Note[] notes;
    }

    [System.Serializable]
    public struct Note
    {
        public ushort id;
        public int x, y, z;
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
        public int id;
        public Image image;
        public GameObject noteGameObject;
        [Space]
        public Button button;
    }

    public class SongMapMaker : MonoBehaviour
    {
        public MakerNote[] notesObject;
        private Camera m_MainCamera;
        public LayerMask notesLayer;

        [Header("Config")]
        public SpriteRenderer preview;
        private Transform m_PreviewTransform;
        
        private GameObject m_SelectedGameObject;
        [Space]
        public List<SoundMap> soundMaps;

        private const string MAPS_FILE = "soundmaps.data";

        public bool isCreating = true;

        private void Awake()
        {
            m_PreviewTransform = preview.transform;
            m_MainCamera = Camera.main;
        }

        private void Start()
        {
            foreach (MakerNote makerNote in notesObject)
            {
                makerNote.button.onClick.AddListener(() =>
                {
                    preview.sprite = makerNote.image.sprite;
                    m_SelectedGameObject = makerNote.noteGameObject;
                });
            }
        }

        private void Update()
        {
            if(!isCreating) return;
            
            Vector3 mousePosition = Input.mousePosition;

            mousePosition.z = Mathf.Abs(m_MainCamera.transform.position.z);

            mousePosition = m_MainCamera.ScreenToWorldPoint(mousePosition);
            
            mousePosition.x = Mathf.RoundToInt(mousePosition.x);
            mousePosition.y = Mathf.RoundToInt(mousePosition.y);
            mousePosition.z = 0;

            m_PreviewTransform.position = mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                if(EventSystem.current.IsPointerOverGameObject() || Physics2D.CircleCast(mousePosition, .4f, Vector2.zero)) return;
                
                if (m_SelectedGameObject) Instantiate(m_SelectedGameObject, mousePosition, m_SelectedGameObject.transform.rotation);
            } else if (Input.GetMouseButtonDown(1))
            {
                RaycastHit2D result = Physics2D.CircleCast(mousePosition, .4f, Vector2.zero, 0, notesLayer);
                if (result)
                {
                    Destroy(result.collider.gameObject);
                }
            }
        }

        public void LoadMapsData()
        {
            if (SaveLoadManager.SaveExists(MAPS_FILE))
            {
                soundMaps.AddRange(SaveLoadManager.Load<SoundMap[]>(MAPS_FILE));
            }
        }

        public void SaveMapsData() => SaveLoadManager.Save(soundMaps, MAPS_FILE);

        public void DeleteMap(string mapName) => soundMaps.RemoveOne(map => map.name.Equals(mapName));


        public void LoadMap(string mapName) { }

        public void SaveMap(string mapName) { }
    }
}
