#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Railek.Unigui
{
    [AddComponentMenu("Railek/UI/UICanvas")]
    [RequireComponent(typeof(Canvas))]
    public class UICanvas : UIComponent<UICanvas>
    {
        public override void Start()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }

        #if UNITY_EDITOR
        [MenuItem("GameObject/Railek/UI/UICanvas")]
        public static UICanvas CreateUICanvas()
        {
            var go = new GameObject("UICanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            return go.AddComponent<UICanvas>();
        }
        #endif
    }
}
