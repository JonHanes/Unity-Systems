using SystemExample.Quests;
using UnityEngine;

namespace SystemExample.Helpers
{
    public static class AssetFinder
    {
        public static Material GetMaterial(string name)
        {
            string path = $"Materials/{name}";
            Material found = (Material)Resources.Load(path, typeof(Material));
            if (found == null) Debug.LogWarning($"No material found with the name {name}!");
            return found;
        }

        public static GameObject GetModel(string type, string name)
        {
            string path = $"Models/{type}/{name}";
            GameObject found = (GameObject)Resources.Load(path, typeof(GameObject));

            if (found == null) Debug.LogWarning($"No model found with the name {name}!");
            return found;
        }

        public static Texture2D GetTexture2D(string name)
        {
            string path = $"Textures/{name}";
            Texture2D found = (Texture2D)Resources.Load(path, typeof(Texture2D));

            if (found == null) Debug.LogWarning($"No Texture2D found with the name {name}!");
            return found;
        }

        public static Quest[] GetQuests()
        {
            string path = $"Quests/";
            Quest[] quests = Resources.LoadAll<Quest>(path);
            return quests;
        }

    }
}