using System;
using System.Collections;
using Railek.Unibase.Extensions;
using Railek.Unigui.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Railek.Unigui
{
    [AddComponentMenu("Railek/UI/UIButton")]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class UIButton : UIComponent<UIButton>, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
        private const float DefaultButtonHeight = 100f;
        private const float DefaultButtonWidth = 300f;

        private static EventSystem _unityEventSystem;

        public UIButtonBehavior onPointerEnter = new UIButtonBehavior(UIButtonBehaviorType.OnPointerEnter);
        public UIButtonBehavior onPointerExit = new UIButtonBehavior(UIButtonBehaviorType.OnPointerExit);
        public UIButtonBehavior onPointerDown = new UIButtonBehavior(UIButtonBehaviorType.OnPointerDown);
        public UIButtonBehavior onPointerUp = new UIButtonBehavior(UIButtonBehaviorType.OnPointerUp);
        public UIButtonBehavior onClick = new UIButtonBehavior(UIButtonBehaviorType.OnClick);
        public UIButtonBehavior onDoubleClick = new UIButtonBehavior(UIButtonBehaviorType.OnDoubleClick);
        public UIButtonBehavior onLongClick = new UIButtonBehavior(UIButtonBehaviorType.OnLongClick);
        public UIButtonBehavior onRightClick = new UIButtonBehavior(UIButtonBehaviorType.OnRightClick);
        public UIButtonBehavior onSelected = new UIButtonBehavior(UIButtonBehaviorType.OnSelected);
        public UIButtonBehavior onDeselected = new UIButtonBehavior(UIButtonBehaviorType.OnDeselected);

        public UIButtonLoopAnimation normalLoopAnimation = new UIButtonLoopAnimation();
        public UIButtonLoopAnimation selectedLoopAnimation = new UIButtonLoopAnimation();

        private Button _button;
        private CanvasGroup _canvasGroup;
        private bool _clickedOnce;
        private Coroutine _disableButtonCoroutine;
        private float _doubleClickTimeoutCounter;
        private bool _executedLongClick;
        private Coroutine _longClickRegisterCoroutine;
        private float _longClickTimeoutCounter;
        private bool _updateStartValuesRequired;

        private Button Button
        {
            get
            {
                if (_button != null)
                {
                    return _button;
                }

                _button = GetComponent<Button>();
                return _button;
            }
        }

        public CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup != null)
                {
                    return _canvasGroup;
                }

                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup != null)
                {
                    return _canvasGroup;
                }

                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }

        private bool Interactive
        {
            get => Button.interactable;
            set => Button.interactable = value;
        }

        private bool IsSelected => UnityEventSystem != null && UnityEventSystem.currentSelectedGameObject == gameObject;

        private static EventSystem UnityEventSystem
        {
            get
            {
                if (_unityEventSystem != null)
                {
                    return _unityEventSystem;
                }

                _unityEventSystem = EventSystem.current;
                if (_unityEventSystem != null)
                {
                    return _unityEventSystem;
                }

                _unityEventSystem = FindObjectOfType<EventSystem>();
                if (_unityEventSystem != null)
                {
                    return _unityEventSystem;
                }

                _unityEventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule))
                    .GetComponent<EventSystem>();
                return _unityEventSystem;
            }
        }

        protected override void Reset()
        {
            base.Reset();

            _disableButtonCoroutine = null;

            _clickedOnce = false;
            _doubleClickTimeoutCounter = 0;

            _longClickTimeoutCounter = 0;
            _executedLongClick = false;
            _longClickRegisterCoroutine = null;
        }

        public override void Start()
        {
            _unityEventSystem = GetComponent<EventSystem>();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (IsSelected)
            {
                StartSelectedLoopAnimation();
            }
            else
            {
                StartNormalLoopAnimation();
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();

            UIAnimator.StopAnimations(RectTransform, AnimationType.Punch);
            UIAnimator.StopAnimations(RectTransform, AnimationType.State);

            StopSelectedLoopAnimation();
            StopNormalLoopAnimation();
            ResetToStartValues();
            ReadyAllBehaviors();

            if (_disableButtonCoroutine == null)
            {
                return;
            }

            StopCoroutine(_disableButtonCoroutine);
            _disableButtonCoroutine = null;
            EnableButton();
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            if (eventData.selectedObject != gameObject)
            {
                return;
            }

            StopSelectedLoopAnimation();
            if (!onDeselected.enabled)
            {
                StartNormalLoopAnimation();
                return;
            }

            TriggerButtonBehavior(onDeselected);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                {
                    TriggerButtonBehavior(onClick);
                    break;
                }
                case PointerEventData.InputButton.Right:
                {
                    TriggerButtonBehavior(onRightClick);
                    break;
                }
                case PointerEventData.InputButton.Middle: break;
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            TriggerButtonBehavior(onPointerDown);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            TriggerButtonBehavior(onPointerEnter);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            TriggerButtonBehavior(onPointerExit);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            TriggerButtonBehavior(onPointerUp);
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (eventData.selectedObject != gameObject)
            {
                return;
            }

            StopNormalLoopAnimation();
            if (!onSelected.enabled)
            {
                StartSelectedLoopAnimation();
                return;
            }

            TriggerButtonBehavior(onSelected);
        }

        #if UNITY_EDITOR
        [MenuItem("GameObject/Railek/UI/UIButton", false, 2)]
        private static void CreateComponent(MenuCommand menuCommand)
        {
            var go = new GameObject("UIButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(UIButton));
            GameObjectUtility.SetParentAndAlign(go, GetCanvasAsParent(menuCommand.context as GameObject));
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            var uiButton = go.GetComponent<UIButton>();
            uiButton.RectTransform.Center(true);
            uiButton.RectTransform.sizeDelta = new Vector2(DefaultButtonWidth, DefaultButtonHeight);
            uiButton.Button.targetGraphic = go.GetComponent<Image>();
            Selection.activeObject = go;
        }
        #endif

        private void DeselectButton()
        {
            if (!IsSelected)
            {
                return;
            }

            UnityEventSystem.SetSelectedGameObject(null);
        }

        private void DisableButton()
        {
            Interactive = false;
        }

        private void EnableButton()
        {
            Interactive = true;
        }

        private void ExecutePointerEnter()
        {
            if (!onPointerEnter.enabled)
            {
                StopNormalLoopAnimation();
                StopSelectedLoopAnimation();

                return;
            }

            StartCoroutine(ExecuteButtonBehaviorEnumerator(onPointerEnter));
        }

        private void ExecutePointerExit()
        {
            if (!onPointerExit.enabled)
            {
                return;
            }

            StartCoroutine(ExecuteButtonBehaviorEnumerator(onPointerExit));
        }

        private void ExecutePointerDown()
        {
            if (onLongClick.enabled && Interactive)
            {
                RegisterLongClick();
            }

            if (!onPointerDown.enabled)
            {
                return;
            }

            StartCoroutine(ExecuteButtonBehaviorEnumerator(onPointerDown));
        }

        private void ExecutePointerUp()
        {
            UnregisterLongClick();
            if (!onPointerUp.enabled)
            {
                return;
            }

            StartCoroutine(ExecuteButtonBehaviorEnumerator(onPointerUp));
        }

        private void ExecuteClick()
        {
            if (onClick.enabled)
            {
                if (Interactive)
                {
                    StartCoroutine(ExecuteButtonBehaviorEnumerator(onClick));
                }
            }

            if (Interactive || !onPointerExit.enabled || !onPointerExit.ready)
            {
                return;
            }

            StartCoroutine(ExecuteButtonBehaviorEnumerator(onPointerExit));
        }

        private void ExecuteDoubleClick()
        {
            if (onDoubleClick.enabled)
            {
                if (Interactive)
                {
                    StartCoroutine(ExecuteButtonBehaviorEnumerator(onDoubleClick));
                }
            }

            if (Interactive || !onPointerExit.enabled || !onPointerExit.ready)
            {
                return;
            }

            StartCoroutine(ExecuteButtonBehaviorEnumerator(onPointerExit));
        }

        private void ExecuteLongClick()
        {
            if (onLongClick.enabled)
            {
                if (Interactive)
                {
                    StartCoroutine(ExecuteButtonBehaviorEnumerator(onLongClick));
                }
            }

            if (Interactive || !onPointerExit.enabled || !onPointerExit.ready)
            {
                return;
            }

            StartCoroutine(ExecuteButtonBehaviorEnumerator(onPointerExit));
        }

        private void ExecuteRightClick()
        {
            if (onRightClick.enabled)
            {
                if (Interactive)
                {
                    StartCoroutine(ExecuteButtonBehaviorEnumerator(onRightClick));
                }
            }

            if (Interactive || !onPointerExit.enabled || !onPointerExit.ready)
            {
                return;
            }

            StartCoroutine(ExecuteButtonBehaviorEnumerator(onPointerExit));
        }

        private void ExecuteOnButtonDeselected()
        {
            if (!onDeselected.enabled)
            {
                return;
            }

            StartCoroutine(ExecuteButtonBehaviorEnumerator(onDeselected));
        }

        private void ExecuteOnButtonSelected()
        {
            if (!onSelected.enabled)
            {
                return;
            }

            StartCoroutine(ExecuteButtonBehaviorEnumerator(onSelected));
        }

        private void StartNormalLoopAnimation()
        {
            if (normalLoopAnimation == null)
            {
                return;
            }

            if (!normalLoopAnimation.enabled)
            {
                return;
            }

            ResetToStartValues();
            normalLoopAnimation.Start(RectTransform, startPosition, startRotation);
        }

        private void StartSelectedLoopAnimation()
        {
            if (selectedLoopAnimation == null)
            {
                return;
            }

            if (!selectedLoopAnimation.enabled)
            {
                return;
            }

            ResetToStartValues();
            selectedLoopAnimation.Start(RectTransform, startPosition, startRotation);
        }

        private void StopNormalLoopAnimation()
        {
            if (normalLoopAnimation == null)
            {
                return;
            }

            if (!normalLoopAnimation.isPlaying)
            {
                return;
            }

            normalLoopAnimation.Stop(RectTransform);
            ResetToStartValues();
        }

        private void StopSelectedLoopAnimation()
        {
            if (selectedLoopAnimation == null)
            {
                return;
            }

            if (!selectedLoopAnimation.isPlaying)
            {
                return;
            }

            selectedLoopAnimation.Stop(RectTransform);
            ResetToStartValues();
        }

        private void TriggerButtonBehavior(UIButtonBehavior behavior)
        {
            switch (behavior.BehaviorType)
            {
                case UIButtonBehaviorType.OnClick:
                {
                    if (!Interactive || UIInteractionsDisabled)
                    {
                        return;
                    }

                    if (!behavior.ready)
                    {
                        return;
                    }

                    InitiateClick();
                    break;
                }
                case UIButtonBehaviorType.OnPointerEnter:
                {
                    StopNormalLoopAnimation();
                    StopSelectedLoopAnimation();
                    if (!Interactive || UIInteractionsDisabled)
                    {
                        return;
                    }

                    if (!behavior.ready)
                    {
                        return;
                    }

                    ExecutePointerEnter();
                    break;
                }
                case UIButtonBehaviorType.OnPointerExit:
                {
                    if (IsSelected)
                    {
                        StartSelectedLoopAnimation();
                    }
                    else
                    {
                        StartNormalLoopAnimation();
                    }

                    if (!Interactive || UIInteractionsDisabled)
                    {
                        return;
                    }

                    if (!behavior.ready)
                    {
                        return;
                    }

                    ExecutePointerExit();
                    break;
                }
                case UIButtonBehaviorType.OnPointerDown:
                {
                    if (!Interactive || UIInteractionsDisabled)
                    {
                        return;
                    }

                    if (!behavior.ready)
                    {
                        return;
                    }

                    ExecutePointerDown();
                    break;
                }
                case UIButtonBehaviorType.OnPointerUp:
                {
                    if (!Interactive || UIInteractionsDisabled)
                    {
                        return;
                    }

                    if (!behavior.ready)
                    {
                        return;
                    }

                    ExecutePointerUp();
                    break;
                }
                case UIButtonBehaviorType.OnRightClick:
                {
                    if (!Interactive || UIInteractionsDisabled)
                    {
                        return;
                    }

                    if (!behavior.ready)
                    {
                        return;
                    }

                    ExecuteRightClick();
                    break;
                }
                case UIButtonBehaviorType.OnSelected:
                {
                    ExecuteOnButtonSelected();
                    break;
                }
                case UIButtonBehaviorType.OnDeselected:
                {
                    ExecuteOnButtonDeselected();
                    break;
                }
                case UIButtonBehaviorType.OnDoubleClick:
                {
                    break;
                }
                case UIButtonBehaviorType.OnLongClick:
                {
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void InitiateClick()
        {
            if (_executedLongClick)
            {
                ResetLongClick();
                return;
            }

            StartCoroutine(RunOnClickEnumerator());
        }

        private void ReadyAllBehaviors()
        {
            onPointerEnter.ready = true;
            onPointerExit.ready = true;
            onPointerUp.ready = true;
            onPointerDown.ready = true;
            onClick.ready = true;
            onDoubleClick.ready = true;
            onLongClick.ready = true;
            onRightClick.ready = true;
            onSelected.ready = true;
            onDeselected.ready = true;
        }

        private void RegisterLongClick()
        {
            if (_executedLongClick)
            {
                return;
            }

            ResetLongClick();
            _longClickRegisterCoroutine = StartCoroutine(RunOnLongClickEnumerator());
        }

        private void UnregisterLongClick()
        {
            if (_executedLongClick)
            {
                return;
            }

            ResetLongClick();
        }

        private void ResetLongClick()
        {
            _executedLongClick = false;
            _longClickTimeoutCounter = 0;
            if (_longClickRegisterCoroutine == null)
            {
                return;
            }

            StopCoroutine(_longClickRegisterCoroutine);
            _longClickRegisterCoroutine = null;
        }

        private IEnumerator ExecuteButtonBehaviorEnumerator(UIButtonBehavior behavior)
        {
            if (!behavior.enabled)
            {
                yield break;
            }

            if (!_updateStartValuesRequired)
            {
                UpdateStartValues();
                _updateStartValuesRequired = true;
            }

            switch (behavior.BehaviorType)
            {
                case UIButtonBehaviorType.OnClick:
                case UIButtonBehaviorType.OnDoubleClick:
                case UIButtonBehaviorType.OnLongClick:
                case UIButtonBehaviorType.OnRightClick:
                case UIButtonBehaviorType.OnPointerEnter:
                case UIButtonBehaviorType.OnPointerExit:
                case UIButtonBehaviorType.OnPointerDown:
                case UIButtonBehaviorType.OnPointerUp:
                {
                    if (!Interactive || UIInteractionsDisabled)
                    {
                        yield break;
                    }

                    break;
                }
                case UIButtonBehaviorType.OnSelected:
                {
                    break;
                }
                case UIButtonBehaviorType.OnDeselected:
                {
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            StopNormalLoopAnimation();
            StopSelectedLoopAnimation();
            behavior.PlayAnimation(this);

            if (behavior.onTrigger != null)
            {
                if (behavior.triggerEventAfterAnimation)
                {
                    yield return new WaitForSecondsRealtime(behavior.GetAnimationTotalDuration());
                }

                behavior.onTrigger.Raise();
            }

            switch (behavior.BehaviorType)
            {
                case UIButtonBehaviorType.OnSelected:
                {
                    StartSelectedLoopAnimation();
                    break;
                }
                case UIButtonBehaviorType.OnDeselected:
                {
                    StartNormalLoopAnimation();
                    break;
                }
                case UIButtonBehaviorType.OnClick:
                case UIButtonBehaviorType.OnDoubleClick:
                case UIButtonBehaviorType.OnLongClick:
                case UIButtonBehaviorType.OnRightClick:
                case UIButtonBehaviorType.OnPointerEnter:
                case UIButtonBehaviorType.OnPointerExit:
                {
                    if (behavior.deselectButton)
                    {
                        DeselectButton();
                    }

                    if (IsSelected)
                    {
                        StartSelectedLoopAnimation();
                    }
                    else
                    {
                        StartNormalLoopAnimation();
                    }

                    break;
                }
                case UIButtonBehaviorType.OnPointerDown:
                case UIButtonBehaviorType.OnPointerUp:
                {
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private IEnumerator RunOnClickEnumerator()
        {
            ExecuteClick();

            if (!_clickedOnce && _doubleClickTimeoutCounter < 0.2f)
            {
                _clickedOnce = true;
            }
            else
            {
                _clickedOnce = false;
                yield break;
            }

            yield return new WaitForEndOfFrame();

            while (_doubleClickTimeoutCounter < 0.2f)
            {
                if (!_clickedOnce)
                {
                    ExecuteDoubleClick();
                    _doubleClickTimeoutCounter = 0f;
                    _clickedOnce = false;
                    yield break;
                }

                if (true)
                {
                    _doubleClickTimeoutCounter += Time.unscaledDeltaTime;
                }

                yield return null;
            }

            _doubleClickTimeoutCounter = 0f;
            _clickedOnce = false;
        }

        private IEnumerator RunOnLongClickEnumerator()
        {
            while (_longClickTimeoutCounter < 0.5f)
            {
                if (true)
                {
                    _longClickTimeoutCounter += Time.unscaledDeltaTime;
                }

                yield return null;
            }

            ExecuteLongClick();
            _executedLongClick = true;
        }
    }
}
