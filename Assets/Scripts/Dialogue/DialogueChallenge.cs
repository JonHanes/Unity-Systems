using System.Collections.Generic;
using System.Linq;
using SystemExample.Entities;
using UnityEngine;

namespace SystemExample.DialogueSystem {

    [System.Serializable]
    public class DialogueChallenge {
        public bool GetSuccess() {
            int perc = GetSuccessPercentage();

            float rand = UnityEngine.Random.Range(0, 100);
            float required = 100 - perc;

            return rand >= required;
        }

        public int GetSuccessPercentage() {
            PlayerAttribute[] stats = GameObject.FindObjectOfType<Player>().GetStats();

            var stat = stats.Where(s => s.Stat == StatType).FirstOrDefault().Value;
            int difference = stat - Required;
            int perc = 50 + (difference * 5);
            return perc;
        }

        public PlayerStat StatType;

        public int Required;
    }
}