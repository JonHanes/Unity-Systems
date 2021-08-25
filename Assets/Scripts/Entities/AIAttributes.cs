using System.Collections.Generic;
using UnityEngine;

namespace SystemExample.Entities {
public class AIAttributes : MonoBehaviour {
    int relationshipStatus = 10;
    List<int> eventIds = new List<int>();

    public int GetPlayerRelations() => relationshipStatus;

    public void Setup(int relations, List<int> events) {
        relationshipStatus = relations;
        eventIds = events;
    }

    public void ChangeRelationship(int eventId, int mod) {
        if (!eventIds.Contains(eventId)) {
            relationshipStatus += mod;
            eventIds.Add(eventId);
        }
    }

}

}
