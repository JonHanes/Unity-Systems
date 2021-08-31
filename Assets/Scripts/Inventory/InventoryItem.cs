using UnityEngine;

namespace SystemExample.InventorySystem {
public class InventoryItem : MonoBehaviour
{
    public InventoryItem(GameObject go, int cnt) {
        itemPrefab = go;
        count = cnt;
    }

    public GameObject itemPrefab;
    public int count;
}

}
