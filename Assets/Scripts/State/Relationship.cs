namespace SystemExample.States {

[System.Serializable]
public class Relationship {

    public Relationship(int value, int patronID, string name) {
        Value = value;
        PatronID = patronID;
        PatronName = name;
    }
    public int Value;
    public int PatronID;
    public string PatronName;
}
}