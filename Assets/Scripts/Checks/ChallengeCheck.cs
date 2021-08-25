using System;
using System.Collections.Generic;
using System.Linq;
using SystemExample.Entities;
using UnityEngine;

namespace SystemExample.Checks {
    [Serializable]
    public class ChallengeCheck {
        public bool GetSuccess() {
            int perc = GetSuccessPercentage();
            Debug.Log("Success percentage: " + perc);

            float rand = UnityEngine.Random.Range(0, 100);
            float required = 100 - perc;

            Debug.Log($"Success rata: {perc} and random index {rand}, needs to be above required {required}");
            return rand >= required;
        }

        public int GetSuccessPercentage() {
            PlayerAttribute[] stats = GameObject.FindObjectOfType<Player>().GetStats();

            var stat = stats.Where(s => s.Stat == StatType).FirstOrDefault();
            int difference = stat.Value - Required;
            int perc = 50 + (difference * 5);
            return perc;
        }

        public PlayerStat StatType;

        public int Required;
    }
}