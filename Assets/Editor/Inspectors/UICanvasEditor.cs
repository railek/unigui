using UnityEditor;
using Railek.Unibase.Editor;
using UnityEngine;

namespace Railek.Unigui.Editor
{
    [CustomEditor(typeof(UICanvas))]
    [CanEditMultipleObjects]
    public class UICanvasEditor : EditorBase
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(8);
            EditorGUILayout.HelpBox("UICanvas: parent to other UI components.", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
