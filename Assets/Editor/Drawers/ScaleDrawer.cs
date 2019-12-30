using Railek.Unigui.Animation;
using UnityEditor;
using UnityEngine;

namespace Railek.Unigui.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(Scale), true)]
    public class ScaleDrawer : BaseAnimationDrawer
    {
        private void Init(SerializedProperty property)
        {
            if (Initialized.ContainsKey(property.propertyPath) && Initialized[property.propertyPath]) return;

            Properties.Add("animationType", property);
            Properties.Add("enabled", property);
            Properties.Add("from", property);
            Properties.Add("to", property);
            Properties.Add("by", property);
            Properties.Add("vibrato", property);
            Properties.Add("elasticity", property);
            Properties.Add("numberOfLoops", property);
            Properties.Add("loopType", property);
            Properties.Add("easeType", property);
            Properties.Add("ease", property);
            Properties.Add("animationCurve", property);
            Properties.Add("startDelay", property);
            Properties.Add("duration", property);

            if (!Initialized.ContainsKey(property.propertyPath))
                Initialized.Add(property.propertyPath, true);
            else
                Initialized[property.propertyPath] = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            Init(property);

            EditorGUI.BeginProperty(position, label, property);
            {
                DrawSelector(property);
                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndProperty();
        }

        protected override void DrawShow(SerializedProperty property)
        {
            DrawStartDelayDuration(property);
            DrawFromToCustom(property);
            DrawLineEaseTypeEaseAnimationCurve(property);
        }

        protected override void DrawHide(SerializedProperty property)
        {
            DrawStartDelayDuration(property);
            DrawToFromCustom(property);
            DrawLineEaseTypeEaseAnimationCurve(property);
        }

        protected override void DrawState(SerializedProperty property)
        {
            DrawStartDelayDuration(property);
            DrawBy(property);
            DrawLineEaseTypeEaseAnimationCurve(property);
        }

        protected override void DrawPunch(SerializedProperty property)
        {
            DrawStartDelayDuration(property);
            DrawVibratoElasticity(property);
            DrawBy(property);
        }

        protected override void DrawLoop(SerializedProperty property)
        {
            DrawStartDelayDuration(property);
            DrawNumberOfLoopsLoopType(property);
            DrawFromTo(property);
            DrawLineEaseTypeEaseAnimationCurve(property);
        }

        protected override void DrawUndefined(SerializedProperty property)
        {
            DrawAnimationType(property);
        }
    }
}
