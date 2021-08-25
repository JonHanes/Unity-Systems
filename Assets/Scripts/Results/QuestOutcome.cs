using System;
using SystemExample.States;
using SystemExample.Quests;
using UnityEngine;

namespace SystemExample.Results {

    [Serializable]

    public enum QuestAction {
        Start,
        Advance,
        Finish
    }

    [Serializable]
    public class QuestOutcome : Outcome {

        public Quest Quest;
        public QuestAction action;
        public override OutcomeType GetOutcomeType() => OutcomeType.QuestOutcome;

        public override void Perform() {
            ProgressHandler progHandler = GameObject.FindObjectOfType<ProgressHandler>();

            switch(action) {
                case QuestAction.Start:
                    progHandler.ActivateQuest(Quest);
                    break;
                case QuestAction.Advance:
                    progHandler.AdvanceQuest(Quest);
                    break;
                case QuestAction.Finish:
                    progHandler.FinishQuest(Quest);
                    break;
            }
            
        }
    }
}