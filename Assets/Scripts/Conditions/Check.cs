namespace SystemExample.Conditions {

    public enum CheckType {
        AttributeCheck,
        InventoryCheck,
        MoneyCheck,
        EquipmentCheck,
        QuestCheck,
        RelationshipCheck
    }

    public abstract class Check {
        public abstract CheckType GetCheckType();
        public abstract bool Validate();
    }
}