using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SystemExample.DialogueSystem.Editor {
    using SystemExample.Helpers;
#if UNITY_EDITOR

    using UnityEditor.Callbacks;
    public class DialogueEditor : EditorWindow {
        Dialogue selectedDialogue = null;
        #region Node styles
        [NonSerialized]
        GUIStyle defaultAINodeStyle;
        [NonSerialized]
        GUIStyle playerNodeStyle;
        [NonSerialized]
        GUIStyle descriptiveNodeStyle;
        [NonSerialized]
        GUIStyle successNodeStyle;
        [NonSerialized]
        GUIStyle failureNodeStyle;
        #endregion
        [NonSerialized]
        DialogueNode currentNode = null;
        [NonSerialized]
        EditorEvent currentEvent = EditorEvent.None;
        [NonSerialized]
        float timeSinceLastUpdated = 0f;

        [NonSerialized]
        Vector2 origPos = Vector2.zero;

        [NonSerialized]
        Vector2 draggingOffset;
        [NonSerialized]
        DialogueNode linkingParentNode = null;
        Vector2 scrollPosition;
        [NonSerialized]
        bool draggingCanvas = false;
        [NonSerialized]
        Vector2 draggingCanvasOffset;

        const float canvasSize = 4000;
        const float backgroundSize = 50;

        float zoomScale = 1f, prevScale = 1f;
        Vector2 prevMousePos;
        bool isZoomed = false;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line) {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null) {
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        private void OnEnable() {
            Selection.selectionChanged += OnSelectionChanged;

            defaultAINodeStyle = SetupStyle("node0", Color.white);
            playerNodeStyle = SetupStyle("node1", Color.white);
            descriptiveNodeStyle = SetupStyle("node1", Color.white, "PurpleTex");
            successNodeStyle = SetupStyle("node0", Color.white, "GreenTex");
            failureNodeStyle = SetupStyle("node0", Color.white, "RedTex");
        }

        private void OnSelectionChanged() {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null) {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI() {
            if (selectedDialogue == null) {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;

                float sizeScaled = backgroundSize / zoomScale;
                Rect texCoords = new Rect(0, 0, canvasSize / sizeScaled, canvasSize / sizeScaled);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes()){
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes()) {
                    DrawNode(node);
                }

                if (zoomScale != 1) DrawZoomResetButton();
                EditorGUILayout.EndScrollView();

                if (currentEvent == EditorEvent.IsCreating) {
                    selectedDialogue.CreateNode(currentNode);
                    ResetEvent();
                }
                if (currentEvent == EditorEvent.IsDeleting && currentNode != null) {
                    selectedDialogue.DeleteNode(currentNode);
                    ResetEvent();
                }
            }
        }

        private void ProcessEvents() {
            var mousePosScaled = Event.current.mousePosition * zoomScale;

            if (Event.current.type == EventType.MouseDown && currentNode == null) {

                currentNode = GetNodeAtPoint(mousePosScaled + scrollPosition);
                if (currentNode != null) {
                    //Check if at corners, if so, scale the rect instead of dragging it
                    Rect rect = currentNode.GetRect();
                    
                    var xDiffRight = Mathf.Abs(mousePosScaled.x - (rect.position.x + rect.size.x));
                    var xDiffLeft = Mathf.Abs(mousePosScaled.x - rect.position.x);

                    if (xDiffRight < 15 || xDiffLeft < 15) {
                        var yDiffTop = Mathf.Abs(rect.position.y - mousePosScaled.y);
                        var yDiffBottom = Mathf.Abs(mousePosScaled.y - (rect.position.y + rect.size.y));

                        if (yDiffTop < 15 || yDiffBottom < 15) {
                            currentEvent = EditorEvent.IsScaling;
                            origPos = currentNode.GetRect().position;
                            return;
                        }
                    } else {
                        currentEvent = EditorEvent.IsDragging;
                    }

                    draggingOffset = rect.position - mousePosScaled;
                    Selection.activeObject = currentNode;
                }
                else {
                    draggingCanvas = true;
                    draggingCanvasOffset = mousePosScaled + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }

            //Dragging events

            else if (Event.current.type == EventType.MouseDrag) {
                //If scaling
                if (currentEvent == EditorEvent.IsScaling) {
                    timeSinceLastUpdated += Time.deltaTime;
                    if (timeSinceLastUpdated > 1.5f) {
                        timeSinceLastUpdated = 0f;
                        var xSize = mousePosScaled.x - origPos.x;
                        var ySize = mousePosScaled.y - origPos.y;

                        currentNode.SetSize(new Vector2(mousePosScaled.x - origPos.x,  mousePosScaled.y - origPos.y));
                    }  
                } else if (currentEvent == EditorEvent.IsDragging) {
                    currentNode.SetPosition(mousePosScaled + draggingOffset);
                    GUI.changed = true;
                } else if (draggingCanvas) {
                    scrollPosition = draggingCanvasOffset - mousePosScaled;
                    GUI.changed = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp) {
                if (currentEvent == EditorEvent.IsScaling) {
                    currentNode.SetSize(new Vector2(mousePosScaled.x - origPos.x,  mousePosScaled.y - origPos.y));
                } else if (draggingCanvas) {
                    draggingCanvas = false;
                }
                ResetEvent();
            } else if (Event.current.type == EventType.KeyDown) { //Input events
                if (Event.current.keyCode == KeyCode.R) zoomScale = 1f;

                //Handle zoom
                if (Event.current.keyCode == KeyCode.LeftControl) {
                    //If left control pressed
                    if (!isZoomed) {
                        prevScale = (float)zoomScale;
                        prevMousePos = Event.current.mousePosition;
                    }
                    isZoomed = true;
                    float diff = Event.current.mousePosition.y - prevMousePos.y;

                    if (diff != 0) {
                        zoomScale = prevScale - diff / 500;
                        Repaint();
                    }  
                }

            } else if (Event.current.type == EventType.KeyUp) {
                if (Event.current.keyCode == KeyCode.LeftControl) isZoomed = false;
            }
            
            
        }

        private void DrawNode(DialogueNode node) {
            GUIStyle style = defaultAINodeStyle;

            switch (node.GetSpeaker()) {
                case SpeakerType.Player:
                    style = playerNodeStyle;
                    break;
                case SpeakerType.AI:
                    if (node.GetNodeType() != DialogueNodeType.Default) {
                        style = node.GetNodeType() == DialogueNodeType.Success ? successNodeStyle : failureNodeStyle;
                    }
                    break;
                case SpeakerType.None:
                    style = descriptiveNodeStyle; 
                    break;
            }

            var rect = node.GetRect();
            rect.size = new Vector2(rect.size.x / zoomScale, rect.size.y / zoomScale);
            rect.position = new Vector2(rect.position.x / zoomScale, rect.position.y / zoomScale);

            GUILayout.BeginArea(rect, style);
            EditorGUI.BeginChangeCheck();

            SetupTextArea(node);

            if (zoomScale < 1.5f) { //Do not show buttons when zoomed out.
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("x")) {
                    currentEvent = EditorEvent.IsDeleting;
                    currentNode = node;
                }
                if (!node.HasParent(selectedDialogue.GetAllNodes())) {
                    if (GUILayout.Button("Root")) {
                        currentNode = null;
                        currentEvent = EditorEvent.IsCreating;
                    }
                }
                
                DrawLinkButtons(node);
                if (GUILayout.Button("speaker")) {
                    node.ToggleSpeaker();
                }
                if (GUILayout.Button("+")) {
                    currentEvent = EditorEvent.IsCreating;
                    currentNode = node;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(">>")) {
                node.ToggleExpanded();
            }
            GUILayout.EndHorizontal();

            if (node.GetExpanded()) {
                SetupNodeDetails(node);
            }
            
            GUILayout.EndArea();
        }

        private void DrawZoomResetButton() {
            //TODO: Fix positioning
            string resetButton = "Reset Zoom";
            if (GUILayout.Button(resetButton)){
                zoomScale = 1f;
            }
        }

        private void DrawLinkButtons(DialogueNode node) {
            if (linkingParentNode == null) {
                if (GUILayout.Button("link")) {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node) {
                if (GUILayout.Button("cancel")) {
                    linkingParentNode = null;
                }
            } else {
                List<string> children = linkingParentNode.GetChildren();
                
                bool found = false;

                foreach (var c in children) {
                    if (c == node.name) found = true; break;
                }

                if (found) {
                    if (GUILayout.Button("unlink")) {
                        linkingParentNode.RemoveChild(node.name);
                        linkingParentNode = null;
                    }
                }
                else {
                    if (GUILayout.Button("child")) {
                        Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                        linkingParentNode.AddChild(node.name);
                        linkingParentNode = null;
                    }
                }
            }
            
        }

        private void DrawConnections(DialogueNode node) {
            Vector3 startPosition = new Vector2(node.GetRect().xMax / zoomScale, node.GetRect().center.y / zoomScale);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node)) {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin / zoomScale, childNode.GetRect().center.y / zoomScale);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;
                Handles.DrawBezier(
                    startPosition, endPosition, 
                    startPosition + controlPointOffset, 
                    endPosition - controlPointOffset, 
                    Color.white, null, 4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point) {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes()) {
                if (node.GetRect().Contains(point)) {
                    foundNode = node;
                }
            }
            return foundNode;
        }

        void ResetEvent() {
            currentNode = null;
            currentEvent = EditorEvent.None;
        }

        void SetupTextArea(DialogueNode node) {
            int charCount = 0; //For checking how big the text field should be in relation to the character count
            if (node.GetText() != null) { //Upon creating, the text field will be null
                charCount = node.GetText().Length;
            }

            float textAreaHeight = Mathf.Min(Mathf.Max(charCount, 20), 300);

            GUIStyle style = (GUIStyle)EditorStyles.textArea;
            style.wordWrap = true;
            style.fixedHeight = textAreaHeight;
            node.SetText(EditorGUILayout.TextArea(node.GetText(), style));
        }

        void SetupNodeDetails(DialogueNode node) {

            Rect rect = node.GetRect();
            int offsetX = (int)rect.size.x / 4;
            var res = node.GetResults();

            GUIStyle myStyle = new GUIStyle (GUI.skin.label); 
            myStyle.margin=new RectOffset(offsetX, offsetX, 0, 0);
            GUILayout.BeginHorizontal();
            if (res.ChangesRelations()) EditorGUILayout.TextField("Has relationship modifier!");

            GUILayout.EndHorizontal();

            if (node.HasConditions()) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.TextField("Has conditions!");
                GUILayout.EndHorizontal();
            }

            if (res.HasOutcome()) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.TextField("Has outcomes!");
                GUILayout.EndHorizontal();
            }
            
        }

        GUIStyle SetupStyle(string background, Color textColor) {
            var style = new GUIStyle();
            style.normal.background = EditorGUIUtility.Load(background) as Texture2D;
            style.normal.textColor = textColor;
            style.padding = new RectOffset(20, 20, 20, 20);
            style.border = new RectOffset(12, 12, 12, 12);

            return style;
        }

        GUIStyle SetupStyle(string background, Color textColor, string backgroundTex) {
            var style = SetupStyle(background, textColor);
            var tex = AssetFinder.GetTexture2D(backgroundTex);

            if (tex != null) style.normal.background = tex;
            return style;
        }
    }
    #endif
}