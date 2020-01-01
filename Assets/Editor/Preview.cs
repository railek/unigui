using System;
using Railek.Unigui.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Railek.Unigui.Editor
{
    public static class Preview
    {
        public static void DrawPreviewAnimationButtons(UIView view, UIViewBehavior viewBehavior)
        {
            var enabled = GUI.enabled;
            if (GUI.enabled) GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

            if (GUILayout.Button("► PREVIEW", GUILayout.ExpandWidth(false)))
            {
                EditorAnimator.PreviewViewAnimation(view, viewBehavior.animation);
            }

            GUI.enabled = enabled;
        }

        public static void DrawPreviewAnimationButtons(UIButton button, UIButtonBehavior buttonBehavior)
        {
            UIAnimation animation;
            switch (buttonBehavior.buttonAnimationType)
            {
                case ButtonAnimationType.Punch:
                {
                    animation = buttonBehavior.punchAnimation;
                    break;
                }
                case ButtonAnimationType.State:
                {
                    animation = buttonBehavior.stateAnimation;
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            var enabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

            if (GUILayout.Button("► PREVIEW", GUILayout.ExpandWidth(false)))
            {
                EditorAnimator.PreviewButtonAnimation(animation, button.RectTransform, button.CanvasGroup);
            }

            GUI.enabled = enabled;
        }

        public static void DrawPreviewAnimationButtons(UIButton button, UIButtonLoopAnimation buttonLoopAnimation)
        {
            var animation = buttonLoopAnimation.animation;

            var enabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

            if (GUILayout.Button("► PREVIEW", GUILayout.ExpandWidth(false)))
            {
                EditorAnimator.PreviewButtonAnimation(animation, button.RectTransform, button.CanvasGroup);
            }

            GUI.enabled = enabled;
        }
    }
}
