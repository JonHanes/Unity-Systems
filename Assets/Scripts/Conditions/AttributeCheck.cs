using System;
using System.Linq;
using SystemExample.Entities;
using UnityEngine;

namespace SystemExample.Conditions {

    [Serializable]
    public class AttributeCheck : Check {

const string OPERATOR_TOOLTIP = @"Greater only include numbers higher than the required.
Equals only includes the required number.
Lesser only includes numbers lower than the required.";
        
        public enum OperatorType {
            Greater,
            Equals,
            Lesser
        }

        public override CheckType GetCheckType() => CheckType.AttributeCheck;

        public override bool Validate() {
            PlayerAttribute[] stats = GameObject.FindObjectOfType<Player>().GetStats();
            int stat = stats.Where(s => s.Stat == StatType).FirstOrDefault().Value;

            switch (Operator) {
                case OperatorType.Greater:
                    return stat > Required;
                case OperatorType.Lesser:
                    return stat < Required;
                case OperatorType.Equals:
                    return stat == Required;
            }
            return false;
        }

        public PlayerStat StatType;

        public float Required;

        [Tooltip(OPERATOR_TOOLTIP)]
        public OperatorType Operator;
    }
}