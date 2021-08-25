using System.Collections.Generic;
using SystemExample.States;
using SystemExample.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SystemExample.UI {
public class InfoPanel : MonoBehaviour {
    [SerializeField] Button button;

    [Header("High-level Containers")]
    [SerializeField] GameObject contentContainer;

    [Header("Low-level containers")]
    [SerializeField] GameObject sectionDescriptionContainer;

    [SerializeField] GameObject sectionListView;
    [SerializeField] GameObject attributesSection;
    [SerializeField] GameObject infoSection;
    [SerializeField] TextMeshProUGUI titleText;

    [Header("Prefabs")]

    [SerializeField] GameObject sectionItemBasePrefab;
    [SerializeField] GameObject sectionItemListPrefab;

    bool inSection = false;

    public void ToggleSection(bool value) {
        inSection = value;
    }

    public void ResetState() {
        inSection = false;
        
        sectionListView.SetActive(true);
        sectionDescriptionContainer.SetActive(false);

        contentContainer.SetActive(true);
        gameObject.SetActive(false);
    }

    public Transform GetSectionInfoContainer() => sectionListView.transform;

    public void HandleReturn() { //Called through button
        if (!inSection) {
            ToggleAttributes(false);

            contentContainer.SetActive(true);
            gameObject.SetActive(false);
        } else {
            sectionListView.SetActive(true);
            sectionDescriptionContainer.SetActive(false);
            ToggleSection(false);
        }
        
    }

    public void ShowAttributes(bool? val) {
        ToggleAttributes(val);
        titleText.text = "Attributes";
    }

    void ToggleAttributes(bool? val) {
        if (val != null) {
            bool value = (bool)val;
            attributesSection.SetActive(value);
            infoSection.SetActive(!value);
            return;
        }
        
        attributesSection.SetActive(!attributesSection.activeInHierarchy);
        infoSection.SetActive(!attributesSection.activeInHierarchy);
    }
    public void ShowQuests(List<QuestState> quests) {
        titleText.text = "Quests";
        const string NO_QUESTS = "No active quests";

        if (quests.Count == 0) { SetupTexts("No quests here!", NO_QUESTS); return; }
        foreach (var q in quests) {
            var data = q.GetQuestForDisplay();
            SetupTexts(data.Item1, data.Item2);
        }
    }

    public void ShowRelations(List<Relationship> relations) {
        titleText.text = "Relations";
        foreach (var r in relations) {
            SetupTexts(r.PatronName, r.Value.ToString(), false);
        }
    }

    public void SetupTexts(string text1, string text2, bool isBase = true) {

        var pf = isBase ? sectionItemBasePrefab : sectionItemListPrefab;
        var inst = Instantiate(pf, Vector3.zero, Quaternion.identity, sectionListView.transform);
        inst.GetComponentInChildren<TextMeshProUGUI>().text = text1;

        if (isBase) {
            inst.GetComponent<Button>().onClick.AddListener(() => {
            ToggleSection(true);
            sectionDescriptionContainer.SetActive(true);
            sectionListView.SetActive(false);

            var textBlocks = sectionDescriptionContainer.GetComponentsInChildren<TextMeshProUGUI>();

            textBlocks[0].text = text1;
            textBlocks[1].text = text2;
        });
        }
        
    }
}
}

