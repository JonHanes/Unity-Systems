using System;

namespace SystemExample.Results {

    [Serializable]
    public class GetItemOutcome : Outcome {
        public override OutcomeType GetOutcomeType() => OutcomeType.GetItem;

        public override void Perform() {
            //TODO: Actually implement
        }
    }
}