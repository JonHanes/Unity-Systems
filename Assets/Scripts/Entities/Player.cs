using System;
using SystemExample.Saving;
using SystemExample.Saving.SaveModel;
using UnityEngine;
using UnityEngine.AI;

namespace SystemExample.Entities {

    public enum PlayerStat {
        Strength,
        Fortitude,
        Intelligence,
        Wit,
        Charisma
    }

    [Serializable]
    public class PlayerAttribute {
        public PlayerStat Stat;
        public int Value;
    }

    public class Player : MonoBehaviour, ISaveable {
        public PlayerAttribute[] stats;
        public PlayerAttribute[] GetStats() => stats;


        NavMeshAgent navMeshAgent;
        private void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        public object CaptureState() => new PlayerSaveModel(transform.position);

        public void RestoreState(object state) {
            var model = (PlayerSaveModel)state;
            navMeshAgent.enabled = false;
            transform.position = model.position.ToVector();
            navMeshAgent.enabled = true;
        }
    }

}
