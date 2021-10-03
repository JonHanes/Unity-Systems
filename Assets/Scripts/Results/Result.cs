using UnityEngine;

namespace SystemExample.Results {
[System.Serializable]
public class Result {
    public GetItemOutcome[] GetItemOutcomes;
    public QuestOutcome[] QuestOutcomes;
    public RelationshipModifierOutcome RelationshipChange;
    public StateChangeOutcome[] StateChanges;

    public bool HasOutcome() {
        return (GetItemOutcomes.Length > 0);
    }

    public bool ChangesRelations() => RelationshipChange.Change != 0;

    public void Perform() {
        //TODO: Add layers of abstraction
        foreach (var i in GetItemOutcomes) i.Perform();
        foreach (var q in QuestOutcomes) q.Perform();
        foreach (var s in StateChanges) s.Perform();
        RelationshipChange.Perform();
    } 
}

}
