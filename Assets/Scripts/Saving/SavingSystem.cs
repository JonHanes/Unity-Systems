using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemExample.Saving {
    /// <summary>
    /// This component provides the interface to the saving system. It provides
    /// methods to save and restore a scene.
    ///
    /// This component should be created once and shared between all subsequent scenes.
    /// </summary>
    public class SavingSystem : MonoBehaviour {
        /// <summary>
        /// Will load the last scene that was saved and restore the state. This
        /// must be run as a coroutine.
        /// </summary>
        /// <param name="saveFile">The save file to consult for loading.</param>
        public IEnumerator LoadLastScene(string saveFile) {
            Dictionary<string, object> state = LoadFile(saveFile);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.ContainsKey("lastSceneBuildIndex")) {
                buildIndex = (int)state["lastSceneBuildIndex"];
            }
            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreState(state);
        }

        /// <summary>
        /// Save the current scene to the provided save file.
        /// </summary>
        public void Save(string saveFile) {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
            Debug.Log("Saved!");
        }

        /// <summary>
        /// Delete the state in the given save file.
        /// </summary>
        public void Delete(string saveFile) {
            Debug.Log("Deleted save!");
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        public void Load(string saveFile) {
            RestoreState(LoadFile(saveFile));
            Debug.Log("Loaded!");
        }

        // PRIVATE

        private Dictionary<string, object> LoadFile(string saveFile) {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path)) {
                return new Dictionary<string, object>();
            }
            using (FileStream stream = File.Open(path, FileMode.Open)) {
                BinaryFormatter formatter = new BinaryFormatter();
                stream.Position = 0;
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void SaveFile(string saveFile, object state) {
            string path = GetPathFromSaveFile(saveFile);
            Debug.Log("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create)) {
                Debug.Log("Attempting to serialize: " + state);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private void CaptureState(Dictionary<string, object> state) {
            var saveableEntities = FindObjectsOfType<SaveableEntity>();
            foreach (SaveableEntity saveable in saveableEntities) {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state) {
            var saveableEntities = FindObjectsOfType<SaveableEntity>();
            foreach (SaveableEntity saveable in saveableEntities) {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id)) {
                    try
                    {
                        saveable.RestoreState(state[id]);
                    }
                    catch (Exception ex)
                    {
                        //Debug.LogError($"{ex.Message}; Inner: {ex.InnerException.Message}");
                        Debug.Log($"Failed to restore state of {saveable.gameObject.name}.");
                    }
                }
            }
        }

        private string GetPathFromSaveFile(string saveFile) {
           // Debug.Log(Application.persistentDataPath);
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}