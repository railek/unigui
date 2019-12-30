using System;
using Railek.Unigui.Animation;
using UnityEditor;
using UnityEngine;

namespace Railek.Unigui.Editor.Drawers
{
    public class BaseAnimationDrawer : BaseDrawer
    {
        private AnimationType _animationType;

        protected void DrawSelector(SerializedProperty property)
        {
            _animationType = (AnimationType)Properties.Get("animationType", property).enumValueIndex;

            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                switch (_animationType)
                {
                    case AnimationType.Show:
                        DrawShow(property);
                        break;
                    case AnimationType.Hide:
                        DrawHide(property);
                        break;
                    case AnimationType.Loop:
                        DrawLoop(property);
                        break;
                    case AnimationType.Punch:
                        DrawPunch(property);
                        break;
                    case AnimationType.State:
                        DrawState(property);
                        break;
                    case AnimationType.Undefined:
                        DrawUndefined(property);
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            GUILayout.EndVertical();
        }

        protected virtual void DrawShow(SerializedProperty property)
        {
        }

        protected virtual void DrawHide(SerializedProperty property)
        {
        }

        protected virtual void DrawLoop(SerializedProperty property)
        {
        }

        protected virtual void DrawPunch(SerializedProperty property)
        {
        }

        protected virtual void DrawState(SerializedProperty property)
        {
        }

        protected virtual void DrawUndefined(SerializedProperty property)
        {
        }

        protected static void DrawStartDelayDuration(SerializedProperty property)
        {
            var startDelay = property.FindPropertyRelative("startDelay");
            var duration = property.FindPropertyRelative("duration");

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Start Delay");
                    EditorGUILayout.PropertyField(startDelay, GUIContent.none,GUILayout.ExpandWidth(true));
                    GUILayout.Label("Duration");
                    EditorGUILayout.PropertyField(duration, GUIContent.none, GUILayout.ExpandWidth(true));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
        }

        protected static void DrawNumberOfLoopsLoopType(SerializedProperty property)
        {
            var numberOfLoops = property.FindPropertyRelative("numberOfLoops");
            var loopType = property.FindPropertyRelative("loopType");

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Number Of Loops");
                    EditorGUILayout.PropertyField(numberOfLoops, GUIContent.none,GUILayout.ExpandWidth(true));
                    GUILayout.Label("Loop Type");
                    EditorGUILayout.PropertyField(loopType, GUIContent.none, GUILayout.ExpandWidth(true));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
        }

        protected static void DrawVibratoElasticity(SerializedProperty property)
        {
            var vibrato = property.FindPropertyRelative("vibrato");
            var elasticity = property.FindPropertyRelative("elasticity");

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Vibrato");
                    EditorGUILayout.PropertyField(vibrato, GUIContent.none,GUILayout.ExpandWidth(true));
                    GUILayout.Label("Elasticity");
                    EditorGUILayout.PropertyField(elasticity, GUIContent.none, GUILayout.ExpandWidth(true));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
        }

        protected static void DrawByRotateMode(SerializedProperty property)
        {
            var by = property.FindPropertyRelative("by");
            var rotateMode = property.FindPropertyRelative("rotateMode");

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("By");
                EditorGUILayout.PropertyField(by, GUIContent.none,GUILayout.ExpandWidth(true));
                GUILayout.Label("Rotate Mode");
                EditorGUILayout.PropertyField(rotateMode, GUIContent.none, GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();
        }

        protected static void DrawAnimationType(SerializedProperty property)
        {
            var animationType = property.FindPropertyRelative("animationType");

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Animation Type");
                EditorGUILayout.PropertyField(animationType, GUIContent.none, GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();
        }

        protected static void DrawDirection(SerializedProperty property)
        {
            var direction = property.FindPropertyRelative("direction");
            var customPosition = property.FindPropertyRelative("customPosition");

            GUILayout.BeginHorizontal();
            {
                if ((Direction)direction.enumValueIndex == Direction.CustomPosition)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Direction");
                        EditorGUILayout.PropertyField(direction, GUIContent.none, GUILayout.ExpandWidth(true));
                        GUILayout.Label("Custom Position");
                        EditorGUILayout.PropertyField(customPosition, GUIContent.none, GUILayout.ExpandWidth(true));
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Direction");
                        EditorGUILayout.PropertyField(direction, GUIContent.none, GUILayout.ExpandWidth(true));
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndHorizontal();
        }

        protected static void DrawBy(SerializedProperty property)
        {
            var by = property.FindPropertyRelative("by");

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("By");
                EditorGUILayout.PropertyField(by, GUIContent.none,GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();
        }

        protected static void DrawFromToCustom(SerializedProperty property)
        {
            var from = property.FindPropertyRelative("from");

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("From");
                EditorGUILayout.PropertyField(from, GUIContent.none,GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();
        }

        protected static void DrawToFromCustom(SerializedProperty property)
        {
            var to = property.FindPropertyRelative("to");

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("To");
                EditorGUILayout.PropertyField(to, GUIContent.none,GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();
        }

        protected static void DrawFromTo(SerializedProperty property)
        {
            var from = property.FindPropertyRelative("from");
            var to = property.FindPropertyRelative("to");

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("From");
                EditorGUILayout.PropertyField(from, GUIContent.none,GUILayout.ExpandWidth(true));
                GUILayout.Label("To");
                EditorGUILayout.PropertyField(to, GUIContent.none,GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();
        }

        protected void DrawLineEaseTypeEaseAnimationCurve(SerializedProperty property)
        {
            var easeType = property.FindPropertyRelative("easeType");
            var ease = property.FindPropertyRelative("ease");
            var animationCurve = property.FindPropertyRelative("animationCurve");

            GUILayout.BeginHorizontal();
            {
                if ((EaseType)easeType.enumValueIndex == EaseType.Ease)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Ease Type");
                        EditorGUILayout.PropertyField(easeType, GUIContent.none,GUILayout.ExpandWidth(true));
                        GUILayout.Label("Ease");
                        EditorGUILayout.PropertyField(ease, GUIContent.none,GUILayout.ExpandWidth(true));
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Ease Type");
                        EditorGUILayout.PropertyField(easeType, GUIContent.none,GUILayout.ExpandWidth(true));
                        GUILayout.Label("Animation Curve");
                        EditorGUILayout.PropertyField(animationCurve, GUIContent.none,GUILayout.ExpandWidth(true));
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndHorizontal();
        }

        protected static void DrawLineEaseTypeEaseAnimationCurveRotateMode(SerializedProperty property)
        {
            var easeType = property.FindPropertyRelative("easeType");
            var ease = property.FindPropertyRelative("ease");
            var animationCurve = property.FindPropertyRelative("animationCurve");
            var rotateMode = property.FindPropertyRelative("rotateMode");

            GUILayout.BeginHorizontal();
            {
                if ((EaseType)easeType.enumValueIndex == EaseType.Ease)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Ease Type");
                        EditorGUILayout.PropertyField(easeType, GUIContent.none,GUILayout.ExpandWidth(true));
                        GUILayout.Label("Ease");
                        EditorGUILayout.PropertyField(ease, GUIContent.none,GUILayout.ExpandWidth(true));
                        GUILayout.Label("Rotate Mode");
                        EditorGUILayout.PropertyField(rotateMode, GUIContent.none,GUILayout.ExpandWidth(true));
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label("Ease Type");
                    EditorGUILayout.PropertyField(easeType, GUIContent.none,GUILayout.ExpandWidth(true));
                    GUILayout.Label("Animation Curve");
                    EditorGUILayout.PropertyField(animationCurve, GUIContent.none,GUILayout.ExpandWidth(true));
                    GUILayout.Label("Rotate Mode");
                    EditorGUILayout.PropertyField(rotateMode, GUIContent.none,GUILayout.ExpandWidth(true));
                }
            }
            GUILayout.EndHorizontal(); ;
        }
    }
}
