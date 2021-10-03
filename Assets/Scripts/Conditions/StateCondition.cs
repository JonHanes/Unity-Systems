using System;
using UnityEngine;

namespace SystemExample.Conditions {

    [Serializable]
    public class StateCheck : Check {

        public static NPC_State State;
        public override CheckType GetCheckType() => CheckType.RelationshipCheck;

        public override bool Validate() {
            return (State == StateNeeded);
        }

        public NPC_State StateNeeded;
    }
}