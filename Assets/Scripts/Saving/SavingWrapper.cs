using System.Collections;
using UnityEngine;

namespace SystemExample.Saving {
    public class SavingWrapper : MonoBehaviour {
        const string defaultSaveFile = "save";

        //TODO: Add some kind of fade to cover up the process of loading
        //[SerializeField] float fadeInTime = 0.2f;

        private IEnumerator LoadLastScene() {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F5)){
                Save();
            } else if (Input.GetKeyDown(KeyCode.F9)) {
                Load();
            } else if (Input.GetKeyDown(KeyCode.Delete)) {
                Delete();
            }
        }

        public void Load() {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Save() {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Delete() {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}