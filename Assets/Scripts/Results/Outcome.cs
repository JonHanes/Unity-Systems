namespace SystemExample.Results {

    public enum OutcomeType {
        GetItem,
        QuestOutcome,
        GetMoney,
        RelationShipModifier,
        TimeSkip,
        Teleport,
        LoseItem,
        LoseMoney
    }

    public abstract class Outcome {
        public abstract OutcomeType GetOutcomeType();
        public abstract void Perform();
    }
}