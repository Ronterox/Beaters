using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// There is a previous saved data
        /// </summary>
        /// <returns></returns>
        public static bool SaveExists(string saveName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = folderName.DetermineSavePath();

            return File.Exists(savePath + saveName);
        }

        /// <summary>
        /// There is a folder for the save
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static bool SaveFolderExists(string folderName = DEFAULT_FOLDER_NAME) => Directory.Exists(folderName.DetermineSavePath());

        /// <summary>
        /// There is a folder for the save
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static bool SaveFolderInGameDirectoryExists(string folderName = DEFAULT_FOLDER_NAME)
        {
            string folderPath = Application.dataPath + $"/{folderName}/";
            return Directory.Exists(folderPath);
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
            return savePath + folderName + "/";
        }

        /// <summary>
        /// Saves the passed object as a binary file
        /// </summary>
        /// <param name="saveObject"></param>
        /// <param name="savePath"></param>
        /// <param name="fileName"></param>
        public static void SaveBinary(object saveObject, string savePath, string fileName)
        {
            // if the directory doesn't already exist, we create it
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            // we serialize and write our object into a file on disk
            var formatter = new BinaryFormatter();
            FileStream saveFile = File.Create(savePath + fileName);
            formatter.Serialize(saveFile, saveObject);
            saveFile.Close();
        }

        /// <summary>
        /// Deserializes the selected binary file from the select path
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="fileName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadBinary<T>(string savePath, string fileName)
        {
            string saveFileName = savePath + fileName;

            if (!Directory.Exists(savePath)) throw new SavedGameNotFoundException(saveFileName);

            return LoadBinary<T>(saveFileName);
        }

        public static T LoadBinary<T>(string fullFilePath)
        {
            if (!File.Exists(fullFilePath)) throw new SavedGameNotFoundException(fullFilePath);
            T returnObject;
            try
            {
                var formatter = new BinaryFormatter();
                FileStream saveFile = File.Open(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                returnObject = (T)formatter.Deserialize(saveFile);
                saveFile.Close();
            }
            catch (Exception exception) { throw new Exception($"Error: {exception.Message}. \nWhile deserializing {fullFilePath}"); }

            return returnObject;
        }

        /// <summary>
        /// Loads a json file
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="fileName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadJsonFile<T>(string savePath, string fileName)
        {
            string saveFileName = savePath + fileName;
            if (!Directory.Exists(savePath) || !File.Exists(saveFileName)) throw new SavedGameNotFoundException(saveFileName);
            string json = File.ReadAllText(saveFileName);
            return JsonUtility.FromJson<T>(json);
        }

        /// <summary>
        /// Saves object as json file
        /// </summary>
        /// <param name="saveObject"></param>
        /// <param name="savePath"></param>
        /// <param name="fileName"></param>
        public static void SaveAsJsonFile(object saveObject, string savePath, string fileName)
        {
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            string json = JsonUtility.ToJson(saveObject);
            File.WriteAllText(savePath + fileName, json);
        }

        /// <summary>
        /// Loads all json files on a folder
        /// </summary>
        /// <param name="saveFolderPath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> LoadJsonsFromFolder<T>(string saveFolderPath)
        {
            if (!Directory.Exists(saveFolderPath)) throw new SavedGameNotFoundException(saveFolderPath);
            string[] filePaths = Directory.GetFiles(saveFolderPath);

            var list = new List<T>();

            foreach (string filePath in filePaths)
            {
                if (!Path.GetExtension(filePath).Equals(".json")) continue;
                try
                {
                    string json = File.ReadAllText(filePath);
                    list.Add(JsonUtility.FromJson<T>(json));
                }
                catch
                {
                    Debug.LogError("(SaveLoadManager) Error while parsing json: " + filePath);
                }
            }
            return list;
        }

        /// <summary>
        /// Save the specified saveObject, fileName and folderName into a file on disk.
        /// </summary>
        /// <param name="saveObject">Save object.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder's name.</param>
        public static void Save(object saveObject, string fileName, string folderName = DEFAULT_FOLDER_NAME) => SaveBinary(saveObject, folderName.DetermineSavePath(), fileName);

        /// <summary>
        /// Save the specified saveObject, fileName and folderName into a file on disk.
        /// </summary>
        /// <param name="saveObject">Save object.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder's name.</param>
        public static void SaveInGameFolder(object saveObject, string fileName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = Application.dataPath + $"/{folderName}/";
            SaveBinary(saveObject, savePath, fileName);
        }

        /// <summary>
        /// Load the specified file based on a file name into a specified folder
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder's name.</param>
        public static T Load<T>(string fileName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = folderName.DetermineSavePath();
            return LoadBinary<T>(savePath, fileName);
        }

        /// <summary>
        /// Load the specified file based on a file name into a specified folder
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder's name.</param>
        public static T LoadFromGameFolder<T>(string fileName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = Application.dataPath + $"/{folderName}/";
            return LoadBinary<T>(savePath, fileName);
        }

        /// <summary>
        /// Save the specified saveObject, fileName and folderName into a file on disk.
        /// </summary>
        /// <param name="saveObject">Save object.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder's name.</param>
        public static void SaveAsJson(object saveObject, string fileName, string folderName = DEFAULT_FOLDER_NAME) => SaveAsJsonFile(saveObject, folderName.DetermineSavePath(), fileName);

        /// <summary>
        /// Save the specified saveObject, fileName and folderName into a file on disk.
        /// </summary>
        /// <param name="saveObject">Save object.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder's name.</param>
        public static void SaveInGameDirectoryFolderAsJson(object saveObject, string fileName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = Application.dataPath + $"/{folderName}/";
            SaveAsJsonFile(saveObject, savePath, fileName);
        }

        /// <summary>
        /// Load the specified file based on a file name into a specified folder
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder's name.</param>
        public static T LoadJson<T>(string fileName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = folderName.DetermineSavePath();
            return LoadJsonFile<T>(savePath, fileName);
        }

        /// <summary>
        /// Load the specified file based on a file name into a specified folder
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder's name.</param>
        public static T LoadJsonFromGameFolder<T>(string fileName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = Application.dataPath + $"/{folderName}/";
            return LoadJsonFile<T>(savePath, fileName);
        }

        /// <summary>
        /// Load the specified file based on a file name into a specified folder
        /// </summary>
        /// <param name="folderName">Folder's name.</param>
        public static IEnumerable<T> LoadMultipleJson<T>(string folderName = DEFAULT_FOLDER_NAME) => LoadJsonsFromFolder<T>(folderName.DetermineSavePath());

        /// <summary>
        /// Load the specified file based on a file name into a specified folder
        /// </summary>
        /// <param name="folderName">Folder's name.</param>
        public static IEnumerable<T> LoadMultipleJsonFromFolderInGameDirectory<T>(string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = Application.dataPath + $"/{folderName}/";
            return LoadJsonsFromFolder<T>(savePath);
        }

        /// <summary>
        /// Removes a save from disk
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder name.</param>
        public static void DeleteSave(string fileName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string filePath = folderName.DetermineSavePath() + fileName;
            File.Delete(filePath);
        }

        public static void DeleteSaveInGameFolder(string fileName, string folderName = DEFAULT_FOLDER_NAME)
        {
            string filePath = Application.dataPath + $"/{folderName}/" + fileName;
            File.Delete(filePath);
        }

        /// <summary>
        /// Deletes save folder
        /// </summary>
        /// <param name="folderName"></param>
        public static void DeleteSaveFolder(string folderName = DEFAULT_FOLDER_NAME)
        {
#if UNITY_EDITOR
            string savePath = folderName.DetermineSavePath();
            FileUtil.DeleteFileOrDirectory(savePath);
#else
            Directory.Delete(folderName.DetermineSavePath(), true);
#endif
        }

        /// <summary>
        /// Deletes save folder
        /// </summary>
        /// <param name="folderName"></param>
        public static void DeleteSaveFolderInGameFolder(string folderName = DEFAULT_FOLDER_NAME)
        {
            string savePath = Application.dataPath + $"/{folderName}/";
            Directory.Delete(savePath, true);
        }
    }
}
