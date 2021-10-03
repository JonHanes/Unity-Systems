using System.Collections.Generic;
using UnityEngine;

namespace SystemExample.Saving {
    /// <summary>
    /// Provides functionality for converting data from serializable formats
    /// </summary>
    public static class SavingHelper {

        /// <summary>
        /// Allows converting a list of serializable vectors to a list of normal vectors
        /// </summary>
        public static List<Vector3> ConvertToVector3List(List<SerializableVector3> vectors) {
            List<Vector3> list = new List<Vector3>();
            foreach (var v in vectors) list.Add(v.ToVector());
            return list;
        }
    }
}