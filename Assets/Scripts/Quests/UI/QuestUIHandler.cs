using SystemExample.Saving;
using SystemExample.States;
using TMPro;
using UnityEngine;

namespace SystemExample.Quests.UI {
public class QuestUIHandler : MonoBehaviour {

    [SerializeField] ProgressHandler progressHandler;
    [SerializeField] Transform UI_Container;
    [SerializeField] GameObject taskPrefab;

    private void Awake() {
        progressHandler.questsUpdated += UpdateQuestUI;
    }

    public void UpdateQuestUI() {
        ClearUI();
        var quests = progressHandler.GetQuests();

        foreach (var quest in quests) {
            Debug.Log("Quest data");
            var inst = Instantiate(taskPrefab, Vector3.zero, Quaternion.identity, UI_Container);
            var textBlocks = inst.GetComponentsInChildren<TextMeshProUGUI>();

            var data = quest.GetQuestForDisplay();
            textBlocks[0].text = data.Item1;
            textBlocks[1].text = data.Item2;
        }
    }

    public void ClearUI() {
        foreach (Transform child in UI_Container) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void ToggleDisplay() {
        GameObject parent = UI_Container.transform.parent.gameObject;
        bool activity = parent.activeInHierarchy;
        parent.gameObject.SetActive(!activity);
    }
}

}
