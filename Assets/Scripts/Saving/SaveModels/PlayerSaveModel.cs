using System;
using UnityEngine;

namespace SystemExample.Saving.SaveModel {

    [Serializable]
    public struct PlayerSaveModel {
        public PlayerSaveModel(Vector3 pos) {
            position = new SerializableVector3(pos);
        }
        public SerializableVector3 position;
    }
}
