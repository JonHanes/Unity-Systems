using System;
using SystemExample.DialogueSystem;
using SystemExample.Entities;
using SystemExample.States;
using UnityEngine;

namespace SystemExample.Results {

    [Serializable]
    public class RelationshipModifierOutcome : Outcome {
        public override OutcomeType GetOutcomeType() => OutcomeType.RelationShipModifier;
        public int Change = 0;
        public int EventId = 0;

        public override void Perform() {
            if (Change == 0) return;

            var ai = GameObject.FindObjectOfType<PlayerConversant>().GetAIConversant(); 
            AIAttributes attributes = ai.GetAttributes();

            GameObject.FindObjectOfType<ProgressHandler>().UpdateRelations(Change, ai.GetGUID());
            attributes.ChangeRelationship(EventId, Change);
        }
    }
}