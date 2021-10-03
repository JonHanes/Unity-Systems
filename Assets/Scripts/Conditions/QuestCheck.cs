using System;
using System.Collections.Generic;
using System.Linq;
using SystemExample.Quests;
using SystemExample.States;

namespace SystemExample.Conditions {

    [Serializable]
    public class QuestCheck : Check {

        public static List<QuestState> quests;
        public override CheckType GetCheckType() => CheckType.QuestCheck;

        public override bool Validate() {
            
            var q = quests.Where(q => q.GetQuest() == RequiredState.GetQuest()).FirstOrDefault();

            if (RequiredState.GetCurrentNode() != null) { //If filtering by specific step of the quest
                if (AllBut) return q.GetCurrentNode() != RequiredState.GetCurrentNode();
                return q.GetCurrentNode() == RequiredState.GetCurrentNode(); 
            }

            if (!AllBut) { //Default case
                switch(RequiredState.GetStage()) {
                    case QuestStage.NotFound:
                        return q.GetStage() == QuestStage.NotFound;
                    case QuestStage.Active:
                        return q.GetStage() == QuestStage.Active;
                    case QuestStage.Completed:
                        return q.GetStage() == QuestStage.Completed;
                }
            } else {
                switch(RequiredState.GetStage()) {
                    case QuestStage.NotFound:
                        return q.GetStage() != QuestStage.NotFound;
                    case QuestStage.Active:
                        return q.GetStage() != QuestStage.Active;
                    case QuestStage.Completed:
                        return q.GetStage() != QuestStage.Completed;
                }
            }
            
                        
            return false;
        }

        public QuestState RequiredState;
        public bool AllBut = false;

    }
}