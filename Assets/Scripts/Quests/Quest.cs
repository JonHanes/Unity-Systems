using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SystemExample.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quests/New Quest", order = 5)]
    public class Quest : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] string questName;
        [SerializeField] int[] patrons;

        [SerializeField]
        List<QuestNode> nodes = new List<QuestNode>();
        [SerializeField]
        Vector2 newNodeOffset = new Vector2(250, 0);

        Dictionary<string, QuestNode> nodeLookup = new Dictionary<string, QuestNode>();

        string nextStep;

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (QuestNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }

        public QuestNode GetNodeByID(string id)
        {
            foreach (var child in nodes)
            {
                if (nodeLookup.ContainsKey(id))
                {
                    return nodeLookup[id];
                }
            }
            return null;
        }

        public IEnumerable<QuestNode> GetAllNodes()
        {
            return nodes;
        }

        public QuestNode GetRootNode()
        {
            return nodes[0];
        }

        public string GetQuestName() => questName;
        public int[] GetPatrons() => patrons;

        public IEnumerable<QuestNode> GetAllChildren(QuestNode parentNode)
        {
            foreach (var child in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(child))
                {
                    yield return nodeLookup[child];
                }
            }
        }

#if UNITY_EDITOR
        public void CreateNode(QuestNode parent)
        {
            QuestNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Quest Node");
            Undo.RecordObject(this, "Added Quest Node");
            AddNode(newNode);
        }

        public void DeleteNode(QuestNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Quest Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private QuestNode MakeNode(QuestNode parent)
        {
            QuestNode newNode = CreateInstance<QuestNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parent != null)
            {
                parent.AddChild(newNode.name);
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
                newNode.SetSize(new Vector2(200, 150));
            }

            return newNode;
        }

        private void AddNode(QuestNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private void CleanDanglingChildren(QuestNode nodeToDelete)
        {
            foreach (QuestNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                QuestNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (QuestNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}