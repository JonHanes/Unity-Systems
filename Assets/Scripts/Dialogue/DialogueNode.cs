using System;
using System.Collections.Generic;
using SystemExample.Results;
using SystemExample.Conditions;
using UnityEditor;
using UnityEngine;

namespace SystemExample.DialogueSystem {

    public enum DialogueNodeType {
        Default,
        Success,
        Failure
    }

    public enum SpeakerType {
        Player,
        AI,
        None
    }


    public class DialogueNode : ScriptableObject {
        [SerializeField] SpeakerType speakerType = SpeakerType.Player;
        [SerializeField][TextArea(4, 20)] string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(0, 0, 200, 100);
        [SerializeField] Result results;
        [SerializeField] Condition[] conditions;
        [SerializeField] DialogueChallenge[] dialogueChallenges;
        [SerializeField] DialogueNodeType dialogueNodeType = DialogueNodeType.Default;
        
        bool isExpanded = false;
        public Rect GetRect() => rect;
        public string GetID() => this.name;
        public string GetText() => text;
        public List<string> GetChildren() => children;
        public bool HasConditions() => conditions.Length > 0;
        public bool HasParent(IEnumerable<DialogueNode> nodes) {
            foreach (var n in nodes) {
                if (n.children.Contains(this.name)) {
                    return true;
                }
            }
            return false;
        }
        public List<InventoryCheck> GetInventoryChecks() {
            var checks = new List<InventoryCheck>();
            foreach (var c in conditions) {
                foreach (var ic in c.InventoryChecks) {
                    checks.Add(ic);
                }
            }

            return checks;
        }
        public DialogueChallenge[] GetDialogueChallenges() => dialogueChallenges;
        public bool HasDialogueChallenge() => dialogueChallenges.Length > 0;
        public bool IsPlayerSpeaking() => speakerType == SpeakerType.Player;
        public SpeakerType GetSpeaker() => speakerType;
        public DialogueNodeType GetNodeType() => dialogueNodeType;
        public bool IsSuccess() => dialogueNodeType == DialogueNodeType.Success;
        public Result GetResults() => results;

        public bool PerformChecks(bool checkAll = true) {
            //If no conditions exist, show this by default
            if (conditions.Length == 0) return true;

            //If any of the conditions work, return true
            if (checkAll) { 
                foreach (var a in conditions) if (a.ValidateAll()) return true;
            } else { 
                foreach (var a in conditions) if (a.ValidateNecessary()) return true; 
            }

            //None of the conditions were met, do not show
            return false;
        }

        public bool GetExpanded() => isExpanded;
        public void ToggleExpanded() {
            isExpanded = !isExpanded;

            Vector2 change = Vector2.up * (isExpanded ? 50 : -50);
            rect.size += change;
        }

        public bool CheckForSuccess() {
            foreach (var dc in dialogueChallenges) {
                if (!dc.GetSuccess()) return false;
            }

            return true;
        }


#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition) {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void ToggleSpeaker() {
            Undo.RecordObject(this, "Toggle Speaker");

            int max = Enum.GetNames(typeof(SpeakerType)).Length;
            int next = (int)speakerType + 1;
            if (next >= max) next = 0;

            speakerType = (SpeakerType)next;
            EditorUtility.SetDirty(this);
        }

        public void SetSize(Vector2 newSize) {
            Undo.RecordObject(this, "Change Dialogue Node Size");
            rect.size = newSize;
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText) {
            if (newText != text) {
                Undo.RecordObject(this, "Update Dialogue Text");
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void AddChild(string childID) {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID) {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

        public void SetPlayerSpeaking(bool newIsPlayerSpeaking) {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            speakerType = newIsPlayerSpeaking ? SpeakerType.Player : SpeakerType.AI;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
