using UnityEngine;

namespace SystemExample.Entities {

    public enum PlayerStat {
        Strength,
        Fortitude,
        Intelligence,
        Wit,
        Charisma
    }

    public class PlayerAttribute {
        public PlayerStat Stat;
        public int Value;
    }

    public class Player : MonoBehaviour
    {
        public PlayerAttribute[] stats;

        public PlayerAttribute[] GetStats() => stats;
    }

}
