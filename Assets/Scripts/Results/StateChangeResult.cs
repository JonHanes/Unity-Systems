using System;
using SystemExample.DialogueSystem;
using SystemExample.Entities;
using SystemExample.States;
using UnityEngine;

namespace SystemExample.Results {

    [Serializable]
    public class StateChangeOutcome : Outcome {
        public override OutcomeType GetOutcomeType() => OutcomeType.StateChange;
        public NPC_State State;
        public int EventId = 0;

        public override void Perform() {

            var ai = GameObject.FindObjectOfType<PlayerConversant>().GetAIConversant();
            var npc_script = ai.GetComponent<NPC>();

            if (State == npc_script.GetState()) return; //No state change

            npc_script.SetState(State); 
        }
    }
}