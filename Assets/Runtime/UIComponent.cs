using System.Collections.Generic;
using Railek.Unigui.Animation;
using UnityEngine;

namespace Railek.Unigui
{
    public abstract class UIComponent<T> : MonoBehaviour
    {
        protected static readonly List<T> Database = new List<T>();

        private static int _uiInteractionsDisableLevel;

        public Vector3 startPosition = UIAnimator.DefaultStartPosition;
        public Vector3 startRotation = UIAnimator.DefaultStartRotation;
        public Vector3 startScale = UIAnimator.DefaultStartScale;
        public float startAlpha = UIAnimator.DefaultStartAlpha;

        private RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }

        protected static bool UIInteractionsDisabled
        {
            get
            {
                if (_uiInteractionsDisableLevel < 0)
                {
                    _uiInteractionsDisableLevel = 0;
                }

                return _uiInteractionsDisableLevel != 0;
            }
        }

        public virtual void Awake()
        {
            Database.Add(GetComponent<T>());
            UpdateStartValues();
        }

        protected virtual void Reset()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void OnEnable()
        {
        }

        public virtual void OnDisable()
        {
        }

        public virtual void OnDestroy()
        {
            Database.Remove(GetComponent<T>());
        }

        protected static GameObject GetCanvasAsParent(GameObject selectedObject)
        {
            if (selectedObject == null)
            {
                return UICanvas.CreateUICanvas().gameObject;
            }

            if (selectedObject.GetComponent<UICanvas>())
            {
                return selectedObject;
            }

            var root = selectedObject.transform.root;
            return root.GetComponent<UICanvas>() ? root.gameObject : UICanvas.CreateUICanvas().gameObject;
        }


        protected static void RemoveAnyNullReferencesFromTheDatabase()
        {
            for (var i = Database.Count; i >= 0; i--)
            {
                if (Database[i] == null)
                {
                    Database.RemoveAt(i);
                }
            }
        }

        public virtual void ResetToStartValues()
        {
            UIAnimator.ResetCanvasGroup(RectTransform);
            ResetPosition();
            ResetRotation();
            ResetScale();
            ResetAlpha();
        }

        public virtual void ResetPosition()
        {
            RectTransform.anchoredPosition3D = startPosition;
        }

        public virtual void ResetRotation()
        {
            RectTransform.localEulerAngles = startRotation;
        }

        public virtual void ResetScale()
        {
            RectTransform.localScale = startScale;
        }

        public virtual void ResetAlpha()
        {
            if (RectTransform.GetComponent<CanvasGroup>() != null)
            {
                RectTransform.GetComponent<CanvasGroup>().alpha = startAlpha;
            }
        }

        public virtual void UpdateStartValues()
        {
            UpdateStartPosition();
            UpdateStartRotation();
            UpdateStartScale();
            UpdateStartAlpha();
        }

        protected virtual void UpdateStartPosition()
        {
            startPosition = RectTransform.anchoredPosition3D;
        }

        protected virtual void UpdateStartRotation()
        {
            startRotation = RectTransform.localEulerAngles;
        }

        protected virtual void UpdateStartScale()
        {
            startScale = RectTransform.localScale;
        }

        protected virtual void UpdateStartAlpha()
        {
            startAlpha = RectTransform.GetComponent<CanvasGroup>() == null
                ? 1
                : RectTransform.GetComponent<CanvasGroup>().alpha;
        }

        protected static void EnableUIInteractions()
        {
            _uiInteractionsDisableLevel--;
            if (_uiInteractionsDisableLevel < 0)
            {
                _uiInteractionsDisableLevel = 0;
            }
        }

        protected static void DisableUIInteractions()
        {
            _uiInteractionsDisableLevel++;
        }
    }
}
