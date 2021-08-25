using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SystemExample.Conditions;

namespace SystemExample.DialogueSystem {
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver {
        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField]
        Vector2 newNodeOffset = new Vector2(250, 0);
        [SerializeField] Condition[] conditions;
        [SerializeField] bool isRandom = true;

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

        private void OnValidate() {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes()) {
                nodeLookup[node.name] = node;
            }
        }

        public DialogueNode GetNodeByID(string id) {
             foreach (var child in nodes) {
                if (nodeLookup.ContainsKey(id)) {
                    return nodeLookup[id];
                }
            }
            return null;
        }

        public IEnumerable<DialogueNode> GetAllNodes() {
            return nodes;
        }

        public DialogueNode GetFirstValidRoot() {
            DialogueNode curr;
            for (int i = 0; i < nodes.Count; i++) {
                curr = nodes[i];

                if (!curr.HasParent(nodes)) { //Check if root node
                    if (curr.PerformChecks(true)) return curr; //If validates, return this
                }
            }

            return null;
        }

        public DialogueNode GetRootNode() {
            return nodes[0];
        }

        public bool IsRandom() => isRandom;

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode) {
            if (parentNode == null) yield break;
            foreach (var child in parentNode.GetChildren()) {
                if (nodeLookup.ContainsKey(child)) {
                    yield return nodeLookup[child];
                }
            }
        }


        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode) {
            foreach (DialogueNode node in GetAllChildren(currentNode)) {
                if (node.GetSpeaker() != SpeakerType.Player) yield return node;
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode) {
            foreach (DialogueNode node in GetAllChildren(currentNode)) {
                if (node.IsPlayerSpeaking()) yield return node;
            }
        }

        public bool IsValid() {
            if (conditions.Length < 1) return true;

            foreach (var c in conditions) {
                if (c.ValidateAll()) return true;
            }

            return false;
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent) {
            DialogueNode newNode = MakeNode(parent);
            if (parent != null) {
                Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
                Undo.RecordObject(this, "Added Dialogue Node");
            } else {
                Undo.RegisterCreatedObjectUndo(newNode, "Created Root Dialogue Node");
                Undo.RecordObject(this, "Added Root Dialogue Node");
            }
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete) {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private DialogueNode MakeNode(DialogueNode parent) {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parent != null) {
                parent.AddChild(newNode.name);
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
                newNode.SetSize(new Vector2(200, 150));
            } else {
                newNode.SetPlayerSpeaking(false);
                newNode.SetSize(new Vector2(250, 200));
            }

            return newNode;
        }

        private void AddNode(DialogueNode newNode) {
            nodes.Add(newNode);
            OnValidate();
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete) {
            foreach (DialogueNode node in GetAllNodes()) {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif

        public void OnBeforeSerialize() {
#if UNITY_EDITOR
            if (nodes.Count == 0)  {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "") {
                foreach (DialogueNode node in GetAllNodes()) {
                    if (AssetDatabase.GetAssetPath(node) == "") {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize() {
        }
    }
}
