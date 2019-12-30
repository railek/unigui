using UnityEditor;
using UnityEngine;

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
    }
}
