using System;
using System.Collections.Generic;
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
        private SerializedProperty _onShowStart;
        private SerializedProperty _onShowFinished;
        private SerializedProperty _onHideStart;
        private SerializedProperty _onHideFinished;

        private AnimBool _showExpanded;
        private AnimBool _hideExpanded;
        private AnimBool _loopExpanded;

        private UIView _target;

        private static bool Draw(string text, AnimBool expanded)
        {
            return Draw(new GUIContent(text), expanded);
        }

        private static bool Draw(GUIContent content, AnimBool expanded)
        {
            if (!Draw(content)) return false;
            expanded.target = !expanded.target;
            Event.current.Use();
            return true;
        }

        private static bool Draw(GUIContent content, bool expandWidth = false)
        {
            var options = new List<GUILayoutOption>();
            if (expandWidth) options.Add(GUILayout.ExpandWidth(false));

            bool result;

            result = GUILayout.Button(content, options.ToArray());

            if (result) Event.current.Use();

            return result;
        }

        private static bool Begin(AnimBool expanded)
        {
            if (expanded.faded < 0.05f) return false;

            EditorGUILayout.BeginFadeGroup(expanded.faded);

            return true;
        }

        private static void End(AnimBool expanded)
        {
            if (expanded.faded < 0.05f) return;

            EditorGUILayout.EndFadeGroup();
        }

        private static Vector3 AdjustToRoundValues(Vector3 v3, int maximumAllowedDecimals = 3)
        {
            return new Vector3(RoundToIntIfNeeded(v3.x, maximumAllowedDecimals),
                RoundToIntIfNeeded(v3.y, maximumAllowedDecimals),
                RoundToIntIfNeeded(v3.z, maximumAllowedDecimals));
        }

        private static void AdjustPositionRotationAndScaleToRoundValues(RectTransform rectTransform)
        {
            rectTransform.anchoredPosition3D = AdjustToRoundValues(rectTransform.anchoredPosition3D);
            rectTransform.localEulerAngles = AdjustToRoundValues(rectTransform.localEulerAngles);
            rectTransform.localScale = AdjustToRoundValues(rectTransform.localScale);
        }

        private static float RoundToIntIfNeeded(float value, int maximumAllowedDecimals = 3)
        {
            int numberOfDecimals = BitConverter.GetBytes(decimal.GetBits((decimal) value)[3])[2];
            return numberOfDecimals >= maximumAllowedDecimals ? Mathf.Round(value) : value;
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
            _onShowStart = GetProperty("onShowStart");
            _onShowFinished = GetProperty("onShowFinished");
            _onHideStart = GetProperty("onHideStart");
            _onHideFinished = GetProperty("onHideFinished");
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
            AdjustPositionRotationAndScaleToRoundValues(Target.RectTransform);
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

            if (Begin(behaviorExpanded))
            {
                DrawBehaviorAnimation(animationProperty);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(startProperty);
                    EditorGUILayout.PropertyField(finishedProperty);
                }
                GUILayout.EndVertical();
            }
            End(behaviorExpanded);
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

            if (Begin(moveExpanded))
            {
                EditorGUILayout.PropertyField(move, GUIContent.none, false);
            }
            End(moveExpanded);

            if (Begin(rotateExpanded))
            {
                EditorGUILayout.PropertyField(rotate, GUIContent.none, false);
            }
            End(rotateExpanded);

            if (Begin(scaleExpanded))
            {
                EditorGUILayout.PropertyField(scale, GUIContent.none, false);
            }
            End(scaleExpanded);

            if (Begin(fadeExpanded))
            {
                EditorGUILayout.PropertyField(fade, GUIContent.none, false);
            }
            End(fadeExpanded);
        }
    }
}
