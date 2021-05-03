using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Plugins.Tools
{
    public class SavedGameNotFoundException : Exception
    {
        public SavedGameNotFoundException(string path) : base(path) { }
    }

    /// <summary>
    /// Allows the save and load of objects in a specific folder and file.
    /// </summary>
    public static class SaveLoadManager
    {
        /// Constants
        private const string BASE_FOLDER_NAME = "/SavedData/";
        private const string DEFAULT_FOLDER_NAME = "SaveManager";
        private const string FILE_EXTENSION = ".data";

        /// <summary>
        /// There is a previous saved data
        /// </summary>
        /// <returns></returns>
        public static bool SaveExists(string saveName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = folderName.DetermineSavePath();

            return File.Exists(savePath + saveName + FILE_EXTENSION);
        }

        /// <summary>
        /// Determines the save path to use when loading and saving a file based on a folder name.
        /// </summary>
        /// <returns>The save path.</returns>
        /// <param name="folderName">Folder name.</param>
        private static string DetermineSavePath(this string folderName)
        {
#if UNITY_EDITOR
            string savePath = Application.dataPath + BASE_FOLDER_NAME;
#else
            string savePath = Application.persistentDataPath + BASE_FOLDER_NAME;
#endif
            savePath = savePath + folderName + "/";
            return savePath;
        }

        /// <summary>
        /// Save the specified saveObject, fileName and folderName into a file on disk.
        /// </summary>
        /// <param name="saveObject">Save object.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder's name.</param>
        public static void Save(object saveObject, string fileName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = folderName.DetermineSavePath();
            string saveFileName = fileName + FILE_EXTENSION;
            // if the directory doesn't already exist, we create it
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            // we serialize and write our object into a file on disk
            var formatter = new BinaryFormatter();
            FileStream saveFile = File.Create(savePath + saveFileName);
            formatter.Serialize(saveFile, saveObject);
            saveFile.Close();
        }

        /// <summary>
        /// Load the specified file based on a file name into a specified folder
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder's name.</param>
        public static T Load<T>(string fileName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = folderName.DetermineSavePath();
            string saveFileName = savePath + fileName + FILE_EXTENSION;

            T returnObject;

            // if the MMSaves directory or the save file doesn't exist, there's nothing to load, we do nothing and exit
            if (!Directory.Exists(savePath) || !File.Exists(saveFileName))
            {
                Debug.LogError("File doesn't exist" + saveFileName);
            }

            try
            {
                var formatter = new BinaryFormatter();
                FileStream saveFile = File.Open(saveFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                returnObject = (T)formatter.Deserialize(saveFile);
                saveFile.Close();
            }
            catch { throw new SavedGameNotFoundException(saveFileName); }

            return returnObject;
        }

        /// <summary>
        /// Removes a save from disk
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder name.</param>
        public static void DeleteSave(string fileName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = folderName.DetermineSavePath();
            string saveFileName = fileName + FILE_EXTENSION;
            File.Delete(savePath + saveFileName);
        }

        /// <summary>
        /// Delete save folder
        /// </summary>
        /// <param name="folderName"></param>
        public static void DeleteSaveFolder(string folderName = DEFAULT_FOLDER_NAME)
        {
#if UNITY_EDITOR
            string savePath = folderName.DetermineSavePath();
            FileUtil.DeleteFileOrDirectory(savePath);
#endif
        }
    }
}
