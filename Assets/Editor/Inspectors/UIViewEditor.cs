using Railek.Unibase.Editor;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Railek.Unigui.Editor
{
    [CustomEditor(typeof(UIView))]
    [CanEditMultipleObjects]
    public class UIViewEditor : EditorBase
    {
        private SerializedProperty _hideAfter;
        private SerializedProperty _behaviorAtStart;
        private SerializedProperty _hideBehavior;
        private SerializedProperty _loopBehavior;
        private SerializedProperty _showBehavior;
        private SerializedProperty _useCustomPosition;
        private SerializedProperty _customPosition;

        private AnimBool _showExpanded;
        private AnimBool _hideExpanded;
        private AnimBool _loopExpanded;

        private UIView _target;

        private static bool Draw(string text, AnimBool expanded)
        {
            if (!Draw(new GUIContent(text)))
            {
                return false;
            }

            expanded.target = !expanded.target;

            return true;
        }

        private static bool Draw(GUIContent content)
        {
            bool result;

            result = GUILayout.Button(content, GUILayout.ExpandWidth(true));

            return result;
        }

        private UIView Target
        {
            get
            {
                if (_target != null) return _target;
                _target = (UIView)target;
                return _target;
            }
        }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            _behaviorAtStart = GetProperty("behaviorAtStart");
            _hideAfter = GetProperty("hideAfter");
            _showBehavior = GetProperty("showBehavior");
            _hideBehavior = GetProperty("hideBehavior");
            _loopBehavior = GetProperty("loopBehavior");
            _useCustomPosition = GetProperty("useCustomPosition");
            _customPosition = GetProperty("customPosition");
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();
            _showExpanded = GetAnimBool(_showBehavior.propertyPath, _showBehavior.isExpanded);
            _hideExpanded = GetAnimBool(_hideBehavior.propertyPath, _hideBehavior.isExpanded);
            _loopExpanded = GetAnimBool(_loopBehavior.propertyPath, _loopBehavior.isExpanded);
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Utilities.AdjustPositionRotationAndScaleToRoundValues(Target.RectTransform);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (EditorAnimator.PreviewIsPlaying)
            {
                EditorAnimator.StopViewPreview(Target);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawBehaviourAtStart();
            DrawAutoHide();

            DrawCustomPosition(_useCustomPosition, _customPosition);

            DrawBehaviors();

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawCustomPosition(SerializedProperty useCustomStartPosition, SerializedProperty customPosition)
        {
            var enabledState = GUI.enabled;

            GUILayout.BeginHorizontal();
            {
                var result = useCustomStartPosition.boolValue;

                EditorGUI.BeginChangeCheck();
                {
                    result = GUILayout.Toggle(result, GUIContent.none, GUILayout.ExpandWidth(false));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    useCustomStartPosition.boolValue = result;
                }

                GUI.enabled = useCustomStartPosition.boolValue;

                GUILayout.Label(customPosition.displayName, GUILayout.ExpandWidth(false));
                EditorGUILayout.PropertyField(customPosition, GUIContent.none);
            }
            GUILayout.EndHorizontal();

            GUI.enabled = enabledState;
        }

        private void DrawBehaviourAtStart()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(_behaviorAtStart.displayName, GUILayout.ExpandWidth(false));
                EditorGUILayout.PropertyField(_behaviorAtStart, GUIContent.none);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawAutoHide()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(_hideAfter.displayName, GUILayout.ExpandWidth(false));
                EditorGUILayout.PropertyField(_hideAfter, GUIContent.none);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawBehaviors()
        {
            DrawBehavior("Show", Target.showBehavior, _showBehavior, _showExpanded);
            DrawBehavior("Hide", Target.hideBehavior, _hideBehavior, _hideExpanded);
            DrawBehavior("Loop", Target.loopBehavior, _loopBehavior, _loopExpanded);
        }

        private void DrawBehavior(string behaviorName, UIViewBehavior behavior, SerializedProperty behaviorProperty, AnimBool behaviorExpanded)
        {
            var animationProperty = GetProperty("animation", behaviorProperty);
            var startProperty = GetProperty("onStart", behaviorProperty);
            var finishedProperty = GetProperty("onFinished", behaviorProperty);

            GUILayout.BeginHorizontal();
            {
                Draw(behaviorName, behaviorExpanded);
                Preview.DrawPreviewAnimationButtons(Target, behavior);
            }
            GUILayout.EndHorizontal();

            if (behaviorExpanded.value)
            {
                EditorGUILayout.BeginFadeGroup(behaviorExpanded.faded);

                DrawBehaviorAnimation(animationProperty);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("On Start", GUILayout.ExpandWidth(false), GUILayout.Width(96));
                        EditorGUILayout.PropertyField(startProperty, GUIContent.none);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("On Finished", GUILayout.ExpandWidth(false), GUILayout.Width(96));
                        EditorGUILayout.PropertyField(finishedProperty, GUIContent.none);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                EditorGUILayout.EndFadeGroup();
            }
        }

        private void DrawBehaviorAnimation(SerializedProperty animationProperty)
        {
            var move = GetProperty("move", animationProperty);
            var rotate = GetProperty("rotate", animationProperty);
            var scale = GetProperty("scale", animationProperty);
            var fade = GetProperty("fade", animationProperty);

            var moveExpanded = GetAnimBool(move.propertyPath);
            var rotateExpanded = GetAnimBool(rotate.propertyPath);
            var scaleExpanded = GetAnimBool(scale.propertyPath);
            var fadeExpanded = GetAnimBool(fade.propertyPath);

            moveExpanded.target = GetProperty("enabled", move).boolValue;
            rotateExpanded.target = GetProperty("enabled", rotate).boolValue;
            scaleExpanded.target = GetProperty("enabled", scale).boolValue;
            fadeExpanded.target = GetProperty("enabled", fade).boolValue;

            GUILayout.BeginHorizontal();
            {
                if (Draw("Move", moveExpanded))
                {
                    GetProperty("enabled", move).boolValue = !GetProperty("enabled", move).boolValue;
                }

                if (Draw("Rotate", rotateExpanded))
                {
                    GetProperty("enabled", rotate).boolValue = !GetProperty("enabled", rotate).boolValue;
                }

                if (Draw("Scale", scaleExpanded))
                {
                    GetProperty("enabled", scale).boolValue = !GetProperty("enabled", scale).boolValue;
                }

                if (Draw("Fade", fadeExpanded))
                {
                    GetProperty("enabled", fade).boolValue = !GetProperty("enabled", fade).boolValue;
                }
            }
            GUILayout.EndHorizontal();

            if (moveExpanded.value)
            {
                EditorGUILayout.BeginFadeGroup(moveExpanded.faded);
                EditorGUILayout.PropertyField(move, GUIContent.none, false);
                EditorGUILayout.EndFadeGroup();
            }

            if (rotateExpanded.value)
            {
                EditorGUILayout.BeginFadeGroup(rotateExpanded.faded);
                EditorGUILayout.PropertyField(rotate, GUIContent.none, false);
                EditorGUILayout.EndFadeGroup();
            }

            if (scaleExpanded.value)
            {
                EditorGUILayout.BeginFadeGroup(scaleExpanded.faded);
                EditorGUILayout.PropertyField(scale, GUIContent.none, false);
                EditorGUILayout.EndFadeGroup();
            }

            if (fadeExpanded.value)
            {
                EditorGUILayout.BeginFadeGroup(fadeExpanded.faded);
                EditorGUILayout.PropertyField(fade, GUIContent.none, false);
                EditorGUILayout.EndFadeGroup();
            }

        }
    }
}
