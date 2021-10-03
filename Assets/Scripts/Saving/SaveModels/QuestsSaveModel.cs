using System;
using System.Collections.Generic;
using SystemExample.States;

    [Serializable]
    public struct QuestSaveModel {
        public QuestSaveModel(string name, string step, QuestStage stage) {
            Name = name;
            CurrentStep = step;
            Stage = stage;
        }
        public string Name;
        public string CurrentStep;
        public QuestStage Stage;
    }

    [Serializable]
    public struct QuestsSaveModel {
    public QuestsSaveModel(List<QuestState> quests) {
        Quests = new List<QuestSaveModel>();

        foreach (var q in quests) {
            Quests.Add(new QuestSaveModel(q.GetQuest().GetQuestName(), q.GetCurrentNode().GetID(), q.GetStage()));
        }
    }
    public List<QuestSaveModel> Quests;

}
