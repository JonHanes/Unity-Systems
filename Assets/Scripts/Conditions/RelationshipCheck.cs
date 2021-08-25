using System;
using UnityEngine;

namespace SystemExample.Conditions {

    [Serializable]
    public class RelationshipCheck : Check {

        public static int relations;
        
        public override CheckType GetCheckType() => CheckType.RelationshipCheck;

        public override bool Validate() {
            if (NeededHigher) return relations >= Required;
            else return relations <= Required;
        }

        public int Required;
        public bool NeededHigher = true;
    }
}