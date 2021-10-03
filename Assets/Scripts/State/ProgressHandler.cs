using System;
using System.Collections.Generic;
using System.Linq;
using SystemExample.Helpers;
using SystemExample.InventorySystem;
using SystemExample.Quests;
using SystemExample.Saving;
using UnityEngine;

namespace SystemExample.States {
public class ProgressHandler : MonoBehaviour, ISaveable {
    public List<QuestState> questStates = new List<QuestState>();
    public List<QuestState> activeQuests = new List<QuestState>();
    List<Relationship> relations = new List<Relationship>();
    Inventory inventory;

    public event Action questsUpdated;

    private void Awake() {
        inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
        inventory.inventoryUpdated += CheckInventoryForQuestItems;

        //Populate quest list with all the quests available
        FetchAllQuests();
    }

    public List<Relationship> GetRelations() => relations;

    public List<QuestState> GetQuests() => activeQuests;
    public List<QuestState> GetAllQuests() => questStates;

    public List<QuestState> GetQuests(int patronId) => activeQuests
        .Where(q => q.GetQuest().GetPatrons().Any(p => p == patronId)).ToList();

    public void AddToRelations(int rel, int patronId, string name) {
        Relationship relation = new Relationship(rel, patronId, name);
        relations.Add(relation);
    }
    
    public void UpdateRelations(int change, int patronId) {
        var rel = relations.Where(r => r.PatronID == patronId).FirstOrDefault();

        if (rel != null) {
            rel.Value += change;
        }
    }

    public bool HasRelation(int patronId) => 
        relations.Any(r => r.PatronID == patronId);
    

    public void ActivateQuest(Quest quest) {
        var currQuest = FindQuestState(quest);

        //If quest was already added, do not allow it to be added again.
        if (currQuest.GetStage() != QuestStage.NotFound) return;
        currQuest.Activate();
        activeQuests.Add(currQuest);

        questsUpdated();
    }

    void CheckInventoryForQuestItems() {

        //Debug.Log("Checking...");
        //TODO: Currently does not take into account that the quest should be then set back if the player loses or sells the items needed
        foreach (var q in activeQuests) {
            foreach (var r in q.GetCurrentNode().GetRequirements()) {
                if (r.Validate()) {
                    Debug.Log($"Condition passed, advancing quest!");
                    AdvanceQuest(q);
                }
            }
        }
        //Debug.Log("Done");
    }

    public void AdvanceQuest(Quest quest) {
        var currQuest = FindQuestState(quest);
        currQuest.Advance();

        questsUpdated();
    }

    public void AdvanceQuest(QuestState quest) {
        quest.Advance();
        questsUpdated();
    }

    public void FinishQuest(Quest quest) {
        var currQuest = FindQuestState(quest);
        currQuest.Finish();
        activeQuests.Remove(currQuest);

        questsUpdated();
    }


    private QuestState FindQuestState(Quest quest) {
        return questStates.First(q => q.GetQuest().GetQuestName() == quest.GetQuestName());
    }

    public object CaptureState()
    {
        QuestsSaveModel progSaveModel = new QuestsSaveModel(GetAllQuests());
        return progSaveModel;
    }

    public void RestoreState(object state) {
        FetchAllQuests();
        var prog = (QuestsSaveModel)state;

        QuestState questState;
        foreach (var q in prog.Quests) {
            questState = questStates.Where(s => s.GetQuest().GetQuestName() == q.Name)
                .FirstOrDefault();

            if (questState != null) {
                QuestNode node = questState.GetQuest().GetNodeByID(q.CurrentStep);
                questState.SetToStage(q.Stage, node);

                if (q.Stage == QuestStage.Active) activeQuests.Add(questState);
            }
        }

        questsUpdated();
    }

    void FetchAllQuests() {
        //Resets state
        activeQuests = new List<QuestState>();
        questStates = new List<QuestState>();

        foreach (var q in AssetFinder.GetQuests()) {
            questStates.Add(new QuestState(q, QuestStage.NotFound, q.GetRootNode()));
        }
    }
    }
}
