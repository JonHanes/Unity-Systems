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
            
            var q = quests.Where(q => q.GetQuest() == Quest).FirstOrDefault();

            if (!AllBut) { //Default case
                switch(StageNeeded) {
                    case QuestStage.NotFound:
                        return q.GetStage() == QuestStage.NotFound;
                    case QuestStage.Active:
                        return q.GetStage() == QuestStage.Active;
                    case QuestStage.Completed:
                        return q.GetStage() == QuestStage.Completed;
                }
            } else {
                switch(StageNeeded) {
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

        public Quest Quest;

        public QuestStage StageNeeded;
        public bool AllBut = false;

    }
}