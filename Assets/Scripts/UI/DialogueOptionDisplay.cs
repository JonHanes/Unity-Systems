using SystemExample.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

namespace SystemExample.UI {
public class DialogueOptionDisplay : MonoBehaviour {
    public string ID;

    public void Setup(string id, PlayerConversant conv, DialogueUI ui) {
        ID = id;

        GetComponent<Button>().onClick.AddListener(() => {
            conv.PlayerResponded(id);
        });
    }
}
}

