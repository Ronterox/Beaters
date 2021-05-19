//#define FORCE_JSON

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Arrow_Game;
using DG.Tweening;
using Managers;
using Plugins.SimpleFileBrowser.Scripts;
using Plugins.Tools;
#if UNITY_EDITOR && !FORCE_JSON
using ScriptableObjects;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Utilities
{
    [Serializable]
    public class SoundMap
    {
        public string name, mapCreator;
        public float bpm;
        public Genre genre = Genre.Custom;

        public AudioClip audioClip;
        public string audioPath;

        public Note[] notes;

        //If slow save this on memory
        public ushort ID => name.GetHashCodeUshort();

        public void SetNotes(Note[] newNotes) => notes = newNotes;

        /// <summary>
        /// Generates the map notes inside the set parent, based on the passed maker notes
        /// </summary>
        /// <param name="makerNotes"></param>
        /// <param name="parent"></param>
        /// <param name="generateCombo">optional parameter to see if you want to generate combo notes</param>
        public void GenerateNotes(MakerNote[] makerNotes, Transform parent, bool generateCombo = true)
        {
            //Check of generation of procedural combos, guitar hero style
            if (generateCombo)
            {
                int length = makerNotes.Length;
                var rng = new System.Random(length);
                int comboCounter = 0, currentCombo = 0;

                const int probability = 10, minComboLength = 5, maxComboLength = 8;

                void GenerateNote(Note note)
                {
                    MakerNote makerNote = makerNotes.First(makeNote => makeNote.id == note.id);

                    var position = new Vector3 { x = note.x, y = note.y, z = note.z };
                    GameObject obj = makerNote.noteObject.gameObject;

                    var noteObject = UnityEngine.Object.Instantiate(obj, position, obj.transform.rotation, parent).GetComponent<NoteObject>();
                    //Set the id so we can save or instantiate same object
                    noteObject.MakerId = makerNote.id;

                    //Check if need to continue a combo or if it generates one
                    if (currentCombo != 0)
                    {
                        noteObject.SetCombo(currentCombo);
                        if (++comboCounter >= currentCombo) currentCombo = 0;
                    }
                    else if (rng.Next(100) <= probability)
                    {
                        noteObject.SetCombo(currentCombo = rng.Next(minComboLength, maxComboLength));
                    }
                }

                foreach (Note note in notes) GenerateNote(note);
            }
            else
            {
                void GenerateNote(Note note)
                {
                    MakerNote makerNote = makerNotes.First(makeNote => makeNote.id == note.id);

                    var position = new Vector3 { x = note.x, y = note.y, z = note.z };
                    GameObject obj = makerNote.noteObject.gameObject;

                    var noteObject = UnityEngine.Object.Instantiate(obj, position, obj.transform.rotation, parent).GetComponent<NoteObject>();
                    //Set the id so we can save or instantiate same object
                    noteObject.MakerId = makerNote.id;
                }

                foreach (Note note in notes) GenerateNote(note);
            }
        }
    }

    [Serializable]
    public struct Note
    {
        public ushort id;
        public float x, y, z;
    }

    [Serializable]
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
        public AudioClip audioSong;
        public string audioClipPath;

        [Header("Config")]
        public MapScroller mapScroller;
        public ArrowButton[] playerButtons;

        [Header("Inputs")]
        public TMP_InputField songNameInputField;
        public TMP_InputField bpmInputField;

        [Header("Feedback Config")]
        public SpriteRenderer preview;
        public ParticleSystem touchParticle;
        [Space]
        public TMP_Text stateText, songNameText;
        public TMP_Dropdown songListDropdown;

        [Header("My Maps")]
        public List<SoundMap> soundMaps;
        public bool IsCreating { get; private set; }

        private GameObject m_CurrentMapGameObject;

        private Camera m_MainCamera;
        private Transform m_PreviewTransform;

        private GameObject m_SelectedGameObject;
        private ushort m_SelectedId;

        private bool m_IsNoteSelected;
        private Vector3 m_ButtonDownPosition;

#if !UNITY_EDITOR || FORCE_JSON
        private const string SONG_FOLDER = "Songs";
#endif
        private const int DEFAULT_BPM = 120;

#if UNITY_EDITOR && !FORCE_JSON
        [Header("Editor Only")]
        public string songScriptablesRelativePath;
        public string audioSongsRelativePath;
#endif
        private void Awake()
        {
            m_PreviewTransform = preview.transform;
            m_MainCamera = Camera.main;
        }

        //TODO: Fix save songs json and load work, change path for mobile
        /// <summary>
        /// Sets the events for all buttons and input fields
        /// </summary>
        private void Start()
        {
#if UNITY_EDITOR && !FORCE_JSON
            //Write the full canonical path for the audios folder on editor
            audioSongsRelativePath = audioSongsRelativePath.Replace("Assets", Application.dataPath);
#endif
            LoadMapsData();

            //Add listener for updates in case of changes of map option
            songListDropdown.onValueChanged.AddListener(option => LoadMap(songListDropdown.options[option].text));

            bpmInputField.onSubmit.AddListener(txt => UpdateBpm(songNameInputField.text));
            songNameInputField.onSubmit.AddListener(StartCreating);

            //On player screen show text
            SetState("Not working on any map");

            foreach (MakerNote makerNote in mapScroller.makerNotes)
            {
                //We add the action of setting preview/objects etc to the list of spawneables
                makerNote.button.onButtonDown += () =>
                {
                    if (IsCreating)
                    {
                        preview.sprite = makerNote.image.sprite;
                        preview.color = makerNote.image.color;
                        m_SelectedGameObject = makerNote.noteObject.gameObject;
                        m_SelectedId = makerNote.id;
                        m_IsNoteSelected = true;
                    }
                };
            }

            //Disable the ability to play game buttons
            EnableButtons(false);
        }

        /// <summary>
        /// Removes the preview of the selected item
        /// </summary>
        private void CleanPreview()
        {
            preview.sprite = null;
            m_SelectedGameObject = null;
        }

        /// <summary>
        /// Enables arrow game buttons to be click or not
        /// </summary>
        /// <param name="enable"></param>
        private void EnableButtons(bool enable) => playerButtons.ForEach(button => button.canBeClick = enable);

        /// <summary>
        /// Detects if a object was selected and updates the preview
        /// </summary>
        private void Update()
        {
            if (!IsCreating) return;

            const float checkNoteCastRange = .4f;
            const int rightClickButton = 1, leftClickButton = 0;

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
#if !UNITY_IPHONE && !UNITY_ANDROID
            if (Input.GetMouseButtonDown(rightClickButton)) CheckNoteAndDestroy(mousePosition, checkNoteCastRange);
#endif
            //Check for input drop and show particle
            bool buttonUp = Input.GetMouseButtonUp(0);
            if (buttonUp) ShowTouchParticle(mousePosition);

            //If not object selected exit
            if (!m_IsNoteSelected) return;

            if (Input.GetMouseButtonDown(leftClickButton)) m_ButtonDownPosition = mousePosition;

            //Check is pressing button, and if there is a note over other note, don't drop it
            if (!buttonUp || EventSystem.current.IsPointerOverGameObject() || m_ButtonDownPosition != mousePosition) return;

            CheckNoteAndDestroy(mousePosition, checkNoteCastRange);

            Instantiate(m_SelectedGameObject,
                        mousePosition,
                        m_SelectedGameObject.transform.rotation,
                        m_CurrentMapGameObject.transform)
                .GetComponent<NoteObject>().MakerId = m_SelectedId;
        }

        /// <summary>
        /// Casts and checks if there is a note around the position passed, if there is, destroys it
        /// </summary>
        /// <param name="mousePosition">the position where to cast</param>
        /// <param name="castRange">the area of the circle cast</param>
        private void CheckNoteAndDestroy(Vector3 mousePosition, float castRange)
        {
            RaycastHit2D result = Physics2D.CircleCast(mousePosition, castRange, Vector2.zero, 0, mapScroller.notesLayer.value);
            if (result) Destroy(result.collider.gameObject);
        }

        /// <summary>
        /// Shows the particle of clicking on the set position
        /// </summary>
        /// <param name="position"></param>
        private void ShowTouchParticle(Vector3 position)
        {
            touchParticle.Play();
            touchParticle.transform.position = position;
        }

        /// <summary>
        /// Starts whe map maker
        /// </summary>
        /// <param name="mapName"></param>
        public void StartCreating(string mapName)
        {
            if (IsMapNameEmpty(mapName)) return;

            //Activate use of creator interface
            IsCreating = true;
            SetState($"Working on {mapName}");

            if (m_CurrentMapGameObject)
            {
                string currentMap = m_CurrentMapGameObject.name;

                //If already working on this map ignored
                if (currentMap.Equals(mapName)) return;

                //else destroy the map and create the new map holder
                Destroy(m_CurrentMapGameObject);

                CreateMapHolder(mapName);
            }
            else
                CreateMapHolder(mapName);

            EnableButtons(false);
        }

        /// <summary>
        /// Updates the state text with an animation
        /// </summary>
        /// <param name="text"></param>
        private void SetState(string text)
        {
            if (stateText) stateText.text = text;

            const float duration = .2f;
            stateText.transform.DOScale(2, duration).OnComplete(() => stateText.transform.DOScale(1, duration));
        }

        /// <summary>
        /// Obtains the bpm written on the inputfield
        /// </summary>
        /// <returns></returns>
        private int GetBpm()
        {
            int.TryParse(bpmInputField.text, out int bpm);
            return bpm;
        }

        /// <summary>
        /// Updates the map bpm by the input field
        /// </summary>
        /// <param name="mapName"></param>
        private void UpdateBpm(string mapName)
        {
            SoundMap soundMap = GetSoundMap(mapName);

            if (soundMap != null)
            {
                soundMap.bpm = GetBpm();
                mapScroller.SetSoundMap(soundMap);
            }

            print("Updated bpm!");
        }

        /// <summary>
        /// Creates a game object holder for the map notes
        /// </summary>
        /// <param name="mapName"></param>
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
                    name = mapName,
                    bpm = beatsPerMinutes,
                    audioClip = audioSong,
                    audioPath = audioClipPath
                };
                soundMaps.Add(soundMap);
            }
            mapScroller.SetSoundMap(soundMap);
        }

        /// <summary>
        /// Stops teh map maker
        /// </summary>
        public void StopCreating()
        {
            IsCreating = false;
            CleanPreview();
            SetState(mapScroller.IsStarted ? "Playing Map" : "Not working on mapName");
            EnableButtons(true);
        }

        /// <summary>
        /// Loads all user songs
        /// </summary>
        public void LoadMapsData()
        {
#if UNITY_EDITOR && !FORCE_JSON
            IEnumerable<SoundMap> songsScriptables = Resources.LoadAll<Song>("Songs").Select(song => song.soundMap);
            soundMaps.AddRange(songsScriptables);
#else
            if (!SaveLoadManager.SaveFolderInGameDirectoryExists(SONG_FOLDER)) return;
            SoundMap[] savedSoundMaps = SaveLoadManager.LoadMultipleJsonFromFolderInGameDirectory<SoundMap>(SONG_FOLDER).ToArray();
            soundMaps.AddRange(savedSoundMaps);
#endif
            UpdateSongsList();
        }

        /// <summary>
        /// Deletes the song by the map name
        /// </summary>
        /// <param name="mapName"></param>
        public void DeleteSoundMap(string mapName)
        {
            if (IsMapNameEmpty(mapName)) return;

            ushort hashName = mapName.GetHashCodeUshort();
            SoundMap soundMap = soundMaps.Find(map => map.ID == hashName);

            //TODO: also delete file scriptable or json in each case
            if (soundMap != null)
            {
                string audioPath = soundMap.audioPath;
                if (File.Exists(audioPath)) File.Delete(audioPath);

                soundMaps.Remove(soundMap);

                SetState($"{mapName} was deleted successfully!");

                UpdateSongsList();
            }
            else
            {
                SetState($"{mapName} was not found!");
            }
        }

        /// <summary>
        /// Checks if the map name is not written on the input and puts an error
        /// </summary>
        /// <param name="mapName"></param>
        /// <returns></returns>
        private bool IsMapNameEmpty(string mapName)
        {
            if (!string.IsNullOrEmpty(mapName)) return false;
            SetState("Please, write the map name!");
            return true;
        }

        /// <summary>
        /// Loads a map by the map name
        /// </summary>
        /// <param name="mapName"></param>
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
                soundMap.GenerateNotes(mapScroller.makerNotes, m_CurrentMapGameObject.transform, false);

                SetState("Loading map clip...");

                void UpdateMakerAndMapScroller(AudioClip clip)
                {
                    soundMap.audioClip = clip;

                    audioSong = clip;

                    audioClipPath = soundMap.audioPath;

                    bpmInputField.text = soundMap.bpm + "";
                    songNameText.text = soundMap.audioClip.name;
                    songNameInputField.text = soundMap.name;

                    mapScroller.SetSoundMap(soundMap);

                    SetState($"Loaded map {mapName} successfully!");
                }

                if (!soundMap.audioClip) GetAudioClip(soundMap.audioPath, UpdateMakerAndMapScroller);
                else UpdateMakerAndMapScroller(soundMap.audioClip);
            }

            mapScroller.ResetPos();
        }

        /// <summary>
        /// Saves a map by the map name
        /// </summary>
        /// <param name="mapName"></param>
        public void SaveMap(string mapName)
        {
            if (IsMapNameEmpty(mapName)) return;

            SoundMap soundMap = GetSoundMap(mapName);

            if (soundMap != null)
            {
                mapScroller.ResetPos();

                NoteObject[] noteObjects = m_CurrentMapGameObject.GetComponentsInChildren<NoteObject>(true);
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
                soundMap.bpm = GetBpm();

                UpdateSongsList();

#if UNITY_EDITOR && !FORCE_JSON
                string soundFilePath = audioSongsRelativePath + audioSong.name;

                if (!Path.HasExtension(soundFilePath)) soundFilePath += ".mp3";
                if (!File.Exists(soundFilePath)) File.Copy(audioClipPath ?? string.Empty, soundFilePath);

                GetAudioClip(soundFilePath, clip =>
                {
                    soundMap.audioPath = soundFilePath;

                    soundMap.audioClip = clip;

                    var song = ScriptableObject.CreateInstance<Song>();
                    song.soundMap = soundMap;

                    UnityEditor.AssetDatabase.CreateAsset(song, $"{songScriptablesRelativePath}/{soundMap.name}.asset");
                });
#else
                string folderPath = Application.dataPath + $"/{SONG_FOLDER}/";
                string soundFilePath = folderPath + audioSong.name;

                if (!Path.HasExtension(soundFilePath)) soundFilePath += ".mp3";
                if (!File.Exists(soundFilePath)) File.Copy(audioClipPath, soundFilePath);

                soundMap.audioClip = audioSong;
                soundMap.audioPath = soundFilePath;

                SaveLoadManager.SaveAsJsonFile(soundMap, folderPath, $"{soundMap.name}.json");
#endif
                mapScroller.SetSoundMap(soundMap);
            }
            else StartCreating(mapName);
        }

        /// <summary>
        /// Obtains a map by the map name
        /// </summary>
        /// <param name="mapName"></param>
        /// <returns></returns>
        private SoundMap GetSoundMap(string mapName)
        {
            ushort hashName = mapName.GetHashCodeUshort();
            return soundMaps.FirstOrDefault(map => map.ID == hashName);
        }

        /// <summary>
        /// Deletes the soundmap written on the input field
        /// </summary>
        public void DeleteSoundMap() => DeleteSoundMap(songNameText.text);

        /// <summary>
        /// Loads the map written on the input field
        /// </summary>
        public void LoadMap() => LoadMap(songNameInputField.text);

        /// <summary>
        /// Saves the map written on the input field
        /// </summary>
        public void SaveMap() => SaveMap(songNameInputField.text);

        /// <summary>
        /// Starts the map maker by the map written on the input field
        /// </summary>
        public void ContinueCreating() => StartCreating(songNameInputField.text);

        /// <summary>
        /// Updates the dropdown list of created songs
        /// </summary>
        private void UpdateSongsList()
        {
            songListDropdown.ClearOptions();

            var mapNames = new List<string> { "" };

            soundMaps.ForEach(map => mapNames.Add(map.name));

            songListDropdown.AddOptions(mapNames);
        }

        /// <summary>
        /// Updates the audio clip of the map written on the input field
        /// </summary>
        private void UpdateAudioMap()
        {
            SoundMap soundMap = GetSoundMap(songNameInputField.text);

            if (soundMap != null)
            {
                soundMap.audioClip = audioSong;
                soundMap.audioPath = audioClipPath;
                mapScroller.SetSoundMap(soundMap);
            }

            print("Updated bpm!");
        }

        /// <summary>
        /// Asks for a sound file and obtains it
        /// </summary>
        public void AskForSoundFile()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            //Check mobile permissions and ask for them if needed
            FileBrowser.Permission permission = FileBrowser.CheckPermission();
            if (permission == FileBrowser.Permission.ShouldAsk) FileBrowser.AskPermissions = true;
#endif
            //Set the filters of the browser for audio file types
            FileBrowser.SetFilters(false, ".mp3", ".wav");

            //Show the browser
            FileBrowser.ShowLoadDialog(paths =>
            {
                //Check for the selected and get the audio clip
                string path = paths[0];

                if (!FileBrowserHelpers.FileExists(path)) return;

                string extension = Path.GetExtension(path);
                if (extension.Equals(".wav") || extension.Equals(".mp3"))
                {
                    GetAudioClip(path, clip =>
                    {
                        audioSong = clip;
                        audioClipPath = path;

                        songNameText.text = Path.GetFileNameWithoutExtension(path);

                        UpdateAudioMap();
                    });
                }
            }, null, FileBrowser.PickMode.Files);
        }

        /// <summary>
        /// Loads an audio clip by the path passed asynchronously
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="action"></param>
        public void GetAudioClip(string fullPath, Action<AudioClip> action) => StartCoroutine(GetAudioClipCoroutine(fullPath, action));

        /// <summary>
        /// Coroutine to get an audio clip by path asynchronously
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private IEnumerator GetAudioClipCoroutine(string fullPath, Action<AudioClip> action)
        {
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"file://{fullPath}", AudioType.MPEG);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"Error {www.error} at {fullPath}");
                SetState(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                clip.name = Path.GetFileName(fullPath);

                action?.Invoke(clip);
            }
        }
    }
}
