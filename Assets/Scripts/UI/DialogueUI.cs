using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SystemExample.DialogueSystem;
using SystemExample.States;
using System;
using System.Text;
using System.Collections.Generic;
using SystemExample.Helpers;

namespace SystemExample.UI {
    public class DialogueUI : MonoBehaviour {
       
        [Header("Main")]
        [SerializeField] GameObject dialogueUI;
        [SerializeField] TextMeshProUGUI textBlock;

        [SerializeField] Transform historyContainer;

        [SerializeField] Transform choiceRoot;

        [Header("Prefabs")]
        [SerializeField] GameObject optionPrefab;

        [SerializeField] GameObject historyPrefab;

        [Header("Buttons")]

        [SerializeField] Button nextButton;
        [SerializeField] Button quitButton;

        [Header("Sidebars")]
        [SerializeField] InfoPanel infoPanelPlayer;
        [SerializeField] InfoPanel infoPanelAI;
        [SerializeField] TextMeshProUGUI relationshipInfo;

        [Header("Entities")]
        [SerializeField] TextMeshProUGUI AIName;
        [SerializeField] Image AIImage;
        [SerializeField] Animator AIAnimator;
        [SerializeField] Animator playerAnimator;

        
        PlayerConversant playerConversant;
        ProgressHandler progHandler;
        int aiConversantGUID;
        Dialogue dialogue;

        private void Awake() {
            progHandler = FindObjectOfType<ProgressHandler>();

            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConversationUpdated += UpdateUI;

            AIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        private void Start() {
            nextButton.onClick.AddListener(() => playerConversant.Next());
            quitButton.onClick.AddListener(() => {
                infoPanelPlayer.ResetState();
                infoPanelAI.ResetState();
                playerConversant.Quit();
            });
            UpdateUI();
        }

        public void SetupAISpeaker(string name, Sprite spr) {
            AIName.text = name;
            AIImage.sprite = spr;
        }

        private void UpdateUI() {
            dialogueUI.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive()) return;

            var aiConversant = playerConversant.GetAIConversant();
            var relations = aiConversant.GetAttributes().GetPlayerRelations().ToString();
            relationshipInfo.text = relations;
            
            aiConversantGUID = aiConversant.GetGUID();
            //TODO: Light up the sprite of the person speaking?
            ClearChoices();
            textBlock.text = playerConversant.GetText();
            
            //Populate options list
            if (playerConversant.GetCurrentDialogueNode() == null) return;

            List<string> options = playerConversant.GetCurrentDialogueNode().GetChildren();
            nextButton.gameObject.SetActive(false);

            if (options.Count <= 0) return;

            //Check if the first option is spoken by the player or the AI

            dialogue = playerConversant.GetCurrentDialogue();
            var firstLine = dialogue.GetNodeByID(options[0]);
            if (firstLine.IsPlayerSpeaking()) {
                playerAnimator.SetBool("isActive", true);
                AIAnimator.SetBool("isActive", false);
                int index = 1;

                foreach (var o in playerConversant.GetChoices()) {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"{index}: ");

                    bool shouldBeDisabled = false;
                    bool shouldBeHidden = false;
                    //Check if this has any dialogue checks and display if so
                    if (o.HasDialogueChallenge()) {
                        foreach (var c in o.GetDialogueChallenges()) {
                            var succPerc = c.GetSuccessPercentage();

                            if (succPerc <= 0) {
                                shouldBeDisabled = true;
                            }

                            var color = ColorMapper.GetColorBasedOnPerc(succPerc);
                            sb.Append($"<color={color}>[{c.StatType.ToString()}");


                            sb.Append($"]</color> ");
                        }
                    }

                    var inventoryChecks = o.GetInventoryChecks();
                    if (inventoryChecks.Count > 0) {
                        foreach (var ic in inventoryChecks) {
                            bool contains = ic.CheckIfContains();
                            string col = ColorMapper.GetInventoryCheckColor(!ic.IsNeeded);

                            sb.Append($"<color={col}>[{ic.ItemName}]</color> ");
                            if (!contains) shouldBeDisabled = true;
                        }
                    }

                    //Do not display options that are beyond our abilities on higher difficulties
                    if (shouldBeHidden) continue;

                    var inst = Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, choiceRoot);
                    inst.GetComponent<DialogueOptionDisplay>().Setup(o.GetID(), playerConversant, this);
                    
                    if (shouldBeDisabled) {

                        string grey = "#a6a6a6";
                        sb.Append($"<color={grey}>{o.GetText()}</color>");
                        inst.GetComponent<Button>().enabled = false;
                    } else {
                        sb.Append($"{o.GetText()}");
                    }

                    inst.GetComponent<TextMeshProUGUI>().text = sb.ToString();
                    index++;
                }
            } else { //AI continues speaking
                AIAnimator.SetBool("isActive", true);
                playerAnimator.SetBool("isActive", false);
                nextButton.gameObject.SetActive(true);
            }
        
        }

        public void SetupSection(string title, bool isPlayer) {
            var infoPanel = isPlayer ? infoPanelPlayer : infoPanelAI;
            ClearSectionContent(infoPanel);
            //Set content
            var sectionType = (SectionType)Enum.Parse(typeof(SectionType), title);

            switch (sectionType) {
                case SectionType.Attributes:
                    infoPanel.ShowAttributes(true);
                    break;
                case SectionType.Quests:
                    var quests = isPlayer ? progHandler.GetQuests() : progHandler.GetQuests(aiConversantGUID);
                    infoPanel.ShowQuests(quests);
                    break;
                case SectionType.Preferences:

                    break;
                case SectionType.Relationship:
                    var relations = progHandler.GetRelations();
                    infoPanel.ShowRelations(relations);
                    break;
            }
        }

        public void AddToHistory(string speaker, string text) {
            var inst = Instantiate(historyPrefab, Vector3.zero, Quaternion.identity, historyContainer);

            var textComponents = inst.GetComponentsInChildren<TextMeshProUGUI>();
            textComponents[0].text = speaker;
            textComponents[1].text = text;
        }

        public void ClearHistory() {
            foreach (Transform item in historyContainer) {
                Destroy(item.gameObject);
            }
        }

        public void ClearChoices() {
            foreach (Transform item in choiceRoot) {
                Destroy(item.gameObject);
            }
        }

        public void ClearSectionContent(InfoPanel infoPanel) {
            foreach (Transform item in infoPanel.GetSectionInfoContainer()) {
                Destroy(item.gameObject);
            }
        }
    }
}