using System.Collections.Generic;
using Railek.Unibase.Editor;
using UnityEditor;
using UnityEngine;

namespace Railek.Unigui.Editor.Drawers
{
    public class BaseDrawer : PropertyDrawer
    {
        private bool _databasesInitialized;

        private void InitDatabases()
        {
            if (_databasesInitialized) return;
            Properties = new PropertyRelative();
            _databasesInitialized = true;
        }

        protected readonly Dictionary<string, bool> Initialized = new Dictionary<string, bool>();

        protected PropertyRelative Properties { get; private set; }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUILayout.Space(-20);
            InitDatabases();
        }
    }
}
