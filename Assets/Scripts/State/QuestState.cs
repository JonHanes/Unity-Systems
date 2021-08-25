using System.Collections;
using SystemExample.Quests;
using UnityEngine;

namespace SystemExample.States {

public enum QuestStage {
    NotFound,
    Active,
    Completed
}

[System.Serializable]
public class QuestState {
    [SerializeField] Quest quest;
    [SerializeField] QuestStage stage;
    [SerializeField] QuestNode currentNode;

    static Stack QuestEvents = new Stack();

    public QuestState(Quest q, QuestStage s, QuestNode curr) {
        quest = q;
        stage = s;
        currentNode = curr;
    }

    public Quest GetQuest() => quest;
    public QuestNode GetCurrentNode() => currentNode;
    public QuestStage GetStage() => stage;
    public void Activate() => stage = QuestStage.Active;

    public void SetToStage(QuestStage s, QuestNode curr) {
        stage = s;
        currentNode = curr;
    }

    public void Revert() {
        var stage = QuestEvents.Pop();
        currentNode = (QuestNode)stage;
    }
    public void Advance() {
        //TODO: For now, assume quests are linear, they shouldn't be in the future
        var nextStr = currentNode.GetChildren()[0];
        QuestEvents.Push(currentNode);
        currentNode = quest.GetNodeByID(nextStr);
    }
    public void Finish() => stage = QuestStage.Completed;

    public (string, string) GetQuestForDisplay() => (quest.GetQuestName(), currentNode.GetText());
}

}
