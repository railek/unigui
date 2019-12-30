using System;
using System.Collections;
using System.Collections.Generic;
using Railek.Unibase.Extensions;
using Railek.Unigui.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Railek.Unigui
{
    [AddComponentMenu("Railek/UI/UIView")]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class UIView : UIComponent<UIView>
    {
        private static readonly List<UIView> VisibleViews = new List<UIView>();
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private UIView[] _childUIViews;
        private GraphicRaycaster _graphicRaycaster;
        private Coroutine _hideCoroutine;
        private bool _initialized;
        private Coroutine _showCoroutine;
        private VisibilityState _visibility;
        private float _visibilityProgress;

        [SerializeField] private float hideAfter = default(float);
        [SerializeField] private UIViewStartBehavior behaviorAtStart = default(UIViewStartBehavior);
        [SerializeField] public UIViewBehavior showBehavior = new UIViewBehavior(AnimationType.Show);
        [SerializeField] public UIViewBehavior hideBehavior = new UIViewBehavior(AnimationType.Hide);
        [SerializeField] public UIViewBehavior loopBehavior = new UIViewBehavior(AnimationType.Loop);
        [SerializeField] private bool autoStartLoop = true;
        [SerializeField] private bool useCustomPosition = default(bool);
        [SerializeField] private Vector3 customPosition = default(Vector3);

        public override void Awake()
        {
            _initialized = false;
            startPosition = RectTransform.anchoredPosition3D;
            Canvas.enabled = false;
            GraphicRaycaster.enabled = false;
        }

        protected override void Reset()
        {
            Visibility = VisibilityState.Visible;
        }

        public override void Start()
        {
            Initialize();
        }

        public override void OnEnable()
        {
            _childUIViews = GetComponentsInChildren<UIView>();
        }

        public override void OnDisable()
        {
            StopHide();
            StopShow();

            UIAnimator.StopAnimations(RectTransform, AnimationType.Hide);
            UIAnimator.StopAnimations(RectTransform, AnimationType.Show);
            UIAnimator.StopAnimations(RectTransform, AnimationType.Loop);

            ResetToStartValues();
        }

        #if UNITY_EDITOR
        [MenuItem("GameObject/Railek/UI/UIView", false, 2)]
        private static void CreateComponent(MenuCommand menuCommand)
        {
            var go = new GameObject("UIView", typeof(RectTransform), typeof(UIView), typeof(Image));
            GameObjectUtility.SetParentAndAlign(go, GetCanvasAsParent(menuCommand.context as GameObject));
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            go.GetComponent<RectTransform>().FullScreen(true);
            go.GetComponent<Image>().color = Color.black;
            Selection.activeObject = go;
        }
        #endif

        private void Initialize()
        {
            _childUIViews = GetComponentsInChildren<UIView>();

            switch (behaviorAtStart)
            {
                case UIViewStartBehavior.Nothing:
                {
                    if (autoStartLoop)
                    {
                        StartLoopAnimation();
                    }

                    Canvas.enabled = true;
                    GraphicRaycaster.enabled = true;
                    _initialized = true;
                    break;
                }
                case UIViewStartBehavior.Hide:
                {
                    InstantHide();
                    break;
                }
                case UIViewStartBehavior.Show:
                {
                    InstantHide();
                    Show();
                    if (HasChildUIViews)
                    {
                        StartCoroutine(ShowViewNextFrame());
                    }

                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void StopHide()
        {
            if (_hideCoroutine == null)
            {
                return;
            }

            StopCoroutine(_hideCoroutine);
            _hideCoroutine = null;
            Visibility = VisibilityState.NotVisible;
            UIAnimator.StopAnimations(RectTransform, AnimationType.Hide);

            if (true)
            {
                EnableUIInteractions();
            }
        }

        private void StopShow()
        {
            if (_showCoroutine == null)
            {
                return;
            }

            StopCoroutine(_showCoroutine);
            _showCoroutine = null;
            Visibility = VisibilityState.Visible;
            UIAnimator.StopAnimations(RectTransform, AnimationType.Show);

            if (true)
            {
                EnableUIInteractions();
            }
        }

        private static IEnumerator ShowViewNextFrame()
        {
            yield return null;
            ShowView();
        }

        private IEnumerator ShowEnumerator()
        {
            if (true)
            {
                DisableUIInteractions();
            }

            UIAnimator.StopAnimations(RectTransform, showBehavior.animation.animationType);

            if (loopBehavior.animation.Enabled)
            {
                UIAnimator.StopAnimations(RectTransform, loopBehavior.animation.animationType);
            }

            Canvas.enabled = true;
            GraphicRaycaster.enabled = true;

            var moveFrom = UIAnimator.GetAnimationMoveFrom(RectTransform, showBehavior.animation, CurrentStartPosition);
            var moveTo = UIAnimator.GetAnimationMoveTo(RectTransform, showBehavior.animation, CurrentStartPosition);
            if (!showBehavior.animation.move.enabled)
            {
                ResetPosition();
            }

            UIAnimator.Move(RectTransform, showBehavior.animation, moveFrom, moveTo);

            var rotateFrom = UIAnimator.GetAnimationRotateFrom(showBehavior.animation, startRotation);
            var rotateTo = UIAnimator.GetAnimationRotateTo(showBehavior.animation, startRotation);
            if (!showBehavior.animation.rotate.enabled)
            {
                ResetRotation();
            }

            UIAnimator.Rotate(RectTransform, showBehavior.animation, rotateFrom, rotateTo);

            var scaleFrom = UIAnimator.GetAnimationScaleFrom(showBehavior.animation, startScale);
            var scaleTo = UIAnimator.GetAnimationScaleTo(showBehavior.animation, startScale);
            if (!showBehavior.animation.scale.enabled)
            {
                ResetScale();
            }

            UIAnimator.Scale(RectTransform, showBehavior.animation, scaleFrom, scaleTo);

            var fadeFrom = UIAnimator.GetAnimationFadeFrom(showBehavior.animation, startAlpha);
            var fadeTo = UIAnimator.GetAnimationFadeTo(showBehavior.animation, startAlpha);
            if (!showBehavior.animation.fade.enabled)
            {
                ResetAlpha();
            }

            UIAnimator.Fade(RectTransform, showBehavior.animation, fadeFrom, fadeTo);

            Visibility = VisibilityState.Showing;
            if (!VisibleViews.Contains(this))
            {
                VisibleViews.Add(this);
            }

            var startTime = Time.realtimeSinceStartup;
            var totalDuration = showBehavior.animation.TotalDuration;
            var elapsedTime = startTime - Time.realtimeSinceStartup;
            var startDelay = showBehavior.animation.StartDelay;
            var invokedOnStart = false;

            while (elapsedTime <= totalDuration)
            {
                elapsedTime = Time.realtimeSinceStartup - startTime;

                if (!invokedOnStart && elapsedTime > startDelay)
                {
                    showBehavior.onStart.Raise();
                    invokedOnStart = true;
                }

                VisibilityProgress = elapsedTime / totalDuration;
                yield return null;
            }

            showBehavior.onFinished.Raise();

            Visibility = VisibilityState.Visible;
            if (!VisibleViews.Contains(this))
            {
                VisibleViews.Add(this);
            }

            StartLoopAnimation();

            if (hideAfter > 0)
            {
                Hide(hideAfter);
            }

            _showCoroutine = null;
            if (true)
            {
                EnableUIInteractions();
            }

            RemoveHiddenFromVisibleViews();
        }

        private IEnumerator HideEnumerator()
        {
            if (true)
            {
                DisableUIInteractions();
            }

            UIAnimator.StopAnimations(RectTransform, hideBehavior.animation.animationType);

            if (loopBehavior.animation.Enabled)
            {
                UIAnimator.StopAnimations(RectTransform, loopBehavior.animation.animationType);
            }

            var moveFrom = UIAnimator.GetAnimationMoveFrom(RectTransform, hideBehavior.animation, CurrentStartPosition);
            var moveTo = UIAnimator.GetAnimationMoveTo(RectTransform, hideBehavior.animation, CurrentStartPosition);
            if (!hideBehavior.animation.move.enabled)
            {
                ResetPosition();
            }

            UIAnimator.Move(RectTransform, hideBehavior.animation, moveFrom, moveTo);

            var rotateFrom = UIAnimator.GetAnimationRotateFrom(hideBehavior.animation, startRotation);
            var rotateTo = UIAnimator.GetAnimationRotateTo(hideBehavior.animation, startRotation);
            if (!hideBehavior.animation.rotate.enabled)
            {
                ResetRotation();
            }

            UIAnimator.Rotate(RectTransform, hideBehavior.animation, rotateFrom, rotateTo);

            var scaleFrom = UIAnimator.GetAnimationScaleFrom(hideBehavior.animation, startScale);
            var scaleTo = UIAnimator.GetAnimationScaleTo(hideBehavior.animation, startScale);
            if (!hideBehavior.animation.scale.enabled)
            {
                ResetScale();
            }

            UIAnimator.Scale(RectTransform, hideBehavior.animation, scaleFrom, scaleTo);

            var fadeFrom = UIAnimator.GetAnimationFadeFrom(hideBehavior.animation, startAlpha);
            var fadeTo = UIAnimator.GetAnimationFadeTo(hideBehavior.animation, startAlpha);
            if (!hideBehavior.animation.fade.enabled)
            {
                ResetAlpha();
            }

            UIAnimator.Fade(RectTransform, hideBehavior.animation, fadeFrom, fadeTo);

            Visibility = VisibilityState.Hiding;
            if (VisibleViews.Contains(this))
            {
                VisibleViews.Remove(this);
            }

            var startTime = Time.realtimeSinceStartup;
            var totalDuration = hideBehavior.animation.TotalDuration;
            var elapsedTime = startTime - Time.realtimeSinceStartup;
            var startDelay = hideBehavior.animation.StartDelay;
            var invokedOnStart = false;

            while (elapsedTime <= totalDuration)
            {
                elapsedTime = Time.realtimeSinceStartup - startTime;

                if (!invokedOnStart && elapsedTime > startDelay)
                {
                    hideBehavior.onStart.Raise();
                    invokedOnStart = true;
                }

                VisibilityProgress = 1 - elapsedTime / totalDuration;
                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.05f);

            if (_initialized)
            {
                hideBehavior.onFinished.Raise();
            }

            Visibility = VisibilityState.NotVisible;
            if (VisibleViews.Contains(this))
            {
                VisibleViews.Remove(this);
            }

            _hideCoroutine = null;
            if (true)
            {
                EnableUIInteractions();
            }

            RemoveHiddenFromVisibleViews();

            if (!_initialized)
            {
                _initialized = true;
            }
        }

        private IEnumerator HideWithDelayEnumerator(float delay)
        {
            if (delay > 0)
            {
                yield return new WaitForSecondsRealtime(delay);
            }

            Hide();
        }

        public static void HideView()
        {
            ExecuteHide();
        }

        public static void ShowView()
        {
            ExecuteShow();
        }

        private static void ExecuteHide()
        {
            var foundNullEntry = false;
            foreach (var view in Database)
            {
                if (view == null)
                {
                    foundNullEntry = true;
                    continue;
                }

                if (!view.gameObject.activeInHierarchy)
                {
                    continue;
                }

                view.Hide();
            }

            if (foundNullEntry)
            {
                RemoveAnyNullReferencesFromTheDatabase();
            }
        }

        private static void ExecuteShow()
        {
            var foundNullEntry = false;
            foreach (var view in Database)
            {
                if (view == null)
                {
                    foundNullEntry = true;
                    continue;
                }

                view.gameObject.SetActive(true);
                if (!view.gameObject.activeInHierarchy)
                {
                    continue;
                }

                view.Show();
            }

            if (foundNullEntry)
            {
                RemoveAnyNullReferencesFromTheDatabase();
            }
        }

        private static void RemoveHiddenFromVisibleViews()
        {
            RemoveNullsFromVisibleViews();
            for (var i = VisibleViews.Count - 1; i >= 0; i--)
            {
                if (VisibleViews[i].IsHidden)
                {
                    VisibleViews.RemoveAt(i);
                }
            }
        }

        private static void RemoveNullsFromVisibleViews()
        {
            for (var i = VisibleViews.Count - 1; i >= 0; i--)
            {
                if (VisibleViews[i] == null)
                {
                    VisibleViews.RemoveAt(i);
                }
            }
        }

        public void Hide()
        {
            StopShow();

            if (!hideBehavior.animation.Enabled)
            {
                return;
            }

            if (Visibility == VisibilityState.Hiding)
            {
                return;
            }

            if (!IsVisible)
            {
                if (VisibleViews.Contains(this))
                {
                    VisibleViews.Remove(this);
                }

                return;
            }

            _hideCoroutine = StartCoroutine(HideEnumerator());
        }

        private void Hide(float delay)
        {
            StartCoroutine(HideWithDelayEnumerator(delay));
        }

        private void InstantHide()
        {
            StopLoopAnimation();
            StopShow();
            StopHide();
            ResetToStartValues();

            Visibility = VisibilityState.NotVisible;
            if (VisibleViews.Contains(this))
            {
                VisibleViews.Remove(this);
            }

            if (!_initialized)
            {
                _initialized = true;
            }
        }

        public override void ResetAlpha()
        {
            CanvasGroup.alpha = startAlpha;
        }

        public override void ResetPosition()
        {
            RectTransform.anchoredPosition3D = CurrentStartPosition;
        }

        public void Show()
        {
            gameObject.SetActive(true);

            StopHide();

            if (!showBehavior.animation.Enabled)
            {
                return;
            }

            if (Visibility == VisibilityState.Showing)
            {
                return;
            }

            if (IsVisible)
            {
                if (!VisibleViews.Contains(this))
                {
                    VisibleViews.Add(this);
                }
                return;
            }

            _showCoroutine = StartCoroutine(ShowEnumerator());

            if (HasChildUIViews)
            {
                StartCoroutine(ShowViewNextFrame());
            }
        }

        private void StartLoopAnimation()
        {
            if (!loopBehavior.animation.Enabled)
            {
                return;
            }

            UIAnimator.MoveLoop(RectTransform, loopBehavior.animation, CurrentStartPosition);
            UIAnimator.RotateLoop(RectTransform, loopBehavior.animation, startRotation);
            UIAnimator.ScaleLoop(RectTransform, loopBehavior.animation);
            UIAnimator.FadeLoop(RectTransform, loopBehavior.animation);
        }

        private void StopLoopAnimation()
        {
            UIAnimator.StopAnimations(RectTransform, AnimationType.Loop);
        }

        private Canvas Canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = GetComponent<Canvas>();
                }

                return _canvas;
            }
        }

        private CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                {
                    _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
                }

                return _canvasGroup;
            }
        }

        public Vector3 CurrentStartPosition => useCustomPosition ? customPosition : startPosition;

        private GraphicRaycaster GraphicRaycaster
        {
            get
            {
                if (_graphicRaycaster == null)
                {
                    _graphicRaycaster = GetComponent<GraphicRaycaster>();
                }

                return _graphicRaycaster;
            }
        }

        private bool IsHidden => Visibility == VisibilityState.NotVisible;

        private bool IsVisible => Visibility == VisibilityState.Visible;

        private VisibilityState Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                switch (value)
                {
                    case VisibilityState.Visible:
                    {
                        VisibilityProgress = 1f;
                        break;
                    }
                    case VisibilityState.NotVisible:
                    {
                        VisibilityProgress = 0f;
                        break;
                    }
                    case VisibilityState.Hiding:
                    {
                        break;
                    }
                    case VisibilityState.Showing:
                    {
                        break;
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                    }
                }
            }
        }

        private float VisibilityProgress
        {
            get => _visibilityProgress;
            set => _visibilityProgress = Mathf.Clamp01(value);
        }

        private bool HasChildUIViews => _childUIViews != null && _childUIViews.Length > 1;
    }
}
