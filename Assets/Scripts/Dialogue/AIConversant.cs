using System.Collections.Generic;
using SystemExample.Entities;
using UnityEngine;

namespace SystemExample.DialogueSystem {

[RequireComponent(typeof(AIAttributes))]
public class AIConversant : MonoBehaviour {
    [SerializeField] string conversantName;
    [SerializeField] Sprite conversantImage;
    [SerializeField] Dialogue[] dialogues = null;
    [SerializeField] int myGUID;

    public int GetGUID() => myGUID;
    public string GetName() => conversantName;
    public Sprite GetSprite() => conversantImage;
    public AIAttributes GetAttributes() => GetComponent<AIAttributes>();
    public int GetRelations() => GetAttributes().GetPlayerRelations();
    
    public void StartConversation() {
        
        var validDialogues = FilterDialogues(dialogues);
        if (validDialogues.Count < 1) return;

        //Get first dialogue node if there is one that is pre-determined to take priority

        Dialogue currDialog = null;
        for (int i = 0; i < validDialogues.Count; i++) {
            if (!validDialogues[i].IsRandom()) {
                currDialog = validDialogues[i];
                break;
            }
        }
        
        if (currDialog == null) { //If there are no important dialogues, choose a random one
            currDialog = validDialogues[Random.Range(0, validDialogues.Count)];
        }
        
        FindObjectOfType<PlayerConversant>().StartDialogue(this, currDialog);
    }

    public List<Dialogue> FilterDialogues(Dialogue[] orig) {
        var list = new List<Dialogue>();

        foreach (var o in orig) {
            if (o.IsValid()) list.Add(o);
        }

        return list;
    }

}


}
