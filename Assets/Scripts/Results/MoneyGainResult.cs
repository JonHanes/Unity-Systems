using SystemExample.InventorySystem;
using UnityEngine;

namespace SystemExample.Results {

    [System.Serializable]
    public class MoneyGainResult : Outcome {

        public int Amount;

        public override OutcomeType GetOutcomeType() => OutcomeType.GetMoney;

        public override void Perform() {
            //TODO: Increase money count

            GameObject.FindWithTag("Player").GetComponent<Inventory>().AddGold(Amount);
        }
    }
}