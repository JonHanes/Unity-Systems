using System;
using System.Collections.Generic;
using System.Linq;
using SystemExample.UI;
using SystemExample.States;
using UnityEngine;

namespace SystemExample.DialogueSystem {
    public class PlayerConversant : MonoBehaviour {

        [SerializeField] string playerName;
        Dialogue currentDialogue;
        DialogueUI dialogueUI;
        DialogueNode prevNode = null;
        DialogueNode currentNode = null;
        AIConversant currentConversant = null;
        ProgressHandler progressHandler;
        public event Action onConversationUpdated;

        private void Awake() {
            dialogueUI = FindObjectOfType<DialogueUI>(true);
            progressHandler = FindObjectOfType<ProgressHandler>();
        }
        
        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue) {
            dialogueUI.ClearHistory();
            dialogueUI.SetupAISpeaker(newConversant.GetName(), newConversant.GetSprite());

            //Upon starting dialogue, update my relations container
            if (!progressHandler.HasRelation(newConversant.GetGUID())) {
                progressHandler.AddToRelations(newConversant.GetRelations(), newConversant.GetGUID(), newConversant.GetName());
            }   
            
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            ChangeNode(currentDialogue.GetFirstValidRoot());
            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).ToArray();
            onConversationUpdated();
        }

        public void Quit() {
            currentDialogue = null;
            ChangeNode(null);
            currentConversant = null;
            onConversationUpdated();
        }

        public bool IsActive() => currentDialogue != null;
        public string GetText() => currentNode != null 
        ? prevNode != null && prevNode.HasDialogueChallenge() 
            ? currentNode.IsSuccess()
                ? $"<color=#006600>[SUCCESS]:</color> {currentNode.GetText()}" 
                : $"<color=#990000>[FAILURE]:</color> {currentNode.GetText()}"
            : currentNode.GetText() 
        : "";
        
        public Dialogue GetCurrentDialogue() => currentDialogue;
        public DialogueNode GetCurrentDialogueNode() => currentNode;

        public AIConversant GetAIConversant() => currentConversant;
        public string GetPlayerName() => playerName;
        
        public IEnumerable<DialogueNode> GetChoices() => FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));

        public void PlayerResponded(string id) {
            var origText = currentNode.GetText();
            
            string origSpeaker = currentNode.GetSpeaker() switch{
                SpeakerType.Player => "Player:",
                SpeakerType.AI => $"{currentConversant.GetName()}",
                _ => ""
            };

            ChangeNode(currentDialogue.GetNodeByID(id));
            if (currentNode == null) return;
            dialogueUI.AddToHistory(origSpeaker, origText);

            
            string newSpeaker = currentNode.GetSpeaker() switch{
                SpeakerType.Player => "Player:",
                SpeakerType.AI => $"{currentConversant.GetName()}",
                _ => ""
            };

            dialogueUI.AddToHistory(newSpeaker, currentNode.GetText());
            
            //Get first response for now
            var nodes = currentNode.GetChildren();

            if (nodes.Count == 0)
            {
                Quit();
                return;
            }
           

            //Nodes exist
            //Check stats for the roll (if any) and then return outcome
            DialogueNode nextNode = currentDialogue.GetNodeByID(nodes[0]);
            if (currentNode.HasDialogueChallenge()) {
                //If this has dialogue challenges, return the success or lose condition
                bool succ = currentNode.CheckForSuccess();

                foreach (var n in nodes) {
                    var node = currentDialogue.GetNodeByID(n);
                    if (node.IsSuccess() == succ) {
                        nextNode = node;
                        break;
                    } 
                }
            }
                
            ChangeNode(nextNode);
            onConversationUpdated();
        }

        public void Next() {
            dialogueUI.AddToHistory($"{currentConversant.GetName()}:", currentNode.GetText());
            bool hasResponses = 
                FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count() > 0;
            if (hasResponses) {
                onConversationUpdated(); 
                return;
            }

            //AI line
            var children = currentDialogue.GetAIChildren(currentNode).ToArray();
            int randomIndex = UnityEngine.Random.Range(0, children.Count());
            
            ChangeNode(children[randomIndex]);
            children = currentDialogue.GetAllChildren(currentNode).ToArray();
            onConversationUpdated();
        }

        private IEnumerable<DialogueNode> FilterOnCondition (IEnumerable<DialogueNode> inputNode) {
            foreach(var node in inputNode) {
                //On all easier difficulties, show the options even if we do not meet the requirements to show it normally
                if (node.PerformChecks(false)) yield return node;
            }
        }

        private void ChangeNode(DialogueNode node) {
            prevNode = (DialogueNode)currentNode;
            currentNode = node;
            TriggerActions();
        }

        private void TriggerActions() {
            if (currentNode == null) return;

            currentNode.GetResults().Perform();
        }

    }
}