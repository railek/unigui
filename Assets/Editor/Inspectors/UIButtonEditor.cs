using System;
using Railek.Unibase.Editor;
using Railek.Unigui.Animation;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;

namespace Railek.Unigui.Editor
{
    [CustomEditor(typeof(UIButton))]
    [CanEditMultipleObjects]
    public class UIButtonEditor : EditorBase
    {
        private string _buttonLabel;

        private AnimBool _normalLoopAnimationExpanded;
        private AnimBool _onClickExpanded;
        private AnimBool _onDeselectedExpanded;
        private AnimBool _onDoubleClickExpanded;
        private AnimBool _onLongClickExpanded;
        private AnimBool _onPointerDownExpanded;
        private AnimBool _onPointerEnterExpanded;
        private AnimBool _onPointerExitExpanded;
        private AnimBool _onPointerUpExpanded;
        private AnimBool _onRightClickExpanded;
        private AnimBool _onSelectedExpanded;
        private AnimBool _selectedLoopAnimationExpanded;

        private SerializedProperty _selectedLoopAnimation;
        private SerializedProperty _onSelected;
        private SerializedProperty _onRightClick;
        private SerializedProperty _onPointerExit;
        private SerializedProperty _onPointerEnter;
        private SerializedProperty _onPointerDown;
        private SerializedProperty _onLongClick;
        private SerializedProperty _onDoubleClick;
        private SerializedProperty _onDeselected;
        private SerializedProperty _onClick;
        private SerializedProperty _mButtonName;
        private SerializedProperty _normalLoopAnimation;
        private SerializedProperty _textProperty;
        private SerializedProperty _onPointerUp;
        private SerializedProperty _buttonCategory;

        private SerializedObject _textSerializedObject;

        private Image _targetImage;
        private Color _imageColor, _textColor;

        private UIButton _target;

        private UIButton Target
        {
            get
            {
                if (_target != null)
                {
                    return _target;
                }

                _target = (UIButton) target;
                return _target;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (UIButton) target;

            Utilities.AdjustPositionRotationAndScaleToRoundValues(Target.RectTransform);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (EditorAnimator.PreviewIsPlaying)
            {
                EditorAnimator.StopButtonPreview(Target.RectTransform, Target.CanvasGroup);
            }
        }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            _onPointerEnter = GetProperty("onPointerEnter");
            _onPointerExit = GetProperty("onPointerExit");
            _onPointerDown = GetProperty("onPointerDown");
            _onPointerUp = GetProperty("onPointerUp");
            _onClick = GetProperty("onClick");
            _onDoubleClick = GetProperty("onDoubleClick");
            _onLongClick = GetProperty("onLongClick");
            _onRightClick = GetProperty("onRightClick");
            _onSelected = GetProperty("onSelected");
            _onDeselected = GetProperty("onDeselected");
            _normalLoopAnimation = GetProperty("normalLoopAnimation");
            _selectedLoopAnimation = GetProperty("selectedLoopAnimation");
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();

            _onPointerEnterExpanded = GetAnimBool(_onPointerEnter.propertyPath, _onPointerEnter.isExpanded);
            _onPointerExitExpanded = GetAnimBool(_onPointerExit.propertyPath, _onPointerExit.isExpanded);
            _onPointerDownExpanded = GetAnimBool(_onPointerDown.propertyPath, _onPointerDown.isExpanded);
            _onPointerUpExpanded = GetAnimBool(_onPointerUp.propertyPath, _onPointerUp.isExpanded);
            _onClickExpanded = GetAnimBool(_onClick.propertyPath, _onClick.isExpanded);
            _onDoubleClickExpanded = GetAnimBool(_onDoubleClick.propertyPath, _onDoubleClick.isExpanded);
            _onLongClickExpanded = GetAnimBool(_onLongClick.propertyPath, _onLongClick.isExpanded);
            _onRightClickExpanded = GetAnimBool(_onRightClick.propertyPath, _onRightClick.isExpanded);
            _onSelectedExpanded = GetAnimBool(_onSelected.propertyPath, _onSelected.isExpanded);
            _onDeselectedExpanded = GetAnimBool(_onDeselected.propertyPath, _onDeselected.isExpanded);
            _normalLoopAnimationExpanded =
                GetAnimBool(_normalLoopAnimation.propertyPath, _normalLoopAnimation.isExpanded);
            _selectedLoopAnimationExpanded =
                GetAnimBool(_selectedLoopAnimation.propertyPath, _selectedLoopAnimation.isExpanded);
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            DrawBehaviors();
            DrawLoopAnimations();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBehaviors()
        {
            DrawBehavior("OnPointerEnter", Target.onPointerEnter, _onPointerEnter, _onPointerEnterExpanded);
            DrawBehavior("OnPointerExit", Target.onPointerExit, _onPointerExit, _onPointerExitExpanded);
            DrawBehavior("OnPointerDown", Target.onPointerDown, _onPointerDown, _onPointerDownExpanded);
            DrawBehavior("OnPointerExit", Target.onPointerUp, _onPointerUp, _onPointerUpExpanded);
            DrawBehavior("OnClick", Target.onClick, _onClick, _onClickExpanded);
            DrawBehavior("OnDoubleClick", Target.onDoubleClick, _onDoubleClick, _onDoubleClickExpanded);
            DrawBehavior("OnLongClick", Target.onLongClick, _onLongClick, _onLongClickExpanded);
            DrawBehavior("OnRightClick", Target.onRightClick, _onRightClick, _onRightClickExpanded);
            DrawBehavior("OnSelected", Target.onSelected, _onSelected, _onSelectedExpanded);
            DrawBehavior("OnDeselected", Target.onDeselected, _onDeselected, _onDeselectedExpanded);
        }

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

        private void DrawBehavior(string behaviorName, UIButtonBehavior behavior, SerializedProperty behaviorProperty,
            AnimBool behaviorExpanded)
        {
            var enabledProperty = GetProperty("enabled", behaviorProperty);
            var triggerProperty = GetProperty("onTrigger", behaviorProperty);

            var enabledState = GUI.enabled;

            GUILayout.BeginHorizontal();
            {
                var result = enabledProperty.boolValue;

                EditorGUI.BeginChangeCheck();
                {
                    result = GUILayout.Toggle(result, GUIContent.none, GUILayout.ExpandWidth(false));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    enabledProperty.boolValue = result;
                }

                GUI.enabled = enabledProperty.boolValue;
                Draw(behaviorName, behaviorExpanded);
                Preview.DrawPreviewAnimationButtons(Target, behavior);
            }
            GUILayout.EndHorizontal();

            GUI.enabled = enabledProperty.boolValue;
            {
                if (!enabledProperty.boolValue && behaviorExpanded.target)
                {
                    behaviorExpanded.target = false;
                }

                if (behaviorExpanded.value)
                {
                    EditorGUILayout.BeginFadeGroup(behaviorExpanded.faded);

                    DrawButtonBehaviorAnimation(behaviorProperty);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(23);
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            EditorGUILayout.PropertyField(triggerProperty);
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.EndFadeGroup();
                }
            }
            GUI.enabled = enabledState;
        }

        private void DrawButtonBehaviorAnimation(SerializedProperty behaviorProperty)
        {
            var buttonAnimationType = GetProperty("buttonAnimationType", behaviorProperty);
            var selectedAnimationType = (ButtonAnimationType) buttonAnimationType.enumValueIndex;
            var triggerEventAfterAnimation = GetProperty("triggerEventAfterAnimation", behaviorProperty);

            DrawBehaviorAnimationTypeSelector(buttonAnimationType, triggerEventAfterAnimation);

            switch (selectedAnimationType)
            {
                case ButtonAnimationType.Punch:
                {
                    DrawBehaviorAnimationsProperty(GetProperty("punchAnimation", behaviorProperty),
                        selectedAnimationType);
                    break;
                }
                case ButtonAnimationType.State:
                {
                    DrawBehaviorAnimationsProperty(GetProperty("stateAnimation", behaviorProperty),
                        selectedAnimationType);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void DrawBehaviorAnimationTypeSelector(SerializedProperty buttonAnimationTypeProperty,
            SerializedProperty triggerEventAfterAnimation)
        {
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(23);

                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(triggerEventAfterAnimation);
                    EditorGUILayout.PropertyField(buttonAnimationTypeProperty, GUIContent.none);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawBehaviorAnimationsProperty(SerializedProperty property, ButtonAnimationType animationType)
        {
            var move = GetProperty("move", property);
            var rotate = GetProperty("rotate", property);
            var scale = GetProperty("scale", property);

            var moveExpanded = GetAnimBool(move.propertyPath);
            var rotateExpanded = GetAnimBool(rotate.propertyPath);
            var scaleExpanded = GetAnimBool(scale.propertyPath);
            moveExpanded.target = GetProperty("enabled", move).boolValue;
            rotateExpanded.target = GetProperty("enabled", rotate).boolValue;
            scaleExpanded.target = GetProperty("enabled", scale).boolValue;
            SerializedProperty fade = null;
            AnimBool fadeExpanded = null;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(23);

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

                if (animationType == ButtonAnimationType.State)
                {
                    fade = GetProperty("fade", property);
                    fadeExpanded = GetAnimBool(fade.propertyPath);
                    fadeExpanded.target = GetProperty("enabled", fade).boolValue;
                    if (Draw("Fade", fadeExpanded))
                    {
                        GetProperty("enabled", fade).boolValue = !GetProperty("enabled", fade).boolValue;
                    }
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(23);
                if (moveExpanded.value)
                {
                    EditorGUILayout.BeginFadeGroup(moveExpanded.faded);
                    EditorGUILayout.PropertyField(move, GUIContent.none, false);
                    EditorGUILayout.EndFadeGroup();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(23);
                if (rotateExpanded.value)
                {
                    EditorGUILayout.BeginFadeGroup(rotateExpanded.faded);
                    EditorGUILayout.PropertyField(rotate, GUIContent.none, false);
                    EditorGUILayout.EndFadeGroup();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(23);
                if (scaleExpanded.value)
                {
                    EditorGUILayout.BeginFadeGroup(scaleExpanded.faded);
                    EditorGUILayout.PropertyField(scale, GUIContent.none, false);
                    EditorGUILayout.EndFadeGroup();
                }

            }
            GUILayout.EndHorizontal();

            if (animationType != ButtonAnimationType.State)
            {
                return;
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(23);
                if (fadeExpanded != null && fadeExpanded.value)
                {
                    EditorGUILayout.BeginFadeGroup(fadeExpanded.faded);
                    EditorGUILayout.PropertyField(fade, GUIContent.none, false);
                    EditorGUILayout.EndFadeGroup();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawLoopAnimations()
        {
            DrawLoopAnimation("Normal Loop Animation", Target.normalLoopAnimation, _normalLoopAnimation,
                _normalLoopAnimationExpanded);
            DrawLoopAnimation("Selected Loop Animation", Target.selectedLoopAnimation, _selectedLoopAnimation,
                _selectedLoopAnimationExpanded);
        }

        private void DrawLoopAnimation(string loopAnimationName, UIButtonLoopAnimation buttonLoopAnimation,
            SerializedProperty loopAnimationProperty, AnimBool loopAnimationExpanded)
        {
            var enabled = GetProperty("enabled", loopAnimationProperty);
            var animationProperty = GetProperty("animation", loopAnimationProperty);

            var enabledState = GUI.enabled;

            GUILayout.BeginHorizontal();
            {
                var result = enabled.boolValue;

                EditorGUI.BeginChangeCheck();
                {
                    result = GUILayout.Toggle(result, GUIContent.none, GUILayout.ExpandWidth(false));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    enabled.boolValue = result;
                }

                GUI.enabled = enabled.boolValue;
                Draw(loopAnimationName, loopAnimationExpanded);
                Preview.DrawPreviewAnimationButtons(Target, buttonLoopAnimation);
            }
            GUILayout.EndHorizontal();

            GUI.enabled = enabled.boolValue;
            {
                if (!enabled.boolValue && loopAnimationExpanded.target)
                {
                    loopAnimationExpanded.target = false;
                }

                if (loopAnimationExpanded.value)
                {
                    EditorGUILayout.BeginFadeGroup(loopAnimationExpanded.faded);

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
                        GUILayout.Space(23);

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

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(23);
                        if (moveExpanded.value)
                        {
                            EditorGUILayout.BeginFadeGroup(moveExpanded.faded);
                            EditorGUILayout.PropertyField(move, GUIContent.none, false);
                            EditorGUILayout.EndFadeGroup();
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(23);
                        if (rotateExpanded.value)
                        {
                            EditorGUILayout.BeginFadeGroup(rotateExpanded.faded);
                            EditorGUILayout.PropertyField(rotate, GUIContent.none, false);
                            EditorGUILayout.EndFadeGroup();
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(23);
                        if (scaleExpanded.value)
                        {
                            EditorGUILayout.BeginFadeGroup(scaleExpanded.faded);
                            EditorGUILayout.PropertyField(scale, GUIContent.none, false);
                            EditorGUILayout.EndFadeGroup();
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(23);
                        if (fadeExpanded.value)
                        {
                            EditorGUILayout.BeginFadeGroup(fadeExpanded.faded);
                            EditorGUILayout.PropertyField(fade, GUIContent.none, false);
                            EditorGUILayout.EndFadeGroup();
                        }
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.EndFadeGroup();
                }
            }
            GUI.enabled = enabledState;

            loopAnimationProperty.isExpanded = loopAnimationExpanded.target;
        }
    }
}
