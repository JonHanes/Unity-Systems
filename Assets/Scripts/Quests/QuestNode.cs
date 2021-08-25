using System.Collections.Generic;
using SystemExample.Requirements;
using SystemExample.Results;
using UnityEditor;
using UnityEngine;

namespace SystemExample.Quests {
    public class QuestNode : ScriptableObject {
        [SerializeField][TextArea(4, 20)] string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(0, 0, 200, 100);
        [SerializeField] MoneyGainResult moneyGain;
        [SerializeField] QuestRequirement[] requirements;
        
        bool isExpanded = false;
        public Rect GetRect() => rect;
        public string GetID() => this.name;
        public string GetText() => text;
        public List<string> GetChildren() => children;
        public QuestRequirement[] GetRequirements() => requirements;
        public bool GetExpanded() => isExpanded;
        public List<Outcome> GetResults() {
            var list = new List<Outcome>();
            list.Add(moneyGain);

            return list;
        }
        public void ToggleExpanded() {
            isExpanded = !isExpanded;

            Vector2 change = Vector2.up * (isExpanded ? 50 : -50);
            rect.size += change;
        }

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition) {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void SetSize(Vector2 newSize) {
            Undo.RecordObject(this, "Change Quest Node Size");
            rect.size = newSize;
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText) {
            if (newText != text) {
                Undo.RecordObject(this, "Update Quest Text");
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void AddChild(string childID) {
            Undo.RecordObject(this, "Add Quest Link");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID) {
            Undo.RecordObject(this, "Remove Quest Link");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}